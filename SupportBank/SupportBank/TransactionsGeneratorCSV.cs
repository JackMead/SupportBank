using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportBank
{
    class TransactionsGeneratorCSV : TransactionsGenerator
    {
        public List<Transaction> GenerateTransactions(string path)
        {

            var lines = GetLinesFromCSV(path);
            var listOfTransactions = new List<Transaction>();

            foreach (var line in lines)
            {
                string[] elements = line.Split(',');
                if (!decimal.TryParse(elements[4], out decimal amount))
                {
                    LogAndPrintError("The transaction amount" + elements[4] + " is not a vaid decimal.Transaction not included.");
                    continue;
                }
                if (!DateTime.TryParse(elements[0], out DateTime date))
                {
                    LogAndPrintError("The date \"" + elements[0] + "\" is not a vaid date. Transaction not included.");
                    continue;
                }
                //Order in CSV is Date,From,To,Narrative,Amount
                listOfTransactions.Add(new Transaction(elements[1], elements[2], elements[3], amount, date));
            }

            return listOfTransactions;

        }

        private string[] GetLinesFromCSV(string path)
        {
            try
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
            catch
            {
                Console.WriteLine();
                Console.WriteLine("ERROR: Problem reading file. See Log for details");
                Console.WriteLine();
                SupportBank.logger.Error("Error reading csv file: " + path);
            }
            string[] empty = { };
            return empty;
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
