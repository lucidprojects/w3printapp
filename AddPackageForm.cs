using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace PrintInvoice
{
    public partial class AddPackageForm : Form
    {
        public enum ResultType
        {
            ADD_SINGLE,
            ADD_BATCH,
            CANCEL
        }

        private AddBatchQuestionForm _fmUnshipBatchQuestion;

        public AddPackageForm()
        {
            InitializeComponent();
            Icon = System.Drawing.Icon.ExtractAssociatedIcon(Assembly.GetEntryAssembly()?.Location ?? "");
        }

        public ResultType Result { get; set; }

        private void btAdd_Click(object sender, EventArgs e)
        {
            //clear empty lines
            tbInvoiceIdList.Lines = tbInvoiceIdList.Lines.Where(t => t.Length != 0).ToArray();

            if (tbInvoiceIdList.Text.Length == 0)
            {
                MessageBox.Show(@"Please specify Tracking ID(s).", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                // create form if not exists
                if (_fmUnshipBatchQuestion == null) _fmUnshipBatchQuestion = new AddBatchQuestionForm();

                _fmUnshipBatchQuestion._result = AddBatchQuestionForm.Result.CANCEL;
                _fmUnshipBatchQuestion.ShowDialog();

                switch (_fmUnshipBatchQuestion._result)
                {
                    case AddBatchQuestionForm.Result.SINGLE:
                        Result = ResultType.ADD_SINGLE;
                        break;

                    case AddBatchQuestionForm.Result.BATCH:
                        Result = ResultType.ADD_BATCH;
                        break;
                }

                Close();
            }
        }

        private void btClose_Click(object sender, EventArgs e)
        {
            Result = ResultType.CANCEL;
            Close();
        }
    }
}