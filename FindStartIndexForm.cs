using System;
using System.Reflection;
using System.Windows.Forms;

namespace PrintInvoice
{
    public partial class FindStartIndexForm : Form
    {
        private readonly MainForm _fmMain;
        private Config _config;

        public FindStartIndexForm(MainForm aMainForm, Config aConfig)
        {
            InitializeComponent();
            Icon = System.Drawing.Icon.ExtractAssociatedIcon(Assembly.GetEntryAssembly()?.Location ?? "");

            _fmMain = aMainForm;
            _config = aConfig;
        }

        private void btFind_Click(object sender, EventArgs e)
        {
            if (edFindValue.Text.Length == 0)
            {
                MessageBox.Show(@"Please specify value.", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (_fmMain.SetCurrentSubsetInvoice(edFindValue.Text, PrintPackageStorage.TrackingNumberColumnIndex, true))
                    Close();
                else
                    MessageBox.Show(@"Invoice not found.", @"Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}