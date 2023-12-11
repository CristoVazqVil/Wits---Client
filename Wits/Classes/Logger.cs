using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wits.Classes
{
    internal class Logger
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Logger));

        static Logger()
        {
            string configFile = "resources/log4net.config";
            string absoluteConfigPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFile);

            XmlConfigurator.Configure(new System.IO.FileInfo(absoluteConfigPath));
        }

        public static void LogException(Exception ex)
        {
            log.Error("Error:", ex);
        }
    }
}
