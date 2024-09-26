using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;


namespace PrintInvoice
{
  public partial class MainForm : Form
  {
    private SetMaxDailyPackagesForm fmSetMaxDailyPackages = null;

    void unshipped_UpdateList(object sender, EventArgs e)
    {
      if (!unshippedFieldNamesIsSet)
      {
        dgvUnshipped.ColumnCount = unshipped.FieldNames == null ? 0 : unshipped.FieldNames.Length;

        if (unshipped.FieldNames != null)
        {
          for (int i = 0; i < unshipped.FieldNames.Length; i++)
          {
            dgvUnshipped.Columns[i].HeaderText = unshipped.FieldNames[i];
            dgvUnshipped.Columns[i].SortMode = DataGridViewColumnSortMode.Automatic;
          }
        }

        fillCopyToClipboardMenuItem(miUnshippedCopyToClipboard, dgvUnshipped);

        unshippedFieldNamesIsSet = true;
      }

      dgvUnshipped.RowCount = unshipped.PackageList.Count;
      for (int i = 0; i < unshipped.PackageList.Count; i++)
      {
        dgvUnshipped.Rows[i].Tag = unshipped.PackageList[i];
        dgvUnshipped.Rows[i].ErrorText = "";
        if (unshipped.PackageList[i].FieldValueList != null)
        {
          for (int j = 0; j < unshipped.PackageList[i].FieldValueList.Length; j++)
          {
            dgvUnshipped.Rows[i].Cells[j].Value = unshipped.getTypedFieldValue(i, j);
          }
        }

        // set cells styles
        foreach (DataGridViewCell cell in dgvUnshipped.Rows[i].Cells)
        {
          UnshippedPackageWrapper package = (UnshippedPackageWrapper)unshipped.PackageList[i];
          switch (package.State)
          { 
            case UnshippedPackageWrapper.StateType.UPDATED_PICKABLE:
              cell.Style = cell.ColumnIndex == 0 ? unshippedUpdatedPickableFirstCellStyle : unshippedUpdatedPickableCellStyle;
              break;

            case UnshippedPackageWrapper.StateType.UPDATED_ONHOLD:
              cell.Style = cell.ColumnIndex == 0 ? unshippedUpdatedOnholdFirstCellStyle : unshippedUpdatedOnholdCellStyle;
              break;

            case UnshippedPackageWrapper.StateType.ERROR:
              cell.Style = cell.ColumnIndex == 0 ? errorFirstCellStyle : errorCellStyle;
              cell.OwningRow.ErrorText = package.ErrorText;
              break;

            default:
              switch (package.Status)
              {
                case UnshippedPackageWrapper.STATUS_NEW:
                  cell.Style = cell.ColumnIndex == 0 ? unshippedNewFirstCellStyle : unshippedNewCellStyle;
                  break;

                case UnshippedPackageWrapper.STATUS_UNSHIPPED:
                  cell.Style = cell.ColumnIndex == 0 ? unshippedUnshippedFirstCellStyle : unshippedUnshippedCellStyle;
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

    private void btReloarUnshipped_Click(object sender, EventArgs e)
    {
      if(MessageBox.Show(
        this,
        "Warning! If you reload packages you will not be able to reset status. Continue?",
        "Question",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Question
        ) == DialogResult.Yes) 
      {
        unshipped.load();
        updateUnshippedStat();
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
      List<PackageStatus> list = new List<PackageStatus>();
      foreach (DataGridViewRow row in dgvUnshipped.SelectedRows)
      {
        if (row.Visible)
        {
          UnshippedPackageWrapper package = (UnshippedPackageWrapper)row.Tag;
          if (package.State != UnshippedPackageWrapper.StateType.ERROR
              && package.State != UnshippedPackageWrapper.StateType.UPDATED_PICKABLE
              && package.State != UnshippedPackageWrapper.StateType.UPDATED_ONHOLD
            )
          {
            list.Add(new PackageStatus(package.PackageId, UnshippedPackageWrapper.STATUS_PICKABLE, UnshippedPackageWrapper.STATUS_NEW));
          }
        }
      }

      if (list.Count == 0)
      {
        MessageBox.Show(
          this,
          "Nothing to update",
          "Message",
          MessageBoxButtons.OK,
          MessageBoxIcon.Information
          );
      }
      else
      {
        //list.Reverse(); // selected list in reverse order
        int failed;
        unshipped.setPickable(list, out failed);
        updateUnshippedStat();

        MessageBox.Show(
          this,
          String.Format("Operation completed.\nTotal requested: {0}.\nFailed: {1}.", list.Count, failed),
          "Message",
          MessageBoxButtons.OK,
          MessageBoxIcon.Information
          );
      }
    }

    private void btUnshippedResetStatus_Click(object sender, EventArgs e)
    {
      List<UnshippedPackageWrapper> list = new List<UnshippedPackageWrapper>();
      foreach (DataGridViewRow row in dgvUnshipped.SelectedRows)
      {
        if (row.Visible)
        {
          UnshippedPackageWrapper package = (UnshippedPackageWrapper)row.Tag;
          if ((package.State == UnshippedPackageWrapper.StateType.UPDATED_PICKABLE
               || package.State == UnshippedPackageWrapper.StateType.UPDATED_ONHOLD
              )
              && package.State != UnshippedPackageWrapper.StateType.ERROR
             )
          {
            list.Add(package);
          }
        }
      }

      if (list.Count == 0)
      {
        MessageBox.Show(
          this,
          "Nothing to update",
          "Message",
          MessageBoxButtons.OK,
          MessageBoxIcon.Information
          );
      }
      else
      {
        //list.Reverse(); // selected list in reverse order
        unshipped.resetStatus(list);
        updateUnshippedStat();

        MessageBox.Show(
          this,
          "Operation completed.",
          "Message",
          MessageBoxButtons.OK,
          MessageBoxIcon.Information
          );
      }
    }

    void unshipped_UpdateMaxDailyPackages(object sender, EventArgs e)
    {
      //edMaxDailyPackages.Text = unshipped.MaxDailyPackages.ToString();
      //edPmodMaxDailyPackages.Text = unshipped.PmodMaxDailyPackages.ToString();
    }

    private void btSetMaxDailyPackages_Click(object sender, EventArgs e)
    {
      if (fmSetMaxDailyPackages == null)
      { 
        fmSetMaxDailyPackages = new SetMaxDailyPackagesForm();
      }
      fmSetMaxDailyPackages.Text = "Max Daily Packages";
      fmSetMaxDailyPackages.Value = unshipped.MaxDailyPackages;

      if (fmSetMaxDailyPackages.ShowDialog() == DialogResult.OK)
      {
        if (fmSetMaxDailyPackages.Value != unshipped.MaxDailyPackages)
        {
          unshipped.setMaxDailyPackages(fmSetMaxDailyPackages.Value);
          MessageBox.Show(
            this,
            "Value successfully changed",
            "Message",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information
            );
        }
        else {
          MessageBox.Show(
            this,
            "Nothing to update",
            "Message",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information
            );
        }
      }
    }


    private void btSetPmodMaxDailyPackages_Click(object sender, EventArgs e)
    {
      if (fmSetMaxDailyPackages == null)
      {
        fmSetMaxDailyPackages = new SetMaxDailyPackagesForm();
      }
      fmSetMaxDailyPackages.Text = "PMOD Max Daily Packages";
      fmSetMaxDailyPackages.Value = unshipped.PmodMaxDailyPackages;

      if (fmSetMaxDailyPackages.ShowDialog() == DialogResult.OK)
      {
        if (fmSetMaxDailyPackages.Value != unshipped.PmodMaxDailyPackages)
        {
          unshipped.setPmodMaxDailyPackages(fmSetMaxDailyPackages.Value);
          MessageBox.Show(
            this,
            "Value successfully changed",
            "Message",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information
            );
        }
        else
        {
          MessageBox.Show(
            this,
            "Nothing to update",
            "Message",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information
            );
        }
      }
    }


    private void btOnHold_Click_2(object sender, EventArgs e)
    {
      List<int> packageIdList = new List<int>();
      foreach (DataGridViewRow row in dgvUnshipped.SelectedRows)
      {
        if (row.Visible)
        {
          UnshippedPackageWrapper package = (UnshippedPackageWrapper)row.Tag;
          packageIdList.Add(package.PackageId);
        }
      }

      if (packageIdList.Count == 0)
      {
        MessageBox.Show(
          this,
          "Nothing to set.",
          "Message",
          MessageBoxButtons.OK,
          MessageBoxIcon.Information
          );
      }
      else
      {
        unshipped.onHold(packageIdList);
        updateUnshippedStat();

        MessageBox.Show(
          this,
          "Process successfully completed.",
          "Message",
          MessageBoxButtons.OK,
          MessageBoxIcon.Information
          );
      }

    }
 
  }

}
