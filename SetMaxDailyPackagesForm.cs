using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PrintInvoice
{
  public partial class SetMaxDailyPackagesForm : Form
  {
    public SetMaxDailyPackagesForm()
    {
      InitializeComponent();
    }

    public int Value
    {
      set { edValue.Text = value.ToString(); }
      get { return Int32.Parse(edValue.Text); }
    }

    private void btCancel_Click(object sender, EventArgs e)
    {
      DialogResult = DialogResult.Cancel;
    }

    private void btSet_Click(object sender, EventArgs e)
    {
      try
      {
        Int32.Parse(edValue.Text);
        DialogResult = DialogResult.OK;
      }
      catch (Exception)
      {
        MessageBox.Show(
          this,
          "Please enter correct integer value",
          "Error",
          MessageBoxButtons.OK,
          MessageBoxIcon.Error
          );
        edValue.Focus();
        edValue.SelectAll();
      }
    }
  }
}