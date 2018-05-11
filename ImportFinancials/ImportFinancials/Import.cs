using IEXTrading;
using IEXTrading.IEXTradingSTOCK_PRICE;
using IEXTrading.IEXTradingSTOCK_STATS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportFinancials
{
    public class Import
    {
        static string ENVIRONMENT;
        static void Main(string[] args)
        {
            ENVIRONMENT = Constants.HOME_ENVIRONMENT;

            IEXTradingConnection connection = IEXTradingConnection.Instance;
            Int_STOCK_PRICE stockPriceOperation = connection.GetQueryObject_STOCK_PRICE();
            Int_STOCK_STATS stockStatsOperation = connection.GetQueryObject_STOCK_STATS();

            IIEXTradingResponse_STOCK_PRICE stockPrice = stockPriceOperation.Query("JBL");
            IIEXTradingResponse_STOCK_STATS stockStats = stockStatsOperation.Query("JBL");

            CompanyFinancialData jabil = new CompanyFinancialData(stockPrice.RawData, stockStats.Data.SharesOutstanding, stockStats.Data.Marketcap);
            Console.WriteLine($"JABIL - Price: {jabil.Price}, Shares Outstanding: {jabil.SharesOutstanding}, Market Cap: {jabil.MarketCap}");
            List<CompanyFinancialData> list_of_companies = new List<CompanyFinancialData>();
            list_of_companies.Add(jabil);
            string output = Utilities.GenerateReport(list_of_companies);

            if (ENVIRONMENT.Equals(Constants.HOME_ENVIRONMENT))
            {
                File.WriteAllText(Constants.HOME_FILE_PATH + Constants.FILE_NAME, output);
            }
            if (ENVIRONMENT.Equals(Constants.DEV_ENVIRONMENT))
            {
                File.WriteAllText(Constants.DEVSERVER_FILE_PATH + Constants.FILE_NAME, output);
                Utilities.ExecuteCommand(Constants.PROCESS_COMMAND);
            }

        }


        class CompanyFinancialData
        {
            public string Price { get; private set; }

            public string SharesOutstanding { get; private set; }

            public string MarketCap { get; private set; }

            public CompanyFinancialData(string price, string sharesOutstanding, string marketCap)
            {
                Price = price;
                SharesOutstanding = sharesOutstanding;
                MarketCap = marketCap;
            }
        }

        static class Constants
        {
            public const string HOME_FILE_PATH = @"C:\workspace\import-jbl-financials\";
            public const string DEVSERVER_FILE_PATH = @"D:\JabilTM1\StrategicPlanning\ProcessFiles\";
            public const string FILE_NAME = "import_external_financials.csv";

            public const string HOME_ENVIRONMENT = "HOME";
            public const string DEV_ENVIRONMENT = "DEV";
            public const string PROCESS_COMMAND = "";

        }

        static class Utilities
        {
            public static void ExecuteCommand(string command)
            {
                ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe", command);
                Process process;

                processInfo.CreateNoWindow = true;
                processInfo.UseShellExecute = false;

                process = Process.Start(processInfo);
                process.WaitForExit();
            }
            public static string GenerateReport<T>(List<T> items) where T : class
            {
                var output = "";
                var delimiter = ";";

                var properties = typeof(T).GetProperties()
                 .Where(n =>
                 n.PropertyType == typeof(string)
                 || n.PropertyType == typeof(bool)
                 || n.PropertyType == typeof(char)
                 || n.PropertyType == typeof(byte)
                 || n.PropertyType == typeof(decimal)
                 || n.PropertyType == typeof(int)
                 || n.PropertyType == typeof(DateTime)
                 || n.PropertyType == typeof(DateTime?));

                using (var sw = new StringWriter())
                {
                    var header = properties
                    .Select(n => n.Name)
                    .Aggregate((a, b) => a + delimiter + b);

                    sw.WriteLine(header);

                    foreach (var item in items)
                    {
                        var row = properties
                        .Select(n => n.GetValue(item, null))
                        .Select(n => n == null ? "null" : n.ToString())
                        .Aggregate((a, b) => a + delimiter + b);

                        sw.WriteLine(row);
                    }

                    output = sw.ToString();
                }

                return output;
            }
        }
    }
}

