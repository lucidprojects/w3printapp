using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PrintInvoice
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      try
      {
        Application.Run(new MainForm());
      }
      catch (Exception e)
      {
        MessageBox.Show(
          e.Message,
          "Error",
          MessageBoxButtons.OK,
          MessageBoxIcon.Error);
      }
    }
  }
}