using System;
using System.Collections.Generic;

namespace PrintInvoice
{
    public class UnshipPackageWrapper : PackageWrapper
    {
        public enum PackageStateType
        {
            SHIPPED,
            UNSHIPPED,
            ERROR
        }

        public UnshipPackageWrapper(int aPackageId) : base(aPackageId)
        {
        }

        public PackageStateType State { get; set; }
    }

    public class Unship : PackageStorage<UnshipPackageWrapper>
    {
        public const int PackageIdColumnIndex = 0;

        private readonly LabelService _client;
        private readonly Config _config;

        public Unship(LabelService aClient, Config aConfig)
        {
            _client = aClient;
            _config = aConfig;
        }

        public void AddSingle(string[] aTrackingNumberList)
        {
            // get package data from service
            var request = new GetPackageDataRequestType
            {
                singleDataQuery = _config.UnshipSinglePackageDataQuery,
                singleTrackingNumberList = aTrackingNumberList
            };

            var response = _client.getPackageData(request);
            
            if (response.status != 0)
                throw new Exception($"Label service returns error status\nStatus: {response.status}\nMessage: {response.message}\nSubstatus: {response.substatus}\nSubmessage: {response.submessage}");

            if (_fieldMetadata == null) _fieldMetadata = response.meta;

            if (response.packageDataList != null)
            {
                foreach (var item in response.packageDataList)
                {
                    var package = new UnshipPackageWrapper(PackageWrapper.NullPackageId)
                    {
                        TrackingNumber = item.trackingNumber
                    };

                    switch (item.status)
                    {
                        case 0:
                            package.PackageId = int.Parse(item.fieldValueList[PackageIdColumnIndex]);
                            package.State = UnshipPackageWrapper.PackageStateType.SHIPPED;
                            package.FieldValueList = item.fieldValueList;
                            break;

                        case 1:
                            package.State = UnshipPackageWrapper.PackageStateType.ERROR;
                            package.ErrorText = "Package record not found";
                            package.FieldValueList = item.fieldValueList;
                            break;
                    }

                    Add(package);
                }

                OnUpdateList(EventArgs.Empty);
            }
        }

        public void AddBatch(string[] aTrackingNumberList)
        {
            // get package data from service
            var request = new GetPackageDataRequestType
            {
                singleDataQuery = _config.UnshipSinglePackageDataQuery, // need for checking if package record exists
                batchDataQuery = _config.UnshipBatchPackageDataQuery,
                batchTrackingNumberList = aTrackingNumberList
            };

            var response = _client.getPackageData(request);
            
            if (response.status != 0)
                throw new Exception($"Label service returns error status\nStatus: {response.status}\nMessage: {response.message}\nSubstatus: {response.substatus}\nSubmessage: {response.submessage}");

            if (_fieldMetadata == null) _fieldMetadata = response.meta;

            if (response.packageDataList != null)
            {
                foreach (var item in response.packageDataList)
                {
                    var package = new UnshipPackageWrapper(PackageWrapper.NullPackageId)
                    {
                        TrackingNumber = item.trackingNumber
                    };

                    switch (item.status)
                    {
                        case 0:
                            package.PackageId = int.Parse(item.fieldValueList[PackageIdColumnIndex]);
                            package.State = UnshipPackageWrapper.PackageStateType.SHIPPED;
                            package.FieldValueList = item.fieldValueList;
                            break;

                        case 1:
                            package.State = UnshipPackageWrapper.PackageStateType.ERROR;
                            package.ErrorText = "Package record not found";
                            package.FieldValueList = item.fieldValueList;
                            break;
                    }

                    Add(package);
                }

                OnUpdateList(EventArgs.Empty);
            }
        }

        public void unship(out int aSuccess, out int aFail)
        {
            aSuccess = 0;
            aFail = 0;

            int success;
            int fail;

            var list = new List<UnshipRequestItemType>();

            foreach (var package in _packageList)
            {
                if (package.State != UnshipPackageWrapper.PackageStateType.ERROR && package.State != UnshipPackageWrapper.PackageStateType.UNSHIPPED)
                {
                    var item = new UnshipRequestItemType
                    {
                        trackingNumber = package.TrackingNumber,
                        packageId = package.PackageId
                    };

                    list.Add(item);
                }

                if (list.Count == 100)
                {
                    UnshipRange(list, out success, out fail);
                    aSuccess += success;
                    aFail += fail;
                    list.Clear();
                }
            }

            if (list.Count > 0)
            {
                UnshipRange(list, out success, out fail);
                aSuccess += success;
                aFail += fail;
            }

            OnUpdateList(EventArgs.Empty);
        }

        private void UnshipRange(List<UnshipRequestItemType> aList, out int aSuccess, out int aFail)
        {
            aSuccess = 0;
            aFail = 0;

            var request = new UnshipRequestType
            {
                itemList = aList.ToArray()
            };

            var response = _client.unship(request);

            if (response.status != 0)
                throw new Exception($"Label service returns error status\nStatus: {response.status}\nMessage: {response.message}\nSubstatus: {response.substatus}\nSubmessage: {response.submessage}");
            
            foreach (var resultItem in response.resultList)
            {
                if (resultItem.isApproved)
                {
                    _packageIdIndex[resultItem.packageId].State = UnshipPackageWrapper.PackageStateType.UNSHIPPED;
                    aSuccess++;
                }
                else
                {
                    _packageIdIndex[resultItem.packageId].State = UnshipPackageWrapper.PackageStateType.ERROR;
                    _packageIdIndex[resultItem.packageId].ErrorText = resultItem.errorMessage;
                    aFail++;
                }
            }
        }

        public new void remove(List<UnshipPackageWrapper> aList)
        {
            base.Remove(aList);
            OnUpdateList(EventArgs.Empty);
        }

        public new void clear()
        {
            base.Clear();
            OnUpdateList(EventArgs.Empty);
        }

        public int AddBatchById(int aBatchId)
        {
            var request = new RunSqlQueryRequestType
            {
                query = _config.UnshipBatchPackageDataQuery,
                clientVersion = Routines.GetVersion(),
                parameters = new string[1]
            };
            
            request.parameters[0] = aBatchId.ToString();

            var response = _client.runSqlQuery(request);

            if (response.status != 0)
                throw new Exception($"Label service returns error status\nStatus: {response.status}\nMessage: {response.message}\nSubstatus: {response.substatus}\nSubmessage: {response.submessage}");

            if (_fieldMetadata == null)
            {
                _fieldMetadata = new DatabaseFieldMetadataType[response.meta.Length - 1];
                Array.Copy(response.meta, 1, _fieldMetadata, 0, _fieldMetadata.Length);
            }

            if (response.rows != null)
            {
                foreach (var item in response.rows)
                {
                    var package = new UnshipPackageWrapper(int.Parse(item.columns[1]))
                    {
                        TrackingNumber = item.columns[0],
                        FieldValueList = new string[item.columns.Length - 1]
                    };

                    Array.Copy(item.columns, 1, package.FieldValueList, 0, package.FieldValueList.Length);
                    Add(package);
                }

                OnUpdateList(EventArgs.Empty);
            }

            return response.rows != null && response.rows.Length > 0 ? response.rows.Length : 0;
        }
    }
}