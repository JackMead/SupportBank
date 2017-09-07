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

namespace SupportBank
{
    class SupportBank
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public void Run()
        {
            SetupNLog();


            string csvPath = @"Resources/Transactions2014.csv";
            var listOfTransactions = GenerateTransactionsFromCSV(csvPath);
            var listOfAccounts = GenerateAccountsFromTransactions(listOfTransactions);

            HandleUserInput(listOfTransactions);
            PrintAccounts(listOfAccounts);
            PrintAccountsSum(listOfAccounts);

            string pathForDodgyCSV = @"Resources/DodgyTransactions2015.csv";
            var dodgyListOfTransactions = GenerateTransactionsFromCSV(pathForDodgyCSV);
            var dodgyListOfAccounts = GenerateAccountsFromTransactions(dodgyListOfTransactions);
            HandleUserInput(dodgyListOfTransactions);
            PrintAccounts(dodgyListOfAccounts);

            string pathFor2013Transactions = @"Resources/Transactions2013.json";
            var listOfTransactions2013 = GenerateTransactionsFromJSON(pathFor2013Transactions);
            var listOfAccounts2013 = GenerateAccountsFromTransactions(listOfTransactions2013);
            HandleUserInput(listOfTransactions2013);
            PrintAccounts(listOfAccounts2013);

            HandleUserProvidedTransactionFile();

        }

        private void SetupNLog()
        {
            var config = new LoggingConfiguration();
            var target = new FileTarget { FileName = @"C:\Work\Logs\SupportBank.log", Layout = @"${longdate} ${level} - ${logger}: ${message}" };
            config.AddTarget("File Logger", target);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, target));
            LogManager.Configuration = config;
        }

        private List<Transaction> GenerateTransactionsFromCSV(string path)
        {

            var lines = GetLinesFromCSV(path);
            var listOfTransactions = new List<Transaction>();

            foreach (var line in lines)
            {
                string[] elements = line.Split(',');
                if (!decimal.TryParse(elements[4], out decimal amount))
                {
                    logger.Error("The transaction amount {0} is not a vaid decimal", elements[4]);
                    Console.WriteLine();
                    Console.WriteLine("ERROR OCCURED PROCESSING TRANSACTIONS: See log for details");
                    Console.WriteLine();
                }
                if (!DateTime.TryParse(elements[0], out DateTime date))
                {
                    logger.Error("The date \"{0}\" is not a vaid decimal", elements[0]);
                    Console.WriteLine();
                    Console.WriteLine("ERROR OCCURED PROCESSING TRANSACTIONS: See log for details");
                    Console.WriteLine();
                }
                //Order in CSV is Date,From,To,Narrative,Amount
                listOfTransactions.Add(new Transaction(elements[1], elements[2], elements[3], amount, date));
            }

            return listOfTransactions;

        }

        private List<Transaction> GenerateTransactionsFromJSON(string path)
        {
            try
            {
                var listOfTransactions = JsonConvert.DeserializeObject<List<Transaction>>(File.ReadAllText(path));
                PrintTransactions(listOfTransactions);
                return listOfTransactions;
            }
            catch (Exception e)
            {
                LogAndPrintJsonError(e);
                return new List<Transaction>();
            }

        }

        private List<Account> GenerateAccountsFromTransactions(List<Transaction> listOfTransactions)
        {
            var listOfAccounts = new List<Account>();
            foreach (var transaction in listOfTransactions)
            {
                if (listOfAccounts.Where(p => p.name == transaction.FromAccount).Count() == 0)
                {
                    listOfAccounts.Add(new Account(transaction.FromAccount, 0));
                }
                if (listOfAccounts.Where(p => p.name == transaction.ToAccount).Count() == 0)
                {
                    listOfAccounts.Add(new Account(transaction.ToAccount, 0));
                }

                EnactTransaction(ref listOfAccounts, transaction);
            }

            return listOfAccounts;
        }

        private void HandleUserInput(List<Transaction> listOfTransactions)
        {
            Console.WriteLine("What would you like to do?");
            Console.WriteLine("Options: \"List All\", \"List [Account name]\", \"Quit\"");
            string userChoice = Console.ReadLine().ToLower();

            while (userChoice != "quit")
            {
                if (userChoice == "list all")
                {
                    PrintTransactions(listOfTransactions);
                }
                else if (userChoice.Length < 5)
                {
                    PrintInvalidChoiceMessage();
                }
                else if (userChoice.Substring(0, 4) == "list")
                {
                    string accountName = GetAccountName(userChoice);
                    //check valid account name
                    if (listOfTransactions.Where(transaction => transaction.FromAccount.ToLower() == accountName || transaction.ToAccount.ToLower() == accountName).Count() == 0)
                    {
                        Console.WriteLine("No account exists with that name");
                    }
                    PrintTransactions(listOfTransactions.Where(transaction => transaction.FromAccount.ToLower() == accountName || transaction.ToAccount.ToLower() == accountName));

                }
                else
                {
                    PrintInvalidChoiceMessage();
                }

                Console.WriteLine();
                Console.WriteLine("Is there anything else you would like to do?");
                userChoice = Console.ReadLine();

            }

        }

        private void HandleUserProvidedTransactionFile()
        {
            Console.WriteLine("If you have transactions (in json or csv form) you would like us to handle, please provide the full path:");
            string userResponse = Console.ReadLine();

            try
            {
                while (userResponse != "quit")
                {
                    if (userResponse.Substring(userResponse.Length - 3) == "csv")
                    {
                        GenerateTransactionsFromCSV(userResponse);
                    }
                    else if (userResponse.Substring(userResponse.Length - 4) == "json")
                    {
                        GenerateTransactionsFromJSON(userResponse);
                    }
                    else
                    {
                        Console.WriteLine("Sorry, that doesn't seem to be a suitable filetype");
                    }
                    Console.WriteLine("Would you like to try typing the path again? Type \"quit\" to leave otherwise");
                    userResponse = Console.ReadLine();
                }
            }
            catch(Exception e)
            {
                logger.Error(e.Message);
                Console.WriteLine();
                Console.WriteLine("Error with the provided path or file. See log for details");
                Console.WriteLine();
            }
        }

        private string[] GetLinesFromCSV(string path)
        {
            var initialLines = File.ReadAllText(path).Split('\n');
            int k = initialLines.Length;
            int emptyLines = 0;
            while (initialLines[k - 1] == "")
            {
                emptyLines++;
                k--;
            }

            var linesForOutput = new string[initialLines.Length - 1 - emptyLines];
            for (int i = 1; i < initialLines.Length - emptyLines; i++)
            {
                linesForOutput[i - 1] = initialLines[i];
            }

            return linesForOutput;

        }

        private void EnactTransaction(ref List<Account> listOfAccounts, Transaction transaction)
        {
            listOfAccounts.First(p => p.name == transaction.FromAccount).GainMoney(transaction.Amount * -1);
            listOfAccounts.First(p => p.name == transaction.ToAccount).GainMoney(transaction.Amount);
        }

        private void PrintAccountsSum(List<Account> listOfAccounts)
        {
            decimal sum = 0;

            foreach (var account in listOfAccounts)
            {
                sum += account.amountOwed;
            }

            Console.WriteLine();
            Console.WriteLine("The sum of the accounts comes to " + sum);
            if (sum == 0)
            {
                Console.WriteLine("This is good!");
            }
        }

        private void PrintTransactions(IEnumerable<Transaction> listOfTransactions)
        {
            if (listOfTransactions.Count() == 0)
            {
                return;
            }
            Console.WriteLine();
            foreach (var transaction in listOfTransactions)
            {
                Console.WriteLine(transaction.ToString());
            }
        }

        private void PrintAccounts(List<Account> listOfAccounts)
        {
            Console.WriteLine();
            foreach (var account in listOfAccounts)
            {
                Console.WriteLine(account.ToString());
            }
        }

        private string GetAccountName(string userChoice)
        {
            string name = userChoice.Substring(5);
            return name;
        }

        private void PrintInvalidChoiceMessage()
        {
            Console.WriteLine("Sorry, I didn't understand that.");
        }

        private void LogAndPrintJsonError(Exception e)
        {
            logger.Error(e.Message);
            Console.WriteLine();
            Console.WriteLine("ERROR deserializing Json, see log for details");
            Console.WriteLine();
        }
    }
}
