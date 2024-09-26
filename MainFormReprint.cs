using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace PrintInvoice
{
    public partial class MainForm
    {
        private void reprint_UpdateList(object sender, EventArgs e)
        {
            var invoiceList = _reprint.PackageList;

            // field names
            if (!_reprintFieldNamesIsSet)
            {
                dgvReprint.ColumnCount = (_reprint.FieldNames == null ? 0 : _reprint.FieldNames.Length) + 1;

                dgvReprint.Columns[0].HeaderText = "Tracking Number";
                dgvReprint.Columns[0].SortMode = DataGridViewColumnSortMode.Automatic;

                if (_reprint.FieldNames != null)
                    for (var i = 1; i < _reprint.FieldNames.Length + 1; i++)
                    {
                        dgvReprint.Columns[i].HeaderText = _reprint.FieldNames[i - 1];
                        dgvReprint.Columns[i].SortMode = DataGridViewColumnSortMode.Automatic;
                    }

                // copy to clipboard context menu values
                FillCopyToClipboardMenuItem(miReprintCopyToClipboard, dgvReprint);

                _reprintFieldNamesIsSet = true;
            }

            _reprintRowIndex.Clear();
            dgvReprint.RowCount = _reprint.PackageList.Count;
            for (var i = 0; i < _reprint.PackageList.Count; i++)
            {
                dgvReprint.Rows[i].Tag = _reprint.PackageList[i];
                dgvReprint.Rows[i].Cells[0].Value = _reprint.PackageList[i].TrackingNumber;
                if (_reprint.PackageList[i].FieldValueList != null)
                    for (var j = 1; j < _reprint.PackageList[i].FieldValueList.Length + 1; j++)
                        dgvReprint.Rows[i].Cells[j].Value = _reprint.getTypedFieldValue(i, j - 1);
                _reprintRowIndex[_reprint.PackageList[i].PackageId] = dgvReprint.Rows[i];

                // set cells styles
                SetRowStyle(dgvReprint.Rows[i], _reprint.PackageList[i]);

                if (_reprint.PackageList[i].IsError)
                    dgvReprint.Rows[i].ErrorText = _reprint.PackageList[i].ErrorText;
                else
                    dgvReprint.Rows[i].ErrorText = "";
            }

            UpdateReprintFilter();
            UpdateReprintStat();
        }

        private void btAddReprintPackage_Click(object sender, EventArgs e)
        {
            if (_fmAddUnshipInvoice == null) _fmAddUnshipInvoice = new AddPackageForm();

            _fmAddUnshipInvoice.tbInvoiceIdList.Clear();
            _fmAddUnshipInvoice.Result = AddPackageForm.ResultType.CANCEL;
            _fmAddUnshipInvoice.ActiveControl = _fmAddUnshipInvoice.tbInvoiceIdList;
            _fmAddUnshipInvoice.ShowDialog();
            switch (_fmAddUnshipInvoice.Result)
            {
                case AddPackageForm.ResultType.ADD_SINGLE:
                    _reprint.addSingle(_fmAddUnshipInvoice.tbInvoiceIdList.Lines);
                    break;

                case AddPackageForm.ResultType.ADD_BATCH:
                    _reprint.addBatch(_fmAddUnshipInvoice.tbInvoiceIdList.Lines);
                    break;
            }

            UpdateReprintFilter();
            UpdateReprintStat();
        }

        private void cmsReprint_Opening(object sender, CancelEventArgs e)
        {
            if (dgvReprint.SelectedRows.Count == 0
                || _printController.State == PrintControllerState.RUNNING)
                tsmiReprintRemoveSelected.Enabled = false;
            else
                tsmiReprintRemoveSelected.Enabled = true;
        }

        private void tsmiReprintRemoveSelected_Click(object sender, EventArgs e)
        {
            var packageList = new List<PrintPackageWrapper>();
            foreach (DataGridViewRow row in dgvReprint.SelectedRows) packageList.Add(row.Tag as PrintPackageWrapper);
            _reprint.remove(packageList);
            dgvReprint.ClearSelection();

            UpdateReprintStat();
        }

        private void btReprintClear_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                    this,
                    "Do you want to clear reprint list?",
                    "Question",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                ) == DialogResult.Yes)
                _reprint.clear();

            UpdateReprintStat();
        }

        private void invoiceProvider_LoadReprint(object sender, InvoiceProviderLoadEventArgs e)
        {
            _reprint.setPackageState(e.PrintInvoiceWrapper.PackageId,
                e.PrintInvoiceWrapper.IsLoaded ? PrintPackageWrapper.Loaded : PrintPackageWrapper.Locked, "");
        }

        private void invoiceProvider_ErrorReprint(object sender, InvoiceProviderErrorEventArgs e)
        {
            _reprint.setPackageState(e.InvoiceId, PrintPackageWrapper.Error, e.Message);
        }

        private void printer_PrintReprint(object sender, PrinterPrintEventArgs e)
        {
            _reprint.setPackageState(e.PackageId, PrintPackageWrapper.Printed, "");
        }

        private void printer_JobErrorReprint(object sender, PrinterJobErrorEventArgs e)
        {
            _reprint.setPackageState(e.PackageId, PrintPackageWrapper.Error, e.Message);
        }

        private void invoiceStatusSaver_SaveReprint(object sender, InvoiceStatusSaverSaveEventArgs e)
        {
            _reprint.setPackageState(e.InvoiceId, PrintPackageWrapper.StatusSaved, "");
        }

        private void invoiceStatusSaver_ErrorReprint(object sender, InvoiceStatusSaverErrorEventArgs e)
        {
            _reprint.setPackageState(e.InvoiceId, PrintPackageWrapper.Error, e.Message);
        }

        private void reprint_UpdatePackageState(object sender, PrintPackageStorageUpdatePackageStateEventArgs e)
        {
            var package = _reprint.getPackageByPackageId(e.PackageId);
            SetRowStyle(_reprintRowIndex[e.PackageId], package);
            UpdateReprintStat();
        }

        private void btReprintPrint_Click(object sender, EventArgs e)
        {
            if (_printController.State == PrintControllerState.RUNNING)
            {
                btReprintPrint.Enabled = false;
                _bwStopReprint.RunWorkerAsync();
            }
            else
            {
                // warning about sequence number
                if (MessageBox.Show(
                        "Invoice sequence number printing is " +
                        (chkReprintSequenceNumber.Checked ? "enabled" : "disabled") + ".\nContinue printing?",
                        "Warning",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    ) == DialogResult.No)
                    return;


                // pack jacket question
                string q;
                q = "You are going to print full invoices (with postage label).\nContinue?";

                if (MessageBox.Show(
                        q,
                        "Confirmation",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    ) == DialogResult.No)
                    return;

                if (dgvReprint.CurrentRow.Index == 0 || (dgvReprint.CurrentRow.Index > 0 && MessageBox.Show(
                        "You are going to print not from the beginning. Continue?",
                        "Confirmation",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    ) == DialogResult.Yes))
                {
                    btReprintPrint.Enabled = false;
                    var packageList = new List<PrintPackageWrapper>();
                    var askRepeatedPrint = true;
                    var askRepeatedPrintResult = true;
                    for (var i = dgvReprint.CurrentRow.Index; i < dgvReprint.Rows.Count; i++)
                        if (dgvReprint.Rows[i].Visible)
                        {
                            var package = dgvReprint.Rows[i].Tag as PrintPackageWrapper;
                            if (package.PackageId != PackageWrapper.NullPackageId)
                            {
                                var add = true;
                                if (package.IsPrinted)
                                {
                                    if (askRepeatedPrint)
                                    {
                                        var fmRepeatedPrint = new RepeatedPrintForm();
                                        fmRepeatedPrint.laMessage.Text =
                                            $"You are going to print invoice No. {dgvReprint.Rows[i].Cells[1].Value} is marked as printed. Do you want to print it again?";
                                        askRepeatedPrintResult = fmRepeatedPrint.ShowDialog() == DialogResult.Yes
                                            ? true
                                            : false;
                                        askRepeatedPrint = !fmRepeatedPrint.ckDontAsk.Checked;
                                    }

                                    add = askRepeatedPrintResult;
                                }

                                if (add)
                                {
                                    package._isPackJacket = false; // chkReprintPackJacket.Checked;
                                    packageList.Add(package);
                                }
                            }
                        }

                    StartPrintJob(packageList, cbReprintPrinter.SelectedItem.ToString(), true,
                        chkReprintSequenceNumber.Checked, false, false);

                    setReprintControlsEnabled(false);

                    btReprintPrint.Text = "Stop";
                    btReprintPrint.Enabled = true;
                }
            }
        }

        private void printController_CompleteReprint(object sender, EventArgs e)
        {
            _bwPrinterErrorMonitor.CancelAsync();
            _printerErrorMonitorResetEvent.WaitOne();
            SetControlText(btReprintPrint, "Print");
            setReprintControlsEnabled(true);
            ShowMessageBox(
                this,
                "Print job complete.",
                "Message",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void btReprintExportErrors_Click(object sender, EventArgs e)
        {
            var errorList = new List<string>();
            foreach (DataGridViewRow row in dgvReprint.Rows)
            {
                var package = row.Tag as PrintPackageWrapper;
                if (package.IsError)
                    errorList.Add($"Package: {package.PackageId}. Error: {package.ErrorText}");
            }

            SaveErrors(errorList);
        }

        private void miPreviewReprintInvoice_Click(object sender, EventArgs e)
        {
            var package = dgvReprint.CurrentRow.Tag as PrintPackageWrapper;
            if (package.PackageId != PackageWrapper.NullPackageId) PreviewInvoice(package.PackageId, null, false);
        }

        private void btReprintLastBatches_Click(object sender, EventArgs e)
        {
            if (_fmLastBatches == null) _fmLastBatches = new LastBatchesForm(_labelService, _config);

            // load data
            Cursor.Current = Cursors.WaitCursor;
            var nLoaded = _fmLastBatches.load();
            Cursor.Current = Cursors.Default;
            if (nLoaded > 0)
            {
                if (_fmLastBatches.ShowDialog() == DialogResult.OK)
                {
                    Cursor.Current = Cursors.WaitCursor;
                    var nAdded = _reprint.addBatchById(_fmLastBatches.BatchId);
                    Cursor.Current = Cursors.Default;
                    if (nAdded == 0)
                        MessageBox.Show(
                            this,
                            "No invoices found in the selected batch.",
                            "Message",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                }
            }
            else
            {
                MessageBox.Show(
                    this,
                    "There is no batches.",
                    "Message",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }

        private void setReprintControlsEnabled(bool aEnabled)
        {
            SetControlEnabled(cbReprintPrinter, aEnabled);
            SetControlEnabled(btAddReprintPackage, aEnabled);
            SetControlEnabled(btReprintLastBatches, aEnabled);
            SetControlEnabled(btReprintClear, aEnabled);
            SetControlEnabled(btReprintExportErrors, aEnabled);
        }
    }
}