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
        private Int16 taxCode;

        private string username = string.Empty;
        private double defTaxRate;
        private int currencyCode;
        private readonly int baseCurrencyCode = (int)setUsr.baseCurrCode;

        private double cusDiscountRate = 0;

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
                string cusCode = order.customer.Code;
                string cusPO = order.customer.PoNumber;
                string address = order.customer.Address;
                OrderType type = order.type;
                List<Models.Product> productList = order.productList;


                //Create a saleRecord with comboAccRef.Text and look for it in database.
                SDOHelper.Write(salesRecord, "ACCOUNT_REF", cusCode);

                if (salesRecord.Find(false))
                {

                    //Check Account Status here.
                    short accStatus = (short)SDOHelper.Read(salesRecord, "ACCOUNT_STATUS");
                    if (accStatus != 0)
                        throw new MyException("405 Customer " + order.customer.Code + "'s account is on hold.");

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


                    //Add each DELIVERY address line to header if using one.
                    if (!string.IsNullOrWhiteSpace(address) && !address.Equals(Models.Customer.DEF_ADDRESS, StringComparison.InvariantCultureIgnoreCase))
                    {
                        // Split the address into separate lines.
                        String[] addressLines = address.Split(new string[] { "\n", "\r", "\f" }, StringSplitOptions.RemoveEmptyEntries);
                        int minLines = Math.Min(5, addressLines.Length);
                        for (int i = 0; i < minLines; i++)
                        {
                            string addressLine = RestrictLength(addressLines[i]);
                            SDOHelper.Write(sopPost.Header, "DEL_ADDRESS_" + (i + 1), addressLine);
                        }//For                          
                    }//If


                    //Add date and customer O/N to header
                    SDOHelper.Write(sopPost.Header, "ORDER_DATE", DateTime.Now);
                    SDOHelper.Write(sopPost.Header, "CUST_ORDER_NUMBER", cusPO);
                    SDOHelper.Write(sopPost.Header, "CARR_NOM_CODE", ((int)setUsr.carrNomCode).ToString());

                    //Check if we are entering an order for a foreign customer.
                    if (currencyCode != baseCurrencyCode)
                    {
                        taxCode = (short)setUsr.taxCodeForeign;
                        SDOHelper.Write(sopPost.Header, "CARR_TAX_CODE", (Int16)taxCode);

                        currData.Read(currencyCode);
                        //Populate Foreign Currency Fields
                        SDOHelper.Write(sopPost.Header, "FOREIGN_RATE", SDOHelper.Read(currData, "EXCHANGE_RATE"));
                        SDOHelper.Write(sopPost.Header, "CURRENCY", SDOHelper.Read(salesRecord, "CURRENCY"));
                        SDOHelper.Write(sopPost.Header, "CURRENCY_USED", SDOHelper.Read(salesRecord, "CURRENCY"));
                    }
                    else
                    {
                        taxCode = (short)setUsr.taxCode;
                        SDOHelper.Write(sopPost.Header, "CARR_TAX_CODE", (Int16)taxCode);
                    }//Else


                    //Check if its a quote or not
                    if (type == OrderType.QUOTE)
                    {
                        // Populate details to generate quote
                        SDOHelper.Write(sopPost.Header, "ORDER_TYPE", (Byte)InvoiceType.sdoSopQuote);
                        SDOHelper.Write(sopPost.Header, "QUOTE_STATUS", (Byte)QuoteStatus.sdoOpen);
                    }//If


                    //Add discount rate (usually 0).
                    cusDiscountRate = (double)SDOHelper.Read(salesRecord, "DISCOUNT_RATE");


                    //Add each product to sopPost items section.
                    foreach (Models.Product product in productList)
                    {
                        AddItem(product, ws);
                    }//ForEach

                    //Add username
                    SDOHelper.Write(sopPost.Header, "TAKEN_BY", username);

                    //Update: will fail if not set up properly
                    if (!sopPost.Update())
                        return false;
                    else
                        return true;
                }
                else
                {
                    throw new MyException("Customer " + order.customer.Code + " does not seem to exist.");
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
                SDOHelper.Write(sopItem, "TAX_CODE", (Int16)taxCode);
            }
            else
            {
                throw new MyException("405 Stock: " + product.Code + " does not seem to exist.");
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
        private string RestrictLength(String line)
        {
         
            int min = Math.Min(line.Length, 60);
            return line.Substring(0, min);
        }//ExInfo


    }//Cls
}//NS
