using PriceFinding.Managing_Data.ReaderInterfaces;
using PriceFinding.Models;
using PriceFinding.Properties;
using PriceFinding.Utility.Sage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace PriceFinding.Managing_Data.ODBC_Readers
{
   class ODBCInvoiceReader : AODBCReader, IInvoiceReader
   {
      #region Variables

      private SageTables tbls = new SageTables();
      private SageColumns cols = new SageColumns();
      private UserSettings sageUsrSet = UserSettings.Default;
      private static readonly string salePriceCol = "SALE_PRICE";
      private static readonly string foreignSalePriceCol = "FOREIGN_SALE_PRICE";

      #endregion

      //-------------------------------------------------------------------------------------------------------//

      /// <summary>
      /// Get Last price of the product that this customer bought.
      /// </summary>
      /// <param name="customerCode">Customer Code</param>
      /// <param name="productCode">Product Code</param>
      /// <returns></returns>
      public Sale GetLastPriceData(string customerCode, string productCode)
      {

         MyDictionary<MyDictionary<List<Sale>>> customerActivity = new MyDictionary<MyDictionary<List<Sale>>>(StringComparer.InvariantCultureIgnoreCase);

         //Start date is lookBackYrs year ago.
         int lookBackYrs = (int)Settings.Default.invoiceLookBackYrs;
         string startDate = "'" + (DateTime.Now.Year - lookBackYrs) + "-" + DateTime.Now.Month + "-01'";


         #region queryString
         string queryString = $@"SELECT 
                                 {tbls.Inv}.{cols.InvNum},
                                 {tbls.Inv}.{cols.CusCode},
                                 {tbls.Inv}.{cols.InvDate},
                                 {tbls.Inv}.{cols.CurrCode},
                                 { tbls.InvItm}.{cols.StockCode},
                                 (({cols.NetAmt}/{cols.Qty})) AS  {salePriceCol},
                                 (({cols.FrgnNetAmt}/{cols.Qty})) AS {foreignSalePriceCol}
                              FROM 
                                 {tbls.Inv} 
                              JOIN 
                                 {tbls.InvItm}
                              ON 
                                 {tbls.Inv}.{cols.InvNum} = {tbls.InvItm}.{cols.InvNum}
                              WHERE 
                                 {tbls.Inv}.{cols.CusCode} = {customerCode}
                              AND 
                                 {tbls.InvItm}.{cols.StockCode} = {productCode}
                              AND 
                                 {startDate}  <= {cols.InvDate}
                              ORDER BY 
                                 {cols.InvDate}  Desc, 
                                 {cols.StockCode}"
                              ;
         #endregion


         string invNum = string.Empty;
         double salePrice = 0;
         DateTime date = new DateTime();

         List<Sale> salesList = new List<Sale>();

         try
         {
            DataTable dataTable = FillTableFromODBC(queryString);

            if (dataTable.Rows.Count > 0)
            {
               var row = dataTable.Rows[0];
               invNum = row[cols.InvNum].ToString();
               if (!DateTime.TryParse(row[cols.InvDate].ToString(), out date))
                  ParseExMsg(cols.InvDate, invNum, productCode, "1900/0/0");
               if (!double.TryParse(row[salePriceCol].ToString(), out salePrice))
               {
                  ParseExMsg(salePriceCol, invNum, productCode, " 0. So is sale price");
               }//If


               //Check if it's a foreign customer.
               int.TryParse(row[cols.CurrCode].ToString(), out int currCode);
               if (currCode != (int)sageUsrSet.baseCurrCode)
               {
                  if (!double.TryParse(row[foreignSalePriceCol].ToString(), out salePrice))
                  {
                     ParseExMsg(foreignSalePriceCol, invNum, productCode, " 0. So is sale price");
                  }//If
               }//If



               //Retrieve sale and add it to salesList.
               return new Sale(date, salePrice, productCode);

            }//If
         }
         catch (Exception e)
         {
            string eString = "Problem reading Invoice Data"
                              + "\r\n    -----------------     \r\n"
                              + e.GetType() + "\r\n" + e.Message
                              + "\r\n    -----------------     \r\n"
                              + "Query: " + queryString
                              + "\r\nInvoice No.: " + invNum + ", Product: " + productCode + "Customer : " + customerCode
                              + "\r\nSale Price : " + salePrice + "\r\n";
            // server.TbInfo = eString;
            throw new Exception(eString);
         }//Catch


         return new Sale();

      }//readInvoiceData

      //-------------------------------------------------------------------------------------------------------//

      /// <summary>
      /// Get Last price of the product that this customer bought.
      /// </summary>
      /// <param name="customerCode">Customer Code</param>
      /// <param name="productCode">Product Code</param>
      /// <returns></returns>
      public Dictionary<string, Sale> GetLastPriceData(string customerCode, IEnumerable<string> productCodes)
      {
         //Start date is lookBackYrs year ago.
         int lookBackYrs = (int)Settings.Default.invoiceLookBackYrs;
         string startDate = "'" + (DateTime.Now.Year - lookBackYrs) + "-" + DateTime.Now.Month + "-01'";


         #region queryString
         string queryString = $@"SELECT 
                                 {tbls.Inv}.{cols.InvNum},
                                 {tbls.Inv}.{cols.CusCode},
                                 {tbls.Inv}.{cols.InvDate},
                                 {tbls.Inv}.{cols.CurrCode},
                                 {tbls.InvItm}.{cols.StockCode},
                                 {tbls.InvItm}.{cols.Qty},
                                 (({cols.NetAmt}/{cols.Qty})) AS  {salePriceCol},
                                 (({cols.FrgnNetAmt}/{cols.Qty})) AS {foreignSalePriceCol}
                              FROM 
                                 {tbls.Inv} 
                              JOIN 
                                 {tbls.InvItm}
                              ON 
                                 {tbls.Inv}.{cols.InvNum} = {tbls.InvItm}.{cols.InvNum}
                              WHERE 
                                 {tbls.Inv}.{cols.CusCode} = {customerCode}
                              AND 
                                 {tbls.InvItm}.{cols.StockCode} IN ('{string.Join("', '", productCodes)}') 
                              AND 
                                 {startDate}  <= {cols.InvDate}
                              ORDER BY 
                                 {cols.InvDate}  Desc, 
                                 {cols.StockCode}"
                              ;

         #endregion

         var salesMap = new Dictionary<string, Sale>();

         try
         {
            DataTable dataTable = FillTableFromODBC(queryString);

            if (dataTable.Rows.Count > 0)
            {
               var results = dataTable
                  .AsEnumerable()
                  .GroupBy(dr => dr[cols.StockCode])
                  .Select(a => a.OrderByDescending(r => r[cols.InvDate]).FirstOrDefault())
                  .Select(r => new
                  {
                     prodCode = r[cols.StockCode].ToString(),
                     invNum = r[cols.InvNum].ToString(),
                     date = r[cols.InvDate].ToString(),
                     salePrice = r[salePriceCol].ToString(),
                     foreignSalePrice = r[foreignSalePriceCol].ToString(),
                     currency = r[cols.CurrCode].ToString(),
                     qty = r[cols.Qty].ToString()
                  });


               foreach (var item in results)
               {
                  if (!DateTime.TryParse(item.date, out DateTime date))
                     ParseExMsg(cols.InvDate, item.invNum, item.prodCode, "1900/0/0");

                  if (!double.TryParse(item.salePrice, out double salePrice))
                     ParseExMsg(salePriceCol, item.invNum, item.prodCode, " 0. So is sale price");

                  if (!int.TryParse(item.qty, out int qty))
                     ParseExMsg("Quantity", item.invNum, item.prodCode, " 0. So is sale price");


                  //Check if it's a foreign customer.
                  int.TryParse(item.currency, out int currCode);
                  if (currCode != (int)sageUsrSet.baseCurrCode)
                  {
                     if (!double.TryParse(item.foreignSalePrice, out salePrice))
                     {
                        ParseExMsg(foreignSalePriceCol, item.invNum, item.prodCode, " 0. So is sale price");
                     }//If
                  }//If


                  salesMap[item.prodCode] = new Sale(date, salePrice, item.prodCode, qty);
               }//foreach


            }//If
         }
         catch (Exception e)
         {
            string eString = "Problem reading Last Price Data"
                              + "\r\n    -----------------     \r\n"
                              + e.GetType() + "\r\n" + e.Message
                              + "\r\n    -----------------     \r\n"
                              + "Query: " + queryString;
                              //+ "\r\nInvoice No.: " + invNum + ", Product: " + prodCode + "Customer : " + customerCode
                              //+ "\r\nSale Price : " + salePrice + "\r\n";
                              // server.TbInfo = eString;
            throw new Exception(eString);
         }//Catch


         return salesMap;

      }//readInvoiceData

      //-------------------------------------------------------------------------------------------------------//

      private void ParseExMsg(string val, string invNum, string prodCode, string defaultVal)
      {
         string exInfo = "Problem parsing " + val + " in invoice: " + invNum + ", product: " + prodCode
                         + "\n Date has been set to " + defaultVal + ".";
         //MessageBox.Show(exInfo);
         MyMessageBox.Show("Error", exInfo); ;
      }//parseExMsg

      //------------------------------------------------------------------------------------------------------------------//


   }//Cls
}//NS