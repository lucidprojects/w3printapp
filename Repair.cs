using System;
using System.Collections.Generic;
using System.Text;
using System.Security;

namespace PrintInvoice
{
  using RepairPackageList = List<RepairPackageWrapper>;

  public class RepairPackageWrapper : PackageWrapper
  {
    //public const string STATUS_NEW = "NEW";
    //public const string STATUS_UNSHIPPED = "UNSHIPPED";
    //public const string STATUS_PICKABLE = "PICKABLE";

    public enum StateType { ORIGINAL, REPAIRED, ERROR };

    public RepairPackageWrapper(int aPackageId)
      : base(aPackageId)
    {
    }

    // private datafields
    private StateType state;

    // public properties

    public StateType State
    {
      get { return state; }
      set { state = value; }
    }
  }


  // Update event delegate
  public delegate void RepairListUpdateEventHandler(object sender, EventArgs e);

  public class Repair : PackageStorage<RepairPackageWrapper>
  {
    public const int PACKAGE_ID_COLUMN_INDEX = 0;

    // private datafields
    private LabelService client;
    private Config config;

    private bool isLoaded = false;

    public Repair(LabelService aClient, Config aConfig)
      : base()
    {
      client = aClient;
      config = aConfig;
    }

    // public properties
    public bool IsLoaded
    {
      get { return isLoaded; }
    }

    // private members

    // private methods

    public void load()
    {
      RunSqlQueryRequestType request = new RunSqlQueryRequestType();
      request.query = SecurityElement.Escape(config.RepairQuery);
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
          RepairPackageWrapper package = new RepairPackageWrapper(packageId);
          package.State = RepairPackageWrapper.StateType.ORIGINAL;
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
    }

    public void repair(List<int> packageIdList)
    {

      RepairResponseType response = client.repair(packageIdList.ToArray());

      if (response.status != 0)
      {
        throw new Exception(String.Format("Label service returns error status\nStatus: {0}\nMessage: {1}\nSubstatus: {2}\nSubmessage: {3}", response.status, response.message, response.substatus, response.submessage));
      }
      else
      {
        if (response.itemList != null)
        {
          foreach (RepairResponseItemType resultItem in response.itemList)
          {
            if (resultItem.isRepaired)
            {
              packageIdIndex[resultItem.packageId].State = RepairPackageWrapper.StateType.REPAIRED;
            }
            else
            {
              packageIdIndex[resultItem.packageId].State = RepairPackageWrapper.StateType.ERROR;
              packageIdIndex[resultItem.packageId].ErrorText = resultItem.errorMessage;
            }
          }

          onUpdateList(new EventArgs());
        }
      }
    }

  }
}
