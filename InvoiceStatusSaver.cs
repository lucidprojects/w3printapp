using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using PrintInvoice.Properties;

namespace PrintInvoice
{
    using InvoiceIdType = Int32;

    public class InvoiceStatusSaverSaveEventArgs : EventArgs
    {
        public InvoiceStatusSaverSaveEventArgs(InvoiceIdType aInvoiceId)
        {
            InvoiceId = aInvoiceId;
        }

        public InvoiceIdType InvoiceId { get; }
    }

    public delegate void InvoiceStatusSaverSaveEventHandler(object sender, InvoiceStatusSaverSaveEventArgs e);

    public class InvoiceStatusSaverErrorEventArgs : EventArgs
    {
        public InvoiceStatusSaverErrorEventArgs(InvoiceIdType aInvoiceId, string aMessage)
        {
            InvoiceId = aInvoiceId;
            Message = aMessage;
        }

        public InvoiceIdType InvoiceId { get; }

        public string Message { get; }
    }

    public delegate void InvoiceStatusSaverErrorEventHandler(object sender, InvoiceStatusSaverErrorEventArgs e);

    public class InvoiceStatusSaverCompleteEventArgs : EventArgs
    {
    }

    public delegate void InvoiceStatusSaverCompleteEventHandler(object sender, InvoiceStatusSaverCompleteEventArgs e);


    public class InvoiceStatusSaver
    {
        private readonly BackgroundWorker _bwSave;
        private readonly LabelService _client;
        private readonly Printer _printer;

        private readonly Queue<InvoiceIdType> _queue = new Queue<InvoiceIdType>();

        private readonly AutoResetEvent _resetEvent;

        private bool _isReprint;

        public InvoiceStatusSaver(Printer aPrinter, LabelService aClient)
        {
            _printer = aPrinter;
            _client = aClient;

            _bwSave = new BackgroundWorker();
            _bwSave.WorkerSupportsCancellation = true;
            _bwSave.DoWork += bwSave_DoWork;

            _printer.Print += printer_Print;

            _resetEvent = new AutoResetEvent(false);
        }

        public int BatchId { set; get; }

        public bool IsReprint
        {
            set => _isReprint = value;
        }

        public bool MayEnqueue
        {
            get
            {
                lock (_queue)
                {
                    return _queue.Count < Settings.Default.SaveStatusQueueMaxSize;
                }
            }
        }

        public void Run()
        {
            _resetEvent.Reset();
            _bwSave.RunWorkerAsync();
        }

        public void Stop()
        {
            _bwSave.CancelAsync();
            _resetEvent.WaitOne();
        }

        private void bwSave_DoWork(object sender, DoWorkEventArgs e)
        {
            Log.Info("StatusSaver thread started");

            var bw = sender as BackgroundWorker;

            while (true)
            {
                lock (_queue)
                {
                    if (_queue.Count > 0)
                    {
                        var invoiceId = _queue.Dequeue();

                        try
                        {
                            var request = new SetLabelPrintedRequestType
                            {
                                labelId = invoiceId,
                                createNewBatch = false,
                                batchId = BatchId,
                                printStationId = Settings.Default.PrintStationId
                            };

                            var response = _client.setLabelPrinted(request);
                            if (response.status != 0) throw new Exception(response.message);

                            // Save
                            var eventArgs = new InvoiceStatusSaverSaveEventArgs(invoiceId);
                            OnSave(eventArgs);
                            Log.Debug($"StatusSaver: {invoiceId.ToString()} status saved");
                        }
                        catch (Exception ex)
                        {
                            // Error
                            _queue.Enqueue(invoiceId);
                            var eventArgs = new InvoiceStatusSaverErrorEventArgs(invoiceId, ex.Message);
                            OnError(eventArgs);
                        }
                    }
                }

                // all statuses must be saved
                lock (_queue)
                {
                    if (bw.CancellationPending && _queue.Count == 0)
                    {
                        e.Cancel = true;
                        break;
                    }
                }

                // check for complete
                lock (_queue)
                {
                    if (_queue.Count == 0 && _printer.IsCompleted)
                    {
                        OnComplete(new InvoiceStatusSaverCompleteEventArgs());
                        break;
                    }
                }

                Thread.Sleep(100);
            }

            _resetEvent.Set();

            Log.Info("StatusSaver thread stopped");
        }

        private void printer_Print(object sender, PrinterPrintEventArgs e)
        {
            lock (_queue)
            {
                _queue.Enqueue(e.PackageId);
            }
        }

        public event InvoiceStatusSaverSaveEventHandler Save;

        private void OnSave(InvoiceStatusSaverSaveEventArgs e)
        {
            Save?.Invoke(this, e);
        }

        public event InvoiceStatusSaverErrorEventHandler Error;

        private void OnError(InvoiceStatusSaverErrorEventArgs e)
        {
            Error?.Invoke(this, e);
        }

        public event InvoiceStatusSaverCompleteEventHandler Complete;

        private void OnComplete(InvoiceStatusSaverCompleteEventArgs e)
        {
            Complete?.Invoke(this, e);
        }

        public void CleanUp()
        {
            lock (_queue)
            {
                _queue.Clear();
            }
        }
    }
}