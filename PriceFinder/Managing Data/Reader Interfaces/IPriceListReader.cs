
using System.Collections.Generic;

namespace PriceFinding.Managing_Data.ReaderInterfaces
{
    interface IPriceListReader
    {
        MyDictionary<double> GetPriceListPrices(string customerCode, IEnumerable<string> productCodes);
        double GetPriceListPrice(string customerCode, string productCode);
    }//Int
}//NS