using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SupportBank
{
    class UserHandler
    {
        public string ReturnFilePathForUserChosenYear()
        {
            Console.WriteLine("Which year would you like to see? 2012-2015");
            string userResponse = "";
            bool validYearProvided = false;
            string path = "";

            var filePathByYearDictionary = new Dictionary<int, string>() {
                { 2012, @"Resources/Transactions2012.xml" },
                { 2013, @"Resources/Transactions2013.json" },
                { 2014, @"Resources/Transactions2014.csv"},
                { 2015, @"Resources/DodgyTransactions2015.csv" }
            };

            while (!validYearProvided)
            {
                userResponse = Console.ReadLine();
                if (int.TryParse(userResponse, out int year))
                {
                    try
                    {
                        path = filePathByYearDictionary[year];
                        validYearProvided = true;
                    }
                    catch
                    {
                        validYearProvided = false;
                        Console.WriteLine("Sorry, we don't have the transactions for that year. Try a year between 2012 and 2015");
                    }
                }
                else
                {
                    Console.WriteLine("Sorry, I didn't receive an integer there. Which year would you like the transactions for?");
                }
            }
            return path;

        }

        public void HandleUserTransactionPrintingRequests(List<Transaction> listOfTransactions)
        {
            Console.WriteLine();
            Console.WriteLine("Would you like to see any of the transactions?");
            Console.WriteLine("Options: \"List All\", \"List [Account name]\", \"Quit\"");
            var transactionsPrinter = new TransactionsPrinter();
            string userChoice = Console.ReadLine().ToLower();

            while (userChoice != "quit")
            {
                if (userChoice == "list all")
                {
                    transactionsPrinter.PrintTransactions(listOfTransactions);
                }
                else if (userChoice.Length < 5)
                {
                    Console.WriteLine("Sorry, I didn't understand that.");
                }
                else if (userChoice.Substring(0, 4) == "list")
                {
                    string accountName = GetAccountName(userChoice);
                    transactionsPrinter.PrintTransactionsForAccount(listOfTransactions, accountName);

                }
                else
                {
                    Console.WriteLine("Sorry, I didn't understand that.");
                }

                Console.WriteLine();
                Console.WriteLine("Is there anything else you would like to do?");
                userChoice = Console.ReadLine().ToLower();

            }
        }

        public string GetUserFilePath()
        {
            Console.WriteLine("If you have transactions (in json,xml or csv form) you would like us to handle, please provide the full path:");
            return Console.ReadLine();

        }

        public void ExportAsXML(List<Transaction> listOfTransactions)
        {
            string fileName = GetUserFileName() + ".xml";

            XmlSerializer serializer =
            new XmlSerializer(typeof(List<Transaction>));

            using (
            var writer = new FileStream(fileName, FileMode.Create))
            {
                // Serialize the object, and close the TextWriter
                serializer.Serialize(writer, listOfTransactions);
            }


        }

        public bool ExternalTransactionFileChosen()
        {
            Console.WriteLine();
            Console.WriteLine("Do you want to: ");
            Console.WriteLine("1. Provide your own Transactions file");
            Console.WriteLine("2. Access a Transactions file already imported");
            Console.WriteLine("Please enter 1 or 2 for the appropriate option");
            string response = Console.ReadLine();
            while (response != "1" && response != "2")
            {
                Console.WriteLine("Sorry, I didn't get that. Option 1 or 2?");
                response = Console.ReadLine();
            }
            if (response == "1")
            {
                return true;
            }

            return false;

        }

        public bool WantsToExport()
        {
            Console.WriteLine();
            Console.WriteLine("Would you like to export these transactions as an XML? y/n");
            string response = Console.ReadLine();
            while (response != "y" && response != "n")
            {
                Console.WriteLine("Sorry, I didn't understand that. Try typing 'y' or 'n'");
                response = Console.ReadLine();
            }
            return response == "y";
        }

        private string GetUserFileName()
        {
            Console.WriteLine("What would you like the file to be called?");
            return Console.ReadLine();
        }

        public string GetAccountName(string userChoice)
        {
            string name = userChoice.Substring(5);
            return name.ToLower();
        }

    }
}
