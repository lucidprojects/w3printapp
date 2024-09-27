using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using PrintInvoice.Properties;

namespace PrintInvoice
{
    public partial class MainForm : Form
    {
        private readonly BackgroundWorker _bwPrinterErrorMonitor;

        private readonly BackgroundWorker _bwStopPrint;
        private readonly BackgroundWorker _bwStopReprint;
        private readonly Config _config;

        private readonly DataGridViewCellStyle _defaultFirstCellStyle = new DataGridViewCellStyle();
        private readonly DataGridViewCellStyle _errorCellStyle = new DataGridViewCellStyle();
        private readonly DataGridViewCellStyle _errorFirstCellStyle = new DataGridViewCellStyle();
        private readonly InvoiceProvider _invoiceProvider;
        private readonly InvoiceProviderErrorEventHandler _invoiceProviderError;
        private readonly InvoiceProviderErrorEventHandler _invoiceProviderErrorReprint;
        private readonly InvoiceProviderLoadEventHandler _invoiceProviderLoad;

        // event handlers
        private readonly InvoiceProviderLoadEventHandler _invoiceProviderLoadReprint;
        private readonly InvoiceStatusSaver _invoiceStatusSaver;
        private readonly InvoiceStatusSaverErrorEventHandler _invoiceStatusSaverError;
        private readonly InvoiceStatusSaverErrorEventHandler _invoiceStatusSaverErrorReprint;
        private readonly InvoiceStatusSaverSaveEventHandler _invoiceStatusSaverSave;

        private readonly InvoiceStatusSaverSaveEventHandler _invoiceStatusSaverSaveReprint;
        private readonly PrintPackageStorage _invoiceStorage;
        private readonly LabelService _labelService;

        private readonly DataGridViewCellStyle _loadedCellStyle = new DataGridViewCellStyle();
        private readonly DataGridViewCellStyle _loadedFirstCellStyle = new DataGridViewCellStyle();
        private readonly DataGridViewCellStyle _lockedCellStyle = new DataGridViewCellStyle();
        private readonly DataGridViewCellStyle _lockedFirstCellStyle = new DataGridViewCellStyle();
        private readonly PrintController _printController;
        private readonly ControllerCompleteEventHandler _printControllerComplete;

        private readonly ControllerCompleteEventHandler _printControllerCompleteReprint;
        private readonly DataGridViewCellStyle _printedCellStyle = new DataGridViewCellStyle();
        private readonly DataGridViewCellStyle _printedFirstCellStyle = new DataGridViewCellStyle();
        private readonly Printer _printer;
        private readonly AutoResetEvent _printerErrorMonitorResetEvent = new AutoResetEvent(false);
        private readonly PrinterJobErrorEventHandler _printerJobError;
        private readonly PrinterJobErrorEventHandler _printerJobErrorReprint;
        private readonly PrinterPrintEventHandler _printerPrint;

        private readonly PrinterPrintEventHandler _printerPrintReprint;
        private readonly Repair _repair;

        private readonly DataGridViewCellStyle _repairOriginalCellStyle = new DataGridViewCellStyle();
        private readonly DataGridViewCellStyle _repairOriginalFirstCellStyle = new DataGridViewCellStyle();
        private readonly DataGridViewCellStyle _repairRepairedCellStyle = new DataGridViewCellStyle();
        private readonly DataGridViewCellStyle _repairRepairedFirstCellStyle = new DataGridViewCellStyle();
        private readonly int _repairTabIndex;
        private readonly Reprint _reprint;
        private readonly Dictionary<int, DataGridViewRow> _reprintRowIndex = new Dictionary<int, DataGridViewRow>();
        private readonly int _reprintTabIndex;

        private readonly Dictionary<int, DataGridViewRow> _rowIndex = new Dictionary<int, DataGridViewRow>();
        private readonly DataGridViewCellStyle _statusSavedCellStyle = new DataGridViewCellStyle();
        private readonly DataGridViewCellStyle _statusSavedFirstCellStyle = new DataGridViewCellStyle();
        private readonly Dictionary<int, DataGridViewRow> _subsetRowIndex = new Dictionary<int, DataGridViewRow>();
        private readonly Unship _unship;

        private readonly DataGridViewCellStyle _unshipErrorCellStyle = new DataGridViewCellStyle();
        private readonly DataGridViewCellStyle _unshipErrorFirstCellStyle = new DataGridViewCellStyle();
        private readonly Unshipped _unshipped;

        private readonly DataGridViewCellStyle _unshippedNewCellStyle = new DataGridViewCellStyle();
        private readonly DataGridViewCellStyle _unshippedNewFirstCellStyle = new DataGridViewCellStyle();
        private readonly int _unshippedTabIndex;
        private readonly DataGridViewCellStyle _unshippedUnshippedCellStyle = new DataGridViewCellStyle();
        private readonly DataGridViewCellStyle _unshippedUnshippedFirstCellStyle = new DataGridViewCellStyle();
        private readonly DataGridViewCellStyle _unshippedUpdatedOnholdCellStyle = new DataGridViewCellStyle();
        private readonly DataGridViewCellStyle _unshippedUpdatedOnholdFirstCellStyle = new DataGridViewCellStyle();
        private readonly DataGridViewCellStyle _unshippedUpdatedPickableCellStyle = new DataGridViewCellStyle();
        private readonly DataGridViewCellStyle _unshippedUpdatedPickableFirstCellStyle = new DataGridViewCellStyle();
        private readonly int _unshipTabIndex;
        private readonly DataGridViewCellStyle _unshipUnshippedCellStyle = new DataGridViewCellStyle();
        private readonly DataGridViewCellStyle _unshipUnshippedFirstCellStyle = new DataGridViewCellStyle();

        private TabPage _controlsTabPage; // tab page contains controls
        private AddPackageForm _fmAddUnshipInvoice;

        private FindInvoiceForm _fmFindInvoice;
        private FindStartIndexForm _fmFindStartIndex;
        private LastBatchesForm _fmLastBatches;

        private bool _printerError;
        private string _printerErrorMessage;
        private bool _repairFieldNamesIsSet;
        private bool _reprintFieldNamesIsSet;
        private bool _unshipFieldNamesIsSet;
        private bool _unshippedFieldNamesIsSet;

        public MainForm()
        {
            InitializeComponent();
            Icon = Icon.ExtractAssociatedIcon(Assembly.GetEntryAssembly()?.Location ?? "");

            _config = new Config();

            _labelService = new LabelService();
            _labelService.Url = _config.LabelServiceUrl;
            _labelService.Timeout = 1000 * 60 * 30;
            _invoiceStorage = new PrintPackageStorage(_config, _labelService);
            _invoiceProvider = new InvoiceProvider(_labelService);
            _printer = new Printer(_invoiceProvider, _labelService);
            _invoiceStatusSaver = new InvoiceStatusSaver(_printer, _labelService);
            _printController = new PrintController(_invoiceProvider, _printer, _invoiceStatusSaver, _labelService);

            _invoiceStorage.Update += invoiceStorage_Update;
            _invoiceStorage.UpdateSubset += invoiceStorage_UpdateSubset;
            _invoiceStorage.UpdatePackageState += invoiceStorage_UpdateInvoiceState;

            _unship = new Unship(_labelService, _config);
            _unship.UpdateList += unship_UpdateList;

            _unshipped = new Unshipped(_labelService, _config);
            _unshipped.UpdateList += unshipped_UpdateList;
            _unshipped.UpdateMaxDailyPackages += unshipped_UpdateMaxDailyPackages;

            _reprint = new Reprint(_config, _labelService);
            _reprint.UpdateList += reprint_UpdateList;
            _reprint.UpdatePackageState += reprint_UpdatePackageState;

            _repair = new Repair(_labelService, _config);
            _repair.UpdateList += repair_UpdateList;

            InitPrinterCombo();

            _printer.PrinterError += printer_PrinterError;

            _bwPrinterErrorMonitor = new BackgroundWorker();
            _bwPrinterErrorMonitor.WorkerSupportsCancellation = true;
            _bwPrinterErrorMonitor.DoWork += bwPrinterErrorMonitor_DoWork;

            _bwStopPrint = new BackgroundWorker();
            _bwStopPrint.DoWork += bwStopPrint_DoWork;

            _bwStopReprint = new BackgroundWorker();
            _bwStopReprint.DoWork += bwStopReprint_DoWork;

            // event handlers
            _invoiceProviderLoadReprint = invoiceProvider_LoadReprint;
            _invoiceProviderLoad = invoiceProvider_Load;
            _invoiceProviderErrorReprint = invoiceProvider_ErrorReprint;
            _invoiceProviderError = invoiceProvider_Error;

            _printerPrintReprint = printer_PrintReprint;
            _printerPrint = printer_Print;
            _printerJobErrorReprint -= printer_JobErrorReprint;
            _printerJobError = printer_JobError;

            _invoiceStatusSaverSaveReprint = invoiceStatusSaver_SaveReprint;
            _invoiceStatusSaverSave = invoiceStatusSaver_Save;
            _invoiceStatusSaverErrorReprint -= invoiceStatusSaver_ErrorReprint;
            _invoiceStatusSaverError = invoiceStatusSaver_Error;

            _printControllerCompleteReprint = printController_CompleteReprint;
            _printControllerComplete = printController_Complete;


            // set up tabs for root queries
            var h = tcQueries.Handle; // bug? see http://social.msdn.microsoft.com/Forums/en-US/winforms/thread/5d10fd0c-1aa6-4092-922e-1fd7af979663

            for (var i = 0; i < _config.QueryList.Count; i++)
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

                tabPage.Text = _config.QueryList[i].Title;
            }

            SetQueryTab(0);
            _controlsTabPage = tcQueries.TabPages[0];
            _unshipTabIndex = tcQueries.TabPages.Count - 4;
            _unshippedTabIndex = tcQueries.TabPages.Count - 3;
            _reprintTabIndex = tcQueries.TabPages.Count - 2;
            _repairTabIndex = tcQueries.TabPages.Count - 1;

            // colors
            var yellow = ColorTranslator.FromHtml("#FFFF00");
            var olive = ColorTranslator.FromHtml("#808000");
            var green = ColorTranslator.FromHtml("#008000");
            var red = ColorTranslator.FromHtml("#FF0000");
            var gray = ColorTranslator.FromHtml("#A0A0A0");

            // print/reprint cell styles
            _loadedCellStyle.BackColor = yellow;
            _printedCellStyle.BackColor = olive;
            _statusSavedCellStyle.BackColor = green;
            _statusSavedCellStyle.ForeColor = Color.White;
            _errorCellStyle.BackColor = red;
            _lockedCellStyle.BackColor = gray;

            UpdateUnshipStat();

            // unship cell styles
            _unshipErrorCellStyle.BackColor = red;
            _unshipUnshippedCellStyle.BackColor = green;

            // unshipped cell styles
            _unshippedNewCellStyle.BackColor = Color.White;
            _unshippedUnshippedCellStyle.BackColor = yellow;
            _unshippedUpdatedPickableCellStyle.BackColor = green;
            _unshippedUpdatedOnholdCellStyle.BackColor = gray;

            // repair cell styles
            _repairOriginalCellStyle.BackColor = Color.White;
            _repairRepairedCellStyle.BackColor = green;

            // first sell styles
            _defaultFirstCellStyle.BackColor = Color.White;
            _defaultFirstCellStyle.SelectionBackColor = Color.White;
            _defaultFirstCellStyle.SelectionForeColor = Color.Black;

            _errorFirstCellStyle.BackColor = _errorCellStyle.BackColor;
            _errorFirstCellStyle.SelectionBackColor = _errorCellStyle.BackColor;

            _loadedFirstCellStyle.BackColor = _loadedCellStyle.BackColor;
            _loadedFirstCellStyle.SelectionBackColor = _loadedCellStyle.BackColor;
            _loadedFirstCellStyle.SelectionForeColor = Color.Black;

            _printedFirstCellStyle.BackColor = _printedCellStyle.BackColor;
            _printedFirstCellStyle.SelectionBackColor = _printedCellStyle.BackColor;

            _statusSavedFirstCellStyle.BackColor = _statusSavedCellStyle.BackColor;
            _statusSavedFirstCellStyle.ForeColor = Color.White;
            _statusSavedFirstCellStyle.SelectionBackColor = _statusSavedCellStyle.BackColor;
            _statusSavedFirstCellStyle.SelectionForeColor = Color.White;

            _lockedFirstCellStyle.BackColor = _lockedCellStyle.BackColor;
            _lockedFirstCellStyle.SelectionBackColor = _lockedCellStyle.BackColor;

            _unshipErrorFirstCellStyle.BackColor = _unshipErrorCellStyle.BackColor;
            _unshipErrorFirstCellStyle.SelectionBackColor = _unshipErrorCellStyle.BackColor;

            _unshipUnshippedFirstCellStyle.BackColor = _unshipUnshippedCellStyle.BackColor;
            _unshipUnshippedFirstCellStyle.SelectionBackColor = _unshipUnshippedCellStyle.BackColor;

            _unshippedNewFirstCellStyle.BackColor = _unshippedNewCellStyle.BackColor;
            _unshippedNewFirstCellStyle.SelectionBackColor = _unshippedNewCellStyle.BackColor;
            _unshippedNewFirstCellStyle.SelectionForeColor = Color.Black;

            _unshippedUnshippedFirstCellStyle.BackColor = _unshippedUnshippedCellStyle.BackColor;
            _unshippedUnshippedFirstCellStyle.SelectionBackColor = _unshippedUnshippedCellStyle.BackColor;
            _unshippedUnshippedFirstCellStyle.SelectionForeColor = Color.Black;

            _unshippedUpdatedPickableFirstCellStyle.BackColor = _unshippedUpdatedPickableCellStyle.BackColor;
            _unshippedUpdatedPickableFirstCellStyle.SelectionBackColor = _unshippedUpdatedPickableCellStyle.BackColor;

            _unshippedUpdatedOnholdFirstCellStyle.BackColor = _unshippedUpdatedOnholdCellStyle.BackColor;
            _unshippedUpdatedOnholdFirstCellStyle.SelectionBackColor = _unshippedUpdatedOnholdCellStyle.BackColor;

            _repairOriginalFirstCellStyle.BackColor = _repairOriginalCellStyle.BackColor;
            _repairOriginalFirstCellStyle.SelectionBackColor = _repairOriginalCellStyle.BackColor;
            _repairOriginalFirstCellStyle.SelectionForeColor = Color.Black;

            _repairRepairedFirstCellStyle.BackColor = _repairRepairedCellStyle.BackColor;
            _repairRepairedFirstCellStyle.SelectionBackColor = _repairRepairedCellStyle.BackColor;

            tbHelp.Text = Settings.Default.HelpText;
            tbHelpUnship.Text = Settings.Default.UnshipHelpText;
            tbHelpUnshipped.Text = Settings.Default.UnshippedHelpText;
            tbHelpRepair.Text = Settings.Default.RepairHelpText;
            tbHelpReprint.Text = Settings.Default.ReprintHelpText;

            Log.GetLogger().Info("Start application");
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var handleParam = base.CreateParams;
                handleParam.ExStyle |= 0x02000000; // WS_EX_COMPOSITED       
                return handleParam;
            }
        }

        private void bwStopReprint_DoWork(object sender, DoWorkEventArgs e)
        {
            _printController.stop();

            _bwPrinterErrorMonitor.CancelAsync();
            _printerErrorMonitorResetEvent.WaitOne();

            // unlock invoices
            var idList = new List<int>();
            for (var i = 0; i < dgvReprint.Rows.Count; i++)
            {
                var package = (PrintPackageWrapper)dgvReprint.Rows[i].Tag;
                if (package.IsLoaded && !package.IsPrinted) idList.Add(package.PackageId);
            }

            if (idList.Count > 0) _reprint.Unlock(idList);

            SetReprintControlsEnabled(true);

            SetControlText(btReprintPrint, "Print");
            SetControlEnabled(btReprintPrint, true);

            if (_printerError)
                ShowMessageBox(
                    this,
                    "Printer return error status (" + _printerErrorMessage + ").\nProcess is stopped.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            //findStartIndex();
        }

        private void bwStopPrint_DoWork(object sender, DoWorkEventArgs e)
        {
            _printController.stop();

            _bwPrinterErrorMonitor.CancelAsync();
            _printerErrorMonitorResetEvent.WaitOne();

            // unlock invoices
            var idList = new List<int>();
            for (var i = 0; i < dgvSubset.Rows.Count; i++)
            {
                var package = (PrintPackageWrapper)dgvSubset.Rows[i].Tag;
                if (package.IsLoaded && !package.IsPrinted) idList.Add(package.PackageId);
            }

            if (idList.Count > 0) _invoiceStorage.Unlock(idList);

            SetPrintControlsEnabled(true);

            SetControlText(btPrint, "Print");
            SetControlEnabled(btPrint, true);

            if (_printerError)
                ShowMessageBox(
                    this,
                    "Printer return error status (" + _printerErrorMessage + ").\nProcess is stopped.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

            FindStartIndex();
        }

        private void ProcessPrinterError()
        {
            btPrint.Enabled = false;

            if (tcQueries.SelectedIndex == _reprintTabIndex)
                _bwStopReprint.RunWorkerAsync();
            else
                _bwStopPrint.RunWorkerAsync();
        }

        private void bwPrinterErrorMonitor_DoWork(object sender, DoWorkEventArgs e)
        {
            var bw = sender as BackgroundWorker;

            for (;;)
            {
                if (_printerError)
                {
                    Invoke(new ProcessPrinterErrorCallback(ProcessPrinterError));
                    break;
                }

                if (bw.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                Thread.Sleep(200);
            }

            _printerErrorMonitorResetEvent.Set();
        }

        private void printer_PrinterError(object sender, PrinterPrinterErrorEventArgs e)
        {
            _printerError = true;
            _printerErrorMessage = e.Message;
        }

        private void printController_Complete(object sender, EventArgs e)
        {
            _bwPrinterErrorMonitor.CancelAsync();
            _printerErrorMonitorResetEvent.WaitOne();
            SetControlText(btPrint, "Print");
            SetPrintControlsEnabled(true);
            ShowMessageBox(
                this,
                "Print job complete.",
                "Message",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        public void SetControlText(Control control, string text)
        {
            if (control.InvokeRequired)
            {
                SetControlTextCallback d = SetControlText;
                Invoke(d, control, text);
            }
            else
            {
                control.Text = text;
            }
        }

        public void SetControlEnabled(Control control, bool aEnabled)
        {
            if (control.InvokeRequired)
            {
                SetControlEnabledCallback d = SetControlEnabled;
                Invoke(d, control, aEnabled);
            }
            else
            {
                control.Enabled = aEnabled;
            }
        }

        public void ShowMessageBox(IWin32Window owner, string text, string caption, MessageBoxButtons buttons,
            MessageBoxIcon icon)
        {
            if (InvokeRequired)
            {
                ShowMessageBoxCallback d = ShowMessageBox;
                Invoke(d, owner, text, caption, buttons, icon);
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

        private void invoiceStatusSaver_Error(object sender, InvoiceStatusSaverErrorEventArgs e)
        {
            _invoiceStorage.SetPackageState(e.InvoiceId, PrintPackageWrapper.Error, e.Message);
        }

        private void invoiceStatusSaver_Save(object sender, InvoiceStatusSaverSaveEventArgs e)
        {
            _invoiceStorage.SetPackageState(e.InvoiceId, PrintPackageWrapper.StatusSaved, "");
        }

        private void invoiceProvider_Load(object sender, InvoiceProviderLoadEventArgs e)
        {
            _invoiceStorage.SetPackageState(e.PrintInvoiceWrapper.PackageId,
                e.PrintInvoiceWrapper.IsLoaded ? PrintPackageWrapper.Loaded : PrintPackageWrapper.Locked, "");
        }

        private void printer_JobError(object sender, PrinterJobErrorEventArgs e)
        {
            _invoiceStorage.SetPackageState(e.PackageId, PrintPackageWrapper.Error, e.Message);
        }

        private void printer_Print(object sender, PrinterPrintEventArgs e)
        {
            _invoiceStorage.SetPackageState(e.PackageId, PrintPackageWrapper.Printed, "");
        }

        private void invoiceStorage_UpdateInvoiceState(object sender, PrintPackageStorageUpdatePackageStateEventArgs e)
        {
            var invoice = _invoiceStorage.GetPackageByPackageId(e.PackageId);
            SetRowStyle(_rowIndex[e.PackageId], invoice);
            SetRowStyle(_subsetRowIndex[e.PackageId], invoice);
            UpdateStat();
            UpdateSubsetStat();
        }

        private void invoiceProvider_Error(object sender, InvoiceProviderErrorEventArgs e)
        {
            _invoiceStorage.SetPackageState(e.InvoiceId, PrintPackageWrapper.Error, e.Message);
        }

        private void invoiceStorage_UpdateSubset(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                UpdateSubsetCallback d = invoiceStorage_UpdateSubset;
                Invoke(d, sender, e);
            }
            else
            {
                dgvSubset.RowCount = _invoiceStorage.SubsetPackageList.Count;
                _subsetRowIndex.Clear();
                if (dgvSubset.RowCount > 0)
                {
                    dgvSubset.ColumnCount = _invoiceStorage.SubsetPackageList[0].FieldValueList.Length;
                    if (dgvSubset.ColumnCount > 0) dgvSubset.CurrentCell = dgvSubset.Rows[0].Cells[0];

                    for (var row = 0; row < _invoiceStorage.SubsetPackageList.Count; row++)
                    for (var col = 0; col < dgvSubset.ColumnCount; col++)
                    {
                        dgvSubset.Rows[row].Cells[col].Value =
                            _invoiceStorage.GetTypedFieldValue(_invoiceStorage.SubsetPackageList[row].FieldValueList[col],
                                col);
                        dgvSubset.Rows[row].Cells[col].Style = dgvSubset.DefaultCellStyle;
                        dgvSubset.Rows[row].Tag = _invoiceStorage.SubsetPackageList[row];
                        _subsetRowIndex[_invoiceStorage.SubsetPackageList[row].PackageId] = dgvSubset.Rows[row];
                        SetRowStyle(dgvSubset.Rows[row], _invoiceStorage.SubsetPackageList[row]);
                    }
                }

                btPrint.Enabled = dgvSubset.Rows.Count > 0;
                tsslSubsetTotal.Text = $@"Total: {dgvSubset.RowCount.ToString()}";
                UpdateSubsetStat();
                chkPrintSequenceNumber.Checked = true;
            }
        }

        private void invoiceStorage_Update(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                UpdateSetDelegate d = invoiceStorage_Update;
                Invoke(d, sender, e);
            }
            else
            {
                // field names
                var fieldNames = _invoiceStorage.FieldNames;
                dgvQuery.ColumnCount = fieldNames.Length;
                dgvSubset.ColumnCount = fieldNames.Length;
                
                for (var i = 0; i < fieldNames.Length; i++)
                {
                    dgvQuery.Columns[i].HeaderText = fieldNames[i];
                    dgvQuery.Columns[i].SortMode = DataGridViewColumnSortMode.Automatic;
                    dgvSubset.Columns[i].HeaderText = fieldNames[i];
                    dgvSubset.Columns[i].SortMode = DataGridViewColumnSortMode.Automatic;
                }

                // copy to clipboard context menu values
                FillCopyToClipboardMenuItem(miSubsetCopyToClipboard, dgvSubset);
                FillCopyToClipboardMenuItem(miSetCopyToClipboard, dgvQuery);

                dgvQuery.RowCount = _invoiceStorage.PackageList.Count;
                _rowIndex.Clear();
                if (dgvQuery.RowCount > 0)
                    for (var row = 0; row < _invoiceStorage.PackageList.Count; row++)
                    for (var col = 0; col < dgvQuery.ColumnCount; col++)
                    {
                        dgvQuery.Rows[row].Cells[col].Value = _invoiceStorage.GetTypedFieldValue(row, col);
                        dgvQuery.Rows[row].Cells[col].Style = dgvQuery.DefaultCellStyle;
                        dgvQuery.Rows[row].Tag = _invoiceStorage.PackageList[row];
                        _rowIndex[_invoiceStorage.PackageList[row].PackageId] = dgvQuery.Rows[row];
                        SetRowStyle(dgvQuery.Rows[row], _invoiceStorage.PackageList[row]);
                    }

                ResetFilter();
                tsslSetTotal.Text = $@"Total: {dgvQuery.RowCount.ToString()}";
                UpdateStat();
            }
        }

        private void FillCopyToClipboardMenuItem(ToolStripMenuItem aMenuItem, DataGridView aDgv)
        {
            aMenuItem.Text = @"Copy Cell Value To Clipboard";
            aMenuItem.DropDownItems.Clear();

            for (var i = 0; i < aDgv.ColumnCount; i++)
            {
                var item = aMenuItem.DropDownItems.Add(aDgv.Columns[i].HeaderText);
                item.Tag = new object[] { i, aDgv };
                item.Click += miCopyToClipboard_Click;
            }
        }

        private void miCopyToClipboard_Click(object sender, EventArgs e)
        {
            var item = (ToolStripItem)sender;
            var data = (object[])item.Tag;
            var fieldIndex = (int)data[0];
            var dgv = (DataGridView)data[1];

            if (dgv.CurrentRow?.Cells[fieldIndex].Value != null)
                try
                {
                    Clipboard.SetText(dgv.CurrentRow.Cells[fieldIndex].Value.ToString());
                }
                catch (Exception)
                {
                    // ignore
                }
        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            var tc = (TabControl)sender;

            if (tc.SelectedIndex != _unshipTabIndex)
            {
                if (tc.SelectedIndex == _unshippedTabIndex)
                {
                    if (!_unshipped.IsLoaded)
                    {
                        _unshipped.Load();
                        UpdateUnshippedFilter();
                        UpdateUnshippedStat();
                    }
                }
                else
                {
                    if (tc.SelectedIndex == _reprintTabIndex)
                    {
                        UpdateReprintFilter();
                        UpdateReprintStat();
                    }
                    else
                    {
                        if (tc.SelectedIndex == _repairTabIndex)
                        {
                            if (!_repair.IsLoaded)
                            {
                                _repair.Load();
                                UpdateRepairFilter();
                                UpdateRepairStat();
                            }
                        }
                        else
                        {
                            if (_controlsTabPage == e.TabPage)
                            {
                                SetQueryTab(e.TabPageIndex);
                            }
                            else
                            {
                                // copy all controls from previously selected tab to newly selected

                                e.TabPage.Controls.Clear();

                                while (_controlsTabPage.Controls.Count > 0)
                                    e.TabPage.Controls.Add(_controlsTabPage.Controls[0]);

                                e.TabPage.Padding = _controlsTabPage.Padding;
                                e.TabPage.UseVisualStyleBackColor = _controlsTabPage.UseVisualStyleBackColor;
                                _controlsTabPage.Controls.Clear();

                                SetQueryTab(e.TabPageIndex);
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
            var tc = (TabControl)sender;

            if (tc.TabPages.IndexOf(e.TabPage) != _unshipTabIndex &&
                tc.TabPages.IndexOf(e.TabPage) != _unshippedTabIndex &&
                tc.TabPages.IndexOf(e.TabPage) != _reprintTabIndex &&
                tc.TabPages.IndexOf(e.TabPage) != _repairTabIndex)
                _controlsTabPage = e.TabPage;
        }

        // fills subqueries control (combobox)
        private void FillSubqueriesControl(int queryIndex)
        {
            cbSubset.Items.Clear();
            cbSubset.Items.Add("All");
            cbSubset.Items.Add("Custom");

            foreach (var subquery in _config.QueryList[queryIndex].SubqueryList) cbSubset.Items.Add(subquery.Title);

            cbSubset.SelectedIndex = 0;
        }

        private void SetQueryTab(int queryIndex)
        {
            FillSubqueriesControl(queryIndex);

            _invoiceStorage.SetQuery(queryIndex);

            if (cbSubset.SelectedIndex != 0)
                cbSubset.SelectedIndex = 0; // hidden calls onUpdateSubset event
            else
                // direct call onUpdateSubset event
                _invoiceStorage.SetSubsetAll();
        }

        private void cbSubqueries_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cbSubset.SelectedIndex)
            {
                case 0: // all
                    _invoiceStorage.SetSubsetAll();
                    break;

                case 1: // custom
                    _invoiceStorage.SetSubsetCustom();
                    break;

                default:
                    _invoiceStorage.SetSubset(cbSubset.SelectedIndex - 2);
                    break;
            }

            RemoveSortGlyph(dgvSubset);
        }

        private void RemoveSortGlyph(DataGridView aDataGridView)
        {
            if (aDataGridView.SortedColumn != null)
                aDataGridView.SortedColumn.HeaderCell.SortGlyphDirection = SortOrder.None;
        }

        private void ResetFilter()
        {
            cbQueryShowUnprinted.Checked = true;
            cbQueryShowPrinted.Checked = true;
            cbQueryShowError.Checked = true;
            cbQueryShowLocked.Checked = true;
        }

        private void UpdateFilter()
        {
            foreach (DataGridViewRow row in dgvQuery.Rows)
            {
                var visible = false;

                var invoiceWrapper = (PrintPackageWrapper)row.Tag;

                if (cbQueryShowUnprinted.Checked && invoiceWrapper.IsUnprinted) visible = true;
                if (cbQueryShowPrinted.Checked && invoiceWrapper.IsPrinted) visible = true;
                if (cbQueryShowError.Checked && invoiceWrapper.IsError) visible = true;
                if (cbQueryShowLocked.Checked && invoiceWrapper.IsLocked) visible = true;

                row.Visible = visible;
            }
        }


        private void cbQueryShowUnprinted_CheckedChanged(object sender, EventArgs e)
        {
            UpdateFilter();
        }

        private void cbQueryShowPrinted_CheckedChanged(object sender, EventArgs e)
        {
            UpdateFilter();
        }

        private void cbQueryShowError_CheckedChanged(object sender, EventArgs e)
        {
            UpdateFilter();
        }

        private void cbQueryShowLocked_CheckedChanged(object sender, EventArgs e)
        {
            UpdateFilter();
        }

        private void InitPrinterCombo()
        {
            var defaultPrinterName = ""; // (string)config.getValue("printerName", new PrintDocument().PrinterSettings.PrinterName);

            foreach (string printerName in PrinterSettings.InstalledPrinters)
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
            if (_printController.State == PrintControllerState.RUNNING)
            {
                btPrint.Enabled = false;
                _bwStopPrint.RunWorkerAsync();
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
                        @"Invoice sequence number printing is " +
                        (chkPrintSequenceNumber.Checked ? "enabled" : "disabled") + @".\nContinue printing?",
                        @"Warning",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    ) == DialogResult.No)
                    return;

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

                if (MessageBox.Show(q, @"Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;

                if (dgvSubset.CurrentRow?.Index == 0 || (dgvSubset.CurrentRow?.Index > 0 && MessageBox.Show(
                        @"You are going to print out of sequence (not from the beginning of this list). Continue?",
                        @"Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes))
                {
                    btPrint.Enabled = false;
                    var invoiceList = new List<PrintPackageWrapper>();
                    var askRepeatedPrint = true;
                    var askRepeatedPrintResult = true;

                    for (var i = dgvSubset.CurrentRow.Index; i < dgvSubset.Rows.Count; i++)
                        if (dgvSubset.Rows[i].Visible)
                        {
                            var add = true;
                            var package = (PrintPackageWrapper)dgvSubset.Rows[i].Tag;

                            if (package.IsPrinted)
                            {
                                if (askRepeatedPrint)
                                {
                                    var fmRepeatedPrint = new RepeatedPrintForm();
                                    fmRepeatedPrint.laMessage.Text =
                                        $@"You are going to print invoice No. {dgvSubset.Rows[i].Cells[PrintPackageStorage.InvoiceNumberColumnIndex]} which is already marked as printed. Do you want to print it again?";
                                    askRepeatedPrintResult = fmRepeatedPrint.ShowDialog() == DialogResult.Yes;
                                    askRepeatedPrint = !fmRepeatedPrint.ckDontAsk.Checked;
                                }

                                add = askRepeatedPrintResult;
                            }

                            if (add)
                            {
                                package._isPackJacket = false; // chkPackJacket.Checked;
                                invoiceList.Add(package);
                            }
                        }

                    StartPrintJob(invoiceList, cbPrinter.SelectedItem.ToString(), false, chkPrintSequenceNumber.Checked, false, false);
                    SetPrintControlsEnabled(false);

                    btPrint.Text = @"Stop";
                    btPrint.Enabled = true;
                }
            }
        }

        private void SetRowStyle(DataGridViewRow row, PrintPackageWrapper invoice)
        {
            if (row.DataGridView.InvokeRequired)
            {
                SetRowStyleCallback d = SetRowStyle;
                Invoke(d, row, invoice);
            }
            else
            {
                DataGridViewCellStyle style = null;
                var firstCellStyle = _defaultFirstCellStyle;

                // unprinted
                if (invoice.IsUnprinted) style = row.DefaultCellStyle;

                // loaded
                if (invoice.IsLoaded)
                {
                    style = _loadedCellStyle;
                    firstCellStyle = _loadedFirstCellStyle;
                }

                // printed
                if (invoice.IsPrinted)
                {
                    style = _printedCellStyle;
                    firstCellStyle = _printedFirstCellStyle;

                    if (_printController.State == PrintControllerState.RUNNING)
                        //row.DataGridView.FirstDisplayedScrollingRowIndex = row.Index;
                        row.DataGridView.CurrentCell = row.Cells[0];
                }

                // status saved
                if (invoice.IsStatusSaved)
                {
                    style = _statusSavedCellStyle;
                    firstCellStyle = _statusSavedFirstCellStyle;
                }

                // error
                if (invoice.IsError)
                {
                    row.ErrorText = invoice.ErrorText;
                    style = _errorCellStyle;
                    firstCellStyle = _errorFirstCellStyle;
                }
                else
                {
                    row.ErrorText = "";
                }

                // locked
                if (invoice.IsLocked)
                {
                    style = _lockedCellStyle;
                    firstCellStyle = _lockedFirstCellStyle;
                }

                if (style != null)
                {
                    foreach (DataGridViewCell cell in row.Cells) cell.Style = style;
                    row.Cells[0].Style = firstCellStyle;
                }
            }
        }

        private void addToCustomSubsetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var list = (from DataGridViewRow row in dgvQuery.SelectedRows select ((PrintPackageWrapper)row.Tag).PackageId).ToList();
            list.Reverse(); // selected list in reverse order
            _invoiceStorage.AddToCustomSubset(list);

            cbSubset.SelectedIndex = 1; // subset
        }

        private void cmsSubset_Opening(object sender, CancelEventArgs e)
        {
            miRemoveFromSubset.Enabled = cbSubset.SelectedIndex == 1 && dgvSubset.SelectedRows.Count != 0 && _printController.State != PrintControllerState.RUNNING;

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
            var list = (from DataGridViewRow row in dgvSubset.SelectedRows select ((PrintPackageWrapper)row.Tag).PackageId).ToList();
            _invoiceStorage.RemoveFromCustomSubset(list);
        }

        private void cmsSet_Opening(object sender, CancelEventArgs e)
        {
            miPreviewSetInvoice.Enabled = dgvQuery.SelectedRows.Count != 0;
            addToCustomSubsetToolStripMenuItem.Enabled = dgvQuery.SelectedRows.Count != 0 && _printController.State != PrintControllerState.RUNNING;
        }

        private void PreviewInvoice(int aInvoiceId, string aSequenceNumber, bool aIsPackJacket)
        {
            try
            {
                var request = new GetLabelRequestType
                {
                    packageId = aInvoiceId,
                    @lock = false,
                    isPackJacket = aIsPackJacket
                };

                var response = _labelService.getLabel(request);

                if (response.status != 0)
                    //invoiceStorage.setPackageError(aInvoiceId, "Error loading invoice: " + response.message);
                    throw new Exception(response.message);

                var path = $"{Application.StartupPath}\\preview.pdf";
                var pdf = Convert.FromBase64String(response.base64data);

                if (aSequenceNumber != null)
                    Routines.AddSequenceNumberToPdf(aSequenceNumber, ref pdf, aIsPackJacket);

                File.WriteAllBytes(path, pdf);
                Process.Start(path);
            }
            catch (Exception e)
            {
                MessageBox.Show(this, @"Error loading invoice: " + e.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void miPreviewSetInvoice_Click(object sender, EventArgs e)
        {
            PreviewInvoice((dgvQuery.CurrentRow.Tag as PrintPackageWrapper).PackageId, null, false);
        }

        private void miPreviewSubsetInvoice_Click(object sender, EventArgs e)
        {
            PreviewInvoice((dgvSubset.CurrentRow.Tag as PrintPackageWrapper).PackageId, null, false);
        }

        private void miFindInvoice_Click(object sender, EventArgs e)
        {
            if (_fmFindInvoice == null)
                _fmFindInvoice = new FindInvoiceForm(this, _config);

            if (!_fmFindInvoice.Visible)
                _fmFindInvoice.Show(this);
        }

        public bool SetCurrentSubsetInvoice(string findValue, int colIndex, bool next)
        {
            var result = false;

            foreach (DataGridViewRow row in dgvSubset.Rows)
                if (row.Visible && row.Cells[colIndex].Value.ToString() == findValue)
                {
                    result = true;
                    dgvSubset.CurrentCell = row.Cells[colIndex];
                    break;
                }

            if (result && next) // need next row
            {
                result = false;

                for (var i = dgvSubset.CurrentRow.Index + 1; i < dgvSubset.Rows.Count; i++)
                    if (dgvSubset.Rows[i].Visible)
                    {
                        dgvSubset.CurrentCell = dgvSubset.Rows[i].Cells[colIndex];
                        result = true;
                        break;
                    }
            }

            return result;
        }

        public bool SetCurrentSetInvoice(string findValue, int colIndex)
        {
            var result = false;

            foreach (DataGridViewRow row in dgvQuery.Rows)
                if (row.Visible && row.Cells[colIndex].Value.ToString() == findValue)
                {
                    result = true;
                    dgvQuery.CurrentCell = row.Cells[colIndex];
                    break;
                }

            return result;
        }

        private void miFindStartIndex_Click(object sender, EventArgs e)
        {
            FindStartIndex();
        }

        private void FindStartIndex()
        {
            if (InvokeRequired)
            {
                FindStartIndexCallback d = FindStartIndex;
                Invoke(d);
            }
            else
            {
                if (_fmFindStartIndex == null) _fmFindStartIndex = new FindStartIndexForm(this, _config);
                _fmFindStartIndex.ShowDialog(this);
            }
        }

        private void btExportErrors_Click(object sender, EventArgs e)
        {
            var errorList = dgvSubset.Rows.Cast<DataGridViewRow>()
                .Select(row => (PrintPackageWrapper)row.Tag)
                .Where(package => package.IsError)
                .Select(package => $"Package: {package.PackageId}. Error: {package.ErrorText}")
                .ToList();

            SaveErrors(errorList);
        }

        private void SaveErrors(List<string> aList)
        {
            if (aList.Count == 0)
            {
                MessageBox.Show(this, @"There are no errors to export.", @"Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                if (sfdExportErrors.ShowDialog() == DialogResult.OK)
                {
                    var sw = new StreamWriter(sfdExportErrors.FileName);
                    sw.WriteLine("Date: {0}", DateTime.Now);
                    foreach (var error in aList) sw.WriteLine(error);
                    sw.Close();
                    Process.Start(sfdExportErrors.FileName);
                }
            }
        }

        private Stat GetStat(DataGridView dgv)
        {
            var stat = new Stat();

            foreach (DataGridViewRow row in dgv.Rows)
            {
                var invoiceWrapper = (PrintPackageWrapper)row.Tag;

                if (invoiceWrapper.IsUnprinted) stat._unprinted++;
                if (invoiceWrapper.IsPrinted) stat._printed++;
                if (invoiceWrapper.IsError) stat._failed++;
                if (invoiceWrapper.IsLocked) stat._locked++;
            }

            return stat;
        }

        private void UpdateStat()
        {
            var stat = GetStat(dgvQuery);

            tsslSetUnprinted.Text = $@"Unprinted: {stat._unprinted}";
            tsslSetPrinted.Text = $@"Printed: {stat._printed}";
            tsslSetFailed.Text = $@"Failed: {stat._failed}";
            tsslSetLocked.Text = $@"Locked: {stat._locked}";
        }

        private void UpdateSubsetStat()
        {
            var stat = GetStat(dgvSubset);

            tsslSubsetUnprinted.Text = $@"Unprinted: {stat._unprinted}";
            tsslSubsetPrinted.Text = $@"Printed: {stat._printed}";
            tsslSubsetFailed.Text = $@"Failed: {stat._failed}";
            tsslSubsetLocked.Text = $@"Locked: {stat._locked}";
        }

        private void miFileExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SetPrintEventHandlers()
        {
            // ensure that only one event handler enabled - first remove then add new one

            _invoiceProvider.Load -= _invoiceProviderLoadReprint;
            _invoiceProvider.Load -= _invoiceProviderLoad;
            _invoiceProvider.Load += _invoiceProviderLoad;
            _invoiceProvider.Error -= _invoiceProviderErrorReprint;
            _invoiceProvider.Error -= _invoiceProviderError;
            _invoiceProvider.Error += _invoiceProviderError;

            _printer.Print -= _printerPrintReprint;
            _printer.Print -= _printerPrint;
            _printer.Print += _printerPrint;
            _printer.JobError -= _printerJobErrorReprint;
            _printer.JobError -= _printerJobError;
            _printer.JobError += _printerJobError;

            _invoiceStatusSaver.Save -= _invoiceStatusSaverSaveReprint;
            _invoiceStatusSaver.Save -= _invoiceStatusSaverSave;
            _invoiceStatusSaver.Save += _invoiceStatusSaverSave;
            _invoiceStatusSaver.Error -= _invoiceStatusSaverErrorReprint;
            _invoiceStatusSaver.Error -= _invoiceStatusSaverError;
            _invoiceStatusSaver.Error += _invoiceStatusSaverError;

            _printController.Complete -= _printControllerCompleteReprint;
            _printController.Complete -= _printControllerComplete;
            _printController.Complete += _printControllerComplete;
        }

        private void SetReprintEventHandlers()
        {
            // ensure that only one event handler enabled - first remove then add new one

            _invoiceProvider.Load -= _invoiceProviderLoad;
            _invoiceProvider.Load -= _invoiceProviderLoadReprint;
            _invoiceProvider.Load += _invoiceProviderLoadReprint;
            _invoiceProvider.Error -= _invoiceProviderError;
            _invoiceProvider.Error -= _invoiceProviderErrorReprint;
            _invoiceProvider.Error += _invoiceProviderErrorReprint;

            _printer.Print -= _printerPrint;
            _printer.Print -= _printerPrintReprint;
            _printer.Print += _printerPrintReprint;
            _printer.JobError -= _printerJobError;
            _printer.JobError -= _printerJobErrorReprint;
            _printer.JobError += _printerJobErrorReprint;

            _invoiceStatusSaver.Save -= _invoiceStatusSaverSave;
            _invoiceStatusSaver.Save -= _invoiceStatusSaverSaveReprint;
            _invoiceStatusSaver.Save += _invoiceStatusSaverSaveReprint;
            _invoiceStatusSaver.Error -= _invoiceStatusSaverError;
            _invoiceStatusSaver.Error -= _invoiceStatusSaverErrorReprint;
            _invoiceStatusSaver.Error += _invoiceStatusSaverErrorReprint;

            _printController.Complete -= _printControllerComplete;
            _printController.Complete -= _printControllerCompleteReprint;
            _printController.Complete += _printControllerCompleteReprint;
        }

        private void StartPrintJob(List<PrintPackageWrapper> aInvoiceList, string aPrinterName, bool aIsReprint, bool aIsSequenceNumberEnabled, bool aIsPackJacket, bool aPrintPickList)
        {
            if (aIsReprint)
            {
                SetReprintEventHandlers();
                _invoiceStatusSaver.IsReprint = true;
                _invoiceProvider.IsReprint = true;
                _invoiceProvider.LockPackages = true;
            }
            else
            {
                SetPrintEventHandlers();
                _invoiceStatusSaver.IsReprint = false;
                _invoiceProvider.IsReprint = false;
                _invoiceProvider.LockPackages = true;
            }

            _printerError = false;
            _printerErrorMonitorResetEvent.Reset();
            _bwPrinterErrorMonitor.RunWorkerAsync();

            _printer.SetName(aPrinterName);
            _printer.IsSequenceNumberEnabled = aIsSequenceNumberEnabled;
            _printer.PrintPickList = aPrintPickList;
            _printController.setJob(aInvoiceList, true, aIsSequenceNumberEnabled, aIsPackJacket);

            _printController.run();
        }

        private void SetPrintControlsEnabled(bool aEnabled)
        {
            SetControlEnabled(cbSubset, aEnabled);
            SetControlEnabled(btExportErrors, aEnabled);
            SetControlEnabled(cbPrinter, aEnabled);
            SetControlEnabled(btReload, aEnabled);
        }

        private void tcQueries_Deselecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPageIndex != _unshipTabIndex
                && e.TabPageIndex != _unshippedTabIndex
                && e.TabPageIndex != _repairTabIndex
                && _printController.State == PrintControllerState.RUNNING)
            {
                MessageBox.Show(this, @"You cannot leave this tab while printing. First stop printing process.", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                e.Cancel = true;
            }
        }

        private void btReload_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            _invoiceStorage.SetQuery(tcQueries.SelectedIndex);

            // remove sort glyphs
            RemoveSortGlyph(dgvQuery);
            RemoveSortGlyph(dgvSubset);

            if (cbSubset.SelectedIndex == 0)
                _invoiceStorage.SetSubsetAll();
            else
                cbSubset.SelectedIndex = 0;

            Cursor.Current = Cursors.Default;

            MessageBox.Show(this, @"Invoce list reloaded.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void miSubsetAddToUnship_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            _unship.AddSingle((from DataGridViewRow row in dgvSubset.SelectedRows select (row.Tag as PrintPackageWrapper).TrackingNumber).ToArray());

            Cursor.Current = Cursors.Default;

            MessageBox.Show(this, @"Selected invoice(s) added to unship list.", @"Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OnUnshippedFilterClick(object sender, EventArgs e)
        {
            UpdateUnshippedFilter();
        }

        public void UpdateUnshippedFilter()
        {
            foreach (DataGridViewRow row in dgvUnshipped.Rows)
            {
                var package = (UnshippedPackageWrapper)row.Tag;

                var show = (chkUnshippedShowNonupdated.Checked && package.State == UnshippedPackageWrapper.StateType.ORIGINAL) ||
                           (chkUnshippedShowUpdated.Checked && package.State == UnshippedPackageWrapper.StateType.UPDATED_ONHOLD) ||
                           (chkUnshippedShowUpdated.Checked && package.State == UnshippedPackageWrapper.StateType.UPDATED_PICKABLE) ||
                           (chkUnshippedShowFailed.Checked && package.State == UnshippedPackageWrapper.StateType.ERROR);

                row.Visible = show;
            }
        }

        public void UpdateUnshippedStat()
        {
            var nNonUpdated = 0;
            var nUpdated = 0;
            var nFailed = 0;

            foreach (var package in _unshipped.PackageList)
                switch (package.State)
                {
                    case UnshippedPackageWrapper.StateType.ORIGINAL:
                        nNonUpdated++;
                        break;

                    case UnshippedPackageWrapper.StateType.UPDATED_ONHOLD:
                    case UnshippedPackageWrapper.StateType.UPDATED_PICKABLE:
                        nUpdated++;
                        break;

                    case UnshippedPackageWrapper.StateType.ERROR:
                        nFailed++;
                        break;
                }

            tsslUnshippedTotal.Text = $@"Total: {_unshipped.PackageList.Count}";
            tsslNonupdated.Text = $@"Nonupdated: {nNonUpdated}";
            tsslUnshippedUpdated.Text = $@"Updated: {nUpdated}";
            tsslUnshippedFailed.Text = $@"Failed: {nFailed}";
        }

        private void OnReprintFilterClick(object sender, EventArgs e)
        {
            UpdateReprintFilter();
        }

        private void UpdateReprintFilter()
        {
            foreach (DataGridViewRow row in dgvReprint.Rows)
            {
                var package = (PrintPackageWrapper)row.Tag;
                row.Visible = (chkReprintShowUnprinted.Checked && package.IsUnprinted) || (chkReprintShowPrinted.Checked && package.IsPrinted) || (chkReprintShowFailed.Checked && package.IsError) ||
                              (chkReprintShowLocked.Checked && package.IsLocked);
                ;
            }
        }

        private void UpdateReprintStat()
        {
            var nUnprinted = 0;
            var nPrinted = 0;
            var nFailed = 0;
            var nLocked = 0;

            foreach (var package in _reprint.PackageList)
            {
                if (package.IsUnprinted) nUnprinted++;
                if (package.IsPrinted) nPrinted++;
                if (package.IsError) nFailed++;
                if (package.IsLocked) nLocked++;
            }

            tsslReprintTotal.Text = $@"Total: {_reprint.PackageList.Count}";
            tsslReprintUnprinted.Text = $@"Unprinted: {nUnprinted}";
            tsslReprintPrinted.Text = $@"Printed: {nPrinted}";
            tsslReprintFailed.Text = $@"Failed: {nFailed}";
            tsslReprintLocked.Text = $@"Locked: {nLocked}";
        }

        private void OnRepairFilterClick(object sender, EventArgs e)
        {
            UpdateRepairFilter();
        }

        private void UpdateRepairFilter()
        {
            foreach (DataGridViewRow row in dgvRepair.Rows)
            {
                var package = row.Tag as RepairPackageWrapper;
                row.Visible = (chkRepairShowNonrepaired.Checked && package.State == RepairPackageWrapper.StateType.ORIGINAL) ||
                              (chkRepairShowRepaired.Checked && package.State == RepairPackageWrapper.StateType.REPAIRED);
            }
        }

        private void UpdateRepairStat()
        {
            var nNonRepaired = 0;
            var nRepaired = 0;

            foreach (var package in _repair.PackageList)
            {
                if (package.State == RepairPackageWrapper.StateType.ORIGINAL) nNonRepaired++;
                if (package.State == RepairPackageWrapper.StateType.REPAIRED) nRepaired++;
            }

            tsslRepairTotal.Text = $@"Total: {_repair.PackageList.Count}";
            tsslRepairNonrepaired.Text = $@"Nonrepaired: {nNonRepaired}";
            tsslRepairRepaired.Text = $@"Repaired: {nRepaired}";
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_printController.State == PrintControllerState.RUNNING)
            {
                MessageBox.Show(this, @"You shouldn't exit application while printing. First stop printing process.", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }
        }

        private void dgvSubset_Sorted(object sender, EventArgs e)
        {
            if (cbSubset.SelectedIndex != 1) // not custom
            {
                MessageBox.Show(@"Invoice sequence number printing is disabled because of custom list sorting.", @"Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                chkPrintSequenceNumber.Checked = false;
            }
        }

        private void miPreviewSubsetInvoiceWithSequenceNumber_Click(object sender, EventArgs e)
        {
            PreviewInvoice((dgvSubset.CurrentRow.Tag as PrintPackageWrapper).PackageId,
                Routines.GenerateSequenceNumber(0, 0, 0, 0), false);
        }

        private void previewPackJacketInvoiceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PreviewInvoice((dgvSubset.CurrentRow.Tag as PrintPackageWrapper).PackageId, null, true);
        }

        private void previewPackJacketInvoiceWithSequenceNumberToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PreviewInvoice((dgvSubset.CurrentRow.Tag as PrintPackageWrapper).PackageId,
                Routines.GenerateSequenceNumber(0, 0, 0, 0), true);
        }

        private void chkPrintSequenceNumber_Click(object sender, EventArgs e)
        {
            /* if (!chkPrintSequenceNumber.Checked) {
              chkPrintPickList.Checked = false;
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

        private class Stat
        {
            public int _failed;
            public int _locked;
            public int _printed;
            public int _unprinted;
        }


        private delegate void ProcessPrinterErrorCallback();

        private delegate void SetControlTextCallback(Control control, string text);

        private delegate void SetControlEnabledCallback(Control control, bool enabled);

        private delegate void ShowMessageBoxCallback(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon);

        private delegate void UpdateSubsetCallback(object sender, EventArgs e);

        private delegate void UpdateSetDelegate(object sender, EventArgs e);

        private delegate void SetRowStyleCallback(DataGridViewRow row, PrintPackageWrapper package);

        private delegate void FindStartIndexCallback();
    }
}