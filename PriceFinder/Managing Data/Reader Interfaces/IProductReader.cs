using System.Collections.Generic;

namespace PriceFinding.Managing_Data.ReaderInterfaces
{
   public interface IProductReader
   {
      double GetCostPrice(string prodCode);
      Dictionary<string, double> GetCostPrices(IEnumerable<string> productCodes);
   }
}