﻿using PriceFinding.Managing_Data.ReaderInterfaces;
using PriceFinding.Models;
using PriceFinding.Properties;
using SageDataObject240;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PriceFinding
{
   class SDOInvoiceReader : IInvoiceReader
   {
      #region Variables

      private static UserSettings sageUsrSet = UserSettings.Default;

      private MyDictionary<Models.Customer> customerMap;
      private MyDictionary<Models.Product> productMap;

      private SDOEngine sdo;
      private WorkSpace ws;
      private InvoiceRecord invoiceRecord;
      private InvoiceItem invoiceItem;

      //The currency code is +1 on the sage50 interface so we need to compensate for it here.
      readonly int baseCurrCode = (int)(sageUsrSet.baseCurrCode - 1);
      #endregion

      //-------------------------------------------------------------------------------------------------------//

      public SDOInvoiceReader(MyDictionary<Models.Customer> customerMap, MyDictionary<Models.Product> productMap)
      {
         this.customerMap = customerMap;
         this.productMap = productMap;
      }//CTOR

      //-------------------------------------------------------------------------------------------------------//

      /// <summary>
      /// Reads all invoices and stores it's values in a List.
      /// </summary>
      /// <param name="customersFileName"></param>
      public MyDictionary<MyDictionary<List<Sale>>> ReadInvoiceData()
      {
         MyDictionary<MyDictionary<List<Sale>>> customerActivity = new MyDictionary<MyDictionary<List<Sale>>>(StringComparer.InvariantCultureIgnoreCase);

         ///Start date is lookBackYrs year ago.
         int lookBackYrs = (int)Settings.Default.invoiceLookBackYrs;
         DateTime startDate = DateTime.Now.AddYears(-lookBackYrs);
         int invNum = -1;
         string cusCode = String.Empty;
         string stockCode = String.Empty;
         double salePrice = -1;
         int currCode = -1;
         double xRate = -1;
         DateTime invDate = new DateTime();
         MyDictionary<List<Sale>> productActivity;

         try
         {
            sdo = new SDOEngine();
            //Try a connection, will throw an exception if it fails
            ws = (WorkSpace)sdo.Workspaces.Add("App Server Update");
            ws.Connect(sageUsrSet.sageDBDir, sageUsrSet.sageUsername, sageUsrSet.sagePassword, "UniqueUpdater");

            //Create instances of the objects
            invoiceRecord = (InvoiceRecord)ws.CreateObject("InvoiceRecord");
            invoiceItem = (InvoiceItem)ws.CreateObject("InvoiceItem");


            //Start at last Invoice
            invoiceRecord.MoveLast();
            do
            {
               //Invoice info
               invDate = (DateTime)SDOHelper.Read(invoiceRecord, "INVOICE_DATE");

               //Only read invoice details if it is recent enough.
               if (invDate >= startDate)
               {
                  invNum = (Int32)SDOHelper.Read(invoiceRecord, "INVOICE_NUMBER");
                  cusCode = (String)SDOHelper.Read(invoiceRecord, "ACCOUNT_REF");
                  currCode = (SByte)SDOHelper.Read(invoiceRecord, "CURRENCY");
                  xRate = (Double)SDOHelper.Read(invoiceRecord, "FOREIGN_RATE");


                  //Check for cusCode entry in customerActivity.
                  if (!customerActivity.ContainsKey(cusCode))
                  {
                     productActivity = new MyDictionary<List<Sale>>();
                     customerActivity[cusCode] = productActivity;
                  }
                  else
                  {
                     productActivity = customerActivity[cusCode];
                  }//Else


                  //Link Items to Record
                  invoiceItem = invoiceRecord.Link;

                  //Invoice Item info.
                  invoiceItem.MoveFirst();
                  do
                  {
                     stockCode = (String)SDOHelper.Read(invoiceItem, "STOCK_CODE");
                     double netAmount = (Double)SDOHelper.Read(invoiceItem, "NET_AMOUNT");
                     double qty = (Double)SDOHelper.Read(invoiceItem, "QTY_ORDER");

                     //if (currCode != baseCurrCode)
                     //{
                     //    salePrice = (netAmount * xRate) / qty;
                     //}
                     //else
                     //{
                     //    salePrice = netAmount / qty;
                     //}//Else

                     salePrice = netAmount / qty;


                     //If Customer or Product were not on lists then skip row.
                     if (!productMap.ContainsKey(stockCode) || !customerMap.ContainsKey(cusCode))
                        continue;


                     List<Sale> salesList;

                     //Check for stockCode entry in productActivity.
                     if (!productActivity.ContainsKey(stockCode))
                     {
                        salesList = new List<Sale>();
                        productActivity[stockCode] = salesList;
                     }
                     else
                     {
                        salesList = productActivity[stockCode];
                     }//Else


                     //Retrieve sale and add it to salesList.
                     Sale sale = new Sale(invDate, salePrice, stockCode);
                     salesList.Add(sale);
                     salesList.Sort();

                  } while (invoiceItem.MoveNext());

               }//If date

            } while (invoiceRecord.MovePrev());

         }
         catch (Exception e)
         {
            string eString = "Problem reading Invoice Data"
                              + "\r\n    -----------------     \r\n"
                              + e.GetType() + "\r\n" + e.Message
                              + "\r\n    -----------------     \r\n"
                              + "\r\nInvoice No.: " + invNum + ", Product: " + stockCode + ", Customer : " + cusCode
                              + "\r\nSale Price : " + salePrice + "\r\n";
            throw new Exception(eString);
         }
         finally
         {
            DestroyAllObjects();
         }//Finally

         return customerActivity;

      }//readInvoiceData

      //-------------------------------------------------------------------------------------------------------//

      /// <summary>
      /// Get Last price of the product that this customer bought.
      /// </summary>
      /// <param name="customerCode">Customer Code</param>
      /// <param name="productCode">Product Code</param>
      /// <returns></returns>
      public Sale GetLastPriceData(string customerCode, string productCode)
      {

         ///Start date is lookBackYrs year ago.
         int lookBackYrs = (int)Settings.Default.invoiceLookBackYrs;
         DateTime startDate = DateTime.Now.AddYears(-lookBackYrs);
         int invNum = -1;
         //string cusCode = String.Empty;
         double salePrice = -1;
         int currCode = -1;
         double xRate = -1;
         DateTime invDate = new DateTime();

         try
         {
            sdo = new SDOEngine();
            //Try a connection, will throw an exception if it fails
            ws = (WorkSpace)sdo.Workspaces.Add("App Server Update");
            ws.Disconnect();
            ws.Connect(sageUsrSet.sageDBDir, sageUsrSet.sageUsername, sageUsrSet.sagePassword, "UniqueUpdater");

            //Create instances of the objects
            invoiceRecord = (InvoiceRecord)ws.CreateObject("InvoiceRecord");
            invoiceItem = (InvoiceItem)ws.CreateObject("InvoiceItem");
            var cs = (string)SDOHelper.Read(invoiceRecord, "ACCOUNT_REF");

            SDOHelper.Write(invoiceRecord, "ACCOUNT_REF", customerCode);
            invoiceRecord.Find(false);
            invNum = (int)SDOHelper.Read(invoiceRecord, "INVOICE_NUMBER");
            //Start at last Invoice
            invoiceRecord.MoveLast();
            do
            {
               //Invoice info
               invDate = (DateTime)SDOHelper.Read(invoiceRecord, "INVOICE_DATE");
               cs =  (string)SDOHelper.Read(invoiceRecord, "ACCOUNT_REF");

               //Only read invoice details if it is recent enough.
               if (invDate < startDate)
                  break;

               invNum = (int)SDOHelper.Read(invoiceRecord, "INVOICE_NUMBER");
               currCode = (sbyte)SDOHelper.Read(invoiceRecord, "CURRENCY");
               xRate = (double)SDOHelper.Read(invoiceRecord, "FOREIGN_RATE");

               //Link Items to Record
               invoiceItem = invoiceRecord.Link;


               var lastPriceResult = invoiceItem.FindFirst("STOCK_CODE", productCode);

               if (lastPriceResult)
               {
                  double netAmount = (double)SDOHelper.Read(invoiceItem, "NET_AMOUNT");
                  double qty = (double)SDOHelper.Read(invoiceItem, "QTY_ORDER");
                  salePrice = netAmount / qty;
                  return new Sale(invDate, salePrice, productCode);
               }//If




            } while (invoiceRecord.MovePrev());

         }
         catch (Exception e)
         {
            string eString = "Problem reading Invoice Data"
                              + "\r\n    -----------------     \r\n"
                              + e.GetType() + "\r\n" + e.Message
                              + "\r\n    -----------------     \r\n"
                              + "\r\nInvoice No.: " + invNum + ", Product: " + productCode + ", Customer : " + customerCode
                              + "\r\nSale Price : " + salePrice + "\r\n";
            throw new Exception(eString);
         }
         finally
         {
            DestroyAllObjects();
         }//Finally

         return null;
         
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

      /// <summary>
      /// Make sure all objects are released for garbage collection.
      /// </summary>
      public void DestroyAllObjects()
      {
         if (invoiceRecord != null)
         {
            Marshal.FinalReleaseComObject(invoiceRecord);
            invoiceRecord = null;
         }//If
         if (invoiceItem != null)
         {
            Marshal.FinalReleaseComObject(invoiceItem);
            invoiceItem = null;
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

      public Dictionary<string, Sale> GetLastPriceData(string customerCode, IEnumerable<string> productCodes)
      {
         throw new NotImplementedException();
      }
   }//Cls
}//NS