using System;
using System.Collections.Generic;
using System.Text;
using System.Security;

namespace PrintInvoice
{
  using UnshippedPackageList = List<UnshippedPackageWrapper>;

  public class UnshippedPackageWrapper : PackageWrapper
  {
    public const string STATUS_NEW = "NEW";
    public const string STATUS_UNSHIPPED = "UNSHIPPED";
    public const string STATUS_PICKABLE = "PICKABLE";

    public enum StateType {ORIGINAL, UPDATED_PICKABLE, UPDATED_ONHOLD, ERROR};

    public UnshippedPackageWrapper(int aPackageId)
      : base(aPackageId)
    {
    }

    // private datafields
    private StateType state;
    private string status;
    private string originalStatus;

    // public properties

    public StateType State
    {
      get { return state; }
      set { state = value; }
    }

    public string Status
    {
      get { return status; }
      set { status = value; }
    }

    public string OriginalStatus
    {
      set { originalStatus = value; }
      get { return originalStatus; }
    }

    public void resetStatus()
    {
      status = originalStatus;
      state = StateType.ORIGINAL;
    }
  }

  public class PackageStatus
  {
    public int packageId;
    public string status;
    public string requiredStatus;

    public PackageStatus(int aPackageId, string aStatus, string aRequiredStatus)
    {
      packageId = aPackageId;
      status = aStatus;
      requiredStatus = aRequiredStatus;
    }
  }


  // Update event delegate
  public delegate void UnshippedListUpdateEventHandler(object sender, EventArgs e);

  // UpdateMaxDailyPackages event delegate
  public delegate void UnshippedUpdateMaxDailyPackagesEventHandler(object sender, EventArgs e);

  public class Unshipped : PackageStorage<UnshippedPackageWrapper>
  {

    public const int PACKAGE_ID_COLUMN_INDEX = 0;
    public const int STATUS_COLUMN_INDEX = 1;

    // public events
    public event UnshippedUpdateMaxDailyPackagesEventHandler UpdateMaxDailyPackages;

    // private datafields
    private LabelService client;
    private Config config;

    private bool isLoaded = false;

    private int maxDailyPackages = 0;
    private int pmodMaxDailyPackages = 0;

    public Unshipped(LabelService aClient, Config aConfig) : base()
    {
      client = aClient;
      config = aConfig;
    }

    // public properties

    public bool IsLoaded
    {
      get { return isLoaded; }
    }

    public int MaxDailyPackages
    {
      get { return maxDailyPackages; }
    }

    public int PmodMaxDailyPackages
    {
      get { return pmodMaxDailyPackages; }
    }

    // private members

    // private methods

    private void onUpdateMaxDailyPackages(EventArgs e)
    {
      if (UpdateMaxDailyPackages != null)
        UpdateMaxDailyPackages(this, e);
    }

    public void load()
    {
      RunSqlQueryRequestType request = new RunSqlQueryRequestType();
      request.query = SecurityElement.Escape(config.UnshippedQuery);
      request.clientVersion = Routines.getVersion();
      RunSqlQueryResponseType response = client.runSqlQuery(request);
      if (response.status != 0)
      {
        throw new Exception(String.Format("Label service returns error status\nStatus: {0}\nMessage: {1}\nSubstatus: {2}\nSubmessage: {3}", response.status, response.message, response.substatus, response.submessage));
      }

      clear();

      if (response.rows != null)
      {
        foreach (RowType row in response.rows)
        {
          int packageId = Int32.Parse(row.columns[PACKAGE_ID_COLUMN_INDEX]);
          UnshippedPackageWrapper package = new UnshippedPackageWrapper(packageId);
          package.State = UnshippedPackageWrapper.StateType.ORIGINAL;
          package.OriginalStatus = row.columns[STATUS_COLUMN_INDEX];
          package.Status = row.columns[STATUS_COLUMN_INDEX];
          package.FieldValueList = row.columns;

          add(package);
        }
      }

      if (fieldMetadata == null)
      {
        fieldMetadata = response.meta;
      }

      isLoaded = true;

      onUpdateList(new EventArgs());

      // load max daily packages
      GetMaxDailyPackagesRequestType maxDailyPackagesRequest = new GetMaxDailyPackagesRequestType();
      maxDailyPackagesRequest.groupId = "PKG_MAX_DAILY_PACKAGES";
      GetMaxDailyPackagesResponseType maxDailyPackagesResponse = client.getMaxDailyPackages(maxDailyPackagesRequest);
      if (maxDailyPackagesResponse.status != 0)
      {
        throw new Exception(String.Format("Label service returns error status\nStatus: {0}\nMessage: {1}\nSubstatus: {2}\nSubmessage: {3}", response.status, response.message, response.substatus, response.submessage));
      }
      maxDailyPackages = maxDailyPackagesResponse.maxDailyPackages;

      // load PMOD max daily packages
      maxDailyPackagesRequest.groupId = "PKG_PMOD_MAX_DAILY_PACKAGES";
      maxDailyPackagesResponse = client.getMaxDailyPackages(maxDailyPackagesRequest);
      if (maxDailyPackagesResponse.status != 0)
      {
        throw new Exception(String.Format("Label service returns error status\nStatus: {0}\nMessage: {1}\nSubstatus: {2}\nSubmessage: {3}", response.status, response.message, response.substatus, response.submessage));
      }
      pmodMaxDailyPackages = maxDailyPackagesResponse.maxDailyPackages;

      onUpdateMaxDailyPackages(new EventArgs());
    }

    public void setPickable(List<PackageStatus> aPackageList, out int aFailed)
    {
      SetPackageStatusRequestType request = new SetPackageStatusRequestType();
      request.itemList = new SetPackageStatusRequestItemType[aPackageList.Count];
      for (int i = 0; i < aPackageList.Count; i++)
      {
        request.itemList[i] = new SetPackageStatusRequestItemType();
        request.itemList[i].packageId = aPackageList[i].packageId;
        request.itemList[i].status = aPackageList[i].status;
        request.itemList[i].requiredStatus = aPackageList[i].requiredStatus;
      }

      SetPackageStatusResponseType response = client.setPackageStatus(request);

      if (response.status != 0)
      {
        throw new Exception(String.Format("Label service returns error status\nStatus: {0}\nMessage: {1}\nSubstatus: {2}\nSubmessage: {3}", response.status, response.message, response.substatus, response.submessage));
      }

      aFailed = 0;
      foreach (SetPackageStatusResponseItemtype responseItem in response.resultList) {
        if (responseItem.isSuccess)
        {
          ((UnshippedPackageWrapper)(packageIdIndex[responseItem.packageId])).State = UnshippedPackageWrapper.StateType.UPDATED_PICKABLE;
        }
        else {
          ((UnshippedPackageWrapper)(packageIdIndex[responseItem.packageId])).State = UnshippedPackageWrapper.StateType.ERROR;
          ((UnshippedPackageWrapper)(packageIdIndex[responseItem.packageId])).ErrorText = responseItem.errorMessage;
          aFailed++;
        }
      }

      onUpdateList(new EventArgs());
    }

    public void resetStatus(List<UnshippedPackageWrapper> aPackageList)
    {
      SetPackageStatusRequestType request = new SetPackageStatusRequestType();
      request.itemList = new SetPackageStatusRequestItemType[aPackageList.Count];
      for (int i = 0; i < aPackageList.Count; i++)
      {
        request.itemList[i] = new SetPackageStatusRequestItemType();
        request.itemList[i].packageId = aPackageList[i].PackageId;
        request.itemList[i].status = aPackageList[i].OriginalStatus;
      }

      SetPackageStatusResponseType response = client.setPackageStatus(request);

      if (response.status != 0)
      {
        throw new Exception(String.Format("Label service returns error status\nStatus: {0}\nMessage: {1}\nSubstatus: {2}\nSubmessage: {3}", response.status, response.message, response.substatus, response.submessage));
      }

      foreach (UnshippedPackageWrapper package in aPackageList)
      {
        ((UnshippedPackageWrapper)(packageIdIndex[package.PackageId])).resetStatus();
      }

      onUpdateList(new EventArgs());
    }

    public void setMaxDailyPackages(int aValue)
    {
      SetMaxDailyPackagesRequestType request = new SetMaxDailyPackagesRequestType();
      request.value = aValue;
      request.groupId = "PKG_MAX_DAILY_PACKAGES";

      SetMaxDailyPackagesResponseType response = client.setMaxDailyPackages(request);

      if (response.status != 0)
      {
        throw new Exception(String.Format("Label service returns error status\nStatus: {0}\nMessage: {1}\nSubstatus: {2}\nSubmessage: {3}", response.status, response.message, response.substatus, response.submessage));
      }

      maxDailyPackages = aValue;
      onUpdateMaxDailyPackages(new EventArgs());
    }


    public void setPmodMaxDailyPackages(int aValue)
    {
      SetMaxDailyPackagesRequestType request = new SetMaxDailyPackagesRequestType();
      request.value = aValue;
      request.groupId = "PKG_PMOD_MAX_DAILY_PACKAGES";

      SetMaxDailyPackagesResponseType response = client.setMaxDailyPackages(request);

      if (response.status != 0)
      {
        throw new Exception(String.Format("Label service returns error status\nStatus: {0}\nMessage: {1}\nSubstatus: {2}\nSubmessage: {3}", response.status, response.message, response.substatus, response.submessage));
      }

      pmodMaxDailyPackages = aValue;
      onUpdateMaxDailyPackages(new EventArgs());
    }

    public void onHold(List<int> aPackageIdList)
    {
      OnHoldRequestType request = new OnHoldRequestType();
      request.packageId = aPackageIdList.ToArray();
      request.printStationId = Properties.Settings.Default.PrintStationId;
        
      ResponseBaseType response = client.onHold(request);

      if (response.status != 0)
      {
        throw new Exception(String.Format("Label service returns error status\nStatus: {0}\nMessage: {1}\nSubstatus: {2}\nSubmessage: {3}", response.status, response.message, response.substatus, response.submessage));
      }

      foreach (int packageId in aPackageIdList)
      {
        ((UnshippedPackageWrapper)(packageIdIndex[packageId])).State = UnshippedPackageWrapper.StateType.UPDATED_ONHOLD;
      }

      onUpdateList(new EventArgs());
    }
  }
}
