using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace PrintInvoice
{
    public partial class MainForm
    {
        private void unship_UpdateList(object sender, EventArgs e)
        {
            if (!_unshipFieldNamesIsSet)
            {
                dgvUnship.ColumnCount = (_unship.FieldNames == null ? 0 : _unship.FieldNames.Length) + 1;

                dgvUnship.Columns[0].HeaderText = "Tracking Number";
                dgvUnship.Columns[0].SortMode = DataGridViewColumnSortMode.Automatic;

                if (_unship.FieldNames != null)
                {
                    for (var i = 1; i < _unship.FieldNames.Length + 1; i++)
                    {
                        dgvUnship.Columns[i].HeaderText = _unship.FieldNames[i - 1];
                        dgvUnship.Columns[i].SortMode = DataGridViewColumnSortMode.Automatic;
                    }

                    // copy to clipboard context menu values
                    FillCopyToClipboardMenuItem(miUnshipCopyToClipboard, dgvUnship);
                }

                _unshipFieldNamesIsSet = true;
            }

            dgvUnship.RowCount = _unship.PackageList.Count;
            for (var i = 0; i < _unship.PackageList.Count; i++)
            {
                dgvUnship.Rows[i].Tag = _unship.PackageList[i];
                dgvUnship.Rows[i].Cells[0].Value = _unship.PackageList[i].TrackingNumber;
                if (_unship.PackageList[i].FieldValueList != null)
                    for (var j = 1; j < _unship.PackageList[i].FieldValueList.Length + 1; j++)
                        dgvUnship.Rows[i].Cells[j].Value = _unship.getTypedFieldValue(i, j - 1);

                // set cells styles
                foreach (DataGridViewCell cell in dgvUnship.Rows[i].Cells)
                    switch (_unship.PackageList[i].State)
                    {
                        case UnshipPackageWrapper.PackageStateType.ERROR:
                            cell.Style = cell.ColumnIndex == 0 ? _unshipErrorFirstCellStyle : _unshipErrorCellStyle;
                            break;

                        case UnshipPackageWrapper.PackageStateType.SHIPPED:
                            cell.Style = cell.ColumnIndex == 0 ? _defaultFirstCellStyle : dgvUnship.DefaultCellStyle;
                            break;

                        case UnshipPackageWrapper.PackageStateType.UNSHIPPED:
                            cell.Style = cell.ColumnIndex == 0
                                ? _unshipUnshippedFirstCellStyle
                                : _unshipUnshippedCellStyle;
                            break;
                    }

                if (_unship.PackageList[i].State == UnshipPackageWrapper.PackageStateType.ERROR)
                    dgvUnship.Rows[i].ErrorText = _unship.PackageList[i].ErrorText;
                else
                    dgvUnship.Rows[i].ErrorText = "";
            }

            updateUnshipFilter();
            updateUnshipStat();
        }

        private void updateUnshipFilter()
        {
            foreach (DataGridViewRow row in dgvUnship.Rows)
            {
                var visible = false;
                var package = row.Tag as UnshipPackageWrapper;
                if (chkUnshipFilterShowShipped.Checked &&
                    package.State == UnshipPackageWrapper.PackageStateType.SHIPPED) visible = true;
                if (chkUnshipFilterShowUnshipped.Checked &&
                    package.State == UnshipPackageWrapper.PackageStateType.UNSHIPPED) visible = true;
                if (chkUnshipFilterShowFailed.Checked && package.State == UnshipPackageWrapper.PackageStateType.ERROR)
                    visible = true;
                row.Visible = visible;
            }
        }

        private void updateUnshipStat()
        {
            var shipped = 0;
            var unshipped = 0;
            var failed = 0;

            foreach (DataGridViewRow row in dgvUnship.Rows)
            {
                var package = row.Tag as UnshipPackageWrapper;
                switch (package.State)
                {
                    case UnshipPackageWrapper.PackageStateType.SHIPPED:
                        shipped++;
                        break;

                    case UnshipPackageWrapper.PackageStateType.UNSHIPPED:
                        unshipped++;
                        break;

                    case UnshipPackageWrapper.PackageStateType.ERROR:
                        failed++;
                        break;
                }
            }

            tsslUnshipTotal.Text = $"Total: {dgvUnship.RowCount}";
            tsslUnshipShipped.Text = $"Shipped: {shipped}";
            tsslUnshipUnshipped.Text = $"Unshipped: {unshipped}";
            tsslUnshipFailed.Text = $"Failed: {failed}";
        }

        private void btAddUnshipInvoice_Click(object sender, EventArgs e)
        {
            if (_fmAddUnshipInvoice == null) _fmAddUnshipInvoice = new AddPackageForm();

            _fmAddUnshipInvoice.tbInvoiceIdList.Clear();
            _fmAddUnshipInvoice.Result = AddPackageForm.ResultType.CANCEL;
            _fmAddUnshipInvoice.ActiveControl = _fmAddUnshipInvoice.tbInvoiceIdList;
            _fmAddUnshipInvoice.ShowDialog();
            switch (_fmAddUnshipInvoice.Result)
            {
                case AddPackageForm.ResultType.ADD_SINGLE:
                    _unship.addSingle(_fmAddUnshipInvoice.tbInvoiceIdList.Lines);
                    break;

                case AddPackageForm.ResultType.ADD_BATCH:
                    _unship.addBatch(_fmAddUnshipInvoice.tbInvoiceIdList.Lines);
                    break;
            }
        }

        private void chkUnshipFilterShowShipped_CheckedChanged(object sender, EventArgs e)
        {
            updateUnshipFilter();
        }

        private void miUnshipRemoveSelected_Click(object sender, EventArgs e)
        {
            var packageList = new List<UnshipPackageWrapper>();
            foreach (DataGridViewRow row in dgvUnship.SelectedRows) packageList.Add(row.Tag as UnshipPackageWrapper);
            _unship.remove(packageList);
            dgvUnship.ClearSelection();
        }

        private void cmsUnship_Opening(object sender, CancelEventArgs e)
        {
            miUnshipRemoveSelected.Enabled = dgvUnship.SelectedRows.Count > 0;
        }

        private void btUnshipClear_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                    this,
                    "Do you want to clear unship list?",
                    "Question",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                ) == DialogResult.Yes)
                _unship.clear();
        }

        private void btExportUnshipErrors_Click(object sender, EventArgs e)
        {
            var errorList = new List<string>();
            foreach (DataGridViewRow row in dgvUnship.Rows)
            {
                var package = row.Tag as UnshipPackageWrapper;
                if (package.State == UnshipPackageWrapper.PackageStateType.ERROR)
                    errorList.Add($"Package ID: {package.TrackingNumber}. Error: {package.ErrorText}");
            }

            SaveErrors(errorList);
        }

        private void btUnship_Click(object sender, EventArgs e)
        {
            if (_unship.PackageList.Count == 0)
            {
                MessageBox.Show(
                    this,
                    "Nothing to unship",
                    "Message",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation
                );
            }
            else
            {
                if (MessageBox.Show(
                        this,
                        "Do you want to unship list?",
                        "Question",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    ) == DialogResult.Yes)
                {
                    _unship.unship(out var success, out var fail);

                    MessageBox.Show(
                        this,
                        $"Total {success + fail} packages was requested to unship.\n{success} was unshipped successfully, {fail} was not.",
                        "Message",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
        }


        private void btUnshipLastBatches_Click(object sender, EventArgs e)
        {
            if (_fmLastBatches == null) _fmLastBatches = new LastBatchesForm(_labelService, _config);

            // load data
            if (_fmLastBatches.load() > 0)
            {
                if (_fmLastBatches.ShowDialog() == DialogResult.OK)
                    if (_unship.addBatchById(_fmLastBatches.BatchId) == 0)
                        MessageBox.Show(
                            this,
                            "No invoices found in the selected batch.",
                            "Message",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
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
    }
}