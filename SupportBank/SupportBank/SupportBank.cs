using Newtonsoft.Json;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace SupportBank
{
    class SupportBank
    {
        public static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public void Run()
        {
            SetupNLog();

            var userHandler = new UserHandler();
            string path = userHandler.ReturnFilePathForUserChosenYear();

            string fileType = userHandler.DetermineFileType(path);

            var transactionsGenerator = new TransactionsGenerator();
            var listOfTransactions = transactionsGenerator.GenerateTransactions(path, fileType);

            var accountsManager = new AccountsManager(listOfTransactions);
            accountsManager.PrintAccounts();

            var transactionsPrinter = new TransactionsPrinter(listOfTransactions);
            transactionsPrinter.HandleUserRequests();
            
            if (userHandler.WantsToExport())
            {
                userHandler.ExportAsXML(listOfTransactions);
            }

            userHandler.HandleUserProvidedTransactionsFile();
        }

        private void SetupNLog()
        {
            var config = new LoggingConfiguration();
            var target = new FileTarget { FileName = @"C:\Work\Logs\SupportBank.log", Layout = @"${longdate} ${level} - ${logger}: ${message}" };
            config.AddTarget("File Logger", target);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, target));
            LogManager.Configuration = config;
        }

    }
}
