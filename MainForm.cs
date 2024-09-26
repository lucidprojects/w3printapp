using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace PrintInvoice
{
  using PackageList = List<PrintPackageWrapper>;

  public partial class MainForm : Form
  {
    private Config config;
    private LabelService labelService;
    private PrintPackageStorage invoiceStorage;
    private InvoiceProvider invoiceProvider;
    private Printer printer;
    private InvoiceStatusSaver invoiceStatusSaver;
    private PrintController printController;
    private Unship unship;
    private Unshipped unshipped;
    private Reprint reprint;
    private Repair repair;

    private Dictionary<int, DataGridViewRow> rowIndex = new Dictionary<int, DataGridViewRow>();
    private Dictionary<int, DataGridViewRow> subsetRowIndex = new Dictionary<int, DataGridViewRow>();
    private Dictionary<int, DataGridViewRow> reprintRowIndex = new Dictionary<int, DataGridViewRow>();

    private DataGridViewCellStyle defaultFirstCellStyle = new DataGridViewCellStyle();

    private DataGridViewCellStyle loadedCellStyle = new DataGridViewCellStyle();
    private DataGridViewCellStyle loadedFirstCellStyle = new DataGridViewCellStyle();
    private DataGridViewCellStyle printedCellStyle = new DataGridViewCellStyle();
    private DataGridViewCellStyle printedFirstCellStyle = new DataGridViewCellStyle();
    private DataGridViewCellStyle statusSavedCellStyle = new DataGridViewCellStyle();
    private DataGridViewCellStyle statusSavedFirstCellStyle = new DataGridViewCellStyle();
    private DataGridViewCellStyle errorCellStyle = new DataGridViewCellStyle();
    private DataGridViewCellStyle errorFirstCellStyle = new DataGridViewCellStyle();
    private DataGridViewCellStyle lockedCellStyle = new DataGridViewCellStyle();
    private DataGridViewCellStyle lockedFirstCellStyle = new DataGridViewCellStyle();

    private DataGridViewCellStyle unshipErrorCellStyle = new DataGridViewCellStyle();
    private DataGridViewCellStyle unshipErrorFirstCellStyle = new DataGridViewCellStyle();
    private DataGridViewCellStyle unshipUnshippedCellStyle = new DataGridViewCellStyle();
    private DataGridViewCellStyle unshipUnshippedFirstCellStyle = new DataGridViewCellStyle();

    private DataGridViewCellStyle unshippedNewCellStyle = new DataGridViewCellStyle();
    private DataGridViewCellStyle unshippedNewFirstCellStyle = new DataGridViewCellStyle();
    private DataGridViewCellStyle unshippedUnshippedCellStyle = new DataGridViewCellStyle();
    private DataGridViewCellStyle unshippedUnshippedFirstCellStyle = new DataGridViewCellStyle();
    private DataGridViewCellStyle unshippedUpdatedPickableCellStyle = new DataGridViewCellStyle();
    private DataGridViewCellStyle unshippedUpdatedPickableFirstCellStyle = new DataGridViewCellStyle();
    private DataGridViewCellStyle unshippedUpdatedOnholdCellStyle = new DataGridViewCellStyle();
    private DataGridViewCellStyle unshippedUpdatedOnholdFirstCellStyle = new DataGridViewCellStyle();

    private DataGridViewCellStyle repairOriginalCellStyle = new DataGridViewCellStyle();
    private DataGridViewCellStyle repairOriginalFirstCellStyle = new DataGridViewCellStyle();
    private DataGridViewCellStyle repairRepairedCellStyle = new DataGridViewCellStyle();
    private DataGridViewCellStyle repairRepairedFirstCellStyle = new DataGridViewCellStyle();

    FindInvoiceForm fmFindInvoice = null;
    FindStartIndexForm fmFindStartIndex = null;
    AddPackageForm fmAddUnshipInvoice = null;
    LastBatchesForm fmLastBatches = null;

    bool printerError;
    string printerErrorMessage;
    BackgroundWorker bwPrinterErrorMonitor;
    private AutoResetEvent printerErrorMonitorResetEvent = new AutoResetEvent(false);

    private BackgroundWorker bwStopPrint;
    private BackgroundWorker bwStopReprint;

    private TabPage controlsTabPage; // tab page contains controls
    private int unshipTabIndex;
    private int unshippedTabIndex;
    private int reprintTabIndex;
    private int repairTabIndex;
    private bool unshipFieldNamesIsSet = false;
    private bool unshippedFieldNamesIsSet = false;
    private bool reprintFieldNamesIsSet = false;
    private bool repairFieldNamesIsSet = false;

    // event handlers
    InvoiceProviderLoadEventHandler invoiceProviderLoadReprint;
    InvoiceProviderLoadEventHandler invoiceProviderLoad;
    InvoiceProviderErrorEventHandler invoiceProviderErrorReprint;
    InvoiceProviderErrorEventHandler invoiceProviderError;

    PrinterPrintEventHandler printerPrintReprint;
    PrinterPrintEventHandler printerPrint;
    PrinterJobErrorEventHandler printerJobErrorReprint;
    PrinterJobErrorEventHandler printerJobError;

    InvoiceStatusSaverSaveEventHandler invoiceStatusSaverSaveReprint;
    InvoiceStatusSaverSaveEventHandler invoiceStatusSaverSave;
    InvoiceStatusSaverErrorEventHandler invoiceStatusSaverErrorReprint;
    InvoiceStatusSaverErrorEventHandler invoiceStatusSaverError;

    ControllerCompleteEventHandler printControllerCompleteReprint;
    ControllerCompleteEventHandler printControllerComplete;

    private class Stat
    {
      public int unprinted = 0;
      public int printed = 0;
      public int failed = 0;
      public int locked = 0;
    }

    public MainForm()
    {
      InitializeComponent();

      config = new Config();

      labelService = new LabelService();
      labelService.Url = config.LabelServiceUrl;
      labelService.Timeout = 1000 * 60 * 30;
      invoiceStorage = new PrintPackageStorage(config, labelService);
      invoiceProvider = new InvoiceProvider(labelService);
      printer = new Printer(invoiceProvider, labelService);
      invoiceStatusSaver = new InvoiceStatusSaver(printer, labelService);
      printController = new PrintController(invoiceProvider, printer, invoiceStatusSaver, labelService);

      invoiceStorage.Update += new InvoiceStorageUpdateEventHandler(invoiceStorage_Update);
      invoiceStorage.UpdateSubset += new InvoiceStorageUpdateSubsetEventHandler(invoiceStorage_UpdateSubset);
      invoiceStorage.UpdatePackageState += new PrintPackageStorageUpdatePackageStateEventHandler(invoiceStorage_UpdateInvoiceState);

      unship = new Unship(labelService, config);
      unship.UpdateList += new PackageStorageUpdateListEventHandler(unship_UpdateList);

      unshipped = new Unshipped(labelService, config);
      unshipped.UpdateList += new PackageStorageUpdateListEventHandler(unshipped_UpdateList);
      unshipped.UpdateMaxDailyPackages += new UnshippedUpdateMaxDailyPackagesEventHandler(unshipped_UpdateMaxDailyPackages);

      reprint = new Reprint(config, labelService);
      reprint.UpdateList += new PackageStorageUpdateListEventHandler(reprint_UpdateList);
      reprint.UpdatePackageState += new PrintPackageStorageUpdatePackageStateEventHandler(reprint_UpdatePackageState);

      repair = new Repair(labelService, config);
      repair.UpdateList += new PackageStorageUpdateListEventHandler(repair_UpdateList);

      initPrinterCombo();

      printer.PrinterError += new PrinterPrinterErrorEventHandler(printer_PrinterError);

      bwPrinterErrorMonitor = new BackgroundWorker();
      bwPrinterErrorMonitor.WorkerSupportsCancellation = true;
      bwPrinterErrorMonitor.DoWork += new DoWorkEventHandler(bwPrinterErrorMonitor_DoWork);

      bwStopPrint = new BackgroundWorker();
      bwStopPrint.DoWork += new DoWorkEventHandler(bwStopPrint_DoWork);

      bwStopReprint = new BackgroundWorker();
      bwStopReprint.DoWork += new DoWorkEventHandler(bwStopReprint_DoWork);

      // event handlers
      invoiceProviderLoadReprint = new InvoiceProviderLoadEventHandler(invoiceProvider_LoadReprint);
      invoiceProviderLoad = new InvoiceProviderLoadEventHandler(invoiceProvider_Load);
      invoiceProviderErrorReprint = new InvoiceProviderErrorEventHandler(invoiceProvider_ErrorReprint);
      invoiceProviderError = new InvoiceProviderErrorEventHandler(invoiceProvider_Error);

      printerPrintReprint = new PrinterPrintEventHandler(printer_PrintReprint);
      printerPrint = new PrinterPrintEventHandler(printer_Print);
      printerJobErrorReprint -= new PrinterJobErrorEventHandler(printer_JobErrorReprint);
      printerJobError = new PrinterJobErrorEventHandler(printer_JobError);

      invoiceStatusSaverSaveReprint = new InvoiceStatusSaverSaveEventHandler(invoiceStatusSaver_SaveReprint);
      invoiceStatusSaverSave = new InvoiceStatusSaverSaveEventHandler(invoiceStatusSaver_Save);
      invoiceStatusSaverErrorReprint -= new InvoiceStatusSaverErrorEventHandler(invoiceStatusSaver_ErrorReprint);
      invoiceStatusSaverError = new InvoiceStatusSaverErrorEventHandler(invoiceStatusSaver_Error);

      printControllerCompleteReprint = new ControllerCompleteEventHandler(printController_CompleteReprint);
      printControllerComplete = new ControllerCompleteEventHandler(printController_Complete);


      // set up tabs for root queries
      IntPtr h = tcQueries.Handle; // bug? see http://social.msdn.microsoft.com/Forums/en-US/winforms/thread/5d10fd0c-1aa6-4092-922e-1fd7af979663

      for (int i = 0; i < config.QueryList.Count; i++)
      {
        TabPage tabPage;
        if (i == 0) // existing tab with controls
        {
          tabPage = tcQueries.TabPages[0];
        }
        else
        {
          tabPage = new TabPage();
          tcQueries.TabPages.Insert(i, tabPage);
        }

        tabPage.Text = config.QueryList[i].Title;
      }

      setQueryTab(0);
      controlsTabPage = tcQueries.TabPages[0];
      unshipTabIndex = tcQueries.TabPages.Count - 4;
      unshippedTabIndex = tcQueries.TabPages.Count - 3;
      reprintTabIndex = tcQueries.TabPages.Count - 2;
      repairTabIndex = tcQueries.TabPages.Count - 1;
      
      // colors
      Color yellow = System.Drawing.ColorTranslator.FromHtml("#FFFF00");
      Color olive = System.Drawing.ColorTranslator.FromHtml("#808000");
      Color green = System.Drawing.ColorTranslator.FromHtml("#008000");
      Color red = System.Drawing.ColorTranslator.FromHtml("#FF0000");
      Color gray = System.Drawing.ColorTranslator.FromHtml("#A0A0A0");

      // print/reprint cell styles
      loadedCellStyle.BackColor = yellow;
      printedCellStyle.BackColor = olive;
      statusSavedCellStyle.BackColor = green;
      statusSavedCellStyle.ForeColor = Color.White;
      errorCellStyle.BackColor = red;
      lockedCellStyle.BackColor = gray;

      updateUnshipStat();

      // unship cell styles
      unshipErrorCellStyle.BackColor = red;
      unshipUnshippedCellStyle.BackColor = green;

      // unshipped cell styles
      unshippedNewCellStyle.BackColor = Color.White;
      unshippedUnshippedCellStyle.BackColor = yellow;
      unshippedUpdatedPickableCellStyle.BackColor = green;
      unshippedUpdatedOnholdCellStyle.BackColor = gray;

      // repair cell styles
      repairOriginalCellStyle.BackColor = Color.White;
      repairRepairedCellStyle.BackColor = green;

      // first sell styles
      defaultFirstCellStyle.BackColor = Color.White;
      defaultFirstCellStyle.SelectionBackColor = Color.White;
      defaultFirstCellStyle.SelectionForeColor = Color.Black;

      errorFirstCellStyle.BackColor = errorCellStyle.BackColor;
      errorFirstCellStyle.SelectionBackColor = errorCellStyle.BackColor;

      loadedFirstCellStyle.BackColor = loadedCellStyle.BackColor;
      loadedFirstCellStyle.SelectionBackColor = loadedCellStyle.BackColor;
      loadedFirstCellStyle.SelectionForeColor = Color.Black;

      printedFirstCellStyle.BackColor = printedCellStyle.BackColor;
      printedFirstCellStyle.SelectionBackColor = printedCellStyle.BackColor;

      statusSavedFirstCellStyle.BackColor = statusSavedCellStyle.BackColor;
      statusSavedFirstCellStyle.ForeColor = Color.White;
      statusSavedFirstCellStyle.SelectionBackColor = statusSavedCellStyle.BackColor;
      statusSavedFirstCellStyle.SelectionForeColor = Color.White;

      lockedFirstCellStyle.BackColor = lockedCellStyle.BackColor;
      lockedFirstCellStyle.SelectionBackColor = lockedCellStyle.BackColor;

      unshipErrorFirstCellStyle.BackColor = unshipErrorCellStyle.BackColor;
      unshipErrorFirstCellStyle.SelectionBackColor = unshipErrorCellStyle.BackColor;

      unshipUnshippedFirstCellStyle.BackColor = unshipUnshippedCellStyle.BackColor;
      unshipUnshippedFirstCellStyle.SelectionBackColor = unshipUnshippedCellStyle.BackColor;

      unshippedNewFirstCellStyle.BackColor = unshippedNewCellStyle.BackColor;
      unshippedNewFirstCellStyle.SelectionBackColor = unshippedNewCellStyle.BackColor;
      unshippedNewFirstCellStyle.SelectionForeColor = Color.Black;

      unshippedUnshippedFirstCellStyle.BackColor = unshippedUnshippedCellStyle.BackColor;
      unshippedUnshippedFirstCellStyle.SelectionBackColor = unshippedUnshippedCellStyle.BackColor;
      unshippedUnshippedFirstCellStyle.SelectionForeColor = Color.Black;

      unshippedUpdatedPickableFirstCellStyle.BackColor = unshippedUpdatedPickableCellStyle.BackColor;
      unshippedUpdatedPickableFirstCellStyle.SelectionBackColor = unshippedUpdatedPickableCellStyle.BackColor;

      unshippedUpdatedOnholdFirstCellStyle.BackColor = unshippedUpdatedOnholdCellStyle.BackColor;
      unshippedUpdatedOnholdFirstCellStyle.SelectionBackColor = unshippedUpdatedOnholdCellStyle.BackColor;

      repairOriginalFirstCellStyle.BackColor = repairOriginalCellStyle.BackColor;
      repairOriginalFirstCellStyle.SelectionBackColor = repairOriginalCellStyle.BackColor;
      repairOriginalFirstCellStyle.SelectionForeColor = Color.Black;

      repairRepairedFirstCellStyle.BackColor = repairRepairedCellStyle.BackColor;
      repairRepairedFirstCellStyle.SelectionBackColor = repairRepairedCellStyle.BackColor;


      tbHelp.Text = Properties.Settings.Default.HelpText;
      tbHelpUnship.Text = Properties.Settings.Default.UnshipHelpText;
      tbHelpUnshipped.Text = Properties.Settings.Default.UnshippedHelpText;
      tbHelpRepair.Text = Properties.Settings.Default.RepairHelpText;
      tbHelpReprint.Text = Properties.Settings.Default.ReprintHelpText;

      Log.getLogger().Info("Start application");

    }

    void bwStopReprint_DoWork(object sender, DoWorkEventArgs e)
    {
      printController.stop();

      bwPrinterErrorMonitor.CancelAsync();
      printerErrorMonitorResetEvent.WaitOne();

      // unlock invoices
      List<int> idList = new List<int>();
      for (int i = 0; i < dgvReprint.Rows.Count; i++)
      {
        PrintPackageWrapper package = (dgvReprint.Rows[i].Tag as PrintPackageWrapper);
        if (package.IsLoaded && !package.IsPrinted)
        {
          idList.Add(package.PackageId);
        }
      }
      if (idList.Count > 0)
      {
        reprint.unlock(idList);
      }

      setReprintControlsEnabled(true);

      setControlText(btReprintPrint, "Print");
      setControlEnabled(btReprintPrint, true);

      if (printerError)
      {
       showMessageBox(
         this,
         "Printer return error status (" + printerErrorMessage + ").\nProcess is stopped.",
         "Error",
         MessageBoxButtons.OK,
         MessageBoxIcon.Error
       );
      }
      //findStartIndex();
    }

    void bwStopPrint_DoWork(object sender, DoWorkEventArgs e)
    {
      printController.stop();

      bwPrinterErrorMonitor.CancelAsync();
      printerErrorMonitorResetEvent.WaitOne();

      // unlock invoices
      List<int> idList = new List<int>();
      for (int i = 0; i < dgvSubset.Rows.Count; i++)
      {
        PrintPackageWrapper package = dgvSubset.Rows[i].Tag as PrintPackageWrapper;
        if (package.IsLoaded && !package.IsPrinted)
        {
          idList.Add(package.PackageId);
        }
      }
      if (idList.Count > 0)
      {
        invoiceStorage.unlock(idList);
      }

      setPrintControlsEnabled(true);

      setControlText(btPrint, "Print");
      setControlEnabled(btPrint, true);

      if (printerError)
      {
        showMessageBox(
          this,
          "Printer return error status (" + printerErrorMessage + ").\nProcess is stopped.",
          "Error",
          MessageBoxButtons.OK,
          MessageBoxIcon.Error
        );
      }

      findStartIndex();
    }


    delegate void ProcessPrinterErrorCallback();

    private void processPrinterError()
    {
      btPrint.Enabled = false;

      if (tcQueries.SelectedIndex == reprintTabIndex)
      {
        bwStopReprint.RunWorkerAsync();
      }
      else
      {
        bwStopPrint.RunWorkerAsync();
      }
    }

    void bwPrinterErrorMonitor_DoWork(object sender, DoWorkEventArgs e)
    {
      BackgroundWorker bw = sender as BackgroundWorker;

      for (; ; )
      {
        if (printerError)
        {
          Invoke(new ProcessPrinterErrorCallback(processPrinterError));
          break;
        }

        if (bw.CancellationPending)
        {
          e.Cancel = true;
          break;
        }

        Thread.Sleep(200);
      }

      printerErrorMonitorResetEvent.Set();
    }

    void printer_PrinterError(object sender, PrinterPrinterErrorEventArgs e)
    {
      printerError = true;
      printerErrorMessage = e.Message;
    }

    void printController_Complete(object sender, EventArgs e)
    {
      bwPrinterErrorMonitor.CancelAsync();
      printerErrorMonitorResetEvent.WaitOne();
      setControlText(btPrint, "Print");
      setPrintControlsEnabled(true);
      showMessageBox(
           this,
           "Print job complete.",
           "Message",
           MessageBoxButtons.OK,
           MessageBoxIcon.Information
           );
    }

    delegate void SetControlTextCallback(Control control, string text);

    public void setControlText(Control control, string text)
    {
      if (control.InvokeRequired)
      {
        SetControlTextCallback d = new SetControlTextCallback(setControlText);
        this.Invoke(d, new object[] { control, text });
      }
      else
      {
        control.Text = text;
      }
    }

    delegate void SetControlEnabledCallback(Control control, bool enabled);

    public void setControlEnabled(Control control, bool aEnabled)
    {
      if (control.InvokeRequired)
      {
        SetControlEnabledCallback d = new SetControlEnabledCallback(setControlEnabled);
        this.Invoke(d, new object[] { control, aEnabled });
      }
      else
      {
        control.Enabled = aEnabled;
      }
    }


    delegate void ShowMessageBoxCallback(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon);

    public void showMessageBox(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
    {
      if (this.InvokeRequired)
      {
        ShowMessageBoxCallback d = new ShowMessageBoxCallback(showMessageBox);
        this.Invoke(d, new object[] { owner, text, caption, buttons, icon });
      }
      else
      {
        MessageBox.Show(
         this,
         text,
         caption,
         buttons,
         icon
         );
      }
    }

    void invoiceStatusSaver_Error(object sender, InvoiceStatusSaverErrorEventArgs e)
    {
      invoiceStorage.setPackageState(e.InvoiceId, PrintPackageWrapper.ERROR, e.Message);
    }

    void invoiceStatusSaver_Save(object sender, InvoiceStatusSaverSaveEventArgs e)
    {
      invoiceStorage.setPackageState(e.InvoiceId, PrintPackageWrapper.STATUS_SAVED, "");
    }

    void invoiceProvider_Load(object sender, InvoiceProviderLoadEventArgs e)
    {
      invoiceStorage.setPackageState(e.PrintInvoiceWrapper.PackageId, e.PrintInvoiceWrapper.IsLoaded ? PrintPackageWrapper.LOADED : PrintPackageWrapper.LOCKED, "");
    }

    void printer_JobError(object sender, PrinterJobErrorEventArgs e)
    {
      invoiceStorage.setPackageState(e.PackageId, PrintPackageWrapper.ERROR, e.Message);
    }

    void printer_Print(object sender, PrinterPrintEventArgs e)
    {
      invoiceStorage.setPackageState(e.PackageId, PrintPackageWrapper.PRINTED, "");
    }

    void invoiceStorage_UpdateInvoiceState(object sender, PrintPackageStorageUpdatePackageStateEventArgs e)
    {
      PrintPackageWrapper invoice = invoiceStorage.getPackageByPackageId(e.PackageId);
      setRowStyle(rowIndex[e.PackageId], invoice);
      setRowStyle(subsetRowIndex[e.PackageId], invoice);
      updateStat();
      updateSubsetStat();
    }

    void invoiceProvider_Error(object sender, InvoiceProviderErrorEventArgs e)
    {
      invoiceStorage.setPackageState(e.InvoiceId, PrintPackageWrapper.ERROR, e.Message);
    }

    delegate void UpdateSubsetCallabck(object sender, EventArgs e);

    void invoiceStorage_UpdateSubset(object sender, EventArgs e)
    {
      if (InvokeRequired)
      {
        UpdateSubsetCallabck d = new UpdateSubsetCallabck(invoiceStorage_UpdateSubset);
        this.Invoke(d, new object[] { sender, e });
      }
      else
      {

        dgvSubset.RowCount = invoiceStorage.SubsetPackageList.Count;
        subsetRowIndex.Clear();
        if (dgvSubset.RowCount > 0)
        {
          dgvSubset.ColumnCount = invoiceStorage.SubsetPackageList[0].FieldValueList.Length;
          if (dgvSubset.ColumnCount > 0)
          {
            dgvSubset.CurrentCell = dgvSubset.Rows[0].Cells[0];
          }

          for (int row = 0; row < invoiceStorage.SubsetPackageList.Count; row++)
          {
            for (int col = 0; col < dgvSubset.ColumnCount; col++)
            {
              dgvSubset.Rows[row].Cells[col].Value = invoiceStorage.getTypedFieldValue(invoiceStorage.SubsetPackageList[row].FieldValueList[col], col);
              dgvSubset.Rows[row].Cells[col].Style = dgvSubset.DefaultCellStyle;
              dgvSubset.Rows[row].Tag = invoiceStorage.SubsetPackageList[row];
              subsetRowIndex[invoiceStorage.SubsetPackageList[row].PackageId] = dgvSubset.Rows[row];
              setRowStyle(dgvSubset.Rows[row], invoiceStorage.SubsetPackageList[row]);
            }
          }
        }

        btPrint.Enabled = dgvSubset.Rows.Count > 0;
        tsslSubsetTotal.Text = String.Format("Total: {0}", dgvSubset.RowCount.ToString());
        updateSubsetStat();
        chkPrintSequenceNumber.Checked = true;
      }
    }

    delegate void UpdateSetDelegate(object sender, EventArgs e);

    void invoiceStorage_Update(object sender, EventArgs e)
    {
      if (this.InvokeRequired)
      {
        UpdateSetDelegate d = new UpdateSetDelegate(invoiceStorage_Update);
        this.Invoke(d, new object[] { sender, e });
      }
      else
      {

        // field names
        string[] fieldNames = invoiceStorage.FieldNames;
        dgvQuery.ColumnCount = fieldNames.Length;
        dgvSubset.ColumnCount = fieldNames.Length;
        for (int i = 0; i < fieldNames.Length; i++)
        {
          dgvQuery.Columns[i].HeaderText = fieldNames[i];
          dgvQuery.Columns[i].SortMode = DataGridViewColumnSortMode.Automatic;
          dgvSubset.Columns[i].HeaderText = fieldNames[i];
          dgvSubset.Columns[i].SortMode = DataGridViewColumnSortMode.Automatic;
        }

        // copy to clipboard context menu values
        fillCopyToClipboardMenuItem(miSubsetCopyToClipboard, dgvSubset);
        fillCopyToClipboardMenuItem(miSetCopyToClipboard, dgvQuery);

        dgvQuery.RowCount = invoiceStorage.PackageList.Count;
        rowIndex.Clear();
        if (dgvQuery.RowCount > 0)
        {
          for (int row = 0; row < invoiceStorage.PackageList.Count; row++)
          {
            for (int col = 0; col < dgvQuery.ColumnCount; col++)
            {
              dgvQuery.Rows[row].Cells[col].Value = invoiceStorage.getTypedFieldValue(row, col);
              dgvQuery.Rows[row].Cells[col].Style = dgvQuery.DefaultCellStyle;
              dgvQuery.Rows[row].Tag = invoiceStorage.PackageList[row];
              rowIndex[invoiceStorage.PackageList[row].PackageId] = dgvQuery.Rows[row];
              setRowStyle(dgvQuery.Rows[row], invoiceStorage.PackageList[row]);
            }
          }
        }
        resetFilter();
        tsslSetTotal.Text = String.Format("Total: {0}", dgvQuery.RowCount.ToString());
        updateStat();
      }
    }

    private void fillCopyToClipboardMenuItem(ToolStripMenuItem aMenuItem, DataGridView aDgv)
    {
      aMenuItem.Text = "Copy Cell Value To Clipboard";
      aMenuItem.DropDownItems.Clear();
      for (int i = 0; i < aDgv.ColumnCount; i++)
      {
        ToolStripItem item = aMenuItem.DropDownItems.Add(aDgv.Columns[i].HeaderText);
        item.Tag = new object[] { i, aDgv };
        item.Click += new EventHandler(miCopyToClipboard_Click);
      }
    }

    void miCopyToClipboard_Click(object sender, EventArgs e)
    {
      ToolStripItem item = sender as ToolStripItem;
      object[] data = (object[])item.Tag;
      int fieldIndex = (int)data[0];
      DataGridView dgv = (DataGridView)data[1];
      if (dgv.CurrentRow != null && dgv.CurrentRow.Cells[fieldIndex].Value != null)
      {
        try
        {
          Clipboard.SetText(dgv.CurrentRow.Cells[fieldIndex].Value.ToString());
        }
        catch (Exception) { }
      }
    }

    private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
    {
      Cursor.Current = Cursors.WaitCursor;

      TabControl tc = sender as TabControl;

      if(tc.SelectedIndex == unshipTabIndex)
      { 
      }
      else
      {
        if(tc.SelectedIndex == unshippedTabIndex)
        {
          if (!unshipped.IsLoaded)
          {
            unshipped.load();
            updateUnshippedFilter();
            updateUnshippedStat();
          }
        }
        else
        {
          if(tc.SelectedIndex == reprintTabIndex)
          {
            updateReprintFilter();
            updateReprintStat();
          }
          else
          {
            if (tc.SelectedIndex == repairTabIndex)
            {
              if (!repair.IsLoaded)
              {
                repair.load();
                updateRepairFilter();
                updateRepairStat();
              }
            }
            else
            {
              if (controlsTabPage == e.TabPage)
              {
                setQueryTab(e.TabPageIndex);
              }
              else
              {
                // copy all controls from previously selected tab to newly selected

                e.TabPage.Controls.Clear();
                while (controlsTabPage.Controls.Count > 0)
                {
                  e.TabPage.Controls.Add(controlsTabPage.Controls[0]);
                };
                e.TabPage.Padding = controlsTabPage.Padding;
                e.TabPage.UseVisualStyleBackColor = controlsTabPage.UseVisualStyleBackColor;
                controlsTabPage.Controls.Clear();

                setQueryTab(e.TabPageIndex);
              }
            }
          }
        }
      }

      Cursor.Current = Cursors.Default;

    }

    private void tabControl1_Deselected(object sender, TabControlEventArgs e)
    {
      // store deselecting tab for using in tabControl1_Selecting function
      TabControl tc = sender as TabControl;
      if (tc.TabPages.IndexOf(e.TabPage) != unshipTabIndex &&
        tc.TabPages.IndexOf(e.TabPage) != unshippedTabIndex &&
        tc.TabPages.IndexOf(e.TabPage) != reprintTabIndex &&
        tc.TabPages.IndexOf(e.TabPage) != repairTabIndex)
      {
        controlsTabPage = e.TabPage;
      }
    }

    // fills sobqueries control (combobox)
    private void fillSubqueriesControl(int queryIndex)
    {
      cbSubset.Items.Clear();
      cbSubset.Items.Add("All");
      cbSubset.Items.Add("Custom");

      foreach (Query subquery in config.QueryList[queryIndex].SubqueryList)
      {
        cbSubset.Items.Add(subquery.Title);
      }

      cbSubset.SelectedIndex = 0;
    }

    private void setQueryTab(int queryIndex)
    {
      fillSubqueriesControl(queryIndex);

      invoiceStorage.setQuery(queryIndex);

      if (cbSubset.SelectedIndex != 0)
      {
        cbSubset.SelectedIndex = 0; // hidden calls onUpdateSubset event
      }
      else 
      {
        // direct call onUpdateSubset event
        invoiceStorage.setSubsetAll();
      }
    }

    private void cbSubqueries_SelectedIndexChanged(object sender, EventArgs e)
    {
      switch (cbSubset.SelectedIndex)
      {
        case 0: // all
          invoiceStorage.setSubsetAll();
          break;

        case 1: // custom
          invoiceStorage.setSubsetCustom();
          break;

        default:
          invoiceStorage.setSubset(cbSubset.SelectedIndex - 2);
          break;
      }

      removeSortGlyph(dgvSubset);
    }

    private void removeSortGlyph(DataGridView aDataGridView) {
      if (aDataGridView.SortedColumn != null) {
        aDataGridView.SortedColumn.HeaderCell.SortGlyphDirection = SortOrder.None;
      }
    }

    private void resetFilter()
    {
      cbQueryShowUnprinted.Checked = true;
      cbQueryShowPrinted.Checked = true;
      cbQueryShowError.Checked = true;
      cbQueryShowLocked.Checked = true;
    }

    private void updateFilter()
    {
      foreach (DataGridViewRow row in dgvQuery.Rows)
      {
        bool visible = false;

        PrintPackageWrapper invoiceWrapper = row.Tag as PrintPackageWrapper;
        
        if (cbQueryShowUnprinted.Checked && invoiceWrapper.IsUnprinted)
        {
          visible = true;
        }
        
        if (cbQueryShowPrinted.Checked && invoiceWrapper.IsPrinted)
        {
          visible = true;
        }
        
        if (cbQueryShowError.Checked && invoiceWrapper.IsError)
        {
          visible = true;
        }

        if (cbQueryShowLocked.Checked && invoiceWrapper.IsLocked)
        {
          visible = true;
        }

        row.Visible = visible;
      }
    }


    private void cbQueryShowUnprinted_CheckedChanged(object sender, EventArgs e)
    {
      updateFilter();
    }

    private void cbQueryShowPrinted_CheckedChanged(object sender, EventArgs e)
    {
      updateFilter();
    }

    private void cbQueryShowError_CheckedChanged(object sender, EventArgs e)
    {
      updateFilter();
    }

    private void cbQueryShowLocked_CheckedChanged(object sender, EventArgs e)
    {
      updateFilter();
    }

    private void initPrinterCombo()
    {
      string defaultPrinterName = "";// (string)config.getValue("printerName", new PrintDocument().PrinterSettings.PrinterName);
      foreach (String printerName in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
      {
        cbPrinter.Items.Add(printerName);
        cbReprintPrinter.Items.Add(printerName);
        if (printerName == defaultPrinterName)
        {
          cbPrinter.SelectedIndex = cbPrinter.Items.IndexOf(printerName);
          cbReprintPrinter.SelectedIndex = cbReprintPrinter.Items.IndexOf(printerName);
        }
      }

      if (cbPrinter.SelectedIndex < 0 && cbPrinter.Items.Count > 0)
        cbPrinter.SelectedIndex = 0;

      if (cbReprintPrinter.SelectedIndex < 0 && cbReprintPrinter.Items.Count > 0)
        cbReprintPrinter.SelectedIndex = 0;
    }

    private void btPrint_Click(object sender, EventArgs e)
    {
      if (printController.State == PrintControllerState.RUNNING)
      {
        btPrint.Enabled = false;
        bwStopPrint.RunWorkerAsync();
      }
      else
      {
        // Pick List and sequence number check
        /* neither var exists anymore
          if(chkPrintPickList.Checked && !chkPrintSequenceNumber.Checked) {
          MessageBox.Show(
            "To print Pick List sequence number printing must be enabled!",
            "Error",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error
          );
          return;
        }
        */
        // warning about sequence number
        if (!chkPrintSequenceNumber.Checked && MessageBox.Show(
          "Invoice sequence number printing is " + (chkPrintSequenceNumber.Checked ? "enabled" : "disabled") + ".\nContinue printing?",
          "Warning",
          MessageBoxButtons.YesNo,
          MessageBoxIcon.Question
        ) == DialogResult.No)
        {
          return;
        }
        else
        { 
          // sequence number printing enabled with custom sorting
          /*
          if (chkPrintSequenceNumber.Checked && dgvSubset.SortedColumn != null)
          {
            if (MessageBox.Show(
              "Invoice sequence number printing is enabled with custom sorting.\nContinue printing?",
              "Warning",
              MessageBoxButtons.YesNo,
              MessageBoxIcon.Question
            ) == DialogResult.No)
            {
              return;
            }
          }
          */
        }

        // warning about pick list
        /*
          if (chkPrintPickList.Checked) {
         MessageBox.Show(
          "Pick List printing is CHECKED. Printing of this type is currently disabled so nothing will be printed.",
          "Warning",
          MessageBoxButtons.YesNo,
          MessageBoxIcon.Question
         );
         return;
        }
         */

        var q = "You are going to print full invoices (with postage label).\nContinue?";
        
        if (MessageBox.Show(
          q,
          "Confirmation",
          MessageBoxButtons.YesNo,
          MessageBoxIcon.Question
        ) == DialogResult.No)
        {
          return;
        }


        if (dgvSubset.CurrentRow.Index == 0 || (dgvSubset.CurrentRow.Index > 0 && MessageBox.Show(
          "You are going to print out of sequence (not from the beginning of this list). Continue?",
          "Confirmation",
          MessageBoxButtons.YesNo,
          MessageBoxIcon.Question
          ) == DialogResult.Yes))
        {
          btPrint.Enabled = false;
          List<PrintPackageWrapper> invoiceList = new List<PrintPackageWrapper>();
          bool askRepeatedPrint = true;
          bool askRepeatedPrintResult = true;
          for (int i = dgvSubset.CurrentRow.Index; i < dgvSubset.Rows.Count; i++)
          {
            if (dgvSubset.Rows[i].Visible)
            {
              bool add = true;
              PrintPackageWrapper package = dgvSubset.Rows[i].Tag as PrintPackageWrapper;
              if (package.IsPrinted)
              {
                if (askRepeatedPrint)
                {
                  RepeatedPrintForm fmRepeatedPrint = new RepeatedPrintForm();
                  fmRepeatedPrint.laMessage.Text = String.Format("You are going to print invoice No. {0} which is already marked as printed. Do you want to print it again?", dgvSubset.Rows[i].Cells[PrintPackageStorage.INVOICE_NUMBER_COLUMN_INDEX]);
                  askRepeatedPrintResult = fmRepeatedPrint.ShowDialog() == DialogResult.Yes ? true : false;
                  askRepeatedPrint = !fmRepeatedPrint.ckDontAsk.Checked;
                }
                add = askRepeatedPrintResult;
              }

              if (add)
              {
                  package.isPackJacket = false; // chkPackJacket.Checked;
                invoiceList.Add(package);
              }
            }
          }

          startPrintJob(invoiceList, cbPrinter.SelectedItem.ToString(), false, chkPrintSequenceNumber.Checked, false, false);

          setPrintControlsEnabled(false);

          btPrint.Text = "Stop";
          btPrint.Enabled = true;
        }
      }
    }

    delegate void SetRowStyleCallback(DataGridViewRow row, PrintPackageWrapper package);

    private void setRowStyle(DataGridViewRow row, PrintPackageWrapper invoice)
    {
      if (row.DataGridView.InvokeRequired)
      {
        SetRowStyleCallback d = new SetRowStyleCallback(setRowStyle);
        this.Invoke(d, new object[] { row, invoice });
      }
      else
      {
        DataGridViewCellStyle style = null;
        DataGridViewCellStyle firstCellStyle = defaultFirstCellStyle;

        // unprinted
        if (invoice.IsUnprinted)
        {
          style = row.DefaultCellStyle;
        }

        // loaded
        if (invoice.IsLoaded)
        {
          style = loadedCellStyle;
          firstCellStyle = loadedFirstCellStyle;
        }

        // printed
        if (invoice.IsPrinted)
        {
          style = printedCellStyle;
          firstCellStyle = printedFirstCellStyle;
          if (printController.State == PrintControllerState.RUNNING)
          {
            //row.DataGridView.FirstDisplayedScrollingRowIndex = row.Index;
            row.DataGridView.CurrentCell = row.Cells[0];
          }
        }

        // status saved
        if (invoice.IsStatusSaved)
        {
          style = statusSavedCellStyle;
          firstCellStyle = statusSavedFirstCellStyle;
        }

        // error
        if (invoice.IsError)
        {
          row.ErrorText = invoice.ErrorText;
          style = errorCellStyle;
          firstCellStyle = errorFirstCellStyle;
        }
        else
        {
          row.ErrorText = "";
        }

        // locked
        if (invoice.IsLocked)
        {
          style = lockedCellStyle;
          firstCellStyle = lockedFirstCellStyle;
        }

        if (style != null)
        {
          foreach (DataGridViewCell cell in row.Cells)
          {
            cell.Style = style;
          }

          row.Cells[0].Style = firstCellStyle;
        }
      }
    }

    private void addToCustomSubsetToolStripMenuItem_Click(object sender, EventArgs e)
    {
      List<int> list = new List<int>();
      foreach (DataGridViewRow row in dgvQuery.SelectedRows)
      {
        list.Add((row.Tag as PrintPackageWrapper).PackageId);
      }
      list.Reverse(); // selected list in reverse order
      invoiceStorage.addToCustomSubset(list);

      cbSubset.SelectedIndex = 1; // subset
    }

    private void cmsSubset_Opening(object sender, CancelEventArgs e)
    {
      if (cbSubset.SelectedIndex != 1 
          || dgvSubset.SelectedRows.Count == 0
          || printController.State == PrintControllerState.RUNNING)
        miRemoveFromSubset.Enabled = false;
      else
        miRemoveFromSubset.Enabled = true;

      if (dgvSubset.SelectedRows.Count == 0)
      {
        miPreviewSubsetInvoice.Enabled = false;
        miSubsetAddToUnship.Enabled = false;
      }
      else
      {
        miPreviewSubsetInvoice.Enabled = true;
        miSubsetAddToUnship.Enabled = true;
      }

    }

    private void miRemoveFromSubset_Click(object sender, EventArgs e)
    {
      List<int> list = new List<int>();
      foreach (DataGridViewRow row in dgvSubset.SelectedRows)
      {
        list.Add((row.Tag as PrintPackageWrapper).PackageId);
      }
      invoiceStorage.removeFromCustomSubset(list);
    }

    private void cmsSet_Opening(object sender, CancelEventArgs e)
    {
      if (dgvQuery.SelectedRows.Count == 0)
        miPreviewSetInvoice.Enabled = false;
      else
        miPreviewSetInvoice.Enabled = true;

      if (dgvQuery.SelectedRows.Count == 0
          || printController.State == PrintControllerState.RUNNING)
        addToCustomSubsetToolStripMenuItem.Enabled = false;
      else
        addToCustomSubsetToolStripMenuItem.Enabled = true;
    }

    private void previewInvoice(int aInvoiceId, string aSequenceNumber, bool aIsPackJacket)
    {
      try
      {
        GetLabelRequestType request = new GetLabelRequestType();
        request.packageId = aInvoiceId;
        request.@lock = false;
        request.isPackJacket = aIsPackJacket;
        GetLabelResponseType response = labelService.getLabel(request);

        if (response.status != 0)
        {
          //invoiceStorage.setPackageError(aInvoiceId, "Error loading invoice: " + response.message);
          throw new Exception(response.message);
        }

        string path = Application.StartupPath + "\\" + "preview.pdf";

        byte[] pdf = Convert.FromBase64String(response.base64data);

        if (aSequenceNumber != null) {
          Routines.addSequenceNumberToPdf(aSequenceNumber, ref pdf, aIsPackJacket);
        }

        System.IO.File.WriteAllBytes(path, pdf);
        System.Diagnostics.Process.Start(path);
      }
      catch (Exception e)
      {
        MessageBox.Show(
          this,
          "Error loading invoice: " + e.Message,
          "Error",
          MessageBoxButtons.OK,
          MessageBoxIcon.Error
          );
      }
    }

    private void miPreviewSetInvoice_Click(object sender, EventArgs e)
    {
      previewInvoice((dgvQuery.CurrentRow.Tag as PrintPackageWrapper).PackageId, null, false);
    }

    private void miPreviewSubsetInvoice_Click(object sender, EventArgs e)
    {
      previewInvoice((dgvSubset.CurrentRow.Tag as PrintPackageWrapper).PackageId, null, false);
    }

    private void miFindInvoice_Click(object sender, EventArgs e)
    {
      if (fmFindInvoice == null)
      {
        fmFindInvoice = new FindInvoiceForm(this, config);
      }

      if (!fmFindInvoice.Visible)
      {
        fmFindInvoice.Show(this);
      }
    }

    public bool setCurrentSubsetInvoice(string findValue, int colIndex, bool next)
    {
      bool result = false;
      foreach (DataGridViewRow row in dgvSubset.Rows)
      {
        if (row.Visible && row.Cells[colIndex].Value.ToString() == findValue)
        {
          result = true;
          dgvSubset.CurrentCell = row.Cells[colIndex];
          break;
        }
      }

      if (result && next) // need next row
      {
        result = false;
        for (int i = dgvSubset.CurrentRow.Index + 1; i < dgvSubset.Rows.Count; i++)
        {
          if (dgvSubset.Rows[i].Visible)
          {
            dgvSubset.CurrentCell = dgvSubset.Rows[i].Cells[colIndex];
            result = true;
            break;
          }
        }
      }

      return result;
    }

    public bool setCurrentSetInvoice(string findValue, int colIndex)
    {
      bool result = false;
      foreach (DataGridViewRow row in dgvQuery.Rows)
      {
        if (row.Visible && row.Cells[colIndex].Value.ToString() == findValue)
        {
          result = true;
          dgvQuery.CurrentCell = row.Cells[colIndex];
          break;
        }
      }
      return result;
    }

    private void miFindStartIndex_Click(object sender, EventArgs e)
    {
      findStartIndex();
    }


    delegate void FindStartIndexCallback();

    private void findStartIndex()
    {
      if (this.InvokeRequired)
      {
        FindStartIndexCallback d = new FindStartIndexCallback(findStartIndex);
        this.Invoke(d);
      }
      else
      {
        if (fmFindStartIndex == null)
        {
          fmFindStartIndex = new FindStartIndexForm(this, config);
        }

        fmFindStartIndex.ShowDialog(this);
      }
    }

    private void btExportErrors_Click(object sender, EventArgs e)
    {
      List<string> errorList = new List<string>();
      foreach (DataGridViewRow row in dgvSubset.Rows)
      {
        PrintPackageWrapper package = row.Tag as PrintPackageWrapper;
        if (package.IsError)
        {
          errorList.Add(String.Format("Package: {0}. Error: {1}", package.PackageId, package.ErrorText));
        }
      }
      saveErrors(errorList);
    }

    private void saveErrors(List<string> aList)
    {
      if (aList.Count == 0)
      {
        MessageBox.Show(
          this,
          "There are no errors to export.",
          "Information",
          MessageBoxButtons.OK,
          MessageBoxIcon.Information
          );
      }
      else
      {
        if (sfdExportErrors.ShowDialog() == DialogResult.OK)
        {
          StreamWriter sw = new StreamWriter(sfdExportErrors.FileName);
          sw.WriteLine(String.Format("Date: {0}", DateTime.Now));
          foreach (string error in aList)
          {
            sw.WriteLine(error);
          }
          sw.Close();
          System.Diagnostics.Process.Start(sfdExportErrors.FileName);
        }
      }
    }

    private Stat getStat(DataGridView dgv)
    {
      Stat stat = new Stat();
      foreach (DataGridViewRow row in dgv.Rows)
      {
        PrintPackageWrapper invoiceWrapper = row.Tag as PrintPackageWrapper;

        if (invoiceWrapper.IsUnprinted)
        {
          stat.unprinted++;
        }
        
        if (invoiceWrapper.IsPrinted)
        {
          stat.printed++;
        }
        
        if (invoiceWrapper.IsError)
        {
          stat.failed++;
        }
        
        if (invoiceWrapper.IsLocked)
        {
          stat.locked++;
        }
      }

      return stat;
    }

    private void updateStat()
    {
      Stat stat;

      stat = getStat(dgvQuery);
      tsslSetUnprinted.Text = String.Format("Unprinted: {0}", stat.unprinted);
      tsslSetPrinted.Text = String.Format("Printed: {0}", stat.printed);
      tsslSetFailed.Text = String.Format("Failed: {0}", stat.failed);
      tsslSetLocked.Text = String.Format("Locked: {0}", stat.locked);
    }

    private void updateSubsetStat()
    {
      Stat stat;

      stat = getStat(dgvSubset);
      tsslSubsetUnprinted.Text = String.Format("Unprinted: {0}", stat.unprinted);
      tsslSubsetPrinted.Text = String.Format("Printed: {0}", stat.printed);
      tsslSubsetFailed.Text = String.Format("Failed: {0}", stat.failed);
      tsslSubsetLocked.Text = String.Format("Locked: {0}", stat.locked);
    }

    private void miFileExit_Click(object sender, EventArgs e)
    {
      Close();
    }

    // set event handlers for main print process
    private void setPrintEventHandlers()
    {
      // ensure that only one event handler enabled - first remove then add new one

      invoiceProvider.Load -= invoiceProviderLoadReprint;
      invoiceProvider.Load -= invoiceProviderLoad;
      invoiceProvider.Load += invoiceProviderLoad;
      invoiceProvider.Error -= invoiceProviderErrorReprint;
      invoiceProvider.Error -= invoiceProviderError;
      invoiceProvider.Error += invoiceProviderError;

      printer.Print -= printerPrintReprint;
      printer.Print -= printerPrint;
      printer.Print += printerPrint;
      printer.JobError -= printerJobErrorReprint;
      printer.JobError -= printerJobError;
      printer.JobError += printerJobError;

      invoiceStatusSaver.Save -= invoiceStatusSaverSaveReprint;
      invoiceStatusSaver.Save -= invoiceStatusSaverSave;
      invoiceStatusSaver.Save += invoiceStatusSaverSave;
      invoiceStatusSaver.Error -= invoiceStatusSaverErrorReprint;
      invoiceStatusSaver.Error -= invoiceStatusSaverError;
      invoiceStatusSaver.Error += invoiceStatusSaverError;

      printController.Complete -= printControllerCompleteReprint;
      printController.Complete -= printControllerComplete;
      printController.Complete += printControllerComplete;
    }

    // set event handlers for reprint process
    private void setReprintEventHandlers()
    {
      // ensure that only one event handler enabled - first remove then add new one

      invoiceProvider.Load -= invoiceProviderLoad;
      invoiceProvider.Load -= invoiceProviderLoadReprint;
      invoiceProvider.Load += invoiceProviderLoadReprint;
      invoiceProvider.Error -= invoiceProviderError;
      invoiceProvider.Error -= invoiceProviderErrorReprint;
      invoiceProvider.Error += invoiceProviderErrorReprint;

      printer.Print -= printerPrint;
      printer.Print -= printerPrintReprint;
      printer.Print += printerPrintReprint;
      printer.JobError -= printerJobError;
      printer.JobError -= printerJobErrorReprint;
      printer.JobError += printerJobErrorReprint;

      invoiceStatusSaver.Save -= invoiceStatusSaverSave;
      invoiceStatusSaver.Save -= invoiceStatusSaverSaveReprint;
      invoiceStatusSaver.Save += invoiceStatusSaverSaveReprint;
      invoiceStatusSaver.Error -= invoiceStatusSaverError;
      invoiceStatusSaver.Error -= invoiceStatusSaverErrorReprint;
      invoiceStatusSaver.Error += invoiceStatusSaverErrorReprint;

      printController.Complete -= printControllerComplete;
      printController.Complete -= printControllerCompleteReprint;
      printController.Complete += printControllerCompleteReprint;
    }

    private void startPrintJob(List<PrintPackageWrapper> aInvoiceList, string aPrinterName, bool aIsReprint, bool aIsSequenceNumberEnabled, bool aIsPackJacket, bool aPrintPickList)
    {
      if (aIsReprint)
      {
        setReprintEventHandlers();
        invoiceStatusSaver.IsReprint = true;
        invoiceProvider.IsReprint = true;
        invoiceProvider.LockPackages = true;
      }
      else 
      {
        setPrintEventHandlers();
        invoiceStatusSaver.IsReprint = false;
        invoiceProvider.IsReprint = false;
        invoiceProvider.LockPackages = true;
      }

      printerError = false;
      printerErrorMonitorResetEvent.Reset();
      bwPrinterErrorMonitor.RunWorkerAsync();

      printer.setName(aPrinterName);
      printer.IsSequenceNumberEnabled = aIsSequenceNumberEnabled;
      printer.PrintPickList = aPrintPickList;
      printController.setJob(aInvoiceList, true, aIsSequenceNumberEnabled, aIsPackJacket);

      printController.run();
    }

    private void setPrintControlsEnabled(bool aEnabled)
    {
      setControlEnabled(cbSubset, aEnabled);
      setControlEnabled(btExportErrors, aEnabled);
      setControlEnabled(cbPrinter, aEnabled);
      setControlEnabled(btReload, aEnabled);
    }

    private void tcQueries_Deselecting(object sender, TabControlCancelEventArgs e)
    {
      if (e.TabPageIndex != unshipTabIndex
         && e.TabPageIndex != unshippedTabIndex
         && e.TabPageIndex != repairTabIndex
         && printController.State == PrintControllerState.RUNNING)
      {
        MessageBox.Show(
          this,
          "You cannot leave this tab while printing. First stop printing process.",
          "Error",
          MessageBoxButtons.OK,
          MessageBoxIcon.Error
          );

        e.Cancel = true;
      }
    }

    private void btReload_Click(object sender, EventArgs e)
    {
      Cursor.Current = Cursors.WaitCursor;

      invoiceStorage.setQuery(tcQueries.SelectedIndex);

      // remove sort glyphs
      removeSortGlyph(dgvQuery);
      removeSortGlyph(dgvSubset);

      if (cbSubset.SelectedIndex == 0)
      {
        invoiceStorage.setSubsetAll();
      }
      else
      {
        cbSubset.SelectedIndex = 0;
      }

      Cursor.Current = Cursors.Default;

      MessageBox.Show(
        this,
        "Invoce list reloaded.",
        "Message",
        MessageBoxButtons.OK,
        MessageBoxIcon.Information
        );
    }

    private void miSubsetAddToUnship_Click(object sender, EventArgs e)
    {
      List<string> trackingNumberList = new List<string>();
      foreach (DataGridViewRow row in dgvSubset.SelectedRows)
      {
        trackingNumberList.Add((row.Tag as PrintPackageWrapper).TrackingNumber);
      }

      Cursor.Current = Cursors.WaitCursor;

      unship.addSingle(trackingNumberList.ToArray());

      Cursor.Current = Cursors.Default;

      MessageBox.Show(
        this,
        "Selected invoice(s) added to unship list.",
        "Information",
        MessageBoxButtons.OK,
        MessageBoxIcon.Information
        );
    }

    private void onUnshippedFilterClick(object sender, EventArgs e)
    {
      updateUnshippedFilter();
    }

    public void updateUnshippedFilter()
    {
      foreach (DataGridViewRow row in dgvUnshipped.Rows)
      {
        UnshippedPackageWrapper package = row.Tag as UnshippedPackageWrapper;

        bool show = false;

        if (chkUnshippedShowNonupdated.Checked && package.State == UnshippedPackageWrapper.StateType.ORIGINAL)
        {
          show = true;
        }

        if (chkUnshippedShowUpdated.Checked && package.State == UnshippedPackageWrapper.StateType.UPDATED_ONHOLD
            || chkUnshippedShowUpdated.Checked && package.State == UnshippedPackageWrapper.StateType.UPDATED_PICKABLE)
        {
          show = true;
        }


        if (chkUnshippedShowFailed.Checked && package.State == UnshippedPackageWrapper.StateType.ERROR)
        {
          show = true;
        }

        row.Visible = show;
      }
    }

    public void updateUnshippedStat()
    { 
      int nNonupdated = 0;
      int nUpdated = 0;
      int nFailed = 0;

      foreach (UnshippedPackageWrapper package in unshipped.PackageList)
      {
        switch (package.State)
        { 
          case UnshippedPackageWrapper.StateType.ORIGINAL:
            nNonupdated++;
            break;

          case UnshippedPackageWrapper.StateType.UPDATED_ONHOLD:
          case UnshippedPackageWrapper.StateType.UPDATED_PICKABLE:
            nUpdated++;
            break;

          case UnshippedPackageWrapper.StateType.ERROR:
            nFailed++;
            break;
        }
      }

      tsslUnshippedTotal.Text = String.Format("Total: {0}", unshipped.PackageList.Count);
      tsslNonupdated.Text = String.Format("Nonupdated: {0}", nNonupdated);
      tsslUnshippedUpdated.Text = String.Format("Updated: {0}", nUpdated);
      tsslUnshippedFailed.Text = String.Format("Failed: {0}", nFailed);
    }

    private void onReprintFilterClick(object sender, EventArgs e)
    {
      updateReprintFilter();
    }

    private void updateReprintFilter()
    { 
      foreach(DataGridViewRow row in dgvReprint.Rows)
      {
        PrintPackageWrapper package = row.Tag as PrintPackageWrapper;

        bool show = false;

        if (chkReprintShowUnprinted.Checked && package.IsUnprinted)
        {
          show = true;
        }

        if (chkReprintShowPrinted.Checked && package.IsPrinted)
        {
          show = true;
        }

        if (chkReprintShowFailed.Checked && package.IsError)
        {
          show = true;
        }

        if (chkReprintShowLocked.Checked && package.IsLocked)
        {
          show = true;
        }

        row.Visible = show;
      }
    }

    private void updateReprintStat()
    {
      int nUnprinted = 0;
      int nPrinted = 0;
      int nFailed = 0;
      int nLocked = 0;

      foreach (PrintPackageWrapper package in reprint.PackageList)
      {
        if (package.IsUnprinted)
        {
          nUnprinted++;
        }

        if (package.IsPrinted)
        {
          nPrinted++;
        }

        if (package.IsError)
        {
          nFailed++;
        }

        if (package.IsLocked)
        {
          nLocked++;
        }
      }

      tsslReprintTotal.Text = String.Format("Total: {0}", reprint.PackageList.Count);
      tsslReprintUnprinted.Text = String.Format("Unprinted: {0}", nUnprinted);
      tsslReprintPrinted.Text = String.Format("Printed: {0}", nPrinted);
      tsslReprintFailed.Text = String.Format("Failed: {0}", nFailed);
      tsslReprintLocked.Text = String.Format("Locked: {0}", nLocked);
    }

    private void onRepairFilterClick(object sender, EventArgs e)
    {
      updateRepairFilter();
    }

    private void updateRepairFilter()
    {
      foreach (DataGridViewRow row in dgvRepair.Rows)
      {
        RepairPackageWrapper package = row.Tag as RepairPackageWrapper;

        bool show = false;

        if (chkRepairShowNonrepaired.Checked && package.State == RepairPackageWrapper.StateType.ORIGINAL)
        {
          show = true;
        }

        if (chkRepairShowRepaired.Checked && package.State == RepairPackageWrapper.StateType.REPAIRED)
        {
          show = true;
        }

        row.Visible = show;
      }
    }

    private void updateRepairStat()
    {
      int nNonrepaired = 0;
      int nRepaired = 0;

      foreach (RepairPackageWrapper package in repair.PackageList)
      {
        if (package.State == RepairPackageWrapper.StateType.ORIGINAL)
        {
          nNonrepaired++;
        }

        if (package.State == RepairPackageWrapper.StateType.REPAIRED)
        {
          nRepaired++;
        }
      }

      tsslRepairTotal.Text = String.Format("Total: {0}", repair.PackageList.Count);
      tsslRepairNonrepaired.Text = String.Format("Nonrepaired: {0}", nNonrepaired);
      tsslRepairRepaired.Text = String.Format("Repaired: {0}", nRepaired);
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (printController.State == PrintControllerState.RUNNING)
      {
        MessageBox.Show(
             this,
             "You shouldn't exit application while printing. First stop printing process.",
             "Error",
             MessageBoxButtons.OK,
             MessageBoxIcon.Error
             ); 
        
        e.Cancel = true;
      }
    }

    private void dgvSubset_Sorted(object sender, EventArgs e)
    {
      if (cbSubset.SelectedIndex != 1) // not custom
      { 
        MessageBox.Show(
          "Invoice sequence number printing is disabled because of custom list sorting.",
          "Warning",
          MessageBoxButtons.OK,
          MessageBoxIcon.Warning);
        chkPrintSequenceNumber.Checked = false;
      }
    }

    private void miPreviewSubsetInvoiceWithSequenceNumber_Click(object sender, EventArgs e)
    {
      previewInvoice((dgvSubset.CurrentRow.Tag as PrintPackageWrapper).PackageId, Routines.generateSequenceNumber(0, 0, 0, 0), false);
    }

    private void previewPackJacketInvoiceToolStripMenuItem_Click(object sender, EventArgs e)
    {
      previewInvoice((dgvSubset.CurrentRow.Tag as PrintPackageWrapper).PackageId, null, true);
    }

    private void previewPackJacketInvoiceWithSequenceNumberToolStripMenuItem_Click(object sender, EventArgs e)
    {
      previewInvoice((dgvSubset.CurrentRow.Tag as PrintPackageWrapper).PackageId, Routines.generateSequenceNumber(0, 0, 0, 0), true);
    }

    private void chkPrintPickList_Click(object sender, EventArgs e)
    {
      /* if (chkPrintPickList.Checked) {
        chkPrintSequenceNumber.Checked = true;
      } */
    }

    private void chkPrintSequenceNumber_Click(object sender, EventArgs e)
    {
      /* if (!chkPrintSequenceNumber.Checked) {
        chkPrintPickList.Checked = false;
      } */
    }

    private void chkReprintPickList_Click(object sender, EventArgs e)
    {  
      /* if (chkReprintPickList.Checked) {
        chkReprintSequenceNumber.Checked = true;
      } */
    }

    private void chkReprintSequenceNumber_Click(object sender, EventArgs e)
    {
      /* if (!chkReprintSequenceNumber.Checked) {
        chkReprintPickList.Checked = false;
      } */
    }

    private void btOnHold_Click(object sender, EventArgs e)
    {

    }

    private void MainForm_Load(object sender, EventArgs e)
    {

    }

  }
}