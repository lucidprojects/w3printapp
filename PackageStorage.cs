using System;
using System.Collections.Generic;
using System.Text;

namespace PrintInvoice
{
  // events delegates

  // UpdateList
  public delegate void PackageStorageUpdateListEventHandler(object sender, EventArgs e);

  public class PackageStorage<T> where T : PackageWrapper
  {
    // public properties
    public List<T> PackageList
    {
      get { return packageList; }
    }

    public string[] FieldNames
    {
      get 
      {
        if (fieldMetadata == null)
        {
          return null;
        }
        else
        {
          List<string> l = new List<string>();
          for (int i = 0; i < fieldMetadata.Length; i++)
          {
            l.Add(fieldMetadata[i].name);
          }
          return l.ToArray();
        }
      }
    }

    // public methods

    // get typed field value by value and col
    public object getTypedFieldValue(string aValue, int aCol) 
    {
      if (aValue.Length == 0)
      {
        return null;
      }
      else
      {
        try
        {
          switch (fieldMetadata[aCol].type)
          {
            case "int":
              return Int32.Parse(aValue);

            case "real":
              return Double.Parse(aValue, cultureInfo);

            case "datetime":
              return DateTime.Parse(aValue, cultureInfo);

            default: // string
              return aValue;
          }
        }
        catch (Exception)
        {
          return null;
        }
      }
    }

    // get typed field value by row and col
    public object getTypedFieldValue(int aRow, int aCol)
    {
      return getTypedFieldValue(packageList[aRow].FieldValueList[aCol], aCol);

    }

    // get package
    public T getPackageByPackageId(int aPackageId)
    { 
      return packageIdIndex[aPackageId];
    }

    // add package to list
    public void add(T aPackage)
    {
      if ((aPackage.PackageId != PackageWrapper.NULL_PACKAGE_ID && !packageIdIndex.ContainsKey(aPackage.PackageId))
          ||
          (aPackage.TrackingNumber != null && !trackingNumberIndex.ContainsKey(aPackage.TrackingNumber))
          ||
          (aPackage.PackageId == PackageWrapper.NULL_PACKAGE_ID && aPackage.TrackingNumber == null))
      {
        packageList.Add(aPackage);

        // add to index by package id
        if (aPackage.PackageId != PackageWrapper.NULL_PACKAGE_ID)
        {
          packageIdIndex[aPackage.PackageId] = aPackage;
        }

        // add to index by tracking number
        if (aPackage.TrackingNumber != null)
        {
          trackingNumberIndex[aPackage.TrackingNumber] = aPackage;
        }
      }
    }

    // remove packages from storage
    public void remove(List<T> aPackageList)
    {
      foreach (T package in aPackageList)
      {
        packageList.Remove(package);
        if (packageIdIndex.ContainsKey(package.PackageId))
        {
          packageIdIndex.Remove(package.PackageId);
        }
        if (package.TrackingNumber != null && trackingNumberIndex.ContainsKey(package.TrackingNumber))
        {
          trackingNumberIndex.Remove(package.TrackingNumber);
        }
      }
    }

    // clear storage
    public void clear()
    {
      packageIdIndex.Clear();
      trackingNumberIndex.Clear();
      packageList.Clear();
    }

    // public events
    public event PackageStorageUpdateListEventHandler UpdateList;

    // data fields
    protected List<T> packageList = new List<T>();
    protected DatabaseFieldMetadataType[] fieldMetadata = null;
    protected System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-US"); 

    // indexes for fast access
    protected Dictionary<int, T> packageIdIndex = new Dictionary<int, T>();
    protected Dictionary<string, T> trackingNumberIndex = new Dictionary<string, T>();


    // events callers
    protected void onUpdateList(EventArgs e)
    {
      if (UpdateList != null)
        UpdateList(this, e);
    }
  }
}
