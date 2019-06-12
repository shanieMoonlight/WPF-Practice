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

      private readonly MyDictionary<Customer> customerMap;
      private MyDictionary<Product> productMap;

      #endregion

      //-------------------------------------------------------------------------------------------------------//

      public ODBCPriceListReader(MyDictionary<Customer> customerMap, MyDictionary<Product> productMap)
      {
         this.customerMap = customerMap;
         this.productMap = productMap;
      }//CTOR

      //-------------------------------------------------------------------------------------------------------//

      /// <summary>
      /// Reads all invoices and stores it's values in a List.
      /// </summary>
      /// <param name="customersFileName"></param>
      public MyDictionary<MyDictionary<double>> ReadPriceListData()
      {
         //
         MyDictionary<MyDictionary<double>> PriceListActivity = new MyDictionary<MyDictionary<double>>();
         //Dictionary of PriceList Name vs PriceList Data.
         MyDictionary<MyDictionary<double>> miniPLActivity = new MyDictionary<MyDictionary<double>>();
         //Dictionary of PriceList Name vs PriceListUserList.
         MyDictionary<List<string>> plUsers = new MyDictionary<List<string>>();

         #region queryStringPLNames
         string queryStringPLNames = $@"SELECT 
                                       {tbls.Cust}.{cols.PrcRef},
                                       {tbls.Cust}.{cols.CusCode},
                                     FROM 
                                       {tbls.Cust}
                                     WHERE 
                                       {cols.PrcRef} <> {string.Empty}
                                     ORDER BY 
                                       {cols.PrcRef}"
                                     ;
         #endregion

         string cusPriceListRef = string.Empty;
         string cusCode = string.Empty;

         //Create a dictionary of PL's that are acually being referenced v's Empty ProductActivities.
         //Single Price Lists can be used for multiple Customers
         try
         {
            DataTable dataTable = FillTableFromODBC(queryStringPLNames);

            foreach (DataRow row in dataTable.Rows)
            {
               cusPriceListRef = row[cols.PrcRef].ToString();
               cusCode = row[cols.CusCode].ToString();

               //If pLists already contains PL then add customer to it's entry. Else make new entry if not empty string.
               if (plUsers.ContainsKey(cusPriceListRef))
               {
                  plUsers[cusPriceListRef].Add(cusCode);
               }
               else if (!cusPriceListRef.Equals(string.Empty))
               {
                  plUsers[cusPriceListRef] = new List<string>
                  {
                     cusCode
                  };

                  //Add new entry for each priceList.
                  miniPLActivity[cusPriceListRef] = new MyDictionary<double>();
               }//Else

            }//ForEach


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
                                     ORDER BY 
                                        {cols.PrcRef}"
                                 ;
            #endregion

            dataTable = FillTableFromODBC(queryStringPLData);

            Dictionary<string, double> productActivity = null;
            string plName = string.Empty;
            string plNamePrev = string.Empty;
            string stockCode;
            double calcValue;
            int calcMeth;
            double xRate;
            double costPrice;
            double salePrice;
            double listPrice;

            foreach (DataRow row in dataTable.Rows)
            {
               plName = row[cols.PrcRef].ToString().Trim();
               stockCode = row[cols.StockCode].ToString();
               calcValue = (double)row[cols.CalcVal];
               calcMeth = (int)row[cols.CalcMeth];
               xRate = (double)row[cols.XRate];
               costPrice = (double)row[cols.LstPurchPrice];
               salePrice = (double)row[cols.SalePrc];

               if (plName.Equals(plNamePrev))
               {
                  //Old plName: Check stock, if found get it's list price & add to productActivity from PLACtivity.
                  if (productMap.ContainsKey(stockCode))
                  {
                     listPrice = CalculateListPrice(calcValue, calcMeth, costPrice, salePrice, xRate);
                     productActivity[stockCode] = listPrice;
                  }//If
               }
               else if (miniPLActivity.ContainsKey(plName))
               {
                  //New name:Check stock, if found get it's list price & add to productActivity from PLACtivity. 
                  //Change plNamePrev
                  //Get new currency, keep till next Price List.
                  if (productMap.ContainsKey(stockCode))
                  {
                     listPrice = CalculateListPrice(calcValue, calcMeth, costPrice, salePrice, xRate);
                     productActivity = miniPLActivity[plName];
                     productActivity[stockCode] = listPrice;
                     plNamePrev = plName;
                  }//If
               }//Else
            }//ForEach


            //Give each customer their own PriceList.
            foreach (string key in plUsers.Keys)
            {
               foreach (string plUserCusCode in plUsers[key])
               {
                  PriceListActivity[plUserCusCode] = miniPLActivity[key];
               }//ForEach
            }//ForEach  
         }
         catch (Exception e)
         {
            string eString = "Problem reading PriceList Data"
                              + "\r\n    -----------------     \r\n"
                              + e.GetType() + "\r\n" + e.Message
                              + "\r\n    -----------------     \r\n"
                              + "Query: " + queryStringPLNames
                              + "\r\n Pricing Ref: " + cusPriceListRef + "Customer : " + cusCode + "\r\n";
            // server.TbInfo = eString;
            throw new Exception(eString);
         }//Catch


         return PriceListActivity;

      }//ReadPriceListData

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
         //MessageBox.Show(exInfo);
         MyMessageBox.Show("Error", exInfo);
      }//ParseExMsg

      //-------------------------------------------------------------------------------------------------------//


   }//Cls
}//NS