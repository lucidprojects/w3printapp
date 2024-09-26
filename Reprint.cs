using System;
using System.Collections.Generic;

namespace PrintInvoice
{
    internal class Reprint : PackageStorage<PrintPackageWrapper>
    {
        private const int PACKAGE_ID_COLUMN_INDEX = 0;

        private readonly LabelService _client;
        private readonly Config _config;

        public Reprint(Config aConfig, LabelService aClient)
        {
            _config = aConfig;
            _client = aClient;
        }

        public event PrintPackageStorageUpdatePackageStateEventHandler UpdatePackageState;

        public void addSingle(string[] aTrackingNumberList)
        {
            // get package data from service
            var request = new GetPackageDataRequestType
            {
                singleDataQuery = _config.ReprintSinglePackageDataQuery,
                singleTrackingNumberList = aTrackingNumberList
            };
            
            var response = _client.getPackageData(request);
            
            if (response.status != 0)
                throw new Exception(
                    $"Label service returns error status\nStatus: {response.status}\nMessage: {response.message}\nSubstatus: {response.substatus}\nSubmessage: {response.submessage}");

            if (_fieldMetadata == null) _fieldMetadata = response.meta;

            if (response.packageDataList != null)
            {
                foreach (var item in response.packageDataList)
                {
                    var package = new PrintPackageWrapper(PackageWrapper.NullPackageId)
                    {
                        TrackingNumber = item.trackingNumber,
                        FieldValueList = item.fieldValueList
                    };
                    switch (item.status)
                    {
                        case 0:
                            package.PackageId = int.Parse(item.fieldValueList[PACKAGE_ID_COLUMN_INDEX]);
                            break;

                        case 1:
                            package.State = PrintPackageWrapper.Error;
                            package.ErrorText = "Package record not found";
                            break;
                    }

                    add(package);
                }

                onUpdateList(EventArgs.Empty);
            }
        }

        public void addBatch(string[] aTrackingNumberList)
        {
            // get package data from service
            var request = new GetPackageDataRequestType
            {
                singleDataQuery = _config.ReprintSinglePackageDataQuery, // need for checking if package record exists
                batchDataQuery = _config.ReprintBatchPackageDataQuery,
                batchTrackingNumberList = aTrackingNumberList
            };
            
            var response = _client.getPackageData(request);
            
            if (response.status != 0)
                throw new Exception(
                    $"Label service returns error status\nStatus: {response.status}\nMessage: {response.message}\nSubstatus: {response.substatus}\nSubmessage: {response.submessage}");

            if (_fieldMetadata == null) _fieldMetadata = response.meta;

            if (response.packageDataList != null)
            {
                foreach (var item in response.packageDataList)
                {
                    var package = new PrintPackageWrapper(PackageWrapper.NullPackageId)
                    {
                        TrackingNumber = item.trackingNumber,
                        FieldValueList = item.fieldValueList
                    };
                    switch (item.status)
                    {
                        case 0:
                            package.PackageId = int.Parse(item.fieldValueList[PACKAGE_ID_COLUMN_INDEX]);
                            break;

                        case 1:
                            package.State = PrintPackageWrapper.Error;
                            package.ErrorText = "Package record not found";
                            break;
                    }

                    add(package);
                }

                onUpdateList(EventArgs.Empty);
            }
        }

        public int addBatchById(int aBatchId)
        {
            var request = new RunSqlQueryRequestType
            {
                query = _config.ReprintBatchPackageDataQuery,
                clientVersion = Routines.getVersion(),
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
                    var package = new PrintPackageWrapper(int.Parse(item.columns[1]))
                    {
                        TrackingNumber = item.columns[0],
                        FieldValueList = new string[item.columns.Length - 1]
                    };
                    Array.Copy(item.columns, 1, package.FieldValueList, 0, package.FieldValueList.Length);
                    add(package);
                }

                onUpdateList(EventArgs.Empty);
            }

            return response.rows != null && response.rows.Length > 0 ? response.rows.Length : 0;
        }

        public void setPackageState(int aPackageId, int aState, string aErrorText)
        {
            var package = getPackageByPackageId(aPackageId);
            package.State = aState;
            if (aState == PrintPackageWrapper.Error) package.ErrorText = aErrorText;
            onUpdatePackageState(new PrintPackageStorageUpdatePackageStateEventArgs(aPackageId));
        }

        private void onUpdatePackageState(PrintPackageStorageUpdatePackageStateEventArgs e)
        {
            UpdatePackageState?.Invoke(this, e);
        }

        public new void remove(List<PrintPackageWrapper> aList)
        {
            base.remove(aList);
            onUpdateList(EventArgs.Empty);
        }

        public new void clear()
        {
            base.clear();
            onUpdateList(EventArgs.Empty);
        }

        public void unlock(List<int> aIdList)
        {
            var response = _client.unlock(aIdList.ToArray());

            if (response.status != 0)
                throw new Exception($"Label service returns error status\nStatus: {response.status}\nMessage: {response.message}\nSubstatus: {response.substatus}\nSubmessage: {response.submessage}");

            foreach (var packageId in aIdList)
            {
                _packageIdIndex[packageId].State = PrintPackageWrapper.Unprinted;
                onUpdatePackageState(new PrintPackageStorageUpdatePackageStateEventArgs(packageId));
            }
        }
    }
}