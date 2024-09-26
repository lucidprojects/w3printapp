using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PrintInvoice
{
  public partial class AddPackageForm : Form
  {
    public enum ResultType { ADD_SINGLE, ADD_BATCH, CANCEL};

    private AddBatchQuestionForm fmUnshipBatchQuestion = null;
    private ResultType result;

    public AddPackageForm()
    {
      InitializeComponent();
    }

    public ResultType Result
    {
      get { return result; }
      set { result = value; }
    }

    private void btAdd_Click(object sender, EventArgs e)
    {
      //clear empty lines
      List<string> lines = new List<string>();
      for (int i = 0; i < tbInvoiceIdList.Lines.Length; i++)
      {
        if (tbInvoiceIdList.Lines[i].Length != 0)
        {
          lines.Add(tbInvoiceIdList.Lines[i]);
        }
      }
      tbInvoiceIdList.Lines = lines.ToArray();

      if (tbInvoiceIdList.Text.Length == 0)
      {
        MessageBox.Show(
          "Please specify Tracking ID(s).",
          "Error",
          MessageBoxButtons.OK,
          MessageBoxIcon.Error
          );
      }
      else
      {
        // create form if not exists
        if (fmUnshipBatchQuestion == null)
        {
          fmUnshipBatchQuestion = new AddBatchQuestionForm();
        }

        fmUnshipBatchQuestion.result = AddBatchQuestionForm.Result.CANCEL;
        fmUnshipBatchQuestion.ShowDialog();

        switch (fmUnshipBatchQuestion.result)
        { 
          case AddBatchQuestionForm.Result.SINGLE:
            result = ResultType.ADD_SINGLE;
            break;

          case AddBatchQuestionForm.Result.BATCH:
            result = ResultType.ADD_BATCH;
            break;
        }

        Close();
      }
    }

    private void btClose_Click(object sender, EventArgs e)
    {
      result = ResultType.CANCEL;
      Close();
    }
  }
}