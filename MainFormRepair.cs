using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PrintInvoice
{
    public partial class MainForm : Form
    {
        private void repair_UpdateList(object sender, EventArgs e)
        {
            if (!_repairFieldNamesIsSet)
            {
                dgvRepair.ColumnCount = _repair.FieldNames == null ? 0 : _repair.FieldNames.Length;

                if (_repair.FieldNames != null)
                    for (var i = 0; i < _repair.FieldNames.Length; i++)
                    {
                        dgvRepair.Columns[i].HeaderText = _repair.FieldNames[i];
                        dgvRepair.Columns[i].SortMode = DataGridViewColumnSortMode.Automatic;
                    }

                // copy to clipboard context menu values
                fillCopyToClipboardMenuItem(miRepairCopyToClipboard, dgvRepair);

                _repairFieldNamesIsSet = true;
            }

            dgvRepair.RowCount = _repair.PackageList.Count;
            for (var i = 0; i < _repair.PackageList.Count; i++)
            {
                dgvRepair.Rows[i].Tag = _repair.PackageList[i];
                if (_repair.PackageList[i].FieldValueList != null)
                    for (var j = 0; j < _repair.PackageList[i].FieldValueList.Length; j++)
                        dgvRepair.Rows[i].Cells[j].Value = _repair.getTypedFieldValue(i, j);

                // set cells styles
                foreach (DataGridViewCell cell in dgvRepair.Rows[i].Cells)
                {
                    var package = _repair.PackageList[i];
                    if (package.State == RepairPackageWrapper.StateType.REPAIRED)
                        cell.Style = cell.ColumnIndex == 0 ? _repairRepairedFirstCellStyle : _repairRepairedCellStyle;
                    else
                        cell.Style = cell.ColumnIndex == 0 ? _repairOriginalFirstCellStyle : _repairOriginalCellStyle;
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

            updateRepairFilter();
            updateRepairStat();
        }

        private void btRepairReload_Click(object sender, EventArgs e)
        {
            _repair.load();
        }

        private void btRepair_Click(object sender, EventArgs e)
        {
            var packageIdList = new List<int>();
            foreach (DataGridViewRow row in dgvRepair.SelectedRows)
                if (row.Visible && (row.Tag as RepairPackageWrapper).State != RepairPackageWrapper.StateType.REPAIRED)
                    packageIdList.Add((row.Tag as RepairPackageWrapper).PackageId);

            if (packageIdList.Count == 0)
            {
                MessageBox.Show(
                    this,
                    "Nothing to repair. Please select rows to repair.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            else
            {
                _repair.repair(packageIdList);

                updateRepairStat();

                MessageBox.Show(
                    this,
                    "Packages records successfully repaired.",
                    "Message",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }

        private void btRepairExportPackagesErrors_Click(object sender, EventArgs e)
        {
            var request = new GetErrorsRequestType();
            var response = _labelService.getErrors(request);

            if (response.status != 0)
                throw new Exception(
                    $"Label service returns error status\nStatus: {response.status}\nMessage: {response.message}\nSubstatus: {response.substatus}\nSubmessage: {response.submessage}");

            var errorList = new List<string>();
            if (response.itemList != null)
                foreach (var item in response.itemList)
                    errorList.Add($"Package: {item.packageId}. Error: {item.errorMessage}");
            saveErrors(errorList);
        }


        private void btRepairExportErrors_Click(object sender, EventArgs e)
        {
            var errorList = new List<string>();
            foreach (DataGridViewRow row in dgvRepair.Rows)
            {
                var package = row.Tag as RepairPackageWrapper;
                if (package.State == RepairPackageWrapper.StateType.ERROR)
                    errorList.Add($"Package: {package.PackageId}. Error: {package.ErrorText}");
            }

            saveErrors(errorList);
        }

        private void btRepairSelectAll_Click(object sender, EventArgs e)
        {
            dgvRepair.SelectAll();
        }

        private void btRepairClearSelection_Click(object sender, EventArgs e)
        {
            dgvRepair.ClearSelection();
        }
    }
}