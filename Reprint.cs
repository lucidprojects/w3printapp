using System;
using System.Collections.Generic;
using System.Text;

namespace PrintInvoice
{
  class Reprint : PackageStorage<PrintPackageWrapper>
  {
    private const int PACKAGE_ID_COLUMN_INDEX = 0;

    private LabelService client;
    private Config config;

    public event PrintPackageStorageUpdatePackageStateEventHandler UpdatePackageState;

    public Reprint(Config aConfig, LabelService aClient) 
      : base()
    {
      config = aConfig;
      client = aClient;
    }

    public void addSingle(string[] aTrackingNumberList)
    {
      // get package data from service
      GetPackageDataRequestType request = new GetPackageDataRequestType();
      request.singleDataQuery = config.ReprintSinglePackageDataQuery;
      request.singleTrackingNumberList = aTrackingNumberList;
      GetPackageDataResponseType response = client.getPackageData(request);
      if (response.status != 0)
      {
        throw new Exception(String.Format("Label service returns error status\nStatus: {0}\nMessage: {1}\nSubstatus: {2}\nSubmessage: {3}", response.status, response.message, response.substatus, response.submessage));
      }
      else
      {
        if (fieldMetadata == null)
        {
          fieldMetadata = response.meta;
        }

        if (response.packageDataList != null)
        {
          foreach (PackageDataListItemType item in response.packageDataList)
          {
            PrintPackageWrapper package = new PrintPackageWrapper(PackageWrapper.NULL_PACKAGE_ID);
            package.TrackingNumber = item.trackingNumber;
            package.FieldValueList = item.fieldValueList;
            switch (item.status)
            {
              case 0:
                package.PackageId = Int32.Parse(item.fieldValueList[PACKAGE_ID_COLUMN_INDEX]);
                break;

              case 1:
                package.State = PrintPackageWrapper.ERROR;
                package.ErrorText = "Package record not found";
                break;
            }
            add(package);
          }

          onUpdateList(new EventArgs());
        }
      }
    }

    public void addBatch(string[] aTrackingNumberList)
    {
      // get package data from service
      GetPackageDataRequestType request = new GetPackageDataRequestType();
      request.singleDataQuery = config.ReprintSinglePackageDataQuery; // need for checking if package record exists
      request.batchDataQuery = config.ReprintBatchPackageDataQuery;
      request.batchTrackingNumberList = aTrackingNumberList;
      GetPackageDataResponseType response = client.getPackageData(request);
      if (response.status != 0)
      {
        throw new Exception(String.Format("Label service returns error status\nStatus: {0}\nMessage: {1}\nSubstatus: {2}\nSubmessage: {3}", response.status, response.message, response.substatus, response.submessage));
      }
      else
      {
        if (fieldMetadata == null)
        {
          fieldMetadata = response.meta;
        }

        if (response.packageDataList != null)
        {
          foreach (PackageDataListItemType item in response.packageDataList)
          {
            PrintPackageWrapper package = new PrintPackageWrapper(PackageWrapper.NULL_PACKAGE_ID);
            package.TrackingNumber = item.trackingNumber;
            package.FieldValueList = item.fieldValueList;
            switch (item.status)
            {
              case 0:
                package.PackageId = Int32.Parse(item.fieldValueList[PACKAGE_ID_COLUMN_INDEX]);
                break;

              case 1:
                package.State = PrintPackageWrapper.ERROR;
                package.ErrorText = "Package record not found";
                break;
            }
            add(package);
          }

          onUpdateList(new EventArgs());
        }
      }
    }

    public int addBatchById(int aBatchId)
    {
      RunSqlQueryRequestType request = new RunSqlQueryRequestType();
      request.query = config.ReprintBatchPackageDataQuery;
      request.clientVersion = Routines.getVersion();
      request.parameters = new string[1];
      request.parameters[0] = aBatchId.ToString();

      RunSqlQueryResponseType response = client.runSqlQuery(request);

      if (response.status != 0)
      {
        throw new Exception(String.Format("Label service returns error status\nStatus: {0}\nMessage: {1}\nSubstatus: {2}\nSubmessage: {3}", response.status, response.message, response.substatus, response.submessage));
      }
      else
      {
        if (fieldMetadata == null)
        {
          fieldMetadata = new DatabaseFieldMetadataType[response.meta.Length - 1];
          Array.Copy(response.meta, 1, fieldMetadata, 0, fieldMetadata.Length);
        }

        if (response.rows != null)
        {
          foreach (RowType item in response.rows)
          {
            PrintPackageWrapper package = new PrintPackageWrapper(Int32.Parse(item.columns[1]));
            package.TrackingNumber = item.columns[0];
            package.FieldValueList = new string[item.columns.Length - 1];
            Array.Copy(item.columns, 1, package.FieldValueList, 0, package.FieldValueList.Length);
            add(package);
          }

          onUpdateList(new EventArgs());
        }
      }

      if (response.rows != null && response.rows.Length > 0)
      {
        return response.rows.Length;
      }
      else
      {
        return 0;
      }
    }

    public void setPackageState(int aPackageId, int aState, string aErrorText)
    {
      PrintPackageWrapper package = getPackageByPackageId(aPackageId);
      package.State = aState;
      if (aState == PrintPackageWrapper.ERROR)
      {
        package.ErrorText = aErrorText;
      }
      onUpdatePackageState(new PrintPackageStorageUpdatePackageStateEventArgs(aPackageId));
    }

    private void onUpdatePackageState(PrintPackageStorageUpdatePackageStateEventArgs e)
    {
      if (UpdatePackageState != null)
        UpdatePackageState(this, e);
    }

    new public void remove(List<PrintPackageWrapper> aList)
    {
      base.remove(aList);
      onUpdateList(new EventArgs());
    }

    new public void clear()
    {
      base.clear();
      onUpdateList(new EventArgs());
    }

    public void unlock(List<int> aIdList)
    {
      UnlockResponseType response = client.unlock(aIdList.ToArray());

      if (response.status != 0)
      {
        throw new Exception(String.Format("Label service returns error status\nStatus: {0}\nMessage: {1}\nSubstatus: {2}\nSubmessage: {3}", response.status, response.message, response.substatus, response.submessage));
      }
      else
      {
        foreach (int packageId in aIdList)
        {
          packageIdIndex[packageId].State = PrintPackageWrapper.UNPRINTED;
          onUpdatePackageState(new PrintPackageStorageUpdatePackageStateEventArgs(packageId));
        }
      }
    }


  }
}
