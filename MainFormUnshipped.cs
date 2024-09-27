using System;
using System.Linq;
using System.Windows.Forms;

namespace PrintInvoice
{
    public partial class MainForm 
    {
        private SetMaxDailyPackagesForm _fmSetMaxDailyPackages;

        private void unshipped_UpdateList(object sender, EventArgs e)
        {
            if (!_unshippedFieldNamesIsSet)
            {
                dgvUnshipped.ColumnCount = _unshipped.FieldNames?.Length ?? 0;

                if (_unshipped.FieldNames != null)
                {
                    for (var i = 0; i < _unshipped.FieldNames.Length; i++)
                    {
                        dgvUnshipped.Columns[i].HeaderText = _unshipped.FieldNames[i];
                        dgvUnshipped.Columns[i].SortMode = DataGridViewColumnSortMode.Automatic;
                    }
                }

                FillCopyToClipboardMenuItem(miUnshippedCopyToClipboard, dgvUnshipped);

                _unshippedFieldNamesIsSet = true;
            }

            dgvUnshipped.RowCount = _unshipped.PackageList.Count;

            for (var i = 0; i < _unshipped.PackageList.Count; i++)
            {
                dgvUnshipped.Rows[i].Tag = _unshipped.PackageList[i];
                dgvUnshipped.Rows[i].ErrorText = "";
            
                if (_unshipped.PackageList[i].FieldValueList != null)
                {
                    for (var j = 0; j < _unshipped.PackageList[i].FieldValueList.Length; j++)
                        dgvUnshipped.Rows[i].Cells[j].Value = _unshipped.GetTypedFieldValue(i, j);
                }

                // set cells styles
                foreach (DataGridViewCell cell in dgvUnshipped.Rows[i].Cells)
                {
                    var package = _unshipped.PackageList[i];
                
                    switch (package.State)
                    {
                        case UnshippedPackageWrapper.StateType.UPDATED_PICKABLE:
                            cell.Style = cell.ColumnIndex == 0
                                ? _unshippedUpdatedPickableFirstCellStyle
                                : _unshippedUpdatedPickableCellStyle;
                            break;

                        case UnshippedPackageWrapper.StateType.UPDATED_ONHOLD:
                            cell.Style = cell.ColumnIndex == 0
                                ? _unshippedUpdatedOnholdFirstCellStyle
                                : _unshippedUpdatedOnholdCellStyle;
                            break;

                        case UnshippedPackageWrapper.StateType.ERROR:
                            cell.Style = cell.ColumnIndex == 0 ? _errorFirstCellStyle : _errorCellStyle;
                            cell.OwningRow.ErrorText = package.ErrorText;
                            break;

                        default:
                            switch (package.Status)
                            {
                                case UnshippedPackageWrapper.StatusNew:
                                    cell.Style = cell.ColumnIndex == 0
                                        ? _unshippedNewFirstCellStyle
                                        : _unshippedNewCellStyle;
                                    break;

                                case UnshippedPackageWrapper.StatusUnshipped:
                                    cell.Style = cell.ColumnIndex == 0
                                        ? _unshippedUnshippedFirstCellStyle
                                        : _unshippedUnshippedCellStyle;
                                    break;
                            }

                            break;
                    }
                }

                /*if (unship.PackageList[i].State == UnshipPackageWrapper.PackageStateType.ERROR)
                {
                  dgvUnship.Rows[i].ErrorText = unship.PackageList[i].ErrorMessage;
                }
                else
                {
                  dgvUnship.Rows[i].ErrorText = "";
                }*/
            }

            //updateUnshipFilter();
            //updateUnshipStat();
        }

        private void btReloadUnshipped_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, @"Warning! If you reload packages you will not be able to reset status. Continue?", @"Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _unshipped.Load();
                UpdateUnshippedStat();
            }
        }

        private void btUnshippedSelectAll_Click(object sender, EventArgs e)
        {
            dgvUnshipped.SelectAll();
        }

        private void btUnshippedClearSelection_Click(object sender, EventArgs e)
        {
            dgvUnshipped.ClearSelection();
        }

        private void btUnshippedSetPickable_Click(object sender, EventArgs e)
        {
            var list = dgvUnshipped.SelectedRows.Cast<DataGridViewRow>()
                .Where(row => row.Visible)
                .Select(row => (UnshippedPackageWrapper)row.Tag)
                .Where(package => package.State != UnshippedPackageWrapper.StateType.ERROR && package.State != UnshippedPackageWrapper.StateType.UPDATED_PICKABLE &&
                                  package.State != UnshippedPackageWrapper.StateType.UPDATED_ONHOLD)
                .Select(package => new PackageStatus(package.PackageId, UnshippedPackageWrapper.StatusPickable, UnshippedPackageWrapper.StatusNew)).ToList();

            if (list.Count == 0)
            {
                MessageBox.Show(this, @"Nothing to update", @"Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                //list.Reverse(); // selected list in reverse order
                _unshipped.SetPickable(list, out var failed);
                UpdateUnshippedStat();

                MessageBox.Show(this, $@"Operation completed.
Total requested: {list.Count}.
Failed: {failed}.", @"Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btUnshippedResetStatus_Click(object sender, EventArgs e)
        {
            var list = dgvUnshipped.SelectedRows.Cast<DataGridViewRow>()
                .Where(row => row.Visible)
                .Select(row => (UnshippedPackageWrapper)row.Tag)
                .Where(package => (package.State == UnshippedPackageWrapper.StateType.UPDATED_PICKABLE || package.State == UnshippedPackageWrapper.StateType.UPDATED_ONHOLD) &&
                                  package.State != UnshippedPackageWrapper.StateType.ERROR)
                .ToList();

            if (list.Count == 0)
            {
                MessageBox.Show(this, @"Nothing to update", @"Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                //list.Reverse(); // selected list in reverse order
                _unshipped.ResetStatus(list);
                UpdateUnshippedStat();

                MessageBox.Show(this, @"Operation completed.", @"Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void unshipped_UpdateMaxDailyPackages(object sender, EventArgs e)
        {
            //edMaxDailyPackages.Text = unshipped.MaxDailyPackages.ToString();
            //edPmodMaxDailyPackages.Text = unshipped.PmodMaxDailyPackages.ToString();
        }
        
        private void btSetPmodMaxDailyPackages_Click(object sender, EventArgs e)
        {
            if (_fmSetMaxDailyPackages == null) _fmSetMaxDailyPackages = new SetMaxDailyPackagesForm();
            _fmSetMaxDailyPackages.Text = @"PMOD Max Daily Packages";
            _fmSetMaxDailyPackages.Value = _unshipped.PmodMaxDailyPackages;

            if (_fmSetMaxDailyPackages.ShowDialog() == DialogResult.OK)
            {
                if (_fmSetMaxDailyPackages.Value != _unshipped.PmodMaxDailyPackages)
                {
                    _unshipped.SetPmodMaxDailyPackages(_fmSetMaxDailyPackages.Value);
                    MessageBox.Show(this, @"Value successfully changed", @"Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(this, @"Nothing to update", @"Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}