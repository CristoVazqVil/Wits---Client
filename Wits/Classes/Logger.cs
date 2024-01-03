using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wits.Classes
{
    public static class Logger
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Logger));

        static Logger()
        {
            string configFile = "resources/log4net.config";
            string absoluteConfigPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFile);

            XmlConfigurator.Configure(new System.IO.FileInfo(absoluteConfigPath));
        }

        public static void LogDebugException(Exception ex)
        {
            log.Debug("Debug:", ex);
        }

        public static void LoginfoException(Exception ex)
        {
            log.Info("Info:", ex);
        }

        public static void LogWarnException(Exception ex)
        {
            log.Warn("Warning:", ex);
        }

        public static void LogErrorException(Exception ex)
        {
            log.Error("Error:", ex);
        }

        public static void LogFatalException(Exception ex)
        {
            log.Fatal("Fatal:", ex);
        }
    }
}
