namespace PrintInvoice
{
    public class PackageWrapper
    {
        public const int NullPackageId = -1;
        protected string _errorText;
        protected string[] _fieldValueList;

        protected int _packageId;
        protected string _trackingNumber;

        // constructor
        public PackageWrapper(int aPackageId)
        {
            _packageId = aPackageId;
        }

        // public properties

        public int PackageId
        {
            get => _packageId;
            set => _packageId = value;
        }

        public string TrackingNumber
        {
            get => _trackingNumber;
            set => _trackingNumber = value;
        }

        public string[] FieldValueList
        {
            get => _fieldValueList;
            set => _fieldValueList = value;
        }

        public string ErrorText
        {
            get => _errorText;
            set => _errorText = value;
        }
    }
}