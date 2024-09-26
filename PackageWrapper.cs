using System;
using System.Collections.Generic;
using System.Text;

namespace PrintInvoice
{
  public class PackageWrapper
  {
    public const int NULL_PACKAGE_ID = -1;

    protected int packageId;
    protected string trackingNumber = null;
    protected string[] fieldValueList;
    protected string errorText;

    // constructor
    public PackageWrapper(int aPackageId)
    {
      packageId = aPackageId;
    }

    // public properties

    public int PackageId
    {
      get { return packageId; }
      set { packageId = value; }
    }

    public string TrackingNumber
    {
      get { return trackingNumber; }
      set { trackingNumber = value; }
    }

    public string[] FieldValueList
    {
      get { return fieldValueList; }
      set { fieldValueList = value; }
    }

    public string ErrorText
    {
      get { return errorText; }
      set { errorText = value; }
    }

  }
}
