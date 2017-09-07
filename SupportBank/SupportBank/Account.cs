using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportBank
{
    class Account
    {
        public string name;
        public decimal amountOwed;

        public Account(string name, decimal amountOwed)
        {
            this.name = name;
            this.amountOwed = amountOwed;
        }

        public void GainMoney(decimal amount)
        {
            amountOwed += amount;
        }

        public override string ToString()
        {
            string output = name + " is owed £" + amountOwed + " in total";
            return output;
        }
    }
}
