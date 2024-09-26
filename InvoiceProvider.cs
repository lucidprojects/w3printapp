using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace PrintInvoice
{
  public class InvoiceProviderLoadEventArgs : EventArgs
  { 
    private PrintPackageWrapper printInvoiceWrapper;

    public InvoiceProviderLoadEventArgs(PrintPackageWrapper aPrintInvoiceWrapper)
    {
      printInvoiceWrapper = aPrintInvoiceWrapper;
    }

    public PrintPackageWrapper PrintInvoiceWrapper
    {
      get { return printInvoiceWrapper; }
    }
  }

  public delegate void InvoiceProviderLoadEventHandler(object sender, InvoiceProviderLoadEventArgs e);

  public class InvoiceProviderErrorEventArgs : EventArgs
  {
    private int packageId;
    private string message;

    public InvoiceProviderErrorEventArgs(int aInvoiceId, string aMessage)
    {
      packageId = aInvoiceId;
      message = aMessage;
    }

    public int InvoiceId
    {
      get { return packageId; }
    }

    public string Message
    {
      get { return message; }
    }
  }

  public delegate void InvoiceProviderErrorEventHandler(object sender, InvoiceProviderErrorEventArgs e);

  public class InvoiceProvider
  {
    private List<PrintPackageWrapper> list = new List<PrintPackageWrapper>();
    private Queue<PrintPackageWrapper> loadCache = new Queue<PrintPackageWrapper>(Properties.Settings.Default.InvoiceLoaderCacheSize);
    private LabelService client = null;
    private BackgroundWorker bwLoad;

    private AutoResetEvent resetEvent = new AutoResetEvent(false);
    private bool completed;

    private bool lockPackages;

    public InvoiceProvider(LabelService aClient)
    {
      client = aClient;

      bwLoad = new BackgroundWorker();
      bwLoad.WorkerSupportsCancellation = true;
      bwLoad.DoWork += new DoWorkEventHandler(bwLoad_DoWork);
    }

    public bool LockPackages
    {
      set { lockPackages = value; }
    }

    void bwLoad_DoWork(object sender, DoWorkEventArgs e)
    {
      Log.getLogger().Info(String.Format("InvoiceProvider thread started"));

      BackgroundWorker bw = sender as BackgroundWorker;

      while (list.Count > 0)
      {
        if (loadCache.Count < Properties.Settings.Default.InvoiceLoaderCacheSize)
        {

          PrintPackageWrapper package = list[0];
          GetLabelRequestType request = new GetLabelRequestType();
          request.packageId = package.PackageId;
          request.@lock = lockPackages;
          request.isReprint = isReprint;
          request.isPackJacket = package.isPackJacket;

          if (!isReprint)
          {
            request.isPrintReprint = package.IsPrinted;
          }


          try
          {
            GetLabelResponseType response = client.getLabel(request);

            if (response.status != 0)
            {
              throw new Exception(response.message);
            }

            if (!lockPackages)
            {
              package.State = PrintPackageWrapper.LOADED;
            }
            else
            {
              if (response.locked)
              {
                package.State = PrintPackageWrapper.LOADED;
              }
              else
              {
                package.State = PrintPackageWrapper.LOCKED; // locked by another process
              }
            }

            if (response.base64data != null)
            {
              package.Pdf = Convert.FromBase64String(response.base64data);
            }

            // Load event
            onLoad(new InvoiceProviderLoadEventArgs(package));

            // enqueue to cache
            if (package.IsLoaded)
            {
              loadCache.Enqueue(package);
            }

            Log.getLogger().Debug(String.Format("InvoiceProvider: {0} enqueued to cache", package.PackageId.ToString()));
          }
          catch (Exception ex)
          {
            // Error event
            onError(new InvoiceProviderErrorEventArgs(package.PackageId, ex.Message));
          }

          list.RemoveAt(0);

        } // if loadCache < max

        if (bw.CancellationPending)
        {
          e.Cancel = true;
          break;
        }

        Thread.Sleep(100);
      } // while idList.Count > 0

      if (!e.Cancel)
      {
        completed = true;
      }

      resetEvent.Set();

      Log.getLogger().Info(String.Format("InvoiceProvider thread stopped"));
    }

    public void run()
    {
      resetEvent.Reset();
      bwLoad.RunWorkerAsync();
    }

    public void stop()
    {
      bwLoad.CancelAsync();
      resetEvent.WaitOne();
    }

    public event InvoiceProviderLoadEventHandler Load;

    private void onLoad(InvoiceProviderLoadEventArgs e)
    {
      if (Load != null)
        Load(this, e);
    }

    public event InvoiceProviderErrorEventHandler Error;

    private void onError(InvoiceProviderErrorEventArgs e)
    {
      if (Error != null)
        Error(this, e);
    }

    public bool IsCompleted
    {
      get { return completed && (loadCache.Count == 0); }
    }

    private bool isReprint;
    public bool IsReprint
    {
      set { isReprint = value; }
    }

    public void setList(List<PrintPackageWrapper> aList)
    {
      list.Clear();

      foreach (PrintPackageWrapper package in aList)
      {
        list.Add(package);
      }

      completed = false;
    }

    public PrintPackageWrapper getInvoice()
    {
      lock (loadCache)
      {
        if (loadCache.Count > 0)
        {
          PrintPackageWrapper package = loadCache.Dequeue();
          Log.getLogger().Debug(String.Format("InvoiceProvider.getInvoice(): {0} returned", package.PackageId.ToString()));
          return package;
        }
        else
        {
          return null;
        }
      }
    }

    public void cleanUp()
    {
      lock (loadCache)
      {
        loadCache.Clear(); 
      }
    }
  }
}
