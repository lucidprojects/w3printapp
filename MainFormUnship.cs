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
  public partial class MainForm : Form
  {
    void unship_UpdateList(object sender, EventArgs e)
    {
      if (!unshipFieldNamesIsSet)
      {
        dgvUnship.ColumnCount = (unship.FieldNames == null ? 0 : unship.FieldNames.Length) + 1;

        dgvUnship.Columns[0].HeaderText = "Tracking Number";
        dgvUnship.Columns[0].SortMode = DataGridViewColumnSortMode.Automatic;

        if (unship.FieldNames != null)
        {
          for (int i = 1; i < unship.FieldNames.Length + 1; i++)
          {
            dgvUnship.Columns[i].HeaderText = unship.FieldNames[i - 1];
            dgvUnship.Columns[i].SortMode = DataGridViewColumnSortMode.Automatic;
          }

          // copy to clipboard context menu values
          fillCopyToClipboardMenuItem(miUnshipCopyToClipboard, dgvUnship);
        }

        unshipFieldNamesIsSet = true;
      }

      dgvUnship.RowCount = unship.PackageList.Count;
      for (int i = 0; i < unship.PackageList.Count; i++)
      {
        dgvUnship.Rows[i].Tag = unship.PackageList[i];
        dgvUnship.Rows[i].Cells[0].Value = unship.PackageList[i].TrackingNumber;
        if (unship.PackageList[i].FieldValueList != null)
        {
          for (int j = 1; j < unship.PackageList[i].FieldValueList.Length + 1; j++)
          {
            dgvUnship.Rows[i].Cells[j].Value = unship.getTypedFieldValue(i, j - 1);
          }
        }

        // set cells styles
        foreach (DataGridViewCell cell in dgvUnship.Rows[i].Cells)
        {
          switch (unship.PackageList[i].State)
          {
            case UnshipPackageWrapper.PackageStateType.ERROR:
              cell.Style = cell.ColumnIndex == 0 ? unshipErrorFirstCellStyle : unshipErrorCellStyle;
              break;

            case UnshipPackageWrapper.PackageStateType.SHIPPED:
              cell.Style = cell.ColumnIndex == 0 ? defaultFirstCellStyle : dgvUnship.DefaultCellStyle;
              break;

            case UnshipPackageWrapper.PackageStateType.UNSHIPPED:
              cell.Style = cell.ColumnIndex == 0 ? unshipUnshippedFirstCellStyle : unshipUnshippedCellStyle;
              break;
          }
        }

        if (unship.PackageList[i].State == UnshipPackageWrapper.PackageStateType.ERROR)
        {
          dgvUnship.Rows[i].ErrorText = unship.PackageList[i].ErrorText;
        }
        else
        {
          dgvUnship.Rows[i].ErrorText = "";
        }
      }

      updateUnshipFilter();
      updateUnshipStat();
    }

    private void updateUnshipFilter()
    {
      foreach (DataGridViewRow row in dgvUnship.Rows)
      {
        bool visible = false;
        UnshipPackageWrapper package = row.Tag as UnshipPackageWrapper;
        if (chkUnshipFilterShowShipped.Checked && package.State == UnshipPackageWrapper.PackageStateType.SHIPPED)
        {
          visible = true;
        }
        if (chkUnshipFilterShowUnshipped.Checked && package.State == UnshipPackageWrapper.PackageStateType.UNSHIPPED)
        {
          visible = true;
        }
        if (chkUnshipFilterShowFailed.Checked && package.State == UnshipPackageWrapper.PackageStateType.ERROR)
        {
          visible = true;
        }
        row.Visible = visible;
      }
    }

    private void updateUnshipStat()
    {
      int shipped = 0;
      int unshipped = 0;
      int failed = 0;

      foreach (DataGridViewRow row in dgvUnship.Rows)
      {
        UnshipPackageWrapper package = row.Tag as UnshipPackageWrapper;
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

      tsslUnshipTotal.Text = String.Format("Total: {0}", dgvUnship.RowCount);
      tsslUnshipShipped.Text = String.Format("Shipped: {0}", shipped);
      tsslUnshipUnshipped.Text = String.Format("Unshipped: {0}", unshipped);
      tsslUnshipFailed.Text = String.Format("Failed: {0}", failed);
    }

    private void btAddUnshipInvoice_Click(object sender, EventArgs e)
    {
      if (fmAddUnshipInvoice == null)
      {
        fmAddUnshipInvoice = new AddPackageForm();
      }

      fmAddUnshipInvoice.tbInvoiceIdList.Clear();
      fmAddUnshipInvoice.Result = AddPackageForm.ResultType.CANCEL;
      fmAddUnshipInvoice.ActiveControl = fmAddUnshipInvoice.tbInvoiceIdList;
      fmAddUnshipInvoice.ShowDialog();
      switch (fmAddUnshipInvoice.Result)
      { 
        case AddPackageForm.ResultType.ADD_SINGLE:
          unship.addSingle(fmAddUnshipInvoice.tbInvoiceIdList.Lines);
          break;

        case AddPackageForm.ResultType.ADD_BATCH:
          unship.addBatch(fmAddUnshipInvoice.tbInvoiceIdList.Lines);
          break;
      }
    }

    private void chkUnshipFilterShowShipped_CheckedChanged(object sender, EventArgs e)
    {
      updateUnshipFilter();
    }

    private void miUnshipRemoveSelected_Click(object sender, EventArgs e)
    {
      List<UnshipPackageWrapper> packageList = new List<UnshipPackageWrapper>();
      foreach (DataGridViewRow row in dgvUnship.SelectedRows)
      {
        packageList.Add(row.Tag as UnshipPackageWrapper);
      }
      unship.remove(packageList);
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
      {
        unship.clear();
      }
    }

    private void btExportUnshipErrors_Click(object sender, EventArgs e)
    {
      List<string> errorList = new List<string>();
      foreach (DataGridViewRow row in dgvUnship.Rows)
      {
        UnshipPackageWrapper package = row.Tag as UnshipPackageWrapper;
        if (package.State == UnshipPackageWrapper.PackageStateType.ERROR)
        {
          errorList.Add(String.Format("Package ID: {0}. Error: {1}", package.TrackingNumber, package.ErrorText));
        }
      }

      saveErrors(errorList);
    }

    private void btUnship_Click(object sender, EventArgs e)
    {
      if (unship.PackageList.Count == 0)
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
          int success;
          int fail;
          unship.unship(out success, out fail);

          MessageBox.Show(
             this,
             String.Format("Total {0} packages was requested to unship.\n{1} was unshipped successfully, {2} was not.", success+fail, success, fail),
             "Message",
             MessageBoxButtons.OK,
             MessageBoxIcon.Information
          );
        }
      }
    }


    private void btUnshipLastBatches_Click(object sender, EventArgs e)
    {
      if (fmLastBatches == null)
      {
        fmLastBatches = new LastBatchesForm(labelService, config);
      }

      // load data
      if (fmLastBatches.load() > 0)
      {
        if (fmLastBatches.ShowDialog() == DialogResult.OK)
        {
          if (unship.addBatchById(fmLastBatches.BatchId) == 0)
          {
            MessageBox.Show(
              this,
              "No invoices found in the selected batch.",
              "Message",
              MessageBoxButtons.OK,
              MessageBoxIcon.Information
              );
          }
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

  }
}
