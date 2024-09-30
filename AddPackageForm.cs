using System;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace PrintInvoice
{
    public partial class AddPackageForm : Form
    {
        public enum ResultType
        {
            AddSingle,
            AddBatch,
            Cancel
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
                this.ShowError(@"Please specify Tracking ID(s).");
            }
            else
            {
                // create form if not exists
                if (_fmUnshipBatchQuestion == null) _fmUnshipBatchQuestion = new AddBatchQuestionForm();

                _fmUnshipBatchQuestion._result = AddBatchQuestionForm.Result.Cancel;
                _fmUnshipBatchQuestion.ShowDialog();

                switch (_fmUnshipBatchQuestion._result)
                {
                    case AddBatchQuestionForm.Result.Single:
                        Result = ResultType.AddSingle;
                        break;

                    case AddBatchQuestionForm.Result.Batch:
                        Result = ResultType.AddBatch;
                        break;
                }

                Close();
            }
        }

        private void btClose_Click(object sender, EventArgs e)
        {
            Result = ResultType.Cancel;
            Close();
        }
    }
}