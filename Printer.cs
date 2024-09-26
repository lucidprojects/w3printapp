using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading;

namespace PrintInvoice
{
  using PackageIdType = Int32;

  public class PrinterPrintEventArgs : EventArgs
  {
    private PackageIdType invoiceId;

    public PrinterPrintEventArgs(PackageIdType aInvoiceId)
    {
      invoiceId = aInvoiceId;
    }

    public PackageIdType PackageId
    {
      get { return invoiceId; }
    }
  }

  public delegate void PrinterPrintEventHandler(object sender, PrinterPrintEventArgs e);

  public class PrinterJobErrorEventArgs : EventArgs
  {
    private PackageIdType invoiceId;
    private string message;

    public PrinterJobErrorEventArgs(PackageIdType aInvoiceId, string aMessage)
    {
      invoiceId = aInvoiceId;
      message = aMessage;
    }

    public PackageIdType PackageId
    {
      get { return invoiceId; }
    }

    public string Message
    {
      get { return message; }
    }
  }

  public delegate void PrinterJobErrorEventHandler(object sender, PrinterJobErrorEventArgs e);

  public class PrinterPrinterErrorEventArgs : EventArgs
  {
    private string message;

    public PrinterPrinterErrorEventArgs(string aMessage)
    {
      message = aMessage;
    }

    public string Message
    {
      get { return message; }
    }
  }

  public delegate void PrinterPrinterErrorEventHandler(object sender, PrinterPrinterErrorEventArgs e);

  public class Printer
  {
    public struct JobData
    {
      public uint jobId;
      public PackageIdType invoiceId;
    }

    private InvoiceProvider invoiceProvider;
    private LabelService labelService;

    private IntPtr printerHandle = new IntPtr(0);
    private RawPrinterHelper.DOCINFOA docInfo = new RawPrinterHelper.DOCINFOA();
    private string printerName = "";
    private Dictionary<uint, JobData> jobList = new Dictionary<uint, JobData>();

    BackgroundWorker bwPrinter;
    BackgroundWorker bwMonitor;

    private bool monitorCompleted;
    private bool printerCompleted;
    private bool printerError;

    private AutoResetEvent printerResetEvent = new AutoResetEvent(false);
    private AutoResetEvent monitorResetEvent = new AutoResetEvent(false);

    private bool isSequenceNumberEnabled = false;
    public bool IsSequenceNumberEnabled {
      set { isSequenceNumberEnabled = value; }
    }

    private bool printPickList = false;
    public bool PrintPickList
    {
      set { printPickList = value; }
    }

    // constructor
    public Printer(InvoiceProvider aInvoiceProvider, LabelService aClient)
    {
      invoiceProvider = aInvoiceProvider;
      labelService = aClient;
      //invoiceProvider.Load += new InvoiceProviderLoadEventHandler(invoiceProvider_Load);

      bwPrinter = new BackgroundWorker();
      bwPrinter.WorkerSupportsCancellation = true;
      bwPrinter.DoWork += new DoWorkEventHandler(bwPrinter_DoWork);
      
      bwMonitor = new BackgroundWorker();
      bwMonitor.WorkerSupportsCancellation = true;
      bwMonitor.DoWork += new DoWorkEventHandler(bwMonitor_DoWork);
    }

    public void run()
    {
      printerHandle = RawPrinterHelper.open(printerName);

      printerCompleted = false;
      monitorCompleted = false;
      printerError = false;

      printerResetEvent.Reset();
      monitorResetEvent.Reset();

      bwMonitor.RunWorkerAsync();
      bwPrinter.RunWorkerAsync();
    }

    public void stop()
    {
      stopPrinter();
      stopMonitor();

      RawPrinterHelper.close(printerHandle);
    }

    private void stopPrinter()
    {
      bwPrinter.CancelAsync();
      printerResetEvent.WaitOne();
    }

    private void stopMonitor()
    {
      bwMonitor.CancelAsync();
      monitorResetEvent.WaitOne();
    }

    void bwPrinter_DoWork(object sender, DoWorkEventArgs e)
    {
      Log.getLogger().Info(String.Format("Printer thread started"));

      BackgroundWorker bw = sender as BackgroundWorker;

      for (; ; )
      {
        if (jobList.Count < Properties.Settings.Default.PrintJobQueueMaxSize)
        {
          PrintPackageWrapper package = invoiceProvider.getInvoice();
          if (package != null && !printerError)
          {
            // Mater Pick List
            if (printPickList && package.isFirstBatchPackage)
            {
              RawPrinterHelper.DOCINFOA docInfo = new RawPrinterHelper.DOCINFOA();
              docInfo.pDocName = "Master pick list";
              docInfo.pDataType = "RAW";
              printPdf(printerHandle, docInfo, Routines.getMasterPickListPdf(package, labelService));
            }

            printPackage(printerHandle, package);
            
            Log.getLogger().Debug(String.Format("Printer: {0} sent to printer", package.PackageId.ToString()));
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

      Log.getLogger().Info(String.Format("Printer thread stopped"));
    }

    /*
     * Check print job status
     * Success if 1) appropriate job status 2) job no more in print queue
     */
    void bwMonitor_DoWork(object sender, DoWorkEventArgs e)
    {
      BackgroundWorker bw = sender as BackgroundWorker;

      for (;;)
      {
        List<RawPrinterHelper.JOB_INFO_1> jobInfoList = RawPrinterHelper.enumJobs(printerHandle);
        foreach (RawPrinterHelper.JOB_INFO_1 jobInfo in jobInfoList)
        {
          // printed (by job status)
          if ((jobInfo.Status & RawPrinterHelper.JOB_STATUS_PRINTED) != 0)
          {
            lock (jobList)
            {
              if (jobList.ContainsKey(jobInfo.JobId))
              {
                onPrint(new PrinterPrintEventArgs(jobList[jobInfo.JobId].invoiceId));
                jobList.Remove(jobInfo.JobId);
              }
            }
          }

          if ((jobInfo.Status & RawPrinterHelper.JOB_STATUS_PRINTING) > 0)
          {
            if ((jobInfo.Status &
                 (RawPrinterHelper.JOB_STATUS_ERROR |
                 RawPrinterHelper.JOB_STATUS_OFFLINE |
                 RawPrinterHelper.JOB_STATUS_PAPEROUT |
                 RawPrinterHelper.JOB_STATUS_BLOCKED_DEVQ)) > 0)
            {
              lock (jobList)
              {
                if (jobList.ContainsKey(jobInfo.JobId))
                {
                  onJobError(new PrinterJobErrorEventArgs(jobList[jobInfo.JobId].invoiceId, "Printer job error(s): [" + RawPrinterHelper.getJobStatusString(jobInfo) + "]"));
                  //jobList.Remove(jobInfo.JobId);
                  /* Don't remove from checking queue. Check again. If error - report error. 
                   * If job removed from print queue - result will be success adn job will be removed from checking queue.
                   */
                }
              }
            }
          }
        }

        // printed (no more in print queue)
        lock (jobList)
        {
          List<uint> completeJobs = new List<uint>(); // to avoid modifuing collection inside foreach
          foreach (uint jobId in jobList.Keys)
          {
            bool inQueue = false;
            foreach (RawPrinterHelper.JOB_INFO_1 jobInfo in jobInfoList)
            {
              if (jobId == jobInfo.JobId)
              {
                inQueue = true;
                break;
              }
            }

            if (!inQueue)
            {
              completeJobs.Add(jobId);
            }
          }

          foreach (uint jobId in completeJobs)
          {
            onPrint(new PrinterPrintEventArgs(jobList[jobId].invoiceId));
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

    public void setName(string printerName)
    {
      this.printerName = printerName;
    }

    public event PrinterPrintEventHandler Print;


    private void onPrint(PrinterPrintEventArgs e)
    {
      if (Print != null)
        Print(this, e);
    }

    public event PrinterJobErrorEventHandler JobError;

    private void onJobError(PrinterJobErrorEventArgs e)
    {
      if (JobError != null)
        JobError(this, e);
    }

    public event PrinterPrinterErrorEventHandler PrinterError;

    private void onPrinterError(PrinterPrinterErrorEventArgs e)
    {
      if (PrinterError != null)
        PrinterError(this, e);
    }

    private uint printPdf(IntPtr aPrinterHandle, RawPrinterHelper.DOCINFOA aDocInfo, byte[] aPdf) {
      uint jobId = RawPrinterHelper.startDoc(aPrinterHandle, aDocInfo);
      RawPrinterHelper.startPage(aPrinterHandle);
      RawPrinterHelper.write(aPrinterHandle, aPdf);
      RawPrinterHelper.endPage(aPrinterHandle);
      RawPrinterHelper.endDoc(aPrinterHandle);

      return jobId;
    }

    private void printPackage(IntPtr aPrinterHandle, PrintPackageWrapper aPrintInvoiceWrapper)
    {
      try
      {
        RawPrinterHelper.PRINTER_INFO_2 printerInfo2 = RawPrinterHelper.getPrinterInfo2(printerHandle);
        if ((printerInfo2.Status &
          (RawPrinterHelper.PRINTER_STATUS_ERROR)) != 0)
        {
          throw new Exception(RawPrinterHelper.getPrinterStatusString(printerInfo2));
        }

        byte[] pdf = aPrintInvoiceWrapper.Pdf;

        if (isSequenceNumberEnabled) {
          Routines.addSequenceNumberToPdf(Routines.generateSequenceNumber(aPrintInvoiceWrapper.printBatchId, aPrintInvoiceWrapper.printBatchCount, aPrintInvoiceWrapper.elementBatch, aPrintInvoiceWrapper.elementBatchCount), ref pdf, aPrintInvoiceWrapper.isPackJacket);
        }

        RawPrinterHelper.DOCINFOA docInfo = new RawPrinterHelper.DOCINFOA();
        docInfo.pDocName = String.Format("Invoice {0}", aPrintInvoiceWrapper.PackageId);
        docInfo.pDataType = "RAW";
        uint jobId = printPdf(aPrinterHandle, docInfo, pdf);

        JobData jobData = new JobData();
        jobData.jobId = jobId;
        jobData.invoiceId = aPrintInvoiceWrapper.PackageId;

        lock (jobList)
        {
          jobList[jobId] = jobData;
        }
      }
      catch (Exception e)
      {
        // Printer error
        printerError = true;
        onPrinterError(new PrinterPrinterErrorEventArgs(e.Message));
      }
    }

    public bool IsCompleted
    {
      get { return printerCompleted && monitorCompleted; }
    }

    public void cleanUp()
    {
      lock (jobList)
      {
        jobList.Clear();
      }
    }

  }
}
