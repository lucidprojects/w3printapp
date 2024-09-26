using System;
using System.Collections.Generic;
using System.Text;

namespace PrintInvoice
{
  class Log
  {
    private static log4net.ILog log = null;

    public static log4net.ILog getLogger() {
      if (log == null) {
        log4net.Config.XmlConfigurator.Configure();
        log = log4net.LogManager.GetLogger(typeof(Program));        
      }

      return log;
    }
  }
}
