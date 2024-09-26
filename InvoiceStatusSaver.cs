using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace PrintInvoice
{
  using InvoiceIdType = Int32;

  public class InvoiceStatusSaverSaveEventArgs : EventArgs
  {
    private InvoiceIdType invoiceId;

    public InvoiceStatusSaverSaveEventArgs(InvoiceIdType aInvoiceId)
    {
      invoiceId = aInvoiceId;
    }

    public InvoiceIdType InvoiceId
    {
      get { return invoiceId; }
    }
  }

  public delegate void InvoiceStatusSaverSaveEventHandler(object sender, InvoiceStatusSaverSaveEventArgs e);

  public class InvoiceStatusSaverErrorEventArgs : EventArgs
  {
    private InvoiceIdType invoiceId;
    private string message;

    public InvoiceStatusSaverErrorEventArgs(InvoiceIdType aInvoiceId, string aMessage)
    {
      invoiceId = aInvoiceId;
      message = aMessage;
    }

    public InvoiceIdType InvoiceId
    {
      get { return invoiceId; }
    }

    public string Message
    {
      get { return message; }
    }
  }

  public delegate void InvoiceStatusSaverErrorEventHandler(object sender, InvoiceStatusSaverErrorEventArgs e);

  public class InvoiceStatusSaverCompleteEventArgs : EventArgs{}

  public delegate void InvoiceStatusSaverCompleteEventHandler(object sender, InvoiceStatusSaverCompleteEventArgs e);


  public class InvoiceStatusSaver
  {
    private Printer printer;
    private LabelService client;

    private Queue<InvoiceIdType> queue = new Queue<InvoiceIdType>();
    private BackgroundWorker bwSave;

    private AutoResetEvent resetEvent;

    private int batchId;
    public int BatchId {
      set { batchId = value; }
      get { return batchId; }
    }

    private bool isReprint;
    public bool IsReprint
    {
      set { isReprint = value; }
    }

    public bool MayEnqueue
    {
      get 
      {
        lock (queue)
        { 
          return (queue.Count < Properties.Settings.Default.SaveStatusQueueMaxSize);
        }
      }
    }

    public InvoiceStatusSaver(Printer aPrinter, LabelService aClient)
    {
      printer = aPrinter;
      client = aClient;

      bwSave = new BackgroundWorker();
      bwSave.WorkerSupportsCancellation = true;
      bwSave.DoWork += new DoWorkEventHandler(bwSave_DoWork);

      printer.Print += new PrinterPrintEventHandler(printer_Print);

      resetEvent = new AutoResetEvent(false);
    }

    public void run()
    {
      resetEvent.Reset();
      bwSave.RunWorkerAsync();
    }

    public void stop()
    {
      bwSave.CancelAsync();
      resetEvent.WaitOne();
    }

    void bwSave_DoWork(object sender, DoWorkEventArgs e)
    {
      Log.getLogger().Info(String.Format("StatusSaver thread started"));

      BackgroundWorker bw = sender as BackgroundWorker;

      for (;;)
      {
        lock (queue)
        {
          if (queue.Count > 0)
          {
            InvoiceIdType invoiceId;
            invoiceId = queue.Dequeue();

            try
            {
              SetLabelPrintedRequestType request = new SetLabelPrintedRequestType();
              request.labelId = invoiceId;
              request.createNewBatch = false;
              request.batchId = batchId;
              request.printStationId = Properties.Settings.Default.PrintStationId;

              SetLabelPrintedResponseType response = client.setLabelPrinted(request);
              if (response.status != 0)
              {
                throw new Exception(response.message);
              }

              // Save
              InvoiceStatusSaverSaveEventArgs eventArgs = new InvoiceStatusSaverSaveEventArgs(invoiceId);
              onSave(eventArgs);
              Log.getLogger().Debug(String.Format("StatusSaver: {0} status saved", invoiceId.ToString()));
            }
            catch (Exception ex)
            {
              // Error
              queue.Enqueue(invoiceId);
              InvoiceStatusSaverErrorEventArgs eventArgs = new InvoiceStatusSaverErrorEventArgs(invoiceId, ex.Message);
              onError(eventArgs);
            }
          }
        }
        
        // all statuses must be saved
        lock (queue)
        {
          if (bw.CancellationPending && queue.Count == 0)
          {
            e.Cancel = true;
            break;
          }
        }

        // check for complete
        lock (queue)
        {
          if (queue.Count == 0 && printer.IsCompleted)
          {
            onComplete(new InvoiceStatusSaverCompleteEventArgs());
            break;
          }
        }

        Thread.Sleep(100);
      }

      resetEvent.Set();

      Log.getLogger().Info(String.Format("StatusSaver thread stopped"));
    }

    void printer_Print(object sender, PrinterPrintEventArgs e)
    {
      lock (queue)
      {
        queue.Enqueue(e.PackageId);
      }
    }

    public event InvoiceStatusSaverSaveEventHandler Save;

    private void onSave(InvoiceStatusSaverSaveEventArgs e)
    {
      if (Save != null)
        Save(this, e);
    }

    public event InvoiceStatusSaverErrorEventHandler Error;

    private void onError(InvoiceStatusSaverErrorEventArgs e)
    {
      if (Error != null)
        Error(this, e);
    }

    public event InvoiceStatusSaverCompleteEventHandler Complete;

    private void onComplete(InvoiceStatusSaverCompleteEventArgs e)
    {
      if (Complete != null)
        Complete(this, e);
    }

    public void cleanUp()
    {
      lock (queue)
      {
        queue.Clear();
      }
    }

  }
}
