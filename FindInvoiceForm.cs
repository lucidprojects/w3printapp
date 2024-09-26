using System;
using System.Windows.Forms;

namespace PrintInvoice
{
    public partial class FindInvoiceForm : Form
    {
        private readonly MainForm _fmMain;
        private Config _config;

        public FindInvoiceForm(MainForm aMainForm, Config aConfig)
        {
            InitializeComponent();

            _fmMain = aMainForm;
            _config = aConfig;
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
                MessageBox.Show(@"Please specify value.", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                var foundInSet = _fmMain.SetCurrentSetInvoice(edFindValue.Text, PrintPackageStorage.InvoiceNumberColumnIndex);
                
                var foundInSubset = _fmMain.SetCurrentSubsetInvoice(edFindValue.Text, PrintPackageStorage.InvoiceNumberColumnIndex, false);

                if (!foundInSet && !foundInSubset)
                    MessageBox.Show(@"Invoice not found.", @"Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}