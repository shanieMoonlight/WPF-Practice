using System.Collections.Generic;

namespace PriceFinding
{
   class PriceTypes
   {

      public const string LAST_PRICE = "Last Price";
      public const string PRICE_LIST_PRICE = "Price List";
      public const string MARGIN = "Margin";
      public const string MANUAL = "Manual";

      //------------------------------------------------------------------------//

      public static List<string> GetPriceTypes()
      {
         return new List<string>()
         {
            LAST_PRICE,
            MARGIN,
            PRICE_LIST_PRICE,
            MANUAL
         };
      }//GetPricingTypes

      //------------------------------------------------------------------------//

   }//Cls
}//NS
