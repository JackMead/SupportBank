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
        public void Run()
        {
            string path = @"Resources/Transactions2014.csv";

            var listOfTransactions = GenerateTransactionsFromCSV(path);
            var listOfAccounts = GenerateAccountsFromTransactions(listOfTransactions);

            HandleUserInput(listOfTransactions);
            PrintAccounts(listOfAccounts);
            PrintAccountsSum(listOfAccounts);

        }

        private List<Transaction> GenerateTransactionsFromCSV(string path)
        {

            var lines = GetLinesFromCSV(path);
            var listOfTransactions = new List<Transaction>();

            foreach (var line in lines)
            {
                string[] elements = lines[i].Split(',');
                if(!decimal.TryParse(elements[4], out decimal amount))
                {
                    amount = 0;
                }
                //Order in CSV is Date,From,To,Narrative,Amount
                listOfTransactions.Add(GenerateTransaction(elements[0], elements[1], elements[2], elements[3], amount));
            }

            return listOfTransactions;

        }

        private List<Account> GenerateAccountsFromTransactions(List<Transaction> listOfTransactions)
        {
            var listOfAccounts = new List<Account>();
            foreach (var transaction in listOfTransactions)
            {
                if (listOfAccounts.Where(p => p.name == transaction.from).Count() == 0)
                {
                    listOfAccounts.Add(new Account(transaction.from, 0));
                }
                if (listOfAccounts.Where(p => p.name == transaction.to).Count() == 0)
                {
                    listOfAccounts.Add(new Account(transaction.to, 0));
                }

                listOfAccounts.First(p => p.name == transaction.from).GainMoney(transaction.amount * -1);
                listOfAccounts.First(p => p.name == transaction.to).GainMoney(transaction.amount);

            }

            return listOfAccounts;
        }

        private Transaction GenerateTransaction(string date, string from, string to, string description, decimal amount)
        {
            return new Transaction(date, from, to, amount, description);
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
                else if (userChoice.Substring(0, 4) == "list")
                {
                    string accountName = GetAccountName(userChoice);

                    PrintTransactions(listOfTransactions.Where(transaction => transaction.from.ToLower() == accountName || transaction.to.ToLower() == accountName));

                }

                Console.WriteLine();
                Console.WriteLine("Is there anything else you would like to do?");
                userChoice = Console.ReadLine();

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
    }
}
