using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportBank
{
    class TransactionsGeneratorJson : TransactionsGenerator
    {
        public List<Transaction> GenerateTransactions(string path)
        {
            try
            {
                var listOfTransactions = JsonConvert.DeserializeObject<List<Transaction>>(File.ReadAllText(path));
                return listOfTransactions;
            }
            catch (Exception e)
            {
                LogAndPrintError(e.Message);
                return new List<Transaction>();
            }

        }

        private void LogAndPrintError(string message)
        {
            SupportBank.logger.Error(message);
            Console.WriteLine();
            Console.WriteLine("ERROR: {0}, see log for details", message);
            Console.WriteLine();
        }


    }
}
