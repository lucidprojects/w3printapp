using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PrintInvoice.Properties;

namespace PrintInvoice
{
    internal class Routines
    {
        public static string GetVersion()
        {
            var v = new Version(Application.ProductVersion);
            return v.ToString(2);
        }

        public static string GenerateSequenceNumber(int aPrintBatchId, int aPrintBatchCount, int aElementBatch,
            int aElementBatchCount)
        {
            return
                $"{aPrintBatchId:00000000}-{aPrintBatchCount:000000}-{aElementBatch:000000}-{aElementBatchCount:000000}";
        }

        public static void AddSequenceNumberToPdf(string aSequenceNumber, ref byte[] aPdf, bool aIsPackJacket)
        {
            Stream inputPdfStream = new MemoryStream(aPdf);
            var pdfReader = new PdfReader(inputPdfStream);
            var outputPdfStream = new MemoryStream();

            var pdfStamper = new PdfStamper(pdfReader, outputPdfStream);

            var pdfPageContents = pdfStamper.GetOverContent(1);
            pdfPageContents.BeginText();
            var baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, Encoding.ASCII.EncodingName, false);
            pdfPageContents.SetFontAndSize(baseFont, Settings.Default.InvoiceSequenceNumberFontSize);
            pdfPageContents.SetRGBColorFill(0, 0, 0);

            pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, aSequenceNumber, aIsPackJacket ? Settings.Default.InvoiceSequenceNumberXPackJacket : Settings.Default.InvoiceSequenceNumberX,
                aIsPackJacket ? Settings.Default.InvoiceSequenceNumberYPackJacket : Settings.Default.InvoiceSequenceNumberY, 0);
            
            pdfPageContents.EndText(); // Done working with text
            pdfStamper.FormFlattening = true; // enable this if you want the PDF flattened. 
            pdfStamper.Close();

            aPdf = outputPdfStream.ToArray();
        }

        public static byte[] GetMasterPickListPdf(PrintPackageWrapper aPackage, LabelService aLabelService)
        {
            var doc = new Document(aPackage._isPackJacket ? PageSize.LEGAL : PageSize.LETTER);
            var outputPdfStream = new MemoryStream();

            doc.Open();

            var paragraph = new Paragraph
            {
                Font =
                {
                    Size = Settings.Default.MasterPickListFontSize
                }
            };
            
            paragraph.SetLeading(0, 1f);

            // common information
            paragraph.Add("Master pick list\n\n");
            paragraph.Add($"Batch ID: {aPackage._printBatchId}\n");
            paragraph.Add($"Element batch ID: {aPackage._elementBatch}\n");
            paragraph.Add($"Element batch count: {aPackage._mplElementBatchCount}\n");

            // pick list
            var request = new GetMasterPickListRequestType
            {
                packageId = aPackage.PackageId
            };
            
            var response = aLabelService.getMasterPickList(request);
            
            if (response.status != 0)
            {
                paragraph.Add("\n" + response.message);
            }
            
            else if (response.items != null)
            {
                var table = new PdfPTable(new[] { 9f, 1f })
                {
                    HorizontalAlignment = 0,
                    WidthPercentage = 100
                };
            
                foreach (var row in response.items)
                {
                    var cell0 = new PdfPCell(new Phrase(row.columns[0], paragraph.Font))
                    {
                        NoWrap = false
                    };
                
                    table.AddCell(cell0);
                    
                    var cell1 = new PdfPCell(new Phrase(row.columns[1], paragraph.Font))
                    {
                        HorizontalAlignment = 1
                    };
                    
                    table.AddCell(cell1);
                }

                paragraph.Add(table);
            }

            doc.Add(paragraph);

            doc.Close();

            return outputPdfStream.ToArray();
        }
    }
}