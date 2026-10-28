using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using EJ2WordDocument = Syncfusion.EJ2.DocumentEditor.WordDocument;
using FormatType = Syncfusion.EJ2.DocumentEditor.FormatType;

namespace DocumentEditor.Controllers
{
    // Back-end service for the Document Editor. The client calls these actions as
    // <serviceUrl>Import, <serviceUrl>SystemClipboard and <serviceUrl>RestrictEditing.
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
