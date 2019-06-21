using PriceFinding.Models;
using PriceFinding.Properties;
using SageDataObject240;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace PriceFinding.Writing
{
   class PostOrderWriter
   {
      #region Variables
      private SDOEngine sdo;
      public WorkSpace ws;
      private SalesRecord salesRecord;
      private SopPost sopPost;
      private SopItem sopItem;
      private StockRecord stockRecord;
      private ControlData conData;
      private CurrencyData currData;
      private Order order;
      private readonly FileStream logWriter;

      private static UserSettings setUsr = UserSettings.Default;
      private readonly Settings set = Settings.Default;

      private const string crlf = "\r\n";
      private short taxCode;

      private readonly string username = string.Empty;
      private double defTaxRate;
      private int currencyCode;
      private readonly int baseCurrencyCode = (int)setUsr.baseCurrCode;

      private double cusDiscountRate = 0;

      private const int MAX_LENGTH_NOTE = 60;

      #endregion

      //----------------------------------------------------------------------------------------------//

      public PostOrderWriter(Order order, string username)
      {
         this.order = order;
         this.username = username;

      }//CTOR

      //----------------------------------------------------------------------------------------------//

      public bool Post()
      {
         try
         {
            //Connect and create.
            sdo = new SDOEngine();
            ws = sdo.Workspaces.Add("App Server");
            ws.Connect(setUsr.sageDBDir, setUsr.sageUsername, setUsr.sagePassword, "Unique");

            salesRecord = (SalesRecord)ws.CreateObject("SalesRecord");
            conData = (ControlData)ws.CreateObject("ControlData");
            currData = (CurrencyData)ws.CreateObject("CurrencyData");
            sopPost = (SopPost)ws.CreateObject("SOPPost");

            //Unwrap order.
            string cusCode = order.Customer.Code;
            OrderType type = order.type;
            List<Models.Product> productList = order.ProductList;


            //Create a saleRecord with comboAccRef.Text and look for it in database.
            SDOHelper.Write(salesRecord, "ACCOUNT_REF", cusCode);

            if (salesRecord.Find(false))
            {

               //Check Account Status here.
               short accStatus = (short)SDOHelper.Read(salesRecord, "ACCOUNT_STATUS");
               if (accStatus != 0)
                  throw new MyException("Customer " + order.Customer.Code + "'s account is on hold.");

               short defTaxCode = (short)SDOHelper.Read(salesRecord, "DEF_TAX_CODE");
               string taxRateField = "T" + defTaxCode + "_Rate";
               defTaxRate = (double)SDOHelper.Read(conData, taxRateField);

               currencyCode = (sbyte)SDOHelper.Read(salesRecord, "CURRENCY") + 1;


               //If customer exists add details to sopPostHeader.
               SDOHelper.Write(sopPost.Header, "ACCOUNT_REF", SDOHelper.Read(salesRecord, "ACCOUNT_REF"));
               SDOHelper.Write(sopPost.Header, "NAME", SDOHelper.Read(salesRecord, "NAME"));


               //Add each address line to header
               for (int i = 1; i <= 5; i++)
               {
                  SDOHelper.Write(sopPost.Header, "ADDRESS_" + i, SDOHelper.Read(salesRecord, "ADDRESS_" + i));
               }//For



               if (order.DeliveryAddress != null)
               {
                  var deliveryAddress = order.DeliveryAddress;

                  SDOHelper.Write(sopPost.Header, "DEL_ADDRESS_1", RestrictLength(deliveryAddress.Line1));
                  SDOHelper.Write(sopPost.Header, "DEL_ADDRESS_2", RestrictLength(deliveryAddress.Line2));
                  SDOHelper.Write(sopPost.Header, "DEL_ADDRESS_3", RestrictLength(deliveryAddress.Line3));
                  SDOHelper.Write(sopPost.Header, "DEL_ADDRESS_4", RestrictLength(deliveryAddress.Line4));
                  SDOHelper.Write(sopPost.Header, "DEL_ADDRESS_5", RestrictLength(deliveryAddress.Line5));
               }//If



               //Add date and customer O/N to header
               SDOHelper.Write(sopPost.Header, "ORDER_DATE", DateTime.Now);
               SDOHelper.Write(sopPost.Header, "CUST_ORDER_NUMBER", order.CustomerOrderNumber);
               SDOHelper.Write(sopPost.Header, "CARR_NOM_CODE", ((int)setUsr.carrNomCode).ToString());
               SDOHelper.Write(sopPost.Header, "CARR_NET", order.Carriage);

               //Check if we are entering an order for a foreign customer.
               if (currencyCode != baseCurrencyCode)
               {
                  taxCode = (short)setUsr.taxCodeForeign;
                  SDOHelper.Write(sopPost.Header, "CARR_TAX_CODE", (short)taxCode);

                  currData.Read(currencyCode);
                  //Populate Foreign Currency Fields
                  SDOHelper.Write(sopPost.Header, "FOREIGN_RATE", SDOHelper.Read(currData, "EXCHANGE_RATE"));
                  SDOHelper.Write(sopPost.Header, "CURRENCY", SDOHelper.Read(salesRecord, "CURRENCY"));
                  SDOHelper.Write(sopPost.Header, "CURRENCY_USED", SDOHelper.Read(salesRecord, "CURRENCY"));
               }
               else
               {
                  taxCode = (short)setUsr.taxCode;
                  SDOHelper.Write(sopPost.Header, "CARR_TAX_CODE", (short)taxCode);
               }//Else


               //Check if its a quote or not
               if (type == OrderType.QUOTE)
               {
                  // Populate details to generate quote
                  SDOHelper.Write(sopPost.Header, "ORDER_TYPE", (byte)InvoiceType.sdoSopQuote);
                  SDOHelper.Write(sopPost.Header, "QUOTE_STATUS", (byte)QuoteStatus.sdoOpen);
               }//If

               //Any notes
               var notes = order.Notes;
               if (!string.IsNullOrWhiteSpace(notes))
               {
                  //Split the note up if it's too long
                  SDOHelper.Write(sopPost.Header, "NOTES_1", RestrictLength(order.Notes.Substring(0, MAX_LENGTH_NOTE)));

                  if (notes.Length > MAX_LENGTH_NOTE)
                     SDOHelper.Write(sopPost.Header, "NOTES_2", RestrictLength(order.Notes.Substring(MAX_LENGTH_NOTE, 2 * MAX_LENGTH_NOTE)));

                  if (notes.Length > 2 * MAX_LENGTH_NOTE)
                     SDOHelper.Write(sopPost.Header, "NOTES_3", RestrictLength(order.Notes.Substring(2 * MAX_LENGTH_NOTE, 3 * MAX_LENGTH_NOTE)));

               }

               //Add discount rate (usually 0).
               cusDiscountRate = (double)SDOHelper.Read(salesRecord, "DISCOUNT_RATE");


               //Add each product to sopPost items section.
               foreach (Models.Product product in productList)
               {
                  AddItem(product, ws);
               }//ForEach

               //Add username
               SDOHelper.Write(sopPost.Header, "TAKEN_BY", GetTakenBy());

               //Update: will fail if not set up properly
               if (!sopPost.Update())
                  return false;
               else
                  return true;
            }
            else
            {
               throw new MyException("Customer " + order.Customer.Code + " does not seem to exist.");
            }//Else
         }
         catch (MyException mE)
         {
            throw new MyException(mE.Message);
         }
         catch (Exception e)
         {
            throw new MyException("Problem posting order to database " + e.GetType().Name + ":" + e.Message);
         }
         finally
         {
            DestroyAllObjects();
         }//Finally

      }//Post

      //----------------------------------------------------------------------------------------------//

      private string GetTakenBy()
      {
         return order.TakenBy ?? username;
      }//GetTakenBy

      //----------------------------------------------------------------------------------------------//

      private void AddItem(Models.Product product, WorkSpace ws)
      {

         stockRecord = (StockRecord)ws.CreateObject("StockRecord");

         //Create a stockRecord with lvi.Text and look for it in database.
         SDOHelper.Write(stockRecord, "STOCK_CODE", product.Code);

         if (stockRecord.Find(false))
         {
            //Add item to the order.
            sopItem = SDOHelper.Add(sopPost.Items);

            //Put product details and prices into sopItem
            SDOHelper.Write(sopItem, "STOCK_CODE", SDOHelper.Read(stockRecord, "STOCK_CODE"));
            SDOHelper.Write(sopItem, "DESCRIPTION", SDOHelper.Read(stockRecord, "DESCRIPTION"));


            double qty = product.Qty;
            double salePrc = product.SalePrice;
            double netAmt = salePrc * qty * (1 - cusDiscountRate / 100);
            double taxAmt = netAmt * defTaxRate / 100;

            SDOHelper.Write(sopItem, "QTY_ORDER", qty);
            SDOHelper.Write(sopItem, "UNIT_PRICE", salePrc);
            SDOHelper.Write(sopItem, "NET_AMOUNT", netAmt);
            SDOHelper.Write(sopItem, "TAX_AMOUNT", taxAmt);
            SDOHelper.Write(sopItem, "TAX_RATE", defTaxRate);
            SDOHelper.Write(sopItem, "NOMINAL_CODE", ((int)setUsr.stockNomCode).ToString());
            SDOHelper.Write(sopItem, "TAX_CODE", (short)taxCode);
         }
         else
         {
            throw new MyException("Stock: " + product.Code + " does not seem to exist.");
         }//Else


      }//AddItem

      //----------------------------------------------------------------------------------------------//

      private void DestroyAllObjects()
      {

         if (conData != null)
         {
            Marshal.FinalReleaseComObject(conData);
            conData = null;
         }//If
         if (currData != null)
         {
            Marshal.FinalReleaseComObject(currData);
            currData = null;
         }//If
         if (sopPost != null)
         {
            Marshal.FinalReleaseComObject(sopPost);
            sopPost = null;
         }//If
         if (sopItem != null)
         {
            Marshal.FinalReleaseComObject(sopItem);
            sopItem = null;
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

      //----------------------------------------------------------------------------------------------//

      /// <summary>
      /// Make sure string has less than 61 chars.
      /// </summary>
      /// <param name="e"></param>
      /// <returns></returns>
      private string RestrictLength(string line)
      {
         if (String.IsNullOrWhiteSpace(line))
            return "";

         int min = Math.Min(line.Length, 60);
         return line.Substring(0, min);
      }//ExInfo


   }//Cls
}//NS
