using PriceFinding.Managing_Data.ReaderInterfaces;
using PriceFinding.Utility.Sage;
using System;
using System.Collections.Generic;
using System.Data;

namespace PriceFinding.Managing_Data.ODBC_Readers
{
   class ODBCPriceListReader : AODBCReader, IPriceListReader
   {
      #region Variables

      private SageTables tbls = new SageTables();
      private SageColumns cols = new SageColumns();


      #endregion

      //-------------------------------------------------------------------------------------------------------//

      public ODBCPriceListReader()
      {
      }//CTOR

      //-------------------------------------------------------------------------------------------------------//

      /// <summary>
      /// Reads all invoices and stores it's values in a List.
      /// </summary>
      /// <param name="customersFileName"></param>
      public double GetPriceListPrice(string cusCode, string prodCode)
      {
         var notFound = -1;

         #region queryStringPLNames
         string queryStringPLNames = $@"SELECT 
                                       {tbls.Cust}.{cols.PrcRef},
                                       {tbls.Cust}.{cols.CusCode},
                                     FROM 
                                       {tbls.Cust}
                                     WHERE 
                                       {tbls.Cust}.{cols.CusCode} = '{cusCode}'
                                     ORDER BY 
                                       {cols.PrcRef}"
                                     ;
         #endregion

         string cusPriceListRef = string.Empty;


         try
         {
            DataTable dataTable = FillTableFromODBC(queryStringPLNames);
            if (dataTable.Rows.Count < 1)
               return notFound;

            cusPriceListRef = dataTable.Rows[0][cols.PrcRef].ToString();

            #region queryStringPLData
            string queryStringPLData = $@"SELECT 
                                     {tbls.Prc}.{cols.PrcRef}, 
                                     {tbls.Prc}.{cols.StockCode}, 
                                     {tbls.Prc}.{cols.CalcMeth}, 
                                     {tbls.Prc}.{cols.CalcVal}, 
                                     {tbls.Curr}.{cols.XRate}, 
                                     {tbls.PrcLst}.{cols.CurrCode}, 
                                     {tbls.Stock}.{cols.LstPurchPrice}, 
                                     {tbls.Stock}.{cols.SalePrc} 
                                     FROM 
                                       (
                                          (
                                             (
                                                 {tbls.PrcLst}
                                               INNER JOIN
                                                 {tbls.Prc}
                                               ON 
                                                 {tbls.Prc}.{cols.PrcRef} = {tbls.PrcLst}.{cols.PrcRef}
                                             ) 
                                             INNER JOIN
                                                 {tbls.Curr}
                                             ON 
                                                 {tbls.PrcLst}.{cols.CurrCode} =  {tbls.Curr}.{cols.CurrNum}
                                         ) 
                                         INNER JOIN 
                                             {tbls.Stock}
                                         ON 
                                             {tbls.Stock}.{cols.StockCode} =  {tbls.Prc}.{cols.StockCode}
                                     )
                                     WHERE 
                                        {tbls.Prc}.{cols.PrcRef} = '{cusPriceListRef}'
                                     AND 
                                        {tbls.Prc}.{cols.StockCode} = '{prodCode}'"
                                 ;
            #endregion

            dataTable = FillTableFromODBC(queryStringPLData);

            if (dataTable.Rows.Count < 1)
               return notFound;


            var row = dataTable.Rows[0];


            string plName = row[cols.PrcRef].ToString().Trim();
            string stockCode = row[cols.StockCode].ToString();
            double calcValue = (double)row[cols.CalcVal];
            int calcMeth = (int)row[cols.CalcMeth];
            double xRate = (double)row[cols.XRate];
            double costPrice = (double)row[cols.LstPurchPrice];
            double salePrice = (double)row[cols.SalePrc];

            return CalculateListPrice(calcValue, calcMeth, costPrice, salePrice, xRate);

         }
         catch (Exception e)
         {
            string eString = "Problem reading PriceList Data"
                              + "\r\n    -----------------     \r\n"
                              + e.GetType() + "\r\n" + e.Message
                              + "\r\n    -----------------     \r\n"
                              + "Query: " + queryStringPLNames
                              + "\r\n Pricing Ref: " + cusPriceListRef + "Customer : " + cusCode + "\r\n";

            throw new Exception(eString);
         }//Catch


      }//GetPriceListPrice

      //-------------------------------------------------------------------------------------------------------//


      public MyDictionary<double> GetPriceListPrices(string cusCode, IEnumerable<string> prodCodes)
      {

         var results = new MyDictionary<double>();


         #region queryStringPLNames
         string queryStringPLNames = $@"SELECT 
                                       {tbls.Cust}.{cols.PrcRef},
                                       {tbls.Cust}.{cols.CusCode},
                                     FROM 
                                       {tbls.Cust}
                                     WHERE 
                                       {tbls.Cust}.{cols.CusCode} = '{cusCode}'
                                     ORDER BY 
                                       {cols.PrcRef}"
                                     ;
         #endregion

         string cusPriceListRef = string.Empty;


         try
         {
            //First find the PriceList name.
            DataTable dataTable = FillTableFromODBC(queryStringPLNames);
            if (dataTable.Rows.Count < 1)
               return results;

            cusPriceListRef = dataTable.Rows[0][cols.PrcRef].ToString();

            #region queryStringPLData
            string queryStringPLData = $@"SELECT 
                                     {tbls.Prc}.{cols.PrcRef}, 
                                     {tbls.Prc}.{cols.StockCode}, 
                                     {tbls.Prc}.{cols.CalcMeth}, 
                                     {tbls.Prc}.{cols.CalcVal}, 
                                     {tbls.Curr}.{cols.XRate}, 
                                     {tbls.PrcLst}.{cols.CurrCode}, 
                                     {tbls.Stock}.{cols.LstPurchPrice}, 
                                     {tbls.Stock}.{cols.SalePrc} 
                                     FROM 
                                       (
                                          (
                                             (
                                                 {tbls.PrcLst}
                                               INNER JOIN
                                                 {tbls.Prc}
                                               ON 
                                                 {tbls.Prc}.{cols.PrcRef} = {tbls.PrcLst}.{cols.PrcRef}
                                             ) 
                                             INNER JOIN
                                                 {tbls.Curr}
                                             ON 
                                                 {tbls.PrcLst}.{cols.CurrCode} =  {tbls.Curr}.{cols.CurrNum}
                                         ) 
                                         INNER JOIN 
                                             {tbls.Stock}
                                         ON 
                                             {tbls.Stock}.{cols.StockCode} =  {tbls.Prc}.{cols.StockCode}
                                     )
                                     WHERE 
                                        {tbls.Prc}.{cols.PrcRef} = '{cusPriceListRef}'
                                     AND 
                                        {tbls.Prc}.{cols.StockCode} IN ('{string.Join("', '", prodCodes)}')"
                                 ;
            #endregion

            dataTable = FillTableFromODBC(queryStringPLData);

            if (dataTable.Rows.Count < 1)
               return results;

            //Fill in the price list prices.
            foreach (DataRow row in dataTable.Rows)
            {
               string plName = row[cols.PrcRef].ToString().Trim();
               string stockCode = row[cols.StockCode].ToString();
               double calcValue = (double)row[cols.CalcVal];
               int calcMeth = (int)row[cols.CalcMeth];
               double xRate = (double)row[cols.XRate];
               double costPrice = (double)row[cols.LstPurchPrice];
               double salePrice = (double)row[cols.SalePrc];

               results[stockCode] = CalculateListPrice(calcValue, calcMeth, costPrice, salePrice, xRate);

            }//foreach

            return results;

         }
         catch (Exception e)
         {
            string eString = "Problem reading PriceList Data"
                              + "\r\n    -----------------     \r\n"
                              + e.GetType() + "\r\n" + e.Message
                              + "\r\n    -----------------     \r\n"
                              + "Query: " + queryStringPLNames
                              + "\r\n Pricing Ref: " + cusPriceListRef + "Customer : " + cusCode + "\r\n";

            throw new Exception(eString);
         }//Catch
      }//GetPriceListPrices

      //-------------------------------------------------------------------------------------------------------//

      private double CalculateListPrice(double calcValue, int calcMeth, double costPrice, double salePrice, double xRate)
      {
         double listPrice = 0;

         switch (calcMeth)
         {
            case (int)ODBCPriceListTypes.plFixed:
               //Fixed Price
               listPrice = calcValue;
               break;

            case (int)ODBCPriceListTypes.plStandard:
               //Standard Sale Price                       
               listPrice = salePrice;
               break;

            case (int)ODBCPriceListTypes.plDecreasePercent:
               //SalePrice - %
               listPrice = salePrice * (1 - calcValue / 100);
               break;

            case (int)ODBCPriceListTypes.plDecreaseValue:
               //SalePrice - Value
               listPrice = salePrice - calcValue;
               break;

            case (int)ODBCPriceListTypes.plMarkupCost:
               //CostPrice + %
               listPrice = costPrice * (1 + calcValue / 100);
               break;

            case (int)ODBCPriceListTypes.plMarkupValue:
               //CostPrice + Value
               listPrice = costPrice + calcValue;
               break;

            case (int)ODBCPriceListTypes.plMarkupSales:
               //SalePrice + %
               listPrice = salePrice + calcValue;
               break;
         }//Switch

         return listPrice * xRate;


      }//CalculateListPrice

      //-------------------------------------------------------------------------------------------------------//

      private void ParseExMsg(string val, string invNum, string prodCode, string defaultVal)
      {
         string exInfo = "Problem parsing " + val + " in invoice: " + invNum + ", product: " + prodCode
                         + "\n Date has been set to " + defaultVal + ".";

         throw new Exception(exInfo);
      }//ParseExMsg

      //-------------------------------------------------------------------------------------------------------//


   }//Cls
}//NS