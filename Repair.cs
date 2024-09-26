using System;
using System.Collections.Generic;
using System.Security;

namespace PrintInvoice
{
    public class RepairPackageWrapper : PackageWrapper
    {
        public enum StateType
        {
            ORIGINAL,
            REPAIRED,
            ERROR
        }

        public RepairPackageWrapper(int aPackageId)
            : base(aPackageId)
        {
        }

        public StateType State { get; set; }
    }


    public class Repair : PackageStorage<RepairPackageWrapper>
    {
        public const int PackageIdColumnIndex = 0;
        
        private readonly LabelService _client;
        private readonly Config _config;

        public Repair(LabelService aClient, Config aConfig)
        {
            _client = aClient;
            _config = aConfig;
        }

        public bool IsLoaded { get; private set; }

        public void Load()
        {
            var request = new RunSqlQueryRequestType
            {
                query = SecurityElement.Escape(_config.RepairQuery),
                clientVersion = Routines.GetVersion()
            };
            
            var response = _client.runSqlQuery(request);
        
            if (response.status != 0)
                throw new Exception($"Label service returns error status\nStatus: {response.status}\nMessage: {response.message}\nSubstatus: {response.substatus}\nSubmessage: {response.submessage}");

            Clear();

            if (response.rows != null)
            {
                foreach (var row in response.rows)
                {
                    var packageId = int.Parse(row.columns[PackageIdColumnIndex]);
            
                    var package = new RepairPackageWrapper(packageId)
                    {
                        State = RepairPackageWrapper.StateType.ORIGINAL,
                        FieldValueList = row.columns
                    };

                    Add(package);
                }
            }

            if (_fieldMetadata == null) _fieldMetadata = response.meta;

            IsLoaded = true;

            OnUpdateList(EventArgs.Empty);
        }

        public void repair(List<int> packageIdList)
        {
            var response = _client.repair(packageIdList.ToArray());

            if (response.status != 0)
                throw new Exception($"Label service returns error status\nStatus: {response.status}\nMessage: {response.message}\nSubstatus: {response.substatus}\nSubmessage: {response.submessage}");

            if (response.itemList != null)
            {
                foreach (var resultItem in response.itemList)
                {
                    if (resultItem.isRepaired)
                    {
                        _packageIdIndex[resultItem.packageId].State = RepairPackageWrapper.StateType.REPAIRED;
                    }
                    else
                    {
                        _packageIdIndex[resultItem.packageId].State = RepairPackageWrapper.StateType.ERROR;
                        _packageIdIndex[resultItem.packageId].ErrorText = resultItem.errorMessage;
                    }
                }

                OnUpdateList(EventArgs.Empty);
            }
        }
    }
}