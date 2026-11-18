using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Syncfusion.DocIO;
using Syncfusion.DocToPDFConverter;
using Syncfusion.EJ2.SpellChecker;
using DLSWordDocument = Syncfusion.DocIO.DLS.WordDocument;
using EJ2WordDocument = Syncfusion.EJ2.DocumentEditor.WordDocument;
using DocIOFormatType = Syncfusion.DocIO.FormatType;
using FormatType = Syncfusion.EJ2.DocumentEditor.FormatType;

namespace DocumentEditor.Controllers
{
    // Back-end service for the Document Editor. The client calls these actions as
    // <serviceUrl>Import, SystemClipboard, RestrictEditing, SpellCheck and SpellCheckByPage.
    // Save and Export are optional endpoints for server-side conversion to formats the client
    // cannot produce itself (DOC, RTF, HTML, ODT, PDF ...).
    public class DocumentEditorController : Controller
    {
        public class CustomParameter
        {
            public string content { get; set; }
            public string type { get; set; }
        }

        public class RestrictParameter
        {
            public string passwordBase64 { get; set; }
            public string saltBase64 { get; set; }
            public int spinCount { get; set; }
        }

        public class SpellCheckJsonData
        {
            public int LanguageID { get; set; }
            public string TexttoCheck { get; set; }
            public bool CheckSpelling { get; set; }
            public bool CheckSuggestion { get; set; }
            public bool AddWord { get; set; }
        }

        public class SaveParameter
        {
            public string Content { get; set; }
            public string FileName { get; set; }
        }

        private const string EmptySpellResult = "{\"SpellCollection\":[],\"HasSpellingError\":false,\"Suggestions\":null}";

        [HttpPost]
        public ActionResult Import(HttpPostedFileBase files)
        {
            if (files == null || files.ContentLength == 0)
            {
                return new HttpStatusCodeResult(400, "No file was uploaded.");
            }

            FormatType type = GetFormatType(Path.GetExtension(files.FileName));
            using (var stream = new MemoryStream())
            {
                files.InputStream.CopyTo(stream);
                stream.Position = 0;
                EJ2WordDocument document = EJ2WordDocument.Load(stream, type);
                string json = JsonConvert.SerializeObject(document);
                document.Dispose();
                return Content(json, "application/json");
            }
        }

        [HttpPost]
        public ActionResult SystemClipboard(CustomParameter param)
        {
            if (param == null || string.IsNullOrEmpty(param.content))
            {
                return Content(string.Empty);
            }

            string type = (param.type ?? ".html").ToLowerInvariant();
            if (type != ".html" && type != ".rtf")
            {
                return new HttpStatusCodeResult(400, "Unsupported clipboard type.");
            }

            EJ2WordDocument document = EJ2WordDocument.LoadString(param.content, GetFormatType(type));
            string json = JsonConvert.SerializeObject(document);
            document.Dispose();
            return Content(json, "application/json");
        }

        [HttpPost]
        public ActionResult RestrictEditing(RestrictParameter param)
        {
            if (param == null || param.saltBase64 == null || param.passwordBase64 == null)
            {
                return new HttpStatusCodeResult(400, "Invalid request.");
            }

            string[] hash = EJ2WordDocument.ComputeHash(param.passwordBase64, param.saltBase64, param.spinCount);
            return Content(JsonConvert.SerializeObject(hash), "application/json");
        }

        [HttpPost]
        public ActionResult SpellCheck(SpellCheckJsonData spellChecker)
        {
            try
            {
                SpellChecker spellCheck = new SpellChecker();
                spellCheck.GetSuggestions(spellChecker.LanguageID, spellChecker.TexttoCheck, spellChecker.CheckSpelling, spellChecker.CheckSuggestion, spellChecker.AddWord);
                return Content(JsonConvert.SerializeObject(spellCheck), "application/json");
            }
            catch
            {
                return Content(EmptySpellResult, "application/json");
            }
        }

        [HttpPost]
        public ActionResult SpellCheckByPage(SpellCheckJsonData spellChecker)
        {
            try
            {
                SpellChecker spellCheck = new SpellChecker();
                spellCheck.CheckSpelling(spellChecker.LanguageID, spellChecker.TexttoCheck);
                return Content(JsonConvert.SerializeObject(spellCheck), "application/json");
            }
            catch
            {
                return Content(EmptySpellResult, "application/json");
            }
        }

        // Body: { "Content": "<SFDT json>", "FileName": "report.pdf" }. The extension of FileName selects
        // the output format. Returns the converted file as a download.
        [HttpPost]
        public ActionResult ExportSFDT(SaveParameter data)
        {
            if (data == null || string.IsNullOrEmpty(data.Content))
            {
                return new HttpStatusCodeResult(400, "No document content was provided.");
            }

            string name = string.IsNullOrWhiteSpace(data.FileName) ? "Document1.docx" : Path.GetFileName(data.FileName);
            using (DLSWordDocument document = EJ2WordDocument.Save(data.Content))
            {
                return ConvertToFile(document, name);
            }
        }

        // Multipart form: "data" = a DOCX file produced by the client (saveAsBlob), "filename" = target name.
        [HttpPost]
        public ActionResult Export(HttpPostedFileBase data, string filename)
        {
            if (data == null || data.ContentLength == 0)
            {
                return new HttpStatusCodeResult(400, "No document was uploaded.");
            }

            string name = string.IsNullOrWhiteSpace(filename) ? "Document1.docx" : Path.GetFileName(filename);
            using (DLSWordDocument document = new DLSWordDocument(data.InputStream, DocIOFormatType.Docx))
            {
                return ConvertToFile(document, name);
            }
        }

        // Saves the document to the server's App_Data/SavedDocuments folder in the format given by the extension.
        [HttpPost]
        public ActionResult Save(SaveParameter data)
        {
            if (data == null || string.IsNullOrEmpty(data.Content))
            {
                return new HttpStatusCodeResult(400, "No document content was provided.");
            }

            string name = string.IsNullOrWhiteSpace(data.FileName) ? "Document1.docx" : Path.GetFileName(data.FileName);
            string folder = Server.MapPath("~/App_Data/SavedDocuments");
            Directory.CreateDirectory(folder);
            string path = Path.Combine(folder, name);

            using (DLSWordDocument document = EJ2WordDocument.Save(data.Content))
            using (Stream output = ConvertToStream(document, Path.GetExtension(name), out _))
            using (var file = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                output.CopyTo(file);
            }
            return new HttpStatusCodeResult(200);
        }

        private ActionResult ConvertToFile(DLSWordDocument document, string fileName)
        {
            Stream stream = ConvertToStream(document, Path.GetExtension(fileName), out string contentType);
            return File(stream, contentType, fileName);
        }

        private static Stream ConvertToStream(DLSWordDocument document, string extension, out string contentType)
        {
            var stream = new MemoryStream();
            string ext = (extension ?? string.Empty).ToLowerInvariant();

            if (ext == ".pdf")
            {
                using (var converter = new DocToPDFConverter())
                using (var pdf = converter.ConvertToPDF(document))
                {
                    pdf.Save(stream);
                }
                contentType = "application/pdf";
            }
            else
            {
                DocIOFormatType type;
                switch (ext)
                {
                    case ".dotx": type = DocIOFormatType.Dotx; contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.template"; break;
                    case ".docm": type = DocIOFormatType.Docm; contentType = "application/vnd.ms-word.document.macroEnabled.12"; break;
                    case ".dotm": type = DocIOFormatType.Dotm; contentType = "application/vnd.ms-word.template.macroEnabled.12"; break;
                    case ".doc": type = DocIOFormatType.Doc; contentType = "application/msword"; break;
                    case ".dot": type = DocIOFormatType.Dot; contentType = "application/msword"; break;
                    case ".rtf": type = DocIOFormatType.Rtf; contentType = "application/rtf"; break;
                    case ".html": case ".htm": type = DocIOFormatType.Html; contentType = "text/html"; break;
                    case ".txt": type = DocIOFormatType.Txt; contentType = "text/plain"; break;
                    case ".xml": type = DocIOFormatType.WordML; contentType = "application/xml"; break;
                    case ".odt": type = DocIOFormatType.Odt; contentType = "application/vnd.oasis.opendocument.text"; break;
                    case ".docx": type = DocIOFormatType.Docx; contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document"; break;
                    default: throw new NotSupportedException("Unsupported file format: " + extension);
                }
                document.Save(stream, type);
            }

            stream.Position = 0;
            return stream;
        }

        private static FormatType GetFormatType(string extension)
        {
            switch ((extension ?? string.Empty).ToLowerInvariant())
            {
                case ".dotx":
                case ".docx":
                case ".docm":
                case ".dotm":
                    return FormatType.Docx;
                case ".dot":
                case ".doc":
                    return FormatType.Doc;
                case ".rtf":
                    return FormatType.Rtf;
                case ".txt":
                    return FormatType.Txt;
                case ".xml":
                    return FormatType.WordML;
                case ".html":
                    return FormatType.Html;
                default:
                    throw new NotSupportedException("Unsupported file format: " + extension);
            }
        }
    }
}
