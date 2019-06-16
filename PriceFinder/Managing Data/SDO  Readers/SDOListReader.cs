using PriceFinding.Managing_Data.ReaderInterfaces;
using PriceFinding.Properties;
using SageDataObject240;
using System;
using System.Runtime.InteropServices;

namespace PriceFinding
{
   /// <summary>
   /// Reads Customer/Product List from database and puts them in dictionaries.
   /// </summary>
   class SDOListReader : IListReader
   {
      #region Fields       
      private static UserSettings sageUsrSet = UserSettings.Default;

      //Declare Sage Objects
      private SDOEngine sdo;
      private WorkSpace ws;
      private SalesRecord salesRecord;
      private StockRecord stockRecord;
      private CurrencyData currencyData;

      private readonly double baseCurrencyCode = (double)sageUsrSet.baseCurrCode;
      #endregion

      //-------------------------------------------------------------------------------------------------------//

      /// <summary>
      /// Reads all customers and stores it's values in a List.
      /// </summary>
      /// <param name="customersFileName"></param>
      public MyDictionary<Models.Customer> ReadCustomerData()
      {
         MyDictionary<Models.Customer> customerMap = new MyDictionary<Models.Customer>();

         try
         {
            sdo = new SDOEngine();
            ws = (WorkSpace)sdo.Workspaces.Add("App Server Update");
            ws.Connect(sageUsrSet.sageDBDir, sageUsrSet.sageUsername, sageUsrSet.sagePassword, "UniqueUpdater");
            salesRecord = (SalesRecord)ws.CreateObject("SalesRecord");
            currencyData = (CurrencyData)ws.CreateObject("CurrencyData");

            string code = string.Empty;
            string desc = string.Empty;

            salesRecord.MoveFirst();
            do
            {
               code = CutQuotes((string)SDOHelper.Read(salesRecord, "ACCOUNT_REF"));
               desc = CutQuotes((string)SDOHelper.Read(salesRecord, "NAME"));

               //Create customer
               Models.Customer customer = new Models.Customer(code, desc);

               //Check if it's a foreign customer. 
               //If it is add XRate to customer.
               //Add 1 to salesrecord.CURRENCY to get the equivilant currencyData record.
               int currCode = (sbyte)SDOHelper.Read(salesRecord, "CURRENCY") + 1;

               if (currCode != baseCurrencyCode)
               {
                  currencyData.Read(currCode);
                  customer.XRate = (double)SDOHelper.Read(currencyData, "EXCHANGE_RATE");
               }//If

               customerMap.Add(code, customer);

            } while (salesRecord.MoveNext());
         }
         finally
         {
            DestroyAllObjects();
         }//finally

         return customerMap;

      }//readCustomerData

      //-------------------------------------------------------------------------------------------------------//

      /// <summary>
      /// Reads all products and stores it's values in a List.
      /// </summary>
      /// <param name="customersFileName"></param>
      public MyDictionary<Models.Product> ReadProductData()
      {
         MyDictionary<Models.Product> productMap = new MyDictionary<Models.Product>();
         //MyDictionary<MyDictionary<Product>> fastProductList = new MyDictionary<MyDictionary<Product>>();

         try
         {
            sdo = new SDOEngine();
            ws = (WorkSpace)sdo.Workspaces.Add("App Server Update");
            ws.Connect(sageUsrSet.sageDBDir, sageUsrSet.sageUsername, sageUsrSet.sagePassword, "UniqueUpdater");
            stockRecord = (StockRecord)ws.CreateObject("StockRecord");

            string code = string.Empty;
            string desc = string.Empty;
            double costPrice = 0;
            double salePrice = 0;

            stockRecord.MoveFirst();
            do
            {
               try
               {
                  code = CutQuotes((string)SDOHelper.Read(stockRecord, "STOCK_CODE"));
                  desc = (string)SDOHelper.Read(stockRecord, "DESCRIPTION");
                  costPrice = (double)SDOHelper.Read(stockRecord, "LAST_PURCHASE_PRICE");
                  salePrice = (double)SDOHelper.Read(stockRecord, "SALES_PRICE");

                  Models.Product product = new Models.Product(code, desc, costPrice);
                  productMap.Add(code, product);

               }
               catch (Exception)
               {
                  //Just skip product if it throws an exception
               }//Catch
            } while (stockRecord.MoveNext());
         }
         finally
         {
            DestroyAllObjects();
         }//finally

         return productMap;

      }//readProductData

      //-------------------------------------------------------------------------------------------------------//

      /// <summary>
      /// Trims and cuts any quotation marks from the ends of a string
      /// </summary>
      /// <param name="str"></param>
      /// <returns></returns>
      private string CutQuotes(string str)
      {
         try
         {
            string strTrim = str.Trim();
            if (strTrim[0] == '\"' && strTrim[strTrim.Length - 1] == '\"')
               return strTrim.Substring(1, strTrim.Length - 1);

            return strTrim;

         }
         catch (ArgumentOutOfRangeException)
         {
            return "";
         }
         catch (IndexOutOfRangeException)
         {
            return "";
         }//Catch
      }// cutQuotes

      //-------------------------------------------------------------------------------------------------------//

      /// <summary>
      /// Make sure all objects are released for garbage collection.
      /// </summary>
      public void DestroyAllObjects()
      {
         if (salesRecord != null)
         {
            Marshal.FinalReleaseComObject(salesRecord);
            salesRecord = null;
         }//If
         if (currencyData != null)
         {
            Marshal.FinalReleaseComObject(currencyData);
            currencyData = null;
         }//If
         if (stockRecord != null)
         {
            Marshal.FinalReleaseComObject(stockRecord);
            stockRecord = null;
         }//If

         if (ws != null)
         {
            ws.Disconnect();
            Marshal.FinalReleaseComObject(ws);
            ws = null;
         }//If

         if (sdo != null)
         {
            Marshal.FinalReleaseComObject(sdo);
            sdo = null;
         }//If

      }//DestroyAllObjects

      //-------------------------------------------------------------------------------------------------------//


   }//Cls
}//NS