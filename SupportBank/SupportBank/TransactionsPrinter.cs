using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportBank
{
    class TransactionsPrinter
    {
        public void PrintTransactions(List<Transaction> listOfTransactions)
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

        public void PrintTransactionsForAccount(List<Transaction> listOfTransactions, string accountName)
        {
            //check valid account name
            if (listOfTransactions.Where(transaction => transaction.FromAccount.ToLower() == accountName || transaction.ToAccount.ToLower() == accountName).Count() == 0)
            {
                Console.WriteLine("No account exists with that name");
            }
            PrintTransactions(listOfTransactions
                .Where(transaction => transaction.FromAccount.ToLower() == accountName
                    || transaction.ToAccount.ToLower() == accountName)
                .ToList());
        }
    }
}
