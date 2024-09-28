using log4net;
using log4net.Config;

namespace PrintInvoice
{
    internal static class Log
    {
        private static ILog _log;

        private static ILog GetLogger()
        {
            if (_log == null)
            {
                XmlConfigurator.Configure();
                _log = LogManager.GetLogger(typeof(Program));
            }

            return _log;
        }

        public static void Info(string message) => GetLogger().Info(message);

        public static void Debug(string message) => GetLogger().Debug(message);
    }
}