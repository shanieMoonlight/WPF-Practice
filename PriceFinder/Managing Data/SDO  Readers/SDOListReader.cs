using PriceFinding.Properties;
using SageDataObject240;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PriceFinding
{ 
    /// <summary>
    /// Reads Customer/Product List from database and puts them in dictionaries.
    /// </summary>
    class SDOListReader : IListReader
    {
        #region Fields       
        private static SageUserSettings sageUsrSet = SageUserSettings.Default;

        //Declare Sage Objects
        private SDOEngine sdo;
        private WorkSpace ws;
        private SalesRecord salesRecord;
        private StockRecord stockRecord;
        private CurrencyData currencyData;

        private double baseCurrencyCode = (double)SageUserSettings.Default.baseCurrCode; 
        #endregion

        //-------------------------------------------------------------------------------------------------------//

        /// <summary>
        /// Reads all customers and stores it's values in a List.
        /// </summary>
        /// <param name="customersFileName"></param>
        public MyDictionary<Customer> ReadCustomerData()
        {
            MyDictionary<Customer> customerMap = new MyDictionary<Customer>(); 

            try
            {
                sdo = new SDOEngine();
                ws = (WorkSpace)sdo.Workspaces.Add("App Server Update");
                ws.Connect(sageUsrSet.sageDBDir, sageUsrSet.sageUsername, sageUsrSet.sagePassword, "UniqueUpdater");
                salesRecord = (SalesRecord)ws.CreateObject("SalesRecord");
                currencyData = (CurrencyData)ws.CreateObject("CurrencyData");

                string code = String.Empty;
                string desc = String.Empty;

                salesRecord.MoveFirst();
                do
                {
                    code = cutQuotes((String)SDOHelper.Read(salesRecord, "ACCOUNT_REF"));
                    desc = cutQuotes((String)SDOHelper.Read(salesRecord, "NAME"));

                    //Create customer
                    Customer customer = new Customer(code, desc);

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
        public MyDictionary<Product> ReadProductData()
        {
            MyDictionary<Product> productMap = new MyDictionary<Product>();
           
            try
            {
                sdo = new SDOEngine();
                ws = (WorkSpace)sdo.Workspaces.Add("App Server Update");
                ws.Connect(sageUsrSet.sageDBDir, sageUsrSet.sageUsername, sageUsrSet.sagePassword, "UniqueUpdater");
                stockRecord = (StockRecord)ws.CreateObject("StockRecord");

                string code = String.Empty;
                string desc = String.Empty;
                double costPrice = 0;
                double salePrice = 0;

                stockRecord.MoveFirst();
                do
                {
                    try
                    {
                        code = cutQuotes((String)SDOHelper.Read(stockRecord, "STOCK_CODE"));
                        desc = cutQuotes((String)SDOHelper.Read(stockRecord, "DESCRIPTION"));
                        costPrice = (Double)SDOHelper.Read(stockRecord, "LAST_PURCHASE_PRICE");
                        salePrice = (Double)SDOHelper.Read(stockRecord, "SALES_PRICE");

                        Product product = new Product(code, desc, costPrice);
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
        private string cutQuotes(string str)
        {
            try
            {
                string strTrim = str.Trim();
                if (strTrim[0] == '\"' && strTrim[strTrim.Length - 1] == '\"')
                {
                    return strTrim.Substring(1, strTrim.Length - 1);
                }
                else
                {
                    return strTrim;
                }// Else
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