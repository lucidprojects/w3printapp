using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace PrintInvoice
{
  using PackageIdType = Int32;

  public delegate void ControllerCompleteEventHandler(object sender, EventArgs e);

  public enum PrintControllerState {IDLE, RUNNING};

  public class PrintController
  {
    private InvoiceProvider invoiceProvider;
    private Printer printer;
    private InvoiceStatusSaver invoiceStatusSaver;
    private PrintControllerState state = PrintControllerState.IDLE;
    private LabelService client;

    public PrintController(InvoiceProvider aInvoiceProvider, Printer aPrinter, InvoiceStatusSaver aInvoiceStatusSaver, LabelService aClient)
    {
      invoiceProvider = aInvoiceProvider;
      printer = aPrinter;
      invoiceStatusSaver = aInvoiceStatusSaver;
      client = aClient;

      invoiceStatusSaver.Complete += new InvoiceStatusSaverCompleteEventHandler(invoiceStatusSaver_Complete);
    }

    void invoiceStatusSaver_Complete(object sender, InvoiceStatusSaverCompleteEventArgs e)
    {
      state = PrintControllerState.IDLE;
      onComplete(new EventArgs());
    }

    public PrintControllerState State
    {
      get { return state; }
    }

    public event ControllerCompleteEventHandler Complete;

    private void onComplete(EventArgs e)
    {
      if (Complete != null)
        Complete(this, e);
    }

    public void setJob(List<PrintPackageWrapper> aInvoiceList, bool newBatch, bool aIsSequenceNumberEnabled, bool aIsPackJacket)
    {
      // log
      List<string> l = new List<string>();
      foreach (PrintPackageWrapper package in aInvoiceList) {
        l.Add(package.PackageId.ToString());
      }
      Log.getLogger().Debug(String.Format("PrintController.setJob([{0}], {1})", String.Join(",", l.ToArray()), newBatch));

      if(newBatch) {
        // request new batch ID
        CreatePrintBatchRequestType request = new CreatePrintBatchRequestType();
        CreatePrintBatchResponseType response = client.createPrintBatch(request);

        invoiceStatusSaver.BatchId = response.batchId;

        Log.getLogger().Debug(String.Format("New print batch created: {0}", invoiceStatusSaver.BatchId));
      }

      // sequence number
      if (aIsSequenceNumberEnabled)
      {
        string currentOrderedElements = null;
        int printBatchCount = 1;
        int elementBatch = 1;
        int elementBatchCount = 1;
        string packageOrderedElements;
        int count = aInvoiceList.Count;
        int firstBatchPackageIndex = 0;
        for (int i = 0; i < count; i++ )
        {
          PrintPackageWrapper package = aInvoiceList[i];
          package.isFirstBatchPackage = false;

          // first iteration
          if (currentOrderedElements == null)
          {
            package.isFirstBatchPackage = true; // master picklist
            try { currentOrderedElements = package.getOrderedElemens(); }
            catch { currentOrderedElements = "NA"; }
          }

          // new ordered elements
          try { packageOrderedElements = package.getOrderedElemens(); }
          catch
          {
              packageOrderedElements = "0";
          }

          if (currentOrderedElements != packageOrderedElements)
          {
            // master picklist
            package.isFirstBatchPackage = true;
            aInvoiceList[firstBatchPackageIndex].mplElementBatchCount = elementBatchCount - 1;
            firstBatchPackageIndex = i;

            elementBatch++;
            elementBatchCount = 1;
            currentOrderedElements = packageOrderedElements;
          }

          package.printBatchId = invoiceStatusSaver.BatchId;
          package.printBatchCount = printBatchCount++;
          package.elementBatch = elementBatch;
          package.elementBatchCount = elementBatchCount++;

          // log
          Log.getLogger().Debug(String.Format("Invoice sequence number: {0:00000000}-{1:000000}-{2:000000}-{3:000000}", package.printBatchId, package.printBatchCount, package.elementBatch, package.elementBatchCount));
        }

        // master picklist last batch
        aInvoiceList[firstBatchPackageIndex].mplElementBatchCount = elementBatchCount - 1;
      }

      invoiceProvider.setList(aInvoiceList);
    }

    public void run()
    {
      Log.getLogger().Info("PrintController.run()");

      invoiceProvider.cleanUp();
      printer.cleanUp();
      invoiceStatusSaver.cleanUp();

      invoiceProvider.run();
      printer.run();
      invoiceStatusSaver.run();
      state = PrintControllerState.RUNNING;
    }

    public void stop()
    {
      Log.getLogger().Info("PrintController.stop()");

      invoiceProvider.stop();
      printer.stop();
      invoiceStatusSaver.stop();
      state = PrintControllerState.IDLE;
    }
  }
}
