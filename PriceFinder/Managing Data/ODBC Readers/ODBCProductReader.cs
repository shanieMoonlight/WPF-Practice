using PriceFinding.Managing_Data.ReaderInterfaces;
using PriceFinding.Properties;
using PriceFinding.Utility.Sage;
using System.Collections.Generic;
using System.Data;

namespace PriceFinding.Managing_Data.ODBC_Readers
{
   public class ODBCProductReader : AODBCReader, IProductReader
   {

      private static UserSettings sageUsrSet = UserSettings.Default;
      private SageTables tbls = new SageTables();
      private SageColumns cols = new SageColumns();



      private readonly double baseCurrencyCode = (double)sageUsrSet.baseCurrCode;

      //-------------------------------------------------------------------------------------------------------//

      public ODBCProductReader()
      {
      }//ctor

      //-------------------------------------------------------------------------------------------------------//

      /// <summary>
      /// Get the cost price of the product
      /// </summary>
      /// <param name="prodCode">Product id</param>
      /// <returns>Cost Price</returns>
      public double GetCostPrice(string prodCode)
      {
         #region queryString
         string queryString = $@"SELECT {tbls.Stock}.{cols.StockCode}, {tbls.Stock}.{cols.ProdDesc}, {tbls.Stock}.{cols.LstPurchPrice}, {tbls.Stock}.{cols.SalePrc}, *
                                  FROM
                                    {tbls.Stock}
                                  WHERE
                                    {tbls.Stock}.{cols.StockCode} = '{prodCode}'
                                  ORDER BY
                                    {tbls.Stock}.{cols.StockCode}";
         #endregion

         double costPrice = 0;

         DataTable dataTable = FillTableFromODBC(queryString);

         if (dataTable.Rows.Count > 0)
         {
            var row = dataTable.Rows[0];

            costPrice = (double)row[cols.LstPurchPrice];
         }//if

         return costPrice;

      }//GetCostPrice

      //-------------------------------------------------------------------------------------------------------//

      /// <summary>
      /// Get the cost price of the product
      /// </summary>
      /// <param name="prodCodes">List product ids</param>
      /// <returns>List of Cost Prices</returns>
      public Dictionary<string, double> GetCostPrices(IEnumerable<string> productCodes)
      {
         #region queryString
         string queryString = $@"SELECT {tbls.Stock}.{cols.StockCode}, {tbls.Stock}.{cols.ProdDesc}, {tbls.Stock}.{cols.LstPurchPrice}, {tbls.Stock}.{cols.SalePrc}
                                  FROM
                                    {tbls.Stock}
                                  WHERE
                                    {tbls.Stock}.{cols.StockCode} IN ('{string.Join("', '", productCodes)}')
                                  ORDER BY
                                    { tbls.Stock}.{cols.StockCode}";
         #endregion

         var costsMap = new Dictionary<string, double>();

         DataTable dataTable = FillTableFromODBC(queryString);

         if (dataTable.Rows.Count > 0)
         {
            foreach (DataRow row in dataTable.Rows)
            {
               var code = row[cols.StockCode].ToString();
               var strCost = row[cols.LstPurchPrice].ToString();
               if (double.TryParse(strCost, out double cost))
                  costsMap[code] = cost;
            }//foreach
         }//If

         return costsMap;

         }//GetCostPrice

      //-------------------------------------------------------------------------------------------------------//

   }//Cls
}//NS
