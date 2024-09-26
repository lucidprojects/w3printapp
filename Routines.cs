using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace PrintInvoice
{
  class Routines
  {
    public static bool checkLabelserviceRequestStatus(IWin32Window aOwner, ResponseBaseType aResponse)
    {
      if (aResponse.status == 0)
      {
        return true;
      }
      else 
      { 
        MessageBox.Show(
          aOwner,
          String.Format("Label service returns error status\nStatus: {0}\nMessage: {1}\nSubstatus: {2}\nSubmessage: {3}", aResponse.status, aResponse.message, aResponse.substatus, aResponse.submessage),
          "Error",
          MessageBoxButtons.OK,
          MessageBoxIcon.Error
          );

        return false;
      }
    }

    public static string getVersion() {
      Version v = new Version(Application.ProductVersion);
      return v.ToString(2);
    }

    public static string generateSequenceNumber(int aPrintBatchId, int aPrintBatchCount, int aElementBatch, int aElementBatchCount) {
      return String.Format("{0:00000000}-{1:000000}-{2:000000}-{3:000000}", aPrintBatchId, aPrintBatchCount, aElementBatch, aElementBatchCount);
    }

    public static void addSequenceNumberToPdf(string aSequenceNumber, ref byte[] aPdf, bool aIsPackJacket) {

      Stream inputPdfStream = new MemoryStream(aPdf);
      iTextSharp.text.pdf.PdfReader pdfReader = new iTextSharp.text.pdf.PdfReader(inputPdfStream);
      MemoryStream outputPdfStream = new MemoryStream();

      iTextSharp.text.pdf.PdfStamper pdfStamper = new iTextSharp.text.pdf.PdfStamper(pdfReader, outputPdfStream);

      iTextSharp.text.pdf.PdfContentByte pdfPageContents = pdfStamper.GetOverContent(1);
      pdfPageContents.BeginText();
      iTextSharp.text.pdf.BaseFont baseFont = iTextSharp.text.pdf.BaseFont.CreateFont(iTextSharp.text.pdf.BaseFont.HELVETICA, Encoding.ASCII.EncodingName, false);
      pdfPageContents.SetFontAndSize(baseFont, Properties.Settings.Default.InvoiceSequenceNumberFontSize);
      pdfPageContents.SetRGBColorFill(0, 0, 0); 
      pdfPageContents.ShowTextAligned(
        iTextSharp.text.pdf.PdfContentByte.ALIGN_LEFT,
        aSequenceNumber,
        aIsPackJacket ? Properties.Settings.Default.InvoiceSequenceNumberXPackJacket : Properties.Settings.Default.InvoiceSequenceNumberX,
        aIsPackJacket ? Properties.Settings.Default.InvoiceSequenceNumberYPackJacket : Properties.Settings.Default.InvoiceSequenceNumberY,
        0);
      pdfPageContents.EndText(); // Done working with text
      pdfStamper.FormFlattening = true; // enable this if you want the PDF flattened. 
      pdfStamper.Close();

      aPdf = outputPdfStream.ToArray();
    }

    public static byte[] getMasterPickListPdf(PrintPackageWrapper aPackage, LabelService aLabelService)
    {
      iTextSharp.text.Document doc = new iTextSharp.text.Document(aPackage.isPackJacket ? iTextSharp.text.PageSize.LEGAL : iTextSharp.text.PageSize.LETTER);
      MemoryStream outputPdfStream = new MemoryStream();
      iTextSharp.text.pdf.PdfWriter pdfWriter = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, outputPdfStream);

      doc.Open();

      iTextSharp.text.Paragraph paragraph = new iTextSharp.text.Paragraph();
      paragraph.Font.Size = Properties.Settings.Default.MasterPickListFontSize;
      paragraph.SetLeading(0, 1f);

      // common information
      paragraph.Add("Master pick list\n\n");
      paragraph.Add(String.Format("Batch ID: {0}\n", aPackage.printBatchId));
      paragraph.Add(String.Format("Element batch ID: {0}\n", aPackage.elementBatch));
      paragraph.Add(String.Format("Element batch count: {0}\n", aPackage.mplElementBatchCount));

      // pick list
      GetMasterPickListRequestType request = new GetMasterPickListRequestType();
      request.packageId = aPackage.PackageId;
      GetMasterPickListResponseType response = aLabelService.getMasterPickList(request);
      if (response.status != 0)
      {
        paragraph.Add("\n" + response.message);
      }
      else if (response.items != null)
      {
        iTextSharp.text.pdf.PdfPTable table = new iTextSharp.text.pdf.PdfPTable(new float[] { 9f, 1f });
        table.HorizontalAlignment = 0;
        table.WidthPercentage = 100;
        foreach (RowType row in response.items)
        {
          iTextSharp.text.pdf.PdfPCell cell0 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(row.columns[0], paragraph.Font));
          cell0.NoWrap = false;
          table.AddCell(cell0);
          iTextSharp.text.pdf.PdfPCell cell1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(row.columns[1], paragraph.Font));
          cell1.HorizontalAlignment = 1;
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
