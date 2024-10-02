using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using PrintInvoice.Properties;

namespace PrintInvoice
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            pbIcon.Image = Resources.w3pack.ToBitmap();
            Text = Application.OpenForms[0].Text ?? Text;
            
            lblVersion.Text = $@"v{Application.ProductVersion}";
            lblProduct.Text =  Application.ProductName;
            lblCopyright.Text = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly()?.Location ?? "").LegalCopyright;
        }
    }
}
