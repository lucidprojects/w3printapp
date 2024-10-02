using System;
using System.Windows.Forms;
using PrintInvoice.UI;

namespace PrintInvoice
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            try
            {
                Application.Run(new MainForm());
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void ShowError(this IWin32Window window, string message)
        {
            MessageBox.Show(window, message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}