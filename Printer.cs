using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using PrintInvoice.Properties;

namespace PrintInvoice
{
    using PackageIdType = Int32;

    public class PrinterPrintEventArgs : EventArgs
    {
        public PrinterPrintEventArgs(PackageIdType aInvoiceId)
        {
            PackageId = aInvoiceId;
        }

        public PackageIdType PackageId { get; }
    }

    public delegate void PrinterPrintEventHandler(object sender, PrinterPrintEventArgs e);

    public class PrinterJobErrorEventArgs : EventArgs
    {
        public PrinterJobErrorEventArgs(PackageIdType aInvoiceId, string aMessage)
        {
            PackageId = aInvoiceId;
            Message = aMessage;
        }

        public PackageIdType PackageId { get; }

        public string Message { get; }
    }

    public delegate void PrinterJobErrorEventHandler(object sender, PrinterJobErrorEventArgs e);

    public class PrinterPrinterErrorEventArgs : EventArgs
    {
        public PrinterPrinterErrorEventArgs(string aMessage)
        {
            Message = aMessage;
        }

        public string Message { get; }
    }

    public delegate void PrinterPrinterErrorEventHandler(object sender, PrinterPrinterErrorEventArgs e);

    public class Printer
    {
        private readonly BackgroundWorker _bwMonitor;

        private readonly BackgroundWorker _bwPrinter;

        private readonly InvoiceProvider _invoiceProvider;
        private readonly Dictionary<uint, JobData> _jobList = new Dictionary<uint, JobData>();
        private readonly LabelService _labelService;
        private readonly AutoResetEvent _monitorResetEvent = new AutoResetEvent(false);

        private readonly AutoResetEvent _printerResetEvent = new AutoResetEvent(false);

        private bool _isSequenceNumberEnabled;

        private bool _monitorCompleted;
        private bool _printerCompleted;
        private bool _printerError;

        private IntPtr _printerHandle = new IntPtr(0);
        private string _printerName = "";

        private bool _printPickList;

        // constructor
        public Printer(InvoiceProvider aInvoiceProvider, LabelService aClient)
        {
            _invoiceProvider = aInvoiceProvider;
            _labelService = aClient;
            //invoiceProvider.Load += new InvoiceProviderLoadEventHandler(invoiceProvider_Load);

            _bwPrinter = new BackgroundWorker();
            _bwPrinter.WorkerSupportsCancellation = true;
            _bwPrinter.DoWork += bwPrinter_DoWork;

            _bwMonitor = new BackgroundWorker();
            _bwMonitor.WorkerSupportsCancellation = true;
            _bwMonitor.DoWork += bwMonitor_DoWork;
        }

        public bool IsSequenceNumberEnabled
        {
            set => _isSequenceNumberEnabled = value;
        }

        public bool PrintPickList
        {
            set => _printPickList = value;
        }

        public bool IsCompleted => _printerCompleted && _monitorCompleted;

        public void Run()
        {
            _printerHandle = RawPrinterHelper.Open(_printerName);

            _printerCompleted = false;
            _monitorCompleted = false;
            _printerError = false;

            _printerResetEvent.Reset();
            _monitorResetEvent.Reset();

            _bwMonitor.RunWorkerAsync();
            _bwPrinter.RunWorkerAsync();
        }

        public void Stop()
        {
            StopPrinter();
            StopMonitor();

            RawPrinterHelper.Close(_printerHandle);
        }

        private void StopPrinter()
        {
            _bwPrinter.CancelAsync();
            _printerResetEvent.WaitOne();
        }

        private void StopMonitor()
        {
            _bwMonitor.CancelAsync();
            _monitorResetEvent.WaitOne();
        }

        private void bwPrinter_DoWork(object sender, DoWorkEventArgs e)
        {
            Log.GetLogger().Info("Printer thread started");

            var bw = sender as BackgroundWorker;

            for (;;)
            {
                if (_jobList.Count < Settings.Default.PrintJobQueueMaxSize)
                {
                    var package = _invoiceProvider.getInvoice();
                    if (package != null && !_printerError)
                    {
                        // Mater Pick List
                        if (_printPickList && package._isFirstBatchPackage)
                        {
                            var docInfo = new RawPrinterHelper.DocInfoA
                            {
                                pDocName = "Master pick list",
                                pDataType = "RAW"
                            };

                            PrintPdf(_printerHandle, docInfo, Routines.GetMasterPickListPdf(package, _labelService));
                        }

                        PrintPackage(_printerHandle, package);

                        Log.GetLogger()
                            .Debug($"Printer: {package.PackageId.ToString()} sent to printer");
                    }
                }

                if (bw.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                // check for complete
                if (_invoiceProvider.IsCompleted)
                {
                    _printerCompleted = true;
                    break;
                }


                Thread.Sleep(100);
            } // infinite loop

            _printerResetEvent.Set();

            Log.GetLogger().Info("Printer thread stopped");
        }

        /*
         * Check print job status
         * Success if 1) appropriate job status 2) job no more in print queue
         */
        private void bwMonitor_DoWork(object sender, DoWorkEventArgs e)
        {
            var bw = sender as BackgroundWorker;

            for (;;)
            {
                var jobInfoList = RawPrinterHelper.EnumJobs(_printerHandle);

                foreach (var jobInfo in jobInfoList)
                {
                    // printed (by job status)
                    if ((jobInfo.Status & RawPrinterHelper.JobStatusPrinted) != 0)
                    {
                        lock (_jobList)
                        {
                            if (_jobList.ContainsKey(jobInfo.JobId))
                            {
                                OnPrint(new PrinterPrintEventArgs(_jobList[jobInfo.JobId]._invoiceId));
                                _jobList.Remove(jobInfo.JobId);
                            }
                        }
                    }

                    if ((jobInfo.Status & RawPrinterHelper.JobStatusPrinting) > 0 && (jobInfo.Status & (RawPrinterHelper.JobStatusError | RawPrinterHelper.JobStatusOffline | RawPrinterHelper.JobStatusPaperOut | RawPrinterHelper.JobStatusBlockedDevq)) > 0) 
                    {
                        lock (_jobList)
                        {
                            if (_jobList.TryGetValue(jobInfo.JobId, out var value)) 
                                OnJobError(new PrinterJobErrorEventArgs(value._invoiceId, $"Printer job error(s): [{RawPrinterHelper.GetJobStatusString(jobInfo)}]"));

                            //jobList.Remove(jobInfo.JobId);
                            /* Don't remove from checking queue. Check again. If error - report error.
                                 * If job removed from print queue - result will be success adn job will be removed from checking queue.
                                 */
                        }
                    }
                }

                // printed (no more in print queue)
                lock (_jobList)
                {
                    var completeJobs = new List<uint>(); // to avoid modifying collection inside foreach
                    
                    foreach (var jobId in _jobList.Keys)
                    {
                        var inQueue = jobInfoList.Any(jobInfo => jobId == jobInfo.JobId);
                        if (!inQueue) completeJobs.Add(jobId);
                    }

                    foreach (var jobId in completeJobs)
                    {
                        OnPrint(new PrinterPrintEventArgs(_jobList[jobId]._invoiceId));
                        _jobList.Remove(jobId);
                    }
                }

                if (bw.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                // check for complete
                lock (_jobList)
                {
                    if (_invoiceProvider.IsCompleted && _printerCompleted && _jobList.Count == 0)
                    {
                        _monitorCompleted = true;
                        break;
                    }
                }

                Thread.Sleep(100);
            } // infinite loop

            _monitorResetEvent.Set();
        }

        public void SetName(string printerName)
        {
            this._printerName = printerName;
        }

        public event PrinterPrintEventHandler Print;


        private void OnPrint(PrinterPrintEventArgs e)
        {
            Print?.Invoke(this, e);
        }

        public event PrinterJobErrorEventHandler JobError;

        private void OnJobError(PrinterJobErrorEventArgs e)
        {
            JobError?.Invoke(this, e);
        }

        public event PrinterPrinterErrorEventHandler PrinterError;

        private void OnPrinterError(PrinterPrinterErrorEventArgs e)
        {
            PrinterError?.Invoke(this, e);
        }

        private uint PrintPdf(IntPtr aPrinterHandle, RawPrinterHelper.DocInfoA aDocInfo, byte[] aPdf)
        {
            var jobId = RawPrinterHelper.StartDoc(aPrinterHandle, aDocInfo);
            RawPrinterHelper.StartPage(aPrinterHandle);
            RawPrinterHelper.Write(aPrinterHandle, aPdf);
            RawPrinterHelper.EndPage(aPrinterHandle);
            RawPrinterHelper.EndDoc(aPrinterHandle);

            return jobId;
        }

        private void PrintPackage(IntPtr aPrinterHandle, PrintPackageWrapper aPrintInvoiceWrapper)
        {
            try
            {
                var printerInfo2 = RawPrinterHelper.GetPrinterInfo2(_printerHandle);
                if ((printerInfo2.Status &
                     RawPrinterHelper.StatusError) != 0)
                {
                    throw new Exception(RawPrinterHelper.GetPrinterStatusString(printerInfo2));
                }

                var pdf = aPrintInvoiceWrapper.Pdf;

                if (_isSequenceNumberEnabled)
                {
                    Routines.AddSequenceNumberToPdf(
                        Routines.GenerateSequenceNumber(aPrintInvoiceWrapper._printBatchId,
                            aPrintInvoiceWrapper._printBatchCount, aPrintInvoiceWrapper._elementBatch,
                            aPrintInvoiceWrapper._elementBatchCount), ref pdf, aPrintInvoiceWrapper._isPackJacket);
                }

                var docInfo = new RawPrinterHelper.DocInfoA
                {
                    pDocName = $"Invoice {aPrintInvoiceWrapper.PackageId}",
                    pDataType = "RAW"
                };
                var jobId = PrintPdf(aPrinterHandle, docInfo, pdf);

                var jobData = new JobData
                {
                    _jobId = jobId,
                    _invoiceId = aPrintInvoiceWrapper.PackageId
                };

                lock (_jobList)
                {
                    _jobList[jobId] = jobData;
                }
            }
            catch (Exception e)
            {
                // Printer error
                _printerError = true;
                OnPrinterError(new PrinterPrinterErrorEventArgs(e.Message));
            }
        }

        public void CleanUp()
        {
            lock (_jobList)
            {
                _jobList.Clear();
            }
        }

        public struct JobData
        {
            public uint _jobId;
            public PackageIdType _invoiceId;
        }
    }
}