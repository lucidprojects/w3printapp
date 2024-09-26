using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PrintInvoice
{
  public partial class FindStartIndexForm : Form
  {
    private MainForm fmMain;
    private Config config;

    public FindStartIndexForm(MainForm aMainForm, Config aConfig)
    {
      InitializeComponent();

      fmMain = aMainForm;
      config = aConfig;
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
        if (fmMain.setCurrentSubsetInvoice(edFindValue.Text, PrintPackageStorage.TRACKING_NUMBER_COLUMN_INDEX, true))
        {
          Close();
        }
        else
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