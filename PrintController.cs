using System;
using System.Collections.Generic;
using System.Linq;

namespace PrintInvoice
{
    public delegate void ControllerCompleteEventHandler(object sender, EventArgs e);

    public enum PrintControllerState
    {
        Idle,
        Running
    }

    public class PrintController
    {
        private readonly LabelService _client;
        private readonly InvoiceProvider _invoiceProvider;
        private readonly InvoiceStatusSaver _invoiceStatusSaver;
        private readonly Printer _printer;

        public PrintController(InvoiceProvider aInvoiceProvider, Printer aPrinter,
            InvoiceStatusSaver aInvoiceStatusSaver, LabelService aClient)
        {
            _invoiceProvider = aInvoiceProvider;
            _printer = aPrinter;
            _invoiceStatusSaver = aInvoiceStatusSaver;
            _client = aClient;

            _invoiceStatusSaver.Complete += invoiceStatusSaver_Complete;
        }

        public PrintControllerState State { get; private set; } = PrintControllerState.Idle;

        private void invoiceStatusSaver_Complete(object sender, InvoiceStatusSaverCompleteEventArgs e)
        {
            State = PrintControllerState.Idle;
            OnComplete(EventArgs.Empty);
        }

        public event ControllerCompleteEventHandler Complete;

        private void OnComplete(EventArgs e)
        {
            Complete?.Invoke(this, e);
        }

        public void SetJob(List<PrintPackageWrapper> aInvoiceList, bool newBatch, bool aIsSequenceNumberEnabled, bool aIsPackJacket)
        {
            // log

            Log.Debug($"PrintController.setJob([{string.Join(",", aInvoiceList.Select(package => package.PackageId.ToString()).ToArray())}], {newBatch})");

            if (newBatch)
            {
                // request new batch ID
                var request = new CreatePrintBatchRequestType();
                var response = _client.createPrintBatch(request);

                _invoiceStatusSaver.BatchId = response.batchId;

                Log.Debug($"New print batch created: {_invoiceStatusSaver.BatchId}");
            }

            // sequence number
            if (aIsSequenceNumberEnabled)
            {
                string currentOrderedElements = null;
                var printBatchCount = 1;
                var elementBatch = 1;
                var elementBatchCount = 1;
                var count = aInvoiceList.Count;
                var firstBatchPackageIndex = 0;
                
                for (var i = 0; i < count; i++)
                {
                    var package = aInvoiceList[i];
                    package._isFirstBatchPackage = false;

                    // first iteration
                    if (currentOrderedElements == null)
                    {
                        package._isFirstBatchPackage = true; // master picklist
                        
                        try
                        {
                            currentOrderedElements = package.GetOrderedElements();
                        }
                        catch
                        {
                            currentOrderedElements = "NA";
                        }
                    }

                    // new ordered elements
                    string packageOrderedElements;
                    
                    try
                    {
                        packageOrderedElements = package.GetOrderedElements();
                    }
                    catch
                    {
                        packageOrderedElements = "0";
                    }

                    if (currentOrderedElements != packageOrderedElements)
                    {
                        // master picklist
                        package._isFirstBatchPackage = true;
                        aInvoiceList[firstBatchPackageIndex]._mplElementBatchCount = elementBatchCount - 1;
                        firstBatchPackageIndex = i;

                        elementBatch++;
                        elementBatchCount = 1;
                        currentOrderedElements = packageOrderedElements;
                    }

                    package._printBatchId = _invoiceStatusSaver.BatchId;
                    package._printBatchCount = printBatchCount++;
                    package._elementBatch = elementBatch;
                    package._elementBatchCount = elementBatchCount++;

                    // log
                    Log.Debug($"Invoice sequence number: {package._printBatchId:00000000}-{package._printBatchCount:000000}-{package._elementBatch:000000}-{package._elementBatchCount:000000}");
                }

                // master picklist last batch
                if (firstBatchPackageIndex >= 0 && firstBatchPackageIndex < aInvoiceList.Count)
                    aInvoiceList[firstBatchPackageIndex]._mplElementBatchCount = elementBatchCount - 1;
            }

            _invoiceProvider.SetList(aInvoiceList);
        }

        public void Run()
        {
            Log.Info("PrintController.run()");

            _invoiceProvider.CleanUp();
            _printer.CleanUp();
            _invoiceStatusSaver.CleanUp();

            _invoiceProvider.Run();
            _printer.Run();
            _invoiceStatusSaver.Run();
            State = PrintControllerState.Running;
        }

        public void Stop()
        {
            Log.Info("PrintController.stop()");

            _invoiceProvider.Stop();
            _printer.Stop();
            _invoiceStatusSaver.Stop();
            State = PrintControllerState.Idle;
        }
    }
}