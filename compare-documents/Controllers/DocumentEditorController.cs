using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Syncfusion.DocIO;
using DLSWordDocument = Syncfusion.DocIO.DLS.WordDocument;
using EJ2WordDocument = Syncfusion.EJ2.DocumentEditor.WordDocument;
using DocIOFormatType = Syncfusion.DocIO.FormatType;

namespace DocumentEditor.Controllers
{
    // Web API used by the Document Editor sample to compare two documents on the server side.
    public class DocumentEditorController : Controller
    {
        // Multipart form: "OriginalFile" and "RevisedFile" = the documents to compare (.docx, .doc, .rtf or .sfdt).
        // Returns the original document with the differences marked as tracked changes, as SFDT.
        [HttpPost]
        public ActionResult CompareDocuments(HttpPostedFileBase originalFile, HttpPostedFileBase revisedFile)
        {
            if (originalFile == null || revisedFile == null)
            {
                return new HttpStatusCodeResult(400, "Both the original and revised files are required.");
            }

            try
            {
                using (DLSWordDocument revisedDocument = GetWordDocument(revisedFile))
                using (DLSWordDocument originalDocument = GetWordDocument(originalFile))
                {
                    // Compares the original document with the revised document and highlights the changes as tracked changes.
                    originalDocument.Compare(revisedDocument);

                    // Loads the compared document into the WordDocument instance of the Document Editor server-side library.
                    EJ2WordDocument document = EJ2WordDocument.Load(originalDocument);

                    // Serializes the compared document as SFDT content.
                    string json = JsonConvert.SerializeObject(document);
                    document.Dispose();
                    return Content(json, "application/json");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error comparing documents: " + ex.Message, ex);
            }
        }

        private static DLSWordDocument GetWordDocument(HttpPostedFileBase file)
        {
            string type = Path.GetExtension(file.FileName);
            if (string.IsNullOrEmpty(type))
            {
                type = ".docx";
            }

            MemoryStream stream = new MemoryStream();
            file.InputStream.CopyTo(stream);
            stream.Position = 0;

            if (type.ToLowerInvariant() == ".sfdt")
            {
                // Converts the SFDT content to DOCX format for comparison.
                using (StreamReader reader = new StreamReader(stream))
                using (DLSWordDocument sfdtDocument = EJ2WordDocument.Save(reader.ReadToEnd()))
                {
                    MemoryStream outStream = new MemoryStream();
                    sfdtDocument.Save(outStream, DocIOFormatType.Docx);
                    outStream.Position = 0;
                    return new DLSWordDocument(outStream, DocIOFormatType.Docx);
                }
            }

            // Loads the document for comparison.
            return new DLSWordDocument(stream, GetDocIOFormatType(type));
        }

        private static DocIOFormatType GetDocIOFormatType(string extension)
        {
            switch (extension.ToLowerInvariant())
            {
                case ".dotx": return DocIOFormatType.Dotx;
                case ".docm": return DocIOFormatType.Docm;
                case ".dotm": return DocIOFormatType.Dotm;
                case ".doc": return DocIOFormatType.Doc;
                case ".dot": return DocIOFormatType.Dot;
                case ".rtf": return DocIOFormatType.Rtf;
                case ".docx": return DocIOFormatType.Docx;
                default: throw new NotSupportedException("Unsupported file format: " + extension);
            }
        }
    }
}
