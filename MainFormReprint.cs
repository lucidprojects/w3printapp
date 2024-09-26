using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Security;

namespace PrintInvoice
{
  using PackageList = List<PrintPackageWrapper>;

  public partial class MainForm : Form
  {
    void reprint_UpdateList(object sender, EventArgs e)
    {
      PackageList invoiceList = reprint.PackageList;

      // field names
      if (!reprintFieldNamesIsSet)
      {
        dgvReprint.ColumnCount = (reprint.FieldNames == null ? 0 : reprint.FieldNames.Length) + 1;

        dgvReprint.Columns[0].HeaderText = "Tracking Number";
        dgvReprint.Columns[0].SortMode = DataGridViewColumnSortMode.Automatic;

        if (reprint.FieldNames != null)
        {
          for (int i = 1; i < reprint.FieldNames.Length + 1; i++)
          {
            dgvReprint.Columns[i].HeaderText = reprint.FieldNames[i - 1];
            dgvReprint.Columns[i].SortMode = DataGridViewColumnSortMode.Automatic;
          }
        }

        // copy to clipboard context menu values
        fillCopyToClipboardMenuItem(miReprintCopyToClipboard, dgvReprint);

        reprintFieldNamesIsSet = true;
      }

      reprintRowIndex.Clear();
      dgvReprint.RowCount = reprint.PackageList.Count;
      for (int i = 0; i < reprint.PackageList.Count; i++)
      {
        dgvReprint.Rows[i].Tag = reprint.PackageList[i];
        dgvReprint.Rows[i].Cells[0].Value = reprint.PackageList[i].TrackingNumber;
        if (reprint.PackageList[i].FieldValueList != null)
        {
          for (int j = 1; j < reprint.PackageList[i].FieldValueList.Length + 1; j++)
          {
            dgvReprint.Rows[i].Cells[j].Value = reprint.getTypedFieldValue(i, j - 1);
          }
        }
        reprintRowIndex[reprint.PackageList[i].PackageId] = dgvReprint.Rows[i];

        // set cells styles
        setRowStyle(dgvReprint.Rows[i], reprint.PackageList[i]);

        if (reprint.PackageList[i].IsError)
        {
          dgvReprint.Rows[i].ErrorText = reprint.PackageList[i].ErrorText;
        }
        else
        {
          dgvReprint.Rows[i].ErrorText = "";
        }
      }

      updateReprintFilter();
      updateReprintStat();
    }

    private void btAddReprintPackage_Click(object sender, EventArgs e)
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
          reprint.addSingle(fmAddUnshipInvoice.tbInvoiceIdList.Lines);
          break;

        case AddPackageForm.ResultType.ADD_BATCH:
          reprint.addBatch(fmAddUnshipInvoice.tbInvoiceIdList.Lines);
          break;
      }

      updateReprintFilter();
      updateReprintStat();
    }

    private void cmsReprint_Opening(object sender, CancelEventArgs e)
    {
      if (dgvReprint.SelectedRows.Count == 0
          || printController.State == PrintControllerState.RUNNING)
      {
        tsmiReprintRemoveSelected.Enabled = false;
      }
      else
      {
        tsmiReprintRemoveSelected.Enabled = true;
      }
    }

    private void tsmiReprintRemoveSelected_Click(object sender, EventArgs e)
    {
      List<PrintPackageWrapper> packageList = new List<PrintPackageWrapper>();
      foreach (DataGridViewRow row in dgvReprint.SelectedRows)
      {
        packageList.Add(row.Tag as PrintPackageWrapper);
      }
      reprint.remove(packageList);
      dgvReprint.ClearSelection();

      updateReprintStat();
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
      {
        reprint.clear();
      }

      updateReprintStat();
    }

    void invoiceProvider_LoadReprint(object sender, InvoiceProviderLoadEventArgs e)
    {
      reprint.setPackageState(e.PrintInvoiceWrapper.PackageId, e.PrintInvoiceWrapper.IsLoaded ? PrintPackageWrapper.LOADED : PrintPackageWrapper.LOCKED, "");
    }

    void invoiceProvider_ErrorReprint(object sender, InvoiceProviderErrorEventArgs e)
    {
      reprint.setPackageState(e.InvoiceId, PrintPackageWrapper.ERROR, e.Message);
    }

    void printer_PrintReprint(object sender, PrinterPrintEventArgs e)
    {
      reprint.setPackageState(e.PackageId, PrintPackageWrapper.PRINTED, "");
    }

    void printer_JobErrorReprint(object sender, PrinterJobErrorEventArgs e)
    {
      reprint.setPackageState(e.PackageId, PrintPackageWrapper.ERROR, e.Message);
    }

    void invoiceStatusSaver_SaveReprint(object sender, InvoiceStatusSaverSaveEventArgs e)
    {
      reprint.setPackageState(e.InvoiceId, PrintPackageWrapper.STATUS_SAVED, "");
    }

    void invoiceStatusSaver_ErrorReprint(object sender, InvoiceStatusSaverErrorEventArgs e)
    {
      reprint.setPackageState(e.InvoiceId, PrintPackageWrapper.ERROR, e.Message);
    }

    void reprint_UpdatePackageState(object sender, PrintPackageStorageUpdatePackageStateEventArgs e)
    {
      PrintPackageWrapper package = reprint.getPackageByPackageId(e.PackageId);
      setRowStyle(reprintRowIndex[e.PackageId], package);
      updateReprintStat();
    }

    private void btReprintPrint_Click(object sender, EventArgs e)
    {
      if (printController.State == PrintControllerState.RUNNING)
      {
        btReprintPrint.Enabled = false;
        bwStopReprint.RunWorkerAsync();
      }
      else
      {

        // warning about sequence number
        if (MessageBox.Show(
          "Invoice sequence number printing is " + (chkReprintSequenceNumber.Checked ? "enabled" : "disabled") + ".\nContinue printing?",
          "Warning",
          MessageBoxButtons.YesNo,
          MessageBoxIcon.Question
        ) == DialogResult.No)
        {
          return;
        }

        

        // pack jacket question
        string q;
        q = "You are going to print full invoices (with postage label).\nContinue?";
        
        if (MessageBox.Show(
          q,
          "Confirmation",
          MessageBoxButtons.YesNo,
          MessageBoxIcon.Question
        ) == DialogResult.No)
        {
          return;
        }

        if (dgvReprint.CurrentRow.Index == 0 || (dgvReprint.CurrentRow.Index > 0 && MessageBox.Show(
          "You are going to print not from the beginning. Continue?",
          "Confirmation",
          MessageBoxButtons.YesNo,
          MessageBoxIcon.Question
          ) == DialogResult.Yes))
        {
          btReprintPrint.Enabled = false;
          List<PrintPackageWrapper> packageList = new List<PrintPackageWrapper>();
          bool askRepeatedPrint = true;
          bool askRepeatedPrintResult = true;
          for (int i = dgvReprint.CurrentRow.Index; i < dgvReprint.Rows.Count; i++)
          {
            if (dgvReprint.Rows[i].Visible)
            {
              PrintPackageWrapper package = dgvReprint.Rows[i].Tag as PrintPackageWrapper;
              if (package.PackageId != PackageWrapper.NULL_PACKAGE_ID)
              {
                bool add = true;
                if (package.IsPrinted)
                {
                  if (askRepeatedPrint)
                  {
                    RepeatedPrintForm fmRepeatedPrint = new RepeatedPrintForm();
                    fmRepeatedPrint.laMessage.Text = String.Format("You are going to print invoice No. {0} is marked as printed. Do you want to print it again?", dgvReprint.Rows[i].Cells[1].Value);
                    askRepeatedPrintResult = fmRepeatedPrint.ShowDialog() == DialogResult.Yes ? true : false;
                    askRepeatedPrint = !fmRepeatedPrint.ckDontAsk.Checked;
                  }
                  add = askRepeatedPrintResult;
                }

                if (add)
                {
                    package.isPackJacket = false; // chkReprintPackJacket.Checked;
                  packageList.Add(package);
                }
              }
            }
          }

          startPrintJob(packageList, cbReprintPrinter.SelectedItem.ToString(), true, chkReprintSequenceNumber.Checked, false, false);

          setReprintControlsEnabled(false);

          btReprintPrint.Text = "Stop";
          btReprintPrint.Enabled = true;
        }
      }
    }

    void printController_CompleteReprint(object sender, EventArgs e)
    {
      bwPrinterErrorMonitor.CancelAsync();
      printerErrorMonitorResetEvent.WaitOne();
      setControlText(btReprintPrint, "Print");
      setReprintControlsEnabled(true);
      showMessageBox(
           this,
           "Print job complete.",
           "Message",
           MessageBoxButtons.OK,
           MessageBoxIcon.Information
           );
    }

    private void btReprintExportErrors_Click(object sender, EventArgs e)
    {
      List<string> errorList = new List<string>();
      foreach (DataGridViewRow row in dgvReprint.Rows)
      {
        PrintPackageWrapper package = row.Tag as PrintPackageWrapper;
        if (package.IsError)
        {
          errorList.Add(String.Format("Package: {0}. Error: {1}", package.PackageId, package.ErrorText));
        }
      }
      saveErrors(errorList);
    }

    private void miPreviewReprintInvoice_Click(object sender, EventArgs e)
    {
      PrintPackageWrapper package = dgvReprint.CurrentRow.Tag as PrintPackageWrapper;
      if (package.PackageId != PrintPackageWrapper.NULL_PACKAGE_ID)
      {
        previewInvoice(package.PackageId, null, false);
      }
    }

    private void btReprintLastBatches_Click(object sender, EventArgs e)
    {
      if (fmLastBatches == null)
      {
        fmLastBatches = new LastBatchesForm(labelService, config);
      }

      // load data
      Cursor.Current = Cursors.WaitCursor;
      int nLoaded = fmLastBatches.load();
      Cursor.Current = Cursors.Default;
      if (nLoaded > 0)
      {
        if (fmLastBatches.ShowDialog() == DialogResult.OK)
        {
          Cursor.Current = Cursors.WaitCursor;
          int nAdded = reprint.addBatchById(fmLastBatches.BatchId);
          Cursor.Current = Cursors.Default;
          if (nAdded == 0)
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

    private void setReprintControlsEnabled(bool aEnabled)
    {
      setControlEnabled(cbReprintPrinter, aEnabled);
      setControlEnabled(btAddReprintPackage, aEnabled);
      setControlEnabled(btReprintLastBatches, aEnabled);
      setControlEnabled(btReprintClear, aEnabled);
      setControlEnabled(btReprintExportErrors, aEnabled);
    }
  }
}
