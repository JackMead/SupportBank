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
    class TransactionsGeneratorFactory
    {
        public TransactionsGenerator ReturnGenerator(string path, string fileType)
        {
            TransactionsGenerator transactionsGenerator;
            if (fileType == "csv")
            {
                transactionsGenerator = new TransactionsGeneratorCSV();
            }
            else if (fileType == "json")
            {
                transactionsGenerator = new TransactionsGeneratorJson();
            }
            else if (fileType == "xml")
            {
                transactionsGenerator = new TransactionsGeneratorXml();
            }
            else
            {
                Console.WriteLine("Error: FileType not accepted. Try using a csv, json or xml file");
                return new TransactionsGeneratorCSV();
            }

            return transactionsGenerator;
        }
    }
}
