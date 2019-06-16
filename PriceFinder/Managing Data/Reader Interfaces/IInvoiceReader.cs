using PriceFinding.Models;
using System.Collections.Generic;

namespace PriceFinding.Managing_Data.ReaderInterfaces
{
    interface IInvoiceReader
    {
      Sale GetLastPriceData(string customerCode, string productCode);
      Dictionary<string, Sale> GetLastPriceData(string customerCode, IEnumerable<string> productCodes);
    }//Int
}//NS
