using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace PrintInvoice
{
    // events delegates

    // UpdateList
    public delegate void PackageStorageUpdateListEventHandler(object sender, EventArgs e);

    public class PackageStorage<T> where T : PackageWrapper
    {
        protected CultureInfo _cultureInfo = new CultureInfo("en-US");
        protected DatabaseFieldMetadataType[] _fieldMetadata = null;

        // indexes for fast access
        protected Dictionary<int, T> _packageIdIndex = new Dictionary<int, T>();

        // data fields
        protected List<T> _packageList = new List<T>();

        protected Dictionary<string, T> _trackingNumberIndex = new Dictionary<string, T>();

        // public properties
        public List<T> PackageList => _packageList;

        public string[] FieldNames => _fieldMetadata?.Select(t => t.name).ToArray();

        // public methods

        // get typed field value by value and col
        public object getTypedFieldValue(string aValue, int aCol)
        {
            if (aValue.Length == 0)
                return null;
            try
            {
                switch (_fieldMetadata[aCol].type)
                {
                    case "int":
                        return int.Parse(aValue);

                    case "real":
                        return double.Parse(aValue, _cultureInfo);

                    case "datetime":
                        return DateTime.Parse(aValue, _cultureInfo);

                    default: // string
                        return aValue;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        // get typed field value by row and col
        public object getTypedFieldValue(int aRow, int aCol)
        {
            return getTypedFieldValue(_packageList[aRow].FieldValueList[aCol], aCol);
        }

        // get package
        public T getPackageByPackageId(int aPackageId)
        {
            return _packageIdIndex[aPackageId];
        }

        // add package to list
        public void add(T aPackage)
        {
            if ((aPackage.PackageId != PackageWrapper.NullPackageId && !_packageIdIndex.ContainsKey(aPackage.PackageId)) ||
                (aPackage.TrackingNumber != null && !_trackingNumberIndex.ContainsKey(aPackage.TrackingNumber)) ||
                (aPackage.PackageId == PackageWrapper.NullPackageId && aPackage.TrackingNumber == null))
            {
                _packageList.Add(aPackage);

                // add to index by package id
                if (aPackage.PackageId != PackageWrapper.NullPackageId) _packageIdIndex[aPackage.PackageId] = aPackage;

                // add to index by tracking number
                if (aPackage.TrackingNumber != null) _trackingNumberIndex[aPackage.TrackingNumber] = aPackage;
            }
        }

        // remove packages from storage
        public void remove(List<T> aPackageList)
        {
            foreach (var package in aPackageList)
            {
                _packageList.Remove(package);
                if (_packageIdIndex.ContainsKey(package.PackageId)) _packageIdIndex.Remove(package.PackageId);
                if (package.TrackingNumber != null && _trackingNumberIndex.ContainsKey(package.TrackingNumber))
                    _trackingNumberIndex.Remove(package.TrackingNumber);
            }
        }

        // clear storage
        public void clear()
        {
            _packageIdIndex.Clear();
            _trackingNumberIndex.Clear();
            _packageList.Clear();
        }

        // public events
        public event PackageStorageUpdateListEventHandler UpdateList;


        // events callers
        protected void onUpdateList(EventArgs e)
        {
            UpdateList?.Invoke(this, e);
        }
    }
}