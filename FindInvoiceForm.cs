using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PrintInvoice
{
  public partial class FindInvoiceForm : Form
  {
    private MainForm fmMain;
    private Config config;

    public FindInvoiceForm(MainForm aMainForm, Config aConfig)
    {
      InitializeComponent();

      fmMain = aMainForm;
      config = aConfig;
    }

    private void FindInvoiceForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (e.CloseReason == CloseReason.UserClosing)
      {
        Hide();
        e.Cancel = true;
      }
    }

    private void btFind_Click(object sender, EventArgs e)
    {
      if (edFindValue.Text.Length == 0)
      {
        MessageBox.Show(
          "Please specify value.",
          "Error",
          MessageBoxButtons.OK,
          MessageBoxIcon.Error
          );
      }
      else
      {
        bool foundInSet = fmMain.setCurrentSetInvoice(edFindValue.Text, PrintPackageStorage.INVOICE_NUMBER_COLUMN_INDEX);
        bool foundInSubset = fmMain.setCurrentSubsetInvoice(edFindValue.Text, PrintPackageStorage.INVOICE_NUMBER_COLUMN_INDEX, false);
        if (!foundInSet && !foundInSubset)
        {
          MessageBox.Show(
            "Invoice not found.",
            "Message",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information
            );
        }
      }
    }
  }
}