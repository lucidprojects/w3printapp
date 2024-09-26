using System;
using System.Collections.Generic;
using System.Text;

namespace PrintInvoice
{
  using UnshipPackageList = List<UnshipPackageWrapper>;

  public class UnshipPackageWrapper : PackageWrapper
  {
    public enum PackageStateType { SHIPPED, UNSHIPPED, ERROR };

    // private datafields
    private PackageStateType state;

    public UnshipPackageWrapper(int aPackageId) : base(aPackageId)
    {
    }

    // public properties
    public PackageStateType State
    {
      get { return state; }
      set { state = value; }
    }
  }

  public class Unship : PackageStorage<UnshipPackageWrapper>
  {
    public const int PACKAGE_ID_COLUMN_INDEX = 0;

    // private datafields
    private LabelService client;
    private Config config;

    public Unship(LabelService aClient, Config aConfig) : base()
    {
      client = aClient;
      config = aConfig;
    }

    public void addSingle(string[] aTrackingNumberList)
    {
      // get package data from service
      GetPackageDataRequestType request = new GetPackageDataRequestType();
      request.singleDataQuery = config.UnshipSinglePackageDataQuery;
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
            UnshipPackageWrapper package = new UnshipPackageWrapper(PackageWrapper.NULL_PACKAGE_ID);
            package.TrackingNumber = item.trackingNumber;
            switch (item.status)
            { 
              case 0:
                package.PackageId = Int32.Parse(item.fieldValueList[PACKAGE_ID_COLUMN_INDEX]);
                package.State = UnshipPackageWrapper.PackageStateType.SHIPPED;
                package.FieldValueList = item.fieldValueList;
                break;

              case 1:
                package.State = UnshipPackageWrapper.PackageStateType.ERROR;
                package.ErrorText = "Package record not found";
                package.FieldValueList = item.fieldValueList;
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
      request.singleDataQuery = config.UnshipSinglePackageDataQuery; // need for checking if package record exists
      request.batchDataQuery = config.UnshipBatchPackageDataQuery;
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
            UnshipPackageWrapper package = new UnshipPackageWrapper(PackageWrapper.NULL_PACKAGE_ID);
            package.TrackingNumber = item.trackingNumber;
            switch (item.status)
            {
              case 0:
                package.PackageId = Int32.Parse(item.fieldValueList[PACKAGE_ID_COLUMN_INDEX]);
                package.State = UnshipPackageWrapper.PackageStateType.SHIPPED;
                package.FieldValueList = item.fieldValueList;
                break;

              case 1:
                package.State = UnshipPackageWrapper.PackageStateType.ERROR;
                package.ErrorText = "Package record not found";
                package.FieldValueList = item.fieldValueList;
                break;
            }
            add(package);
          }

          onUpdateList(new EventArgs());
        }
      }
    }

    public void unship(out int aSuccess, out int aFail)
    {
      aSuccess = 0;
      aFail = 0;

      int success;
      int fail;

      List<UnshipRequestItemType> list = new List<UnshipRequestItemType>();
      foreach (UnshipPackageWrapper package in packageList)
      {
        if (package.State != UnshipPackageWrapper.PackageStateType.ERROR && package.State != UnshipPackageWrapper.PackageStateType.UNSHIPPED)
        {
          UnshipRequestItemType item = new UnshipRequestItemType();
          item.trackingNumber = package.TrackingNumber;
          item.packageId = package.PackageId;
          list.Add(item);
        }

        if (list.Count == 100)
        {
          unshipRange(list, out success, out fail);
          aSuccess += success;
          aFail += fail;
          list.Clear();
        }
      }

      if (list.Count > 0)
      {
        unshipRange(list, out success, out fail);
        aSuccess += success;
        aFail += fail;
      }

      onUpdateList(new EventArgs());
    }

    private void unshipRange(List<UnshipRequestItemType> aList, out int aSuccess, out int aFail)
    {
      aSuccess = 0;
      aFail = 0;

      UnshipRequestType request = new UnshipRequestType();

      request.itemList = aList.ToArray();

      UnshipResponseType response = client.unship(request);

      if (response.status != 0)
      {
        throw new Exception(String.Format("Label service returns error status\nStatus: {0}\nMessage: {1}\nSubstatus: {2}\nSubmessage: {3}", response.status, response.message, response.substatus, response.submessage));
      }
      else
      {
        foreach (UnshipResultListItem resultItem in response.resultList)
        {
          if (resultItem.isApproved)
          {
            packageIdIndex[resultItem.packageId].State = UnshipPackageWrapper.PackageStateType.UNSHIPPED;
            aSuccess++;
          }
          else
          {
            packageIdIndex[resultItem.packageId].State = UnshipPackageWrapper.PackageStateType.ERROR;
            packageIdIndex[resultItem.packageId].ErrorText = resultItem.errorMessage;
            aFail++;
          }
        }

      }
    }

    new public void remove(List<UnshipPackageWrapper> aList)
    {
      base.remove(aList);
      onUpdateList(new EventArgs());
    }

    new public void clear()
    {
      base.clear();
      onUpdateList(new EventArgs());
    }

    public int addBatchById(int aBatchId)
    {
      RunSqlQueryRequestType request = new RunSqlQueryRequestType();
      request.query = config.UnshipBatchPackageDataQuery;
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
            UnshipPackageWrapper package = new UnshipPackageWrapper(Int32.Parse(item.columns[1]));
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
  }
}
