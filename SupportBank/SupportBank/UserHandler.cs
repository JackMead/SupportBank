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
            string path = "";

            while (userResponse != "quit")
            {
                userResponse = Console.ReadLine();
                switch (userResponse)
                {
                    case "quit":
                        path = "None";
                        break;

                    case "2012":
                        path = @"Resources/Transactions2012.xml";
                        userResponse = "quit";
                        break;

                    case "2013":
                        path = @"Resources/Transactions2013.json";
                        userResponse = "quit";
                        break;

                    case "2014":
                        path = @"Resources/Transactions2014.csv";
                        userResponse = "quit";
                        break;

                    case "2015":
                        path = @"Resources/DodgyTransactions2015.csv";
                        userResponse = "quit";
                        break;

                    default:
                        Console.WriteLine("Sorry, didn't understand that. Try again? (or type \"quit\" to leave");
                        break;
                }
            }
            return path;

        }

        public void HandleUserProvidedTransactionsFile()
        {
            var transactionsGenerator = new TransactionsGenerator();
            Console.WriteLine("If you have transactions (in json or csv form) you would like us to handle, please provide the full path:");
            string userResponse = Console.ReadLine();

            try
            {
                while (userResponse != "quit")
                {
                    string fileType = DetermineFileType(userResponse);
                    if (fileType == "csv")
                    {
                        transactionsGenerator.GenerateFromCSV(userResponse);
                        break;
                    }
                    else if (fileType == "json")
                    {
                        transactionsGenerator.GenerateFromJson(userResponse);
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Sorry, that doesn't seem to be a suitable filetype");
                        Console.WriteLine("Would you like to try typing the path again? Type \"quit\" to leave otherwise");
                        userResponse = Console.ReadLine();

                    }

                }
            }
            catch (Exception e)
            {
                SupportBank.logger.Error(e.Message);
                Console.WriteLine();
                Console.WriteLine("Error with the provided path or file. See log for details");
                Console.WriteLine();
            }
        }

        public void ExportAsXML(List<Transaction> listOfTransactions)
        {
            string fileName = GetUserFileName();
            
            XmlSerializer serializer =
            new XmlSerializer(typeof(List<Transaction>));

            using (
            var writer = new FileStream(fileName, FileMode.Create))
            {
                // Serialize the object, and close the TextWriter
                serializer.Serialize(writer, listOfTransactions);
            }


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
