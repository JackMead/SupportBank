using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportBank
{
    class TransactionsPrinter
    {
        private List<Transaction> ListOfTransactions { get; set; }

        public TransactionsPrinter(List<Transaction> listOfTransactions)
        {
            ListOfTransactions = listOfTransactions;
        }

        public void HandleUserRequests()
        {
            Console.WriteLine();
            Console.WriteLine("Would you like to see any of the transactions?");
            Console.WriteLine("Options: \"List All\", \"List [Account name]\", \"Quit\"");
            string userChoice = Console.ReadLine().ToLower();

            while (userChoice != "quit")
            {
                if (userChoice == "list all")
                {
                    PrintTransactions(ListOfTransactions);
                }
                else if (userChoice.Length < 5)
                {
                    Console.WriteLine("Sorry, I didn't understand that.");
                }
                else if (userChoice.Substring(0, 4) == "list")
                {
                    string accountName = GetAccountName(userChoice);
                    //check valid account name
                    if (ListOfTransactions.Where(transaction => transaction.FromAccount.ToLower() == accountName || transaction.ToAccount.ToLower() == accountName).Count() == 0)
                    {
                        Console.WriteLine("No account exists with that name");
                    }
                    PrintTransactions(ListOfTransactions
                        .Where(transaction => transaction.FromAccount.ToLower() == accountName
                            || transaction.ToAccount.ToLower() == accountName)
                        .ToList());

                }
                else
                {
                    Console.WriteLine("Sorry, I didn't understand that.");
                }

                Console.WriteLine();
                Console.WriteLine("Is there anything else you would like to do?");
                userChoice = Console.ReadLine();

            }
        }

        private void PrintTransactions(List<Transaction> listOfTransactions)
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

        private string GetAccountName(string userChoice)
        {
            string name = userChoice.Substring(5);
            return name;
        }


    }
}
