using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceFinding
{
   class PriceTypes
   {

      public const string LAST_PRICE = "Last Price";
      public const string PRICE_LIST_PRICE = "Price List";
      public const string MARGIN = "Margin";

      //------------------------------------------------------------------------//

      public static List<string> GetPriceTypes()
      {
         return new List<string>()
         {
            LAST_PRICE,
            MARGIN,
            PRICE_LIST_PRICE
         };
      }//GetPricingTypes

      //------------------------------------------------------------------------//

   }//Cls
}//NS
