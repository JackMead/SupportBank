using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportBank
{
    class Transaction
    {
        public string FromAccount;
        public string ToAccount;
        public string Narrative;
        public decimal Amount;
        public DateTime Date;

        public Transaction(string from, string to, string description, decimal amount, DateTime date)
        {
            this.FromAccount = from;
            this.ToAccount = to;
            this.Narrative = description;
            this.Date = date;
            this.Amount = amount;
        }

        public override string ToString()
        {
            string output = FromAccount + " transferred " + Amount + " to " + ToAccount + " for " + Narrative + " on the date " + Date.ToShortDateString();
            return output;
        }
    }
}
