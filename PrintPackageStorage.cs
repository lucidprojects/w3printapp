﻿using System;
using System.Collections.Generic;
using System.Security;
using PrintInvoice.Properties;

namespace PrintInvoice
{
    using PackageList = List<PrintPackageWrapper>;

    public class PrintPackageWrapper : PackageWrapper
    {
        public const int Unprinted = 0x01;
        public const int Loaded = 0x02;
        public const int Printed = 0x04;
        public const int StatusSaved = 0x08;
        public const int Error = 0x10;
        public const int Locked = 0x20;
        public int _elementBatch;
        public int _elementBatchCount;

        // Master Pisk List
        public bool _isFirstBatchPackage;

        // Pack Jacket
        public bool _isPackJacket;
        public int _mplElementBatchCount; // mater picklist element batch count
        public int _printBatchCount;

        // sequence number
        public int _printBatchId;

        private int _state;

        public PrintPackageWrapper(int aPackageId)
            : base(aPackageId)
        {
            State = Unprinted;
        }

        public int State
        {
            set
            {
                if (value == Unprinted) _state = Unprinted;

                if (value == Loaded) _state = Unprinted | Loaded;

                if (value == Printed) _state = Loaded | Printed;

                if (value == StatusSaved) _state = Loaded | Printed | StatusSaved;

                if (value == Error) _state = Error;

                if (value == Locked) _state = Locked;
            }
        }

        public bool IsUnprinted => (_state & Unprinted) != 0;
        public bool IsLoaded => (_state & Loaded) != 0;
        public bool IsPrinted => (_state & Printed) != 0;
        public bool IsStatusSaved => (_state & StatusSaved) != 0;
        public bool IsError => (_state & Error) != 0;
        public bool IsLocked => (_state & Locked) != 0; // locked by another process

        public byte[] Pdf { get; set; }

        public string getOrderedElemens()
        {
            return FieldValueList[Settings.Default.ElementsOrderedFieldIndex];
        }
    }

    // Update event delegate
    public delegate void InvoiceStorageUpdateEventHandler(object sender, EventArgs e);

    // UpdateSubset event delegate
    public delegate void InvoiceStorageUpdateSubsetEventHandler(object sender, EventArgs e);

    // UpdatePackageState event args
    public class PrintPackageStorageUpdatePackageStateEventArgs : EventArgs
    {
        public PrintPackageStorageUpdatePackageStateEventArgs(int aPackageId)
        {
            PackageId = aPackageId;
        }

        public int PackageId { get; }
    }

    // UpdateInvoiceState event delegate
    public delegate void PrintPackageStorageUpdatePackageStateEventHandler(object sender,
        PrintPackageStorageUpdatePackageStateEventArgs e);


    internal class PrintPackageStorage : PackageStorage<PrintPackageWrapper>
    {
        // consts
        public const int PackageIdColumnIndex = 0;
        public const int InvoiceNumberColumnIndex = 1;
        public const int TrackingNumberColumnIndex = 2;
        private readonly Config _config;
        private readonly PackageList _customPackageList = new PackageList();

        private readonly LabelService _labelService;

        private readonly PackageList _subsetPackageList = new PackageList();
        private List<string> _fieldNames = new List<string>();

        private int _queryIndex;

        public PrintPackageStorage(Config aConfig, LabelService aLabelService)
        {
            _config = aConfig;
            _labelService = aLabelService;
        }

        public PackageList SubsetPackageList { get; private set; }

        public event InvoiceStorageUpdateEventHandler Update;
        public event InvoiceStorageUpdateSubsetEventHandler UpdateSubset;
        public event PrintPackageStorageUpdatePackageStateEventHandler UpdatePackageState;

        public void setQuery(int aQueryIndex)
        {
            clear();
            _customPackageList.Clear();
            _subsetPackageList.Clear();

            var request = new RunSqlQueryRequestType
            {
                query = SecurityElement.Escape(_config.QueryList[aQueryIndex].Text),
                clientVersion = Routines.getVersion()
            };
            var response = _labelService.runSqlQuery(request);
            if (response.status != 0)
                throw new Exception(
                    $"Label service returns error status\nStatus: {response.status}\nMessage: {response.message}\nSubstatus: {response.substatus}\nSubmessage: {response.submessage}");

            if (response.rows != null)
                foreach (var row in response.rows)
                {
                    var packageId = int.Parse(row.columns[PackageIdColumnIndex]);
                    var package = new PrintPackageWrapper(packageId)
                    {
                        // TODO: set tracking number column index in settings
                        TrackingNumber = row.columns[TrackingNumberColumnIndex],
                        FieldValueList = row.columns
                    };
                    add(package);
                }

            _fieldMetadata = response.meta;

            onUpdate(EventArgs.Empty);

            _queryIndex = aQueryIndex;
        }

        public void setSubsetAll()
        {
            SubsetPackageList = _packageList;
            onUpdateSubset(EventArgs.Empty);
        }

        public void setSubsetCustom()
        {
            SubsetPackageList = _customPackageList;
            onUpdateSubset(EventArgs.Empty);
        }

        public void setSubset(int aSubqueryIndex)
        {
            _subsetPackageList.Clear();
            var request = new RunSqlQueryRequestType
            {
                query = SecurityElement.Escape(_config.QueryList[_queryIndex].SubqueryList[aSubqueryIndex].Text),
                clientVersion = Routines.getVersion()
            };
            var response = _labelService.runSqlQuery(request);
            if (response.status != 0)
                throw new Exception(
                    $"Label service returns error status\nStatus: {response.status}\nMessage: {response.message}\nSubstatus: {response.substatus}\nSubmessage: {response.submessage}");

            if (response.rows != null)
                foreach (var responseRow in response.rows)
                {
                    var invoiceId = responseRow.columns[0];
                    foreach (var invoiceWrapper in _packageList)
                        if (invoiceWrapper.FieldValueList[0] == invoiceId)
                        {
                            _subsetPackageList.Add(invoiceWrapper);
                            break;
                        }
                }

            SubsetPackageList = _subsetPackageList;
            onUpdateSubset(EventArgs.Empty);
        }

        public void addToCustomSubset(List<int> aIdList)
        {
            foreach (var id in aIdList)
            {
                var invoice = _packageIdIndex[id];
                if (!_customPackageList.Contains(invoice)) _customPackageList.Add(invoice);
            }

            onUpdateSubset(EventArgs.Empty);
        }

        public void removeFromCustomSubset(List<int> aIdList)
        {
            foreach (var id in aIdList)
            {
                var invoice = _packageIdIndex[id];
                _customPackageList.Remove(invoice);
            }

            onUpdateSubset(EventArgs.Empty);
        }

        public void setPackageState(int aPackageId, int aState, string aErrorText)
        {
            var package = getPackageByPackageId(aPackageId);
            package.State = aState;
            if (aState == PrintPackageWrapper.Error) package.ErrorText = aErrorText;
            onUpdatePackageState(new PrintPackageStorageUpdatePackageStateEventArgs(aPackageId));
        }


        private void onUpdate(EventArgs e)
        {
            Update?.Invoke(this, e);
        }

        private void onUpdateSubset(EventArgs e)
        {
            UpdateSubset?.Invoke(this, e);
        }

        private void onUpdatePackageState(PrintPackageStorageUpdatePackageStateEventArgs e)
        {
            UpdatePackageState?.Invoke(this, e);
        }

        public void unlock(List<int> aIdList)
        {
            var response = _labelService.unlock(aIdList.ToArray());

            if (response.status != 0)
                throw new Exception(
                    $"Label service returns error status\nStatus: {response.status}\nMessage: {response.message}\nSubstatus: {response.substatus}\nSubmessage: {response.submessage}");
            foreach (var packageId in aIdList)
            {
                _packageIdIndex[packageId].State = PrintPackageWrapper.Unprinted;
                onUpdatePackageState(new PrintPackageStorageUpdatePackageStateEventArgs(packageId));
            }
        }
    }
}