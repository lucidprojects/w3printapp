using System;
using System.Collections.Generic;
using System.Security;
using PrintInvoice.Properties;

namespace PrintInvoice
{
    public class UnshippedPackageWrapper : PackageWrapper
    {
        public enum StateType
        {
            ORIGINAL,
            UPDATED_PICKABLE,
            UPDATED_ONHOLD,
            ERROR
        }

        public const string StatusNew = "NEW";
        public const string StatusUnshipped = "UNSHIPPED";
        public const string StatusPickable = "PICKABLE";

        // private datafields

        public UnshippedPackageWrapper(int aPackageId)
            : base(aPackageId)
        {
        }

        // public properties

        public StateType State { get; set; }

        public string Status { get; set; }

        public string OriginalStatus { set; get; }

        public void resetStatus()
        {
            Status = OriginalStatus;
            State = StateType.ORIGINAL;
        }
    }

    public class PackageStatus
    {
        public int _packageId;
        public string _requiredStatus;
        public string _status;

        public PackageStatus(int aPackageId, string aStatus, string aRequiredStatus)
        {
            _packageId = aPackageId;
            _status = aStatus;
            _requiredStatus = aRequiredStatus;
        }
    }


    // Update event delegate
    public delegate void UnshippedListUpdateEventHandler(object sender, EventArgs e);

    // UpdateMaxDailyPackages event delegate
    public delegate void UnshippedUpdateMaxDailyPackagesEventHandler(object sender, EventArgs e);

    public class Unshipped : PackageStorage<UnshippedPackageWrapper>
    {
        public const int PackageIdColumnIndex = 0;
        public const int StatusColumnIndex = 1;

        // private datafields
        private readonly LabelService _client;
        private readonly Config _config;

        public Unshipped(LabelService aClient, Config aConfig)
        {
            _client = aClient;
            _config = aConfig;
        }

        // public properties

        public bool IsLoaded { get; private set; }

        public int MaxDailyPackages { get; private set; }

        public int PmodMaxDailyPackages { get; private set; }

        // public events
        public event UnshippedUpdateMaxDailyPackagesEventHandler UpdateMaxDailyPackages;

        // private members

        // private methods

        private void onUpdateMaxDailyPackages(EventArgs e)
        {
            UpdateMaxDailyPackages?.Invoke(this, e);
        }

        public void load()
        {
            var request = new RunSqlQueryRequestType
            {
                query = SecurityElement.Escape(_config.UnshippedQuery),
                clientVersion = Routines.getVersion()
            };
            
            var response = _client.runSqlQuery(request);
            
            if (response.status != 0)
                throw new Exception($"Label service returns error status\nStatus: {response.status}\nMessage: {response.message}\nSubstatus: {response.substatus}\nSubmessage: {response.submessage}");

            clear();

            if (response.rows != null)
                foreach (var row in response.rows)
                {
                    var packageId = int.Parse(row.columns[PackageIdColumnIndex]);
                    var package = new UnshippedPackageWrapper(packageId)
                    {
                        State = UnshippedPackageWrapper.StateType.ORIGINAL,
                        OriginalStatus = row.columns[StatusColumnIndex],
                        Status = row.columns[StatusColumnIndex],
                        FieldValueList = row.columns
                    };

                    add(package);
                }

            if (_fieldMetadata == null) _fieldMetadata = response.meta;

            IsLoaded = true;

            onUpdateList(EventArgs.Empty);

            // load max daily packages
            var maxDailyPackagesRequest = new GetMaxDailyPackagesRequestType
            {
                groupId = "PKG_MAX_DAILY_PACKAGES"
            };
            var maxDailyPackagesResponse = _client.getMaxDailyPackages(maxDailyPackagesRequest);
            
            if (maxDailyPackagesResponse.status != 0)
                throw new Exception($"Label service returns error status\nStatus: {response.status}\nMessage: {response.message}\nSubstatus: {response.substatus}\nSubmessage: {response.submessage}");
            
            MaxDailyPackages = maxDailyPackagesResponse.maxDailyPackages;

            // load PMOD max daily packages
            maxDailyPackagesRequest.groupId = "PKG_PMOD_MAX_DAILY_PACKAGES";
            maxDailyPackagesResponse = _client.getMaxDailyPackages(maxDailyPackagesRequest);
            
            if (maxDailyPackagesResponse.status != 0)
                throw new Exception($"Label service returns error status\nStatus: {response.status}\nMessage: {response.message}\nSubstatus: {response.substatus}\nSubmessage: {response.submessage}");
            
            PmodMaxDailyPackages = maxDailyPackagesResponse.maxDailyPackages;

            onUpdateMaxDailyPackages(EventArgs.Empty);
        }

        public void setPickable(List<PackageStatus> aPackageList, out int aFailed)
        {
            var request = new SetPackageStatusRequestType
            {
                itemList = new SetPackageStatusRequestItemType[aPackageList.Count]
            };
            
            for (var i = 0; i < aPackageList.Count; i++)
            {
                request.itemList[i] = new SetPackageStatusRequestItemType
                {
                    packageId = aPackageList[i]._packageId,
                    status = aPackageList[i]._status,
                    requiredStatus = aPackageList[i]._requiredStatus
                };
            }

            var response = _client.setPackageStatus(request);

            if (response.status != 0)
                throw new Exception($"Label service returns error status\nStatus: {response.status}\nMessage: {response.message}\nSubstatus: {response.substatus}\nSubmessage: {response.submessage}");

            aFailed = 0;

            foreach (var responseItem in response.resultList)
                if (responseItem.isSuccess)
                {
                    _packageIdIndex[responseItem.packageId].State = UnshippedPackageWrapper.StateType.UPDATED_PICKABLE;
                }
                else
                {
                    _packageIdIndex[responseItem.packageId].State = UnshippedPackageWrapper.StateType.ERROR;
                    _packageIdIndex[responseItem.packageId].ErrorText = responseItem.errorMessage;
                    aFailed++;
                }

            onUpdateList(EventArgs.Empty);
        }

        public void resetStatus(List<UnshippedPackageWrapper> aPackageList)
        {
            var request = new SetPackageStatusRequestType
            {
                itemList = new SetPackageStatusRequestItemType[aPackageList.Count]
            };
            
            for (var i = 0; i < aPackageList.Count; i++)
            {
                request.itemList[i] = new SetPackageStatusRequestItemType
                {
                    packageId = aPackageList[i].PackageId,
                    status = aPackageList[i].OriginalStatus
                };
            }

            var response = _client.setPackageStatus(request);

            if (response.status != 0)
                throw new Exception($"Label service returns error status\nStatus: {response.status}\nMessage: {response.message}\nSubstatus: {response.substatus}\nSubmessage: {response.submessage}");

            foreach (var package in aPackageList) _packageIdIndex[package.PackageId].resetStatus();

            onUpdateList(EventArgs.Empty);
        }

        public void setMaxDailyPackages(int aValue)
        {
            var request = new SetMaxDailyPackagesRequestType
            {
                value = aValue,
                groupId = "PKG_MAX_DAILY_PACKAGES"
            };

            var response = _client.setMaxDailyPackages(request);

            if (response.status != 0)
                throw new Exception($"Label service returns error status\nStatus: {response.status}\nMessage: {response.message}\nSubstatus: {response.substatus}\nSubmessage: {response.submessage}");

            MaxDailyPackages = aValue;
            onUpdateMaxDailyPackages(EventArgs.Empty);
        }


        public void setPmodMaxDailyPackages(int aValue)
        {
            var request = new SetMaxDailyPackagesRequestType
            {
                value = aValue,
                groupId = "PKG_PMOD_MAX_DAILY_PACKAGES"
            };

            var response = _client.setMaxDailyPackages(request);

            if (response.status != 0)
                throw new Exception($"Label service returns error status\nStatus: {response.status}\nMessage: {response.message}\nSubstatus: {response.substatus}\nSubmessage: {response.submessage}");

            PmodMaxDailyPackages = aValue;
            onUpdateMaxDailyPackages(EventArgs.Empty);
        }

        public void onHold(List<int> aPackageIdList)
        {
            var request = new OnHoldRequestType
            {
                packageId = aPackageIdList.ToArray(),
                printStationId = Settings.Default.PrintStationId
            };

            var response = _client.onHold(request);

            if (response.status != 0)
                throw new Exception($"Label service returns error status\nStatus: {response.status}\nMessage: {response.message}\nSubstatus: {response.substatus}\nSubmessage: {response.submessage}");

            foreach (var packageId in aPackageIdList)
                _packageIdIndex[packageId].State = UnshippedPackageWrapper.StateType.UPDATED_ONHOLD;

            onUpdateList(EventArgs.Empty);
        }
    }
}