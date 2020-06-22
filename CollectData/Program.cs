using DataAccess.Data;
using log4net.Config;
using log4net.Core;
using System;
using System.IO;
using System.Reflection;
using System.Threading;

namespace CollectData
{
    class Program
    {
        private static readonly CancellationTokenSource cancellationParserSource = new CancellationTokenSource();

        static void Main(string[] args)
        {
            Console.CancelKeyPress += Console_CancelKeyPress;
            AppDomain.CurrentDomain.ProcessExit += App_ProcessExit;

            var defaultRepository = LoggerManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(defaultRepository, new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "log4net.config"));

            using var context = new DictionaryContext();
            string url = args.Length > 0 ? args[0] : null;
            new TratuParser(context).Parse(url, cancellationParserSource.Token);
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            // Request to cancel the parser
            cancellationParserSource.Cancel();

            // Prevent the app from being closed to wait for the remaining process
            e.Cancel = true;
        }

        private static void App_ProcessExit(object sender, EventArgs e)
        {
        }
    }
}
