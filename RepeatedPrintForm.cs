using System;
using System.Reflection;
using System.Windows.Forms;

namespace PrintInvoice
{
    public partial class RepeatedPrintForm : Form
    {
        public RepeatedPrintForm()
        {
            InitializeComponent();
            Icon = System.Drawing.Icon.ExtractAssociatedIcon(Assembly.GetEntryAssembly()?.Location ?? "");
            
            DialogResult = DialogResult.No;
        }

        private void btPrint_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
            Close();
        }

        private void btDontPrint_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
            Close();
        }
    }
}