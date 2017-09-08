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

            if (userHandler.UserHasChosenExternalFile())
            {
                string path = userHandler.GetUserFilePath();
                string fileType = DetermineFileType(path);
                HandleUserRequestsUsingTransactionsFile(path, fileType);
            }
            else
            {
                string path = userHandler.ReturnFilePathForUserChosenYear();
                string fileType = DetermineFileType(path);
                HandleUserRequestsUsingTransactionsFile(path, fileType);
            }
        }

        private void SetupNLog()
        {
            var config = new LoggingConfiguration();
            var target = new FileTarget { FileName = @"C:\Work\Logs\SupportBank.log", Layout = @"${longdate} ${level} - ${logger}: ${message}" };
            config.AddTarget("File Logger", target);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, target));
            LogManager.Configuration = config;
        }
        
        private void HandleUserRequestsUsingTransactionsFile(string path, string fileType)
        {
            var transactionsGeneratorFactory = new TransactionsGeneratorFactory();
            var transactionsGenerator = transactionsGeneratorFactory.ReturnGenerator(path,fileType);
            var listOfTransactions = transactionsGenerator.GenerateTransactions(path);

            if (listOfTransactions.Count() != 0)
            {
                var accounts = new Accounts(listOfTransactions);
                accounts.PrintAccounts();

                var userHandler = new UserHandler();
                userHandler.HandleUserTransactionPrintingRequests(listOfTransactions);

                if (userHandler.WantsToExport())
                {
                    userHandler.ExportAsXML(listOfTransactions);
                }
            }

            
        }

        public string DetermineFileType(string path)
        {
            if (path.Length < 3)
            {
                return "other";
            }
            if (path.Substring(path.Length - 3) == "csv")
            {
                return "csv";
            }
            else if (path.Substring(path.Length - 4) == "json")
            {
                return "json";
            }
            else if (path.Substring(path.Length - 3) == "xml")
            {
                return "xml";
            }
            else
            {
                return "other";
            }

        }

    }
}
