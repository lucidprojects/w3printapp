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
        private readonly BackgroundWorker bwMonitor;

        private readonly BackgroundWorker bwPrinter;

        private readonly InvoiceProvider invoiceProvider;
        private readonly Dictionary<uint, JobData> jobList = new Dictionary<uint, JobData>();
        private readonly LabelService labelService;
        private readonly AutoResetEvent monitorResetEvent = new AutoResetEvent(false);

        private readonly AutoResetEvent printerResetEvent = new AutoResetEvent(false);
        private RawPrinterHelper.DocInfoA docInfo = new RawPrinterHelper.DocInfoA();

        private bool isSequenceNumberEnabled;

        private bool monitorCompleted;
        private bool printerCompleted;
        private bool printerError;

        private IntPtr printerHandle = new IntPtr(0);
        private string printerName = "";

        private bool printPickList;

        // constructor
        public Printer(InvoiceProvider aInvoiceProvider, LabelService aClient)
        {
            invoiceProvider = aInvoiceProvider;
            labelService = aClient;
            //invoiceProvider.Load += new InvoiceProviderLoadEventHandler(invoiceProvider_Load);

            bwPrinter = new BackgroundWorker();
            bwPrinter.WorkerSupportsCancellation = true;
            bwPrinter.DoWork += bwPrinter_DoWork;

            bwMonitor = new BackgroundWorker();
            bwMonitor.WorkerSupportsCancellation = true;
            bwMonitor.DoWork += bwMonitor_DoWork;
        }

        public bool IsSequenceNumberEnabled
        {
            set => isSequenceNumberEnabled = value;
        }

        public bool PrintPickList
        {
            set => printPickList = value;
        }

        public bool IsCompleted => printerCompleted && monitorCompleted;

        public void Run()
        {
            printerHandle = RawPrinterHelper.Open(printerName);

            printerCompleted = false;
            monitorCompleted = false;
            printerError = false;

            printerResetEvent.Reset();
            monitorResetEvent.Reset();

            bwMonitor.RunWorkerAsync();
            bwPrinter.RunWorkerAsync();
        }

        public void Stop()
        {
            StopPrinter();
            StopMonitor();

            RawPrinterHelper.Close(printerHandle);
        }

        private void StopPrinter()
        {
            bwPrinter.CancelAsync();
            printerResetEvent.WaitOne();
        }

        private void StopMonitor()
        {
            bwMonitor.CancelAsync();
            monitorResetEvent.WaitOne();
        }

        private void bwPrinter_DoWork(object sender, DoWorkEventArgs e)
        {
            Log.GetLogger().Info("Printer thread started");

            var bw = sender as BackgroundWorker;

            for (;;)
            {
                if (jobList.Count < Settings.Default.PrintJobQueueMaxSize)
                {
                    var package = invoiceProvider.getInvoice();
                    if (package != null && !printerError)
                    {
                        // Mater Pick List
                        if (printPickList && package._isFirstBatchPackage)
                        {
                            var docInfo = new RawPrinterHelper.DocInfoA
                            {
                                pDocName = "Master pick list",
                                pDataType = "RAW"
                            };

                            PrintPdf(printerHandle, docInfo, Routines.getMasterPickListPdf(package, labelService));
                        }

                        PrintPackage(printerHandle, package);

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
                if (invoiceProvider.IsCompleted)
                {
                    printerCompleted = true;
                    break;
                }


                Thread.Sleep(100);
            } // infinite loop

            printerResetEvent.Set();

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
                var jobInfoList = RawPrinterHelper.EnumJobs(printerHandle);

                foreach (var jobInfo in jobInfoList)
                {
                    // printed (by job status)
                    if ((jobInfo.Status & RawPrinterHelper.JobStatusPrinted) != 0)
                    {
                        lock (jobList)
                        {
                            if (jobList.ContainsKey(jobInfo.JobId))
                            {
                                OnPrint(new PrinterPrintEventArgs(jobList[jobInfo.JobId]._invoiceId));
                                jobList.Remove(jobInfo.JobId);
                            }
                        }
                    }

                    if ((jobInfo.Status & RawPrinterHelper.JobStatusPrinting) > 0 && (jobInfo.Status & (RawPrinterHelper.JobStatusError | RawPrinterHelper.JobStatusOffline | RawPrinterHelper.JobStatusPaperOut | RawPrinterHelper.JobStatusBlockedDevq)) > 0) 
                    {
                        lock (jobList)
                        {
                            if (jobList.TryGetValue(jobInfo.JobId, out var value)) 
                                OnJobError(new PrinterJobErrorEventArgs(value._invoiceId, $"Printer job error(s): [{RawPrinterHelper.GetJobStatusString(jobInfo)}]"));

                            //jobList.Remove(jobInfo.JobId);
                            /* Don't remove from checking queue. Check again. If error - report error.
                                 * If job removed from print queue - result will be success adn job will be removed from checking queue.
                                 */
                        }
                    }
                }

                // printed (no more in print queue)
                lock (jobList)
                {
                    var completeJobs = new List<uint>(); // to avoid modifying collection inside foreach
                    
                    foreach (var jobId in jobList.Keys)
                    {
                        var inQueue = jobInfoList.Any(jobInfo => jobId == jobInfo.JobId);
                        if (!inQueue) completeJobs.Add(jobId);
                    }

                    foreach (var jobId in completeJobs)
                    {
                        OnPrint(new PrinterPrintEventArgs(jobList[jobId]._invoiceId));
                        jobList.Remove(jobId);
                    }
                }

                if (bw.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                // check for complete
                lock (jobList)
                {
                    if (invoiceProvider.IsCompleted && printerCompleted && jobList.Count == 0)
                    {
                        monitorCompleted = true;
                        break;
                    }
                }

                Thread.Sleep(100);
            } // infinite loop

            monitorResetEvent.Set();
        }

        public void SetName(string printerName)
        {
            this.printerName = printerName;
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
                var printerInfo2 = RawPrinterHelper.GetPrinterInfo2(printerHandle);
                if ((printerInfo2.Status &
                     RawPrinterHelper.StatusError) != 0)
                {
                    throw new Exception(RawPrinterHelper.GetPrinterStatusString(printerInfo2));
                }

                var pdf = aPrintInvoiceWrapper.Pdf;

                if (isSequenceNumberEnabled)
                {
                    Routines.addSequenceNumberToPdf(
                        Routines.generateSequenceNumber(aPrintInvoiceWrapper._printBatchId,
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

                lock (jobList)
                {
                    jobList[jobId] = jobData;
                }
            }
            catch (Exception e)
            {
                // Printer error
                printerError = true;
                OnPrinterError(new PrinterPrinterErrorEventArgs(e.Message));
            }
        }

        public void CleanUp()
        {
            lock (jobList)
            {
                jobList.Clear();
            }
        }

        public struct JobData
        {
            public uint _jobId;
            public PackageIdType _invoiceId;
        }
    }
}