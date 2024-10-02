using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace PrintInvoice.UI
{
    public partial class MainForm
    {
        private void unship_UpdateList(object sender, EventArgs e)
        {
            if (!_unshipFieldNamesIsSet)
            {
                dgvUnship.ColumnCount = (_unship.FieldNames?.Length ?? 0) + 1;

                dgvUnship.Columns[0].HeaderText = @"Tracking Number";
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
                {
                    for (var j = 1; j < _unship.PackageList[i].FieldValueList.Length + 1; j++)
                        dgvUnship.Rows[i].Cells[j].Value = _unship.GetTypedFieldValue(i, j - 1);
                }

                // set cells styles
                foreach (DataGridViewCell cell in dgvUnship.Rows[i].Cells)
                {
                    switch (_unship.PackageList[i].State)
                    {
                        case UnshipPackageWrapper.PackageStateType.Error:
                            cell.Style = cell.ColumnIndex == 0 ? _unshipErrorFirstCellStyle : _unshipErrorCellStyle;
                            break;

                        case UnshipPackageWrapper.PackageStateType.Shipped:
                            cell.Style = cell.ColumnIndex == 0 ? _defaultFirstCellStyle : dgvUnship.DefaultCellStyle;
                            break;

                        case UnshipPackageWrapper.PackageStateType.Unshipped:
                            cell.Style = cell.ColumnIndex == 0
                                ? _unshipUnshippedFirstCellStyle
                                : _unshipUnshippedCellStyle;
                            break;
                    }
                }

                dgvUnship.Rows[i].ErrorText = _unship.PackageList[i].State == UnshipPackageWrapper.PackageStateType.Error ? _unship.PackageList[i].ErrorText : "";
            }

            UpdateUnshipFilter();
            UpdateUnshipStat();
        }

        private void UpdateUnshipFilter()
        {
            foreach (DataGridViewRow row in dgvUnship.Rows)
            {
                var visible = false;
                var package = (UnshipPackageWrapper)row.Tag;
                
                if (chkUnshipFilterShowShipped.Checked &&
                    package.State == UnshipPackageWrapper.PackageStateType.Shipped) visible = true;
                
                if (chkUnshipFilterShowUnshipped.Checked &&
                    package.State == UnshipPackageWrapper.PackageStateType.Unshipped) visible = true;
                
                if (chkUnshipFilterShowFailed.Checked && package.State == UnshipPackageWrapper.PackageStateType.Error)
                    visible = true;
                
                row.Visible = visible;
            }
        }

        private void UpdateUnshipStat()
        {
            var shipped = 0;
            var unshipped = 0;
            var failed = 0;

            foreach (DataGridViewRow row in dgvUnship.Rows)
            {
                var package = (UnshipPackageWrapper)row.Tag;
                
                switch (package.State)
                {
                    case UnshipPackageWrapper.PackageStateType.Shipped:
                        shipped++;
                        break;

                    case UnshipPackageWrapper.PackageStateType.Unshipped:
                        unshipped++;
                        break;

                    case UnshipPackageWrapper.PackageStateType.Error:
                        failed++;
                        break;
                }
            }

            tsslUnshipTotal.Text = $@"Total: {dgvUnship.RowCount}";
            tsslUnshipShipped.Text = $@"Shipped: {shipped}";
            tsslUnshipUnshipped.Text = $@"Unshipped: {unshipped}";
            tsslUnshipFailed.Text = $@"Failed: {failed}";
        }

        private void btAddUnshipInvoice_Click(object sender, EventArgs e)
        {
            if (_fmAddUnshipInvoice == null) _fmAddUnshipInvoice = new AddPackageForm();

            _fmAddUnshipInvoice.tbInvoiceIdList.Clear();
            _fmAddUnshipInvoice.Result = AddPackageForm.ResultType.Cancel;
            _fmAddUnshipInvoice.ActiveControl = _fmAddUnshipInvoice.tbInvoiceIdList;
            _fmAddUnshipInvoice.ShowDialog();
            
            switch (_fmAddUnshipInvoice.Result)
            {
                case AddPackageForm.ResultType.AddSingle:
                    _unship.AddSingle(_fmAddUnshipInvoice.tbInvoiceIdList.Lines);
                    break;

                case AddPackageForm.ResultType.AddBatch:
                    _unship.AddBatch(_fmAddUnshipInvoice.tbInvoiceIdList.Lines);
                    break;
            }
        }

        private void chkUnshipFilterShowShipped_CheckedChanged(object sender, EventArgs e)
        {
            UpdateUnshipFilter();
        }

        private void miUnshipRemoveSelected_Click(object sender, EventArgs e)
        {
            var packageList = dgvUnship.SelectedRows.Cast<DataGridViewRow>().Select(row => row.Tag as UnshipPackageWrapper).ToList();
            _unship.Remove(packageList);
            dgvUnship.ClearSelection();
        }

        private void cmsUnship_Opening(object sender, CancelEventArgs e)
        {
            miUnshipRemoveSelected.Enabled = dgvUnship.SelectedRows.Count > 0;
        }

        private void btUnshipClear_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, @"Do you want to clear unship list?", @"Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                _unship.Clear();
        }

        private void btExportUnshipErrors_Click(object sender, EventArgs e)
        {
            var errorList = new List<string>();

            foreach (DataGridViewRow row in dgvUnship.Rows)
            {
                var package = (UnshipPackageWrapper)row.Tag;
                
                if (package.State == UnshipPackageWrapper.PackageStateType.Error)
                    errorList.Add($"Package ID: {package.TrackingNumber}. Error: {package.ErrorText}");
            }

            SaveErrors(errorList);
        }

        private void btUnship_Click(object sender, EventArgs e)
        {
            if (_unship.PackageList.Count == 0)
            {
                MessageBox.Show(this, @"Nothing to unship", @"Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                if (MessageBox.Show(this, @"Do you want to unship list?", @"Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _unship.unship(out var success, out var fail);

                    MessageBox.Show(this, $@"Total {success + fail} packages was requested to unship.
{success} was unshipped successfully, {fail} was not.", @"Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }


        private void btUnshipLastBatches_Click(object sender, EventArgs e)
        {
            if (_fmLastBatches == null) _fmLastBatches = new LastBatchesForm(_labelService, _config);

            // load data
            if (_fmLastBatches.OnLoad() > 0)
            {
                if (_fmLastBatches.ShowDialog() == DialogResult.OK && _unship.AddBatchById(_fmLastBatches.BatchId) == 0)
                {
                    MessageBox.Show(this, @"No invoices found in the selected batch.", @"Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show(this, @"There is no batches.", @"Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}