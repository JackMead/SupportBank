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
    class TransactionsGeneratorXml : TransactionsGenerator
    {
        public List<Transaction> GenerateTransactions(string path)
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
            catch (Exception e)
            {
                LogAndPrintError("Problem deserializing xml file: " + e.Message);
            }

            return listOfTransactions;

        }

        public List<Transaction> GenerateTransactionsAlternative(string path)
        {
            XmlSerializer serializer =
            new XmlSerializer(typeof(List<Transaction>));

            var listOfTransactions = new List<Transaction>();
            using (
            var reader = new FileStream(path, FileMode.Open))
            {
                // Serialize the object, and close the TextWriter
                listOfTransactions = (List<Transaction>)serializer.Deserialize(reader);
            }
            return listOfTransactions;
        }

        public void LogAndPrintError(string message)
        {
            SupportBank.logger.Error(message);
            Console.WriteLine();
            Console.WriteLine("ERROR: {0}, see log for details", message);
            Console.WriteLine();
        }


    }
}
