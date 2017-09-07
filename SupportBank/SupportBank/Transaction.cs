using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportBank
{
    class Transaction
    {
        public string from;
        public string to;
        public string description;
        public decimal amount;
        public string date;
        public bool validTransaction = true;

        public Transaction(string from, string to, string description, decimal amount, string date)
        {
            this.from = from;
            this.to = to;
            this.description = description;
            this.date = date;
            if (amount == 0)
            {
                validTransaction = false;
            }
            this.amount = amount;
        }

        public override string ToString()
        {
            if (!validTransaction)
            {
                return "An invalid transaction was made between " + from + " and " + to;
            }
            string output = from + " transferred " + amount + " to " + to + " for " + description + " on the date " + date;
            return output;
        }
    }
}
