using DataAccess.Data;
using log4net.Config;
using log4net.Core;
using System;
using System.IO;
using System.Reflection;

namespace CollectData
{
    class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += App_ProcessExit;

            var defaultRepository = LoggerManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(defaultRepository, new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "log4net.config"));

            using var context = new DictionaryContext();
            string url = args.Length > 0 ? args[0] : null;
            new TratuParser(context).Parse(url);
        }

        private static void App_ProcessExit(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

    }
}
