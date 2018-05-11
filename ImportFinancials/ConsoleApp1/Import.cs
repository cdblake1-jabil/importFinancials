
using ConsoleApp1.Utilities;
using ConsoleApp1.Models;
using IEXTrading;
using IEXTrading.IEXTradingSTOCK_PRICE;
using IEXTrading.IEXTradingSTOCK_STATS;
using System.Collections.Generic;
using System.IO;

namespace ImportFinancials
{
    public class Import
    {
        static string ENVIRONMENT;
        static void Main(string[] args)
        {
            ENVIRONMENT = Constants.DEV_ENVIRONMENT; 

            IEXTradingConnection connection = IEXTradingConnection.Instance;
            Int_STOCK_PRICE stockPriceOperation = connection.GetQueryObject_STOCK_PRICE();
            Int_STOCK_STATS stockStatsOperation = connection.GetQueryObject_STOCK_STATS();

            IIEXTradingResponse_STOCK_PRICE stockPrice = stockPriceOperation.Query("JBL");
            IIEXTradingResponse_STOCK_STATS stockStats = stockStatsOperation.Query("JBL");

            CompanyFinancialData jabil = new CompanyFinancialData(stockPrice.RawData, stockStats.Data.SharesOutstanding, stockStats.Data.Marketcap);
            List<CompanyFinancialData> list_of_companies = new List<CompanyFinancialData>();
            list_of_companies.Add(jabil);
            string output = Utilities.GenerateReport(list_of_companies);

            if (ENVIRONMENT.Equals(Constants.HOME_ENVIRONMENT))
            {
                File.WriteAllText(Constants.HOME_FILE_PATH + Constants.FILE_NAME, output);
            }
            if (ENVIRONMENT.Equals(Constants.DEV_ENVIRONMENT))
            {
                File.WriteAllText(Constants.TEST_FILE_PATH + Constants.FILE_NAME, output);
                //Utilities.ExecuteCommand(Constants.PROCESS_COMMAND);
            }

        }
    }
}

