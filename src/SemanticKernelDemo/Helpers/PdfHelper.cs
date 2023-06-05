using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.Content;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticKernelDemo.Helpers
{
    public class PdfHelper
    {
        public static string GetTextFromPdf(string FilePdf)
        {
            PdfDocument PDFDoc = PdfReader.Open(FilePdf, PdfDocumentOpenMode.ReadOnly);
            PdfDocument PDFNewDoc = new PdfDocument();
            var sb = new StringBuilder();
            foreach ( var page in PDFDoc.Pages)
            {
                foreach(var text in page.ExtractText())
                {
                    sb.AppendLine(text);
                }
            }
            return sb.ToString();

        }
        public static string GetTextFromPdf(Stream FilePdf)
        {
            PdfDocument doc = PdfReader.Open(FilePdf, PdfDocumentOpenMode.ReadOnly);

            StringBuilder sb = new StringBuilder();
            using (PdfSharpTextExtractor.Extractor extractor = new PdfSharpTextExtractor.Extractor(doc))
            {
                foreach (PdfPage page in doc.Pages)
                {
                    extractor.ExtractText(page, sb);
                }

            }
            return sb.ToString();

        }
        public static IEnumerable<string> GetTextPerPageFromPdf(Stream FilePdf)
        {
            PdfDocument doc = PdfReader.Open(FilePdf, PdfDocumentOpenMode.ReadOnly);

            StringBuilder sb = new StringBuilder();
            using (PdfSharpTextExtractor.Extractor extractor = new PdfSharpTextExtractor.Extractor(doc))
            {
                foreach (PdfPage page in doc.Pages)
                {
                    sb.Clear();
                    extractor.ExtractText(page, sb);
                    yield return sb.ToString();
                }
            }
        }
    }
}
