using log4net;
using log4net.Config;

namespace PrintInvoice
{
    internal class Log
    {
        private static ILog _log;

        public static ILog getLogger()
        {
            if (_log == null)
            {
                XmlConfigurator.Configure();
                _log = LogManager.GetLogger(typeof(Program));
            }

            return _log;
        }
    }
}