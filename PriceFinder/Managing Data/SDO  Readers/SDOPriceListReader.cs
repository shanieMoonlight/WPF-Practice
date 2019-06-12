using PriceFinding.Managing_Data.ODBC_Readers;
using PriceFinding.Managing_Data.ReaderInterfaces;
using PriceFinding.Properties;
using SageDataObject240;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace PriceFinding
{
   class SDOPriceListReader : IPriceListReader
    {
        #region Variables

        // private SageTableSettings sageTblSet = SageTableSettings.Default;
        private UserSettings sageUserSet = UserSettings.Default;

        private readonly MyDictionary<Customer> customerMap;
        private MyDictionary<Product> productMap;

        private SDOEngine sdo;
        private WorkSpace ws;
        private PriceRecord priceRecord;
        private SalesRecord salesRecord;
        private StockRecord stockRecord;
        private CurrencyData currencyData;

        //private object baton = new object();

        #endregion

        //-------------------------------------------------------------------------------------------------------//

        public SDOPriceListReader(MyDictionary<Customer> customerMap, MyDictionary<Product> productMap)
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
            //PriceListActivity set to new() so null is not returned.
            MyDictionary<MyDictionary<double>> PriceListActivity = new MyDictionary<MyDictionary<double>>();
            try
            {
                sdo = new SDOEngine();
                //Try a connection, will throw an exception if it fails
                ws = (WorkSpace)sdo.Workspaces.Add("App Server Update");
                ws.Connect(sageUserSet.sageDBDir, sageUserSet.sageUsername, sageUserSet.sagePassword, "UniqueUpdater");

                //Dictionary of PriceList Name vs PriceList Data.
                MyDictionary<MyDictionary<double>> miniPLActivity = new MyDictionary<MyDictionary<double>>();
                //Dictionary of PriceList Name vs PriceListUserList.
                MyDictionary<List<string>> plUsers = new MyDictionary<List<string>>();

                //Create instances of the objects
                priceRecord = (PriceRecord)ws.CreateObject("PriceRecord");
                salesRecord = (SalesRecord)ws.CreateObject("SalesRecord");
                stockRecord = (StockRecord)ws.CreateObject("StockRecord");


                //Create a dictionary of PL's that are acually being referenced v's Empty ProductActivities.
                //Single Price Lists can be used for multiple Customers
                salesRecord.MoveFirst();
                do
                {
                    string cusCode;
                    string cusPriceListRef = ((String)SDOHelper.Read(salesRecord, "PRICE_LIST_REF")).Trim();

                    //If pLists already contains PL then add customer to it's entry. Else make new entry if not empty string.
                    if (plUsers.ContainsKey(cusPriceListRef))
                    {
                        cusCode = (String)SDOHelper.Read(salesRecord, "ACCOUNT_REF");
                        plUsers[cusPriceListRef].Add(cusCode);
                    }
                    else if (!cusPriceListRef.Equals(String.Empty))
                    {
                        plUsers[cusPriceListRef] = new List<string>();
                        cusCode = (String)SDOHelper.Read(salesRecord, "ACCOUNT_REF");
                        plUsers[cusPriceListRef].Add(cusCode);

                        //Add new entry for each priceList.
                        miniPLActivity[cusPriceListRef] = new MyDictionary<double>();
                    }//Else

                } while (salesRecord.MoveNext());

                Dictionary<string, double> productActivity = null;
                string plName = String.Empty;
                string plNamePrev = String.Empty;
                double calcValue;
                int calcMeth;
                double xRate = 1;
                double costPrice;
                double salePrice;
                double listPrice;
                string stockCode;

                //Start at first pricerecord
                priceRecord.MoveFirst();
                do
                {
                    //Get first stock code in Price List.
                    stockCode = (String)SDOHelper.Read(priceRecord, "STOCK_CODE");
                    //Create SDO stockRecord with this stockCode to use for searching later.
                    SDOHelper.Write(stockRecord, "STOCK_CODE", stockCode);

                    plName = ((String)SDOHelper.Read(priceRecord, "EXT_REF")).Trim();
                    calcValue = (Double)SDOHelper.Read(priceRecord, "VALUE");
                    calcMeth = (sbyte)SDOHelper.Read(priceRecord, "DISCOUNT_TYPE");
                    salePrice = (Double)SDOHelper.Read(stockRecord, "SALES_PRICE");
                    costPrice = CheckProduct(stockCode).CostPrice;

                    if (plName.Equals(plNamePrev))
                    {
                        //Old plName: Check stock, if found get it's list price & add to productActivity from PLACtivity.
                        if (stockRecord.Find(false))
                        {
                            listPrice = CalculateListPrice(calcValue, calcMeth, costPrice, salePrice, xRate);
                            productActivity[stockCode] = listPrice;
                        }//If
                    }
                    else if (miniPLActivity.ContainsKey(plName))
                    {
                        //New name: Check stock, if found get it's list price & add to productActivity from PLACtivity. 
                        //Change plNamePrev
                        //Get new currency, keep till next Price List.
                        if (stockRecord.Find(false))
                        {
                            xRate = GetCurrency(priceRecord);
                            listPrice = CalculateListPrice(calcValue, calcMeth, costPrice, salePrice, xRate);
                            productActivity = miniPLActivity[plName];
                            productActivity[stockCode] = listPrice;
                            plNamePrev = plName;
                        }//If
                    }//Else

                } while (priceRecord.MoveNext());


                //Give each customer their own PriceList.
                foreach (string key in plUsers.Keys)
                {
                    foreach (string cusCode in plUsers[key])
                    {
                        PriceListActivity[cusCode] = miniPLActivity[key];
                    }//ForEach
                }//ForEach   
            }
            finally
            {
                DestroyAllObjects();
            }//Finally


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


        }//calculateListPrice

        //-----------------------------------------------------------------------------------------------------//

        private double GetCurrency(PriceRecord priceRecord)
        {
            PriceListRecord priceListRecord = null;
            try
            {
                currencyData = ws.CreateObject("CurrencyData");
                double xRate = 1;
                priceListRecord = (PriceListRecord)ws.CreateObject("PriceListRecord");
                string priceListName = (String)SDOHelper.Read(priceRecord, "EXT_REF");
                SDOHelper.Write(priceListRecord, "REFERENCE", priceListName);
                if (!priceListRecord.Find(false))
                    return xRate;

                int currCode = (sbyte)SDOHelper.Read(priceListRecord, "CURRENCY") + 1;
                double baseCurrencyCode = (double)sageUserSet.baseCurrCode;
                if (currCode != baseCurrencyCode)
                {
                    currencyData.Read(currCode);
                    xRate = (double)SDOHelper.Read(currencyData, "EXCHANGE_RATE");
                }//If

                return xRate;
            }
            finally
            {
                if (priceListRecord != null)
                {
                    Marshal.FinalReleaseComObject(priceListRecord);
                    priceListRecord = null;
                }//If
            }//Finally

        }//getCurrency

        //-------------------------------------------------------------------------------------------------------//

        /// <summary>
        /// Checks whether product exists.
        /// </summary>
        /// <param name="prodCode">Product Code</param>
        /// <returns>This Product or blank Product.</returns>
        private Product CheckProduct(string prodCode)
        {
            Product product;
            if (productMap.ContainsKey(prodCode))
            {
                product = productMap[prodCode];
            }
            else
            {
                product = new Product();
            }//else  

            return product;
        }//CheckProduct

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
            if (priceRecord != null)
            {
                Marshal.FinalReleaseComObject(priceRecord);
                priceRecord = null;
            }//If
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

    }//Cls
}//NS