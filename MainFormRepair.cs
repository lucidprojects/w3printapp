using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;


namespace PrintInvoice
{
  public partial class MainForm : Form
  {
    void repair_UpdateList(object sender, EventArgs e)
    {
      if (!repairFieldNamesIsSet)
      {
        dgvRepair.ColumnCount = repair.FieldNames == null ? 0 : repair.FieldNames.Length;

        if (repair.FieldNames != null)
        {
          for (int i = 0; i < repair.FieldNames.Length; i++)
          {
            dgvRepair.Columns[i].HeaderText = repair.FieldNames[i];
            dgvRepair.Columns[i].SortMode = DataGridViewColumnSortMode.Automatic;
          }
        }

        // copy to clipboard context menu values
        fillCopyToClipboardMenuItem(miRepairCopyToClipboard, dgvRepair);

        repairFieldNamesIsSet = true;
      }

      dgvRepair.RowCount = repair.PackageList.Count;
      for (int i = 0; i < repair.PackageList.Count; i++)
      {
        dgvRepair.Rows[i].Tag = repair.PackageList[i];
        if (repair.PackageList[i].FieldValueList != null)
        {
          for (int j = 0; j < repair.PackageList[i].FieldValueList.Length; j++)
          {
            dgvRepair.Rows[i].Cells[j].Value = repair.getTypedFieldValue(i, j);
          }
        }

        // set cells styles
        foreach (DataGridViewCell cell in dgvRepair.Rows[i].Cells)
        {
          RepairPackageWrapper package = repair.PackageList[i];
          if (package.State == RepairPackageWrapper.StateType.REPAIRED)
          {
            cell.Style = cell.ColumnIndex == 0 ? repairRepairedFirstCellStyle : repairRepairedCellStyle;
          }
          else
          {
            cell.Style = cell.ColumnIndex == 0 ? repairOriginalFirstCellStyle : repairOriginalCellStyle;
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

      updateRepairFilter();
      updateRepairStat();
    }

    private void btRepairReload_Click(object sender, EventArgs e)
    {
      repair.load();
    }

    private void btRepair_Click(object sender, EventArgs e)
    {
      List<int> packageIdList = new List<int>();
      foreach (DataGridViewRow row in dgvRepair.SelectedRows)
      {
        if(row.Visible && (row.Tag as RepairPackageWrapper).State != RepairPackageWrapper.StateType.REPAIRED)
        packageIdList.Add((row.Tag as RepairPackageWrapper).PackageId);
      }

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
        repair.repair(packageIdList);

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
      GetErrorsRequestType request = new GetErrorsRequestType();
      GetErrorsResponseType response = labelService.getErrors(request);

      if (response.status != 0)
      {
        throw new Exception(String.Format("Label service returns error status\nStatus: {0}\nMessage: {1}\nSubstatus: {2}\nSubmessage: {3}", response.status, response.message, response.substatus, response.submessage));
      }
      else
      {
        List<string> errorList = new List<string>();
        if (response.itemList != null)
        {
          foreach (GetErrorsResponseItemType item in response.itemList)
          {
            errorList.Add(String.Format("Package: {0}. Error: {1}", item.packageId, item.errorMessage));
          }
        }
        saveErrors(errorList);
      }
    }


    private void btRepairExportErrors_Click(object sender, EventArgs e)
    {
      List<string> errorList = new List<string>();
      foreach (DataGridViewRow row in dgvRepair.Rows)
      {
        RepairPackageWrapper package = row.Tag as RepairPackageWrapper;
        if (package.State == RepairPackageWrapper.StateType.ERROR)
        {
          errorList.Add(String.Format("Package: {0}. Error: {1}", package.PackageId, package.ErrorText));
        }
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
