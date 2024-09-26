using System;
using System.Collections.Generic;
using System.Text;
using System.Security;

namespace PrintInvoice
{
  using PackageList = List<PrintPackageWrapper>;

  public class PrintPackageWrapper : PackageWrapper
  {
    public PrintPackageWrapper(int aPackageId)
      : base(aPackageId)
    {
      State = UNPRINTED;
    }

    public const int UNPRINTED    = 0x01;
    public const int LOADED       = 0x02;
    public const int PRINTED      = 0x04;
    public const int STATUS_SAVED = 0x08;
    public const int ERROR        = 0x10;
    public const int LOCKED       = 0x20;

    private int state;
    private byte[] pdf;

    public int State
    {
      set 
      { 
        if (value == UNPRINTED)
        {
          state = UNPRINTED;
        }

        if (value == LOADED)
        {
          state = UNPRINTED | LOADED;
        }

        if (value == PRINTED)
        {
          state = LOADED | PRINTED;
        }

        if (value == STATUS_SAVED)
        {
          state = LOADED | PRINTED | STATUS_SAVED;
        }

        if (value == ERROR)
        {
          state = ERROR;
        }

        if (value == LOCKED)
        {
          state = LOCKED;
        }
      }
    }

    public bool IsUnprinted { get { return (state & UNPRINTED) != 0; } }
    public bool IsLoaded { get { return (state & LOADED) != 0; } }
    public bool IsPrinted { get { return (state & PRINTED) != 0; } }
    public bool IsStatusSaved { get { return (state & STATUS_SAVED) != 0; } }
    public bool IsError { get { return (state & ERROR) != 0; } }
    public bool IsLocked { get { return (state & LOCKED) != 0; } } // locked by another process

    public byte[] Pdf
    {
      get { return pdf; }
      set { pdf = value; }
    }

    // sequence number
    public int printBatchId;
    public int printBatchCount;
    public int elementBatch;
    public int elementBatchCount;

    public string getOrderedElemens() { 
      return FieldValueList[Properties.Settings.Default.ElementsOrderedFieldIndex];
    }

    // Master Pisk List
    public bool isFirstBatchPackage;
    public int mplElementBatchCount; // mater picklist element batch count

    // Pack Jacket
    public bool isPackJacket;
  }

  // Update event delegate
  public delegate void InvoiceStorageUpdateEventHandler(object sender, EventArgs e);

  // UpdateSubset event delegate
  public delegate void InvoiceStorageUpdateSubsetEventHandler(object sender, EventArgs e);

  // UpdatePackageState event args
  public class PrintPackageStorageUpdatePackageStateEventArgs : EventArgs 
  {
    private int packageId;

    public PrintPackageStorageUpdatePackageStateEventArgs(int aPackageId)
    {
      packageId = aPackageId;
    }

    public int PackageId
    {
      get { return packageId; }
    }
	
  }

  // UpdateInvoiceState event delegate
  public delegate void PrintPackageStorageUpdatePackageStateEventHandler(object sender, PrintPackageStorageUpdatePackageStateEventArgs e);


  class PrintPackageStorage : PackageStorage<PrintPackageWrapper>
  {
    // consts
    public const int PACKAGE_ID_COLUMN_INDEX = 0;
    public const int INVOICE_NUMBER_COLUMN_INDEX = 1;
    public const int TRACKING_NUMBER_COLUMN_INDEX = 2;

    private int queryIndex;

    private LabelService labelService;
    private Config config;

    private PackageList subsetPackageList = new PackageList();
    private PackageList customPackageList = new PackageList();
    private PackageList currentSubset = null;
    private List<string> fieldNames = new List<string>();

    public event InvoiceStorageUpdateEventHandler Update;
    public event InvoiceStorageUpdateSubsetEventHandler UpdateSubset;
    public event PrintPackageStorageUpdatePackageStateEventHandler UpdatePackageState;
    
    public PrintPackageStorage(Config aConfig, LabelService aLabelService)
    {
      config = aConfig;
      labelService = aLabelService;
    }

    public void setQuery(int aQueryIndex)
    {
      clear();
      customPackageList.Clear();
      subsetPackageList.Clear();

      RunSqlQueryRequestType request = new RunSqlQueryRequestType();
      request.query = SecurityElement.Escape(config.QueryList[aQueryIndex].Text);
      request.clientVersion = Routines.getVersion();
      RunSqlQueryResponseType response = labelService.runSqlQuery(request);
      if (response.status != 0)
      {
        throw new Exception(String.Format("Label service returns error status\nStatus: {0}\nMessage: {1}\nSubstatus: {2}\nSubmessage: {3}", response.status, response.message, response.substatus, response.submessage));
      }

      if (response.rows != null)
      {
        foreach (RowType row in response.rows)
        {
          int packageId = Int32.Parse(row.columns[PACKAGE_ID_COLUMN_INDEX]);
          PrintPackageWrapper package = new PrintPackageWrapper(packageId);
          // TODO: set tracking number column index in settings
          package.TrackingNumber = row.columns[TRACKING_NUMBER_COLUMN_INDEX];
          package.FieldValueList = row.columns;
          add(package);
        }
      }

      fieldMetadata = response.meta;

      onUpdate(new EventArgs());

      queryIndex = aQueryIndex;
    }

    public void setSubsetAll()
    {
      currentSubset = packageList;
      onUpdateSubset(new EventArgs());
    }

    public void setSubsetCustom()
    {
      currentSubset = customPackageList;
      onUpdateSubset(new EventArgs());
    }

    public void setSubset(int aSubqueryIndex)
    {
      subsetPackageList.Clear();
      RunSqlQueryRequestType request = new RunSqlQueryRequestType();
      request.query = SecurityElement.Escape(config.QueryList[queryIndex].SubqueryList[aSubqueryIndex].Text);
      request.clientVersion = Routines.getVersion();
      RunSqlQueryResponseType response = labelService.runSqlQuery(request);
      if (response.status != 0)
      {
        throw new Exception(String.Format("Label service returns error status\nStatus: {0}\nMessage: {1}\nSubstatus: {2}\nSubmessage: {3}", response.status, response.message, response.substatus, response.submessage));
      }

      if (response.rows != null)
      {
        foreach (RowType responseRow in response.rows)
        {
          string invoiceId = responseRow.columns[0];
          foreach (PrintPackageWrapper invoiceWrapper in packageList)
          {
            if (invoiceWrapper.FieldValueList[0] == invoiceId)
            {
              subsetPackageList.Add(invoiceWrapper);
              break;
            }
          }
        }
      }

      currentSubset = subsetPackageList;
      onUpdateSubset(new EventArgs());
    }

    public void addToCustomSubset(List<int> aIdList)
    {
      foreach (int id in aIdList)
      {
        PrintPackageWrapper invoice = packageIdIndex[id];
        if (!customPackageList.Contains(invoice))
        {
          customPackageList.Add(invoice);
        }
      }
      onUpdateSubset(new EventArgs());
    }

    public void removeFromCustomSubset(List<int> aIdList)
    {
      foreach (int id in aIdList)
      {
        PrintPackageWrapper invoice = packageIdIndex[id];
        customPackageList.Remove(invoice);
      }
      onUpdateSubset(new EventArgs());
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

    public PackageList SubsetPackageList
    {
      get { return currentSubset; }
    }


    private void onUpdate(EventArgs e)
    {
      if (Update != null)
        Update(this, e);
    }

    private void onUpdateSubset(EventArgs e)
    {
      if (UpdateSubset != null)
        UpdateSubset(this, e);
    }

    private void onUpdatePackageState(PrintPackageStorageUpdatePackageStateEventArgs e)
    {
      if (UpdatePackageState != null)
        UpdatePackageState(this, e);
    }

    public void unlock(List<int> aIdList)
    {
      UnlockResponseType response = labelService.unlock(aIdList.ToArray());

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
