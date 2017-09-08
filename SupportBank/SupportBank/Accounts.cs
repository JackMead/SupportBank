using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportBank
{
    class Accounts
    {
        public List<Account> ListOfAccounts { get; set; }

        public Accounts(List<Transaction> listOfTransactions)
        {
            ListOfAccounts = GenerateAccountsFromTransactions(listOfTransactions);
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

        private void EnactTransaction(ref List<Account> listOfAccounts, Transaction transaction)
        {
            listOfAccounts.First(p => p.name == transaction.FromAccount).GainMoney(transaction.Amount * -1);
            listOfAccounts.First(p => p.name == transaction.ToAccount).GainMoney(transaction.Amount);
        }

        public void PrintAccounts()
        {
            Console.WriteLine();
            Console.WriteLine("Account Summary Below:");
            foreach (var account in ListOfAccounts)
            {
                Console.WriteLine(account.ToString());
            }
        }

        public void PrintSumOfAccountBalances()
        {
            decimal sum = 0;

            foreach (var account in ListOfAccounts)
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



    }
}
