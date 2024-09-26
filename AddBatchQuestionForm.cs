using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PrintInvoice
{
  public partial class AddBatchQuestionForm : Form
  {
    public enum Result { CANCEL, SINGLE, BATCH };

    public Result result;

    public AddBatchQuestionForm()
    {
      InitializeComponent();
    }

    private void btSingle_Click(object sender, EventArgs e)
    {
      result = Result.SINGLE;
      Close();
    }

    private void btBatch_Click(object sender, EventArgs e)
    {
      result = Result.BATCH;
      Close();
    }

    private void btCancel_Click(object sender, EventArgs e)
    {
      result = Result.CANCEL;
      Close();
    }
  }
}