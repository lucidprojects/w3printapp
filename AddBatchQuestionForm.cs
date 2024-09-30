using System;
using System.Reflection;
using System.Windows.Forms;

namespace PrintInvoice
{
    public partial class AddBatchQuestionForm : Form
    {
        public enum Result
        {
            Cancel,
            Single,
            Batch
        }

        public Result _result;

        public AddBatchQuestionForm()
        {
            InitializeComponent();
            Icon = System.Drawing.Icon.ExtractAssociatedIcon(Assembly.GetEntryAssembly()?.Location ?? "");
        }

        private void btSingle_Click(object sender, EventArgs e)
        {
            _result = Result.Single;
            Close();
        }

        private void btBatch_Click(object sender, EventArgs e)
        {
            _result = Result.Batch;
            Close();
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            _result = Result.Cancel;
            Close();
        }
    }
}