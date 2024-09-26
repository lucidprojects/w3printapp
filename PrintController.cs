﻿using System;
using System.Collections.Generic;

namespace PrintInvoice
{
    public delegate void ControllerCompleteEventHandler(object sender, EventArgs e);

    public enum PrintControllerState
    {
        IDLE,
        RUNNING
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

        public PrintControllerState State { get; private set; } = PrintControllerState.IDLE;

        private void invoiceStatusSaver_Complete(object sender, InvoiceStatusSaverCompleteEventArgs e)
        {
            State = PrintControllerState.IDLE;
            onComplete(EventArgs.Empty);
        }

        public event ControllerCompleteEventHandler Complete;

        private void onComplete(EventArgs e)
        {
            Complete?.Invoke(this, e);
        }

        public void setJob(List<PrintPackageWrapper> aInvoiceList, bool newBatch, bool aIsSequenceNumberEnabled,
            bool aIsPackJacket)
        {
            // log
            var l = new List<string>();
            foreach (var package in aInvoiceList) l.Add(package.PackageId.ToString());
            Log.getLogger().Debug($"PrintController.setJob([{string.Join(",", l.ToArray())}], {newBatch})");

            if (newBatch)
            {
                // request new batch ID
                var request = new CreatePrintBatchRequestType();
                var response = _client.createPrintBatch(request);

                _invoiceStatusSaver.BatchId = response.batchId;

                Log.getLogger().Debug($"New print batch created: {_invoiceStatusSaver.BatchId}");
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
                            currentOrderedElements = package.getOrderedElemens();
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
                        packageOrderedElements = package.getOrderedElemens();
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
                    Log.getLogger()
                        .Debug(
                            $"Invoice sequence number: {package._printBatchId:00000000}-{package._printBatchCount:000000}-{package._elementBatch:000000}-{package._elementBatchCount:000000}");
                }

                // master picklist last batch
                aInvoiceList[firstBatchPackageIndex]._mplElementBatchCount = elementBatchCount - 1;
            }

            _invoiceProvider.setList(aInvoiceList);
        }

        public void run()
        {
            Log.getLogger().Info("PrintController.run()");

            _invoiceProvider.cleanUp();
            _printer.cleanUp();
            _invoiceStatusSaver.cleanUp();

            _invoiceProvider.run();
            _printer.run();
            _invoiceStatusSaver.run();
            State = PrintControllerState.RUNNING;
        }

        public void stop()
        {
            Log.getLogger().Info("PrintController.stop()");

            _invoiceProvider.stop();
            _printer.stop();
            _invoiceStatusSaver.stop();
            State = PrintControllerState.IDLE;
        }
    }
}