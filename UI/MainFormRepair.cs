using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PrintInvoice.UI
{
    public partial class MainForm
    {
        private void repair_UpdateList(object sender, EventArgs e)
        {
            if (!_repairFieldNamesIsSet)
            {
                dgvRepair.ColumnCount = _repair.FieldNames?.Length ?? 0;

                if (_repair.FieldNames != null)
                {
                    for (var i = 0; i < _repair.FieldNames.Length; i++)
                    {
                        dgvRepair.Columns[i].HeaderText = _repair.FieldNames[i];
                        dgvRepair.Columns[i].SortMode = DataGridViewColumnSortMode.Automatic;
                    }
                }

                // copy to clipboard context menu values
                FillCopyToClipboardMenuItem(miRepairCopyToClipboard, dgvRepair);

                _repairFieldNamesIsSet = true;
            }

            dgvRepair.RowCount = _repair.PackageList.Count;
            
            for (var i = 0; i < _repair.PackageList.Count; i++)
            {
                dgvRepair.Rows[i].Tag = _repair.PackageList[i];
                
                if (_repair.PackageList[i].FieldValueList != null)
                {
                    for (var j = 0; j < _repair.PackageList[i].FieldValueList.Length; j++)
                        dgvRepair.Rows[i].Cells[j].Value = _repair.GetTypedFieldValue(i, j);
                }

                // set cells styles
                foreach (DataGridViewCell cell in dgvRepair.Rows[i].Cells)
                {
                    var package = _repair.PackageList[i];

                    cell.Style = package.State == RepairPackageWrapper.StateType.REPAIRED 
                        ? cell.ColumnIndex == 0 ? _repairRepairedFirstCellStyle : _repairRepairedCellStyle 
                        : cell.ColumnIndex == 0 ? _repairOriginalFirstCellStyle : _repairOriginalCellStyle;
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

            UpdateRepairFilter();
            UpdateRepairStat();
        }

        private void btRepairReload_Click(object sender, EventArgs e)
        {
            _repair.Load();
        }

        private void btRepair_Click(object sender, EventArgs e)
        {
            var packageIdList = dgvRepair.SelectedRows.Cast<DataGridViewRow>()
                .Where(row => row.Visible && ((RepairPackageWrapper)row.Tag).State != RepairPackageWrapper.StateType.REPAIRED)
                .Select(row => ((RepairPackageWrapper)row.Tag).PackageId).ToList();

            if (packageIdList.Count == 0)
            {
                MessageBox.Show(this, @"Nothing to repair. Please select rows to repair.", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                _repair.repair(packageIdList);
                UpdateRepairStat();
                MessageBox.Show(this, @"Packages records successfully repaired.", @"Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btRepairExportPackagesErrors_Click(object sender, EventArgs e)
        {
            var request = new GetErrorsRequestType();
            var response = _labelService.getErrors(request);

            if (response.status != 0)
                throw new Exception($"Label service returns error status\nStatus: {response.status}\nMessage: {response.message}\nSubstatus: {response.substatus}\nSubmessage: {response.submessage}");

            var errorList = new List<string>();
            
            if (response.itemList != null)
            {
                errorList.AddRange(response.itemList.Select(item => $"Package: {item.packageId}. Error: {item.errorMessage}"));
            }

            SaveErrors(errorList);
        }


        private void btRepairExportErrors_Click(object sender, EventArgs e)
        {
            var errorList = dgvRepair.Rows.Cast<DataGridViewRow>()
                .Select(row => (RepairPackageWrapper)row.Tag)
                .Where(package => package.State == RepairPackageWrapper.StateType.ERROR)
                .Select(package => $"Package: {package.PackageId}. Error: {package.ErrorText}").ToList();

            SaveErrors(errorList);
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