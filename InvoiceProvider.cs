using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using PrintInvoice.Properties;

namespace PrintInvoice
{
    public class InvoiceProviderLoadEventArgs : EventArgs
    {
        public InvoiceProviderLoadEventArgs(PrintPackageWrapper aPrintInvoiceWrapper)
        {
            PrintInvoiceWrapper = aPrintInvoiceWrapper;
        }

        public PrintPackageWrapper PrintInvoiceWrapper { get; }
    }

    public delegate void InvoiceProviderLoadEventHandler(object sender, InvoiceProviderLoadEventArgs e);

    public class InvoiceProviderErrorEventArgs : EventArgs
    {
        public InvoiceProviderErrorEventArgs(int aInvoiceId, string aMessage)
        {
            InvoiceId = aInvoiceId;
            Message = aMessage;
        }

        public int InvoiceId { get; }

        public string Message { get; }
    }

    public delegate void InvoiceProviderErrorEventHandler(object sender, InvoiceProviderErrorEventArgs e);

    public class InvoiceProvider
    {
        private readonly BackgroundWorker _bwLoad;
        private readonly LabelService _client;
        private readonly List<PrintPackageWrapper> _list = new List<PrintPackageWrapper>();
        private readonly Queue<PrintPackageWrapper> _loadCache = new Queue<PrintPackageWrapper>(Settings.Default.InvoiceLoaderCacheSize);
        private readonly AutoResetEvent _resetEvent = new AutoResetEvent(false);
        private bool _completed;
        private bool _isReprint;
        private bool _lockPackages;

        public InvoiceProvider(LabelService aClient)
        {
            _client = aClient;

            _bwLoad = new BackgroundWorker();
            _bwLoad.WorkerSupportsCancellation = true;
            _bwLoad.DoWork += bwLoad_DoWork;
        }

        public bool LockPackages
        {
            set => _lockPackages = value;
        }

        public bool IsCompleted => _completed && _loadCache.Count == 0;

        public bool IsReprint
        {
            set => _isReprint = value;
        }

        private void bwLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            Log.Info("InvoiceProvider thread started");

            var bw = sender as BackgroundWorker;

            while (_list.Count > 0)
            {
                if (_loadCache.Count < Settings.Default.InvoiceLoaderCacheSize)
                {
                    var package = _list[0];
                    
                    var request = new GetLabelRequestType
                    {
                        packageId = package.PackageId,
                        @lock = _lockPackages,
                        isReprint = _isReprint,
                        isPackJacket = package._isPackJacket
                    };

                    if (!_isReprint) request.isPrintReprint = package.IsPrinted;


                    try
                    {
                        var response = _client.getLabel(request);

                        if (response.status != 0) throw new Exception(response.message);

                        package.State = !_lockPackages ? PrintPackageWrapper.Loaded : response.locked ? PrintPackageWrapper.Loaded : PrintPackageWrapper.Locked; // locked by another process

                        if (response.base64data != null) package.Pdf = Convert.FromBase64String(response.base64data);

                        // Load event
                        OnLoad(new InvoiceProviderLoadEventArgs(package));

                        // enqueue to cache
                        if (package.IsLoaded) _loadCache.Enqueue(package);

                        Log.Debug($"InvoiceProvider: {package.PackageId.ToString()} enqueued to cache");
                    }
                    catch (Exception ex)
                    {
                        // Error event
                        OnError(new InvoiceProviderErrorEventArgs(package.PackageId, ex.Message));
                    }

                    _list.RemoveAt(0);
                } // if loadCache < max

                if (bw.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                Thread.Sleep(100);
            } // while idList.Count > 0

            if (!e.Cancel) _completed = true;

            _resetEvent.Set();

            Log.Info("InvoiceProvider thread stopped");
        }

        public void Run()
        {
            _resetEvent.Reset();
            _bwLoad.RunWorkerAsync();
        }

        public void Stop()
        {
            _bwLoad.CancelAsync();
            _resetEvent.WaitOne();
        }

        public event InvoiceProviderLoadEventHandler Load;

        private void OnLoad(InvoiceProviderLoadEventArgs e)
        {
            Load?.Invoke(this, e);
        }

        public event InvoiceProviderErrorEventHandler Error;

        private void OnError(InvoiceProviderErrorEventArgs e)
        {
            Error?.Invoke(this, e);
        }

        public void SetList(List<PrintPackageWrapper> aList)
        {
            _list.Clear();

            foreach (var package in aList) 
                _list.Add(package);

            _completed = false;
        }

        public PrintPackageWrapper GetInvoice()
        {
            lock (_loadCache)
            {
                if (_loadCache.Count > 0)
                {
                    var package = _loadCache.Dequeue();
                    Log.Debug($"InvoiceProvider.getInvoice(): {package.PackageId.ToString()} returned");
                    return package;
                }

                return null;
            }
        }

        public void CleanUp()
        {
            lock (_loadCache) 
                _loadCache.Clear();
        }
    }
}