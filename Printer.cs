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

        public event PrinterPrintEventHandler Print;
        public event PrinterJobErrorEventHandler JobError;

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
            Log.Info("Printer thread started");

            var bw = sender as BackgroundWorker;

            while (true)
            {
                if (_jobList.Count < Settings.Default.PrintJobQueueMaxSize)
                {
                    var package = _invoiceProvider.GetInvoice();

                    if (package != null && !_printerError)
                    {
                        // Mater Pick List
                        if (_printPickList && package._isFirstBatchPackage)
                        {
                            PrintPdf("Master pick list", Routines.GetMasterPickListPdf(package, _labelService));
                        }

                        PrintPackage(package);

                        Log.Debug($"Printer: {package.PackageId.ToString()} sent to printer");
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
            }

            _printerResetEvent.Set();

            Log.Info("Printer thread stopped");
        }

        /*
         * Check print job status
         * Success if 1) appropriate job status 2) job no more in print queue
         */
        private void bwMonitor_DoWork(object sender, DoWorkEventArgs e)
        {
            var bw = sender as BackgroundWorker;

            while(true)
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

                    if ((jobInfo.Status & RawPrinterHelper.JobStatusPrinting) > 0 
                        && (jobInfo.Status & (RawPrinterHelper.JobStatusError | RawPrinterHelper.JobStatusOffline | RawPrinterHelper.JobStatusPaperOut | RawPrinterHelper.JobStatusBlockedDevq)) > 0)
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

        public void SetName(string printerName) => _printerName = printerName;

        private void OnPrint(PrinterPrintEventArgs e) => Print?.Invoke(this, e);

        private void OnJobError(PrinterJobErrorEventArgs e) => JobError?.Invoke(this, e);

        public event PrinterPrinterErrorEventHandler PrinterError;

        private void OnPrinterError(PrinterPrinterErrorEventArgs e) => PrinterError?.Invoke(this, e);

        private uint PrintPdf(string docName, byte[] pdf)
        {
            var printerHandle = _printerHandle;
            
            var docInfo = new RawPrinterHelper.DocInfoA
            {
                pDocName = docName,
                pDataType = Environment.OSVersion.Version.Major * 100 + Environment.OSVersion.Version.Minor == 601 ? "RAW" : "XPS_PASS"
            };

            var jobId = RawPrinterHelper.StartDoc(printerHandle, docInfo);

            RawPrinterHelper.StartPage(printerHandle);
            RawPrinterHelper.Write(printerHandle, pdf);
            RawPrinterHelper.EndPage(printerHandle);
            RawPrinterHelper.EndDoc(printerHandle);

            return jobId;
        }

        private void PrintPackage(PrintPackageWrapper printInvoiceWrapper)
        {
            try
            {
                var printerInfo2 = RawPrinterHelper.GetPrinterInfo2(_printerHandle);

                if ((printerInfo2.Status & RawPrinterHelper.StatusError) != 0)
                    throw new Exception(RawPrinterHelper.GetPrinterStatusString(printerInfo2));

                var pdf = printInvoiceWrapper.Pdf;

                if (_isSequenceNumberEnabled)
                {
                    var sequenceNumber = Routines.GenerateSequenceNumber(printInvoiceWrapper._printBatchId,
                        printInvoiceWrapper._printBatchCount, printInvoiceWrapper._elementBatch,
                        printInvoiceWrapper._elementBatchCount);

                    pdf = Routines.AddSequenceNumberToPdf(sequenceNumber, pdf, printInvoiceWrapper._isPackJacket);
                }

                var jobId = PrintPdf($"Invoice {printInvoiceWrapper.PackageId}", pdf);

                var jobData = new JobData
                {
                    _jobId = jobId,
                    _invoiceId = printInvoiceWrapper.PackageId
                };

                lock (_jobList) 
                    _jobList[jobId] = jobData;
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
                _jobList.Clear();
        }

        public struct JobData
        {
            public uint _jobId;
            public PackageIdType _invoiceId;
        }
    }
}