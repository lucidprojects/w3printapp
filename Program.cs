using System;
using System.Windows.Forms;

namespace PrintInvoice
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
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
    }
}