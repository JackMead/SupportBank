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

        public Transaction(string from, string to, string description, decimal amount, string date)
        {
            this.from = from;
            this.to = to;
            this.description = description;
            this.amount = amount;
            this.date = date;
        }

        public override string ToString()
        {
            string output = from + " transferred " + amount + " to " + to + " for " + description + " on the date " + date;
            return output;
        }
    }
}
