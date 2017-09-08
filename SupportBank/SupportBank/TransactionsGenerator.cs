using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace SupportBank
{
    class TransactionsGenerator
    {

        public List<Transaction> GenerateTransactions(string path, string fileType)
        {
            if (fileType == "csv")
            {
                return GenerateFromCSV(path);
            }
            else if (fileType == "json")
            {
                return GenerateFromJson(path);
            }
            else if (fileType == "xml")
            {
                return GenerateFromXMLAlternative(path);
            }
            else
            {
                Console.WriteLine("Error: FileType not accepted. Try using a csv, json or xml file");
                return new List<Transaction>();
            }
        }

        public List<Transaction> GenerateFromCSV(string path)
        {

            var lines = GetLinesFromCSV(path);
            var listOfTransactions = new List<Transaction>();

            foreach (var line in lines)
            {
                string[] elements = line.Split(',');
                if (!decimal.TryParse(elements[4], out decimal amount))
                {
                    SupportBank.logger.Error("The transaction amount {0} is not a vaid decimal. Transaction not included.", elements[4]);
                    Console.WriteLine();
                    Console.WriteLine("ERROR OCCURED PROCESSING TRANSACTIONS: See log for details");
                    Console.WriteLine();
                    continue;
                }
                if (!DateTime.TryParse(elements[0], out DateTime date))
                {
                    SupportBank.logger.Error("The date \"{0}\" is not a vaid date. Transaction not included.", elements[0]);
                    Console.WriteLine();
                    Console.WriteLine("ERROR OCCURED PROCESSING TRANSACTIONS: See log for details");
                    Console.WriteLine();
                    continue;
                }
                //Order in CSV is Date,From,To,Narrative,Amount
                listOfTransactions.Add(new Transaction(elements[1], elements[2], elements[3], amount, date));
            }

            return listOfTransactions;

        }

        public List<Transaction> GenerateFromJson(string path)
        {
            try
            {
                var listOfTransactions = JsonConvert.DeserializeObject<List<Transaction>>(File.ReadAllText(path));
                return listOfTransactions;
            }
            catch (Exception e)
            {
                LogAndPrintJsonError(e);
                return new List<Transaction>();
            }

        }

        public List<Transaction> GenerateFromXML(string path)
        {
            var listOfTransactions = new List<Transaction>();
            try
            {
                XmlReader xmlReader = XmlReader.Create(path);
                string dateAsString = "";
                string description = "";
                string from = "";
                string to = "";
                decimal value = 0;
                while (xmlReader.Read())
                {
                    if ((xmlReader.Name == "SupportTransaction") && (xmlReader.NodeType != XmlNodeType.EndElement))
                    {
                        dateAsString = xmlReader.GetAttribute("Date");
                    }
                    else if ((xmlReader.Name == "Description"))
                    {
                        description = xmlReader.ReadElementContentAsString();
                    }
                    else if ((xmlReader.Name == "Value"))
                    {
                        value = xmlReader.ReadElementContentAsDecimal();
                    }
                    else if ((xmlReader.Name == "From"))
                    {
                        from = xmlReader.ReadElementContentAsString();
                    }
                    else if ((xmlReader.Name == "To"))
                    {
                        to = xmlReader.ReadElementContentAsString();
                    }

                    if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == "SupportTransaction")
                    {
                        if (!int.TryParse(dateAsString, out int dateAsInt))
                        {
                            SupportBank.logger.Error("Problem with interpreting date from XML file");
                        }
                        DateTime date = DateTime.FromOADate(dateAsInt);
                        listOfTransactions.Add(new Transaction(from, to, description, value, date));
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine();
                Console.WriteLine("ERROR: Problem deserializing xml file. See log for details");
                Console.WriteLine();

                SupportBank.logger.Error("Problem deserializing xml. Exception message:" + e.Message);
            }

            return listOfTransactions;

        }

        public List<Transaction> GenerateFromXMLAlternative(string path)
        {
            XmlSerializer serializer =
            new XmlSerializer(typeof(List<Transaction>));

            var listOfTransactions = new List<Transaction>();
            using (
            var reader = new FileStream(path, FileMode.Open))
            {
                // Serialize the object, and close the TextWriter
                listOfTransactions=(List<Transaction>)serializer.Deserialize(reader);
            }
            return listOfTransactions;
        }

        private void LogAndPrintJsonError(Exception e)
        {
            SupportBank.logger.Error(e.Message);
            Console.WriteLine();
            Console.WriteLine("ERROR deserializing Json, see log for details");
            Console.WriteLine();
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

    }
}
