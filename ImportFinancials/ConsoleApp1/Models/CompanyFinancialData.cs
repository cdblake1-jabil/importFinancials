using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Models
{
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
}
