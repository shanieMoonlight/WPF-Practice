using PriceFinding.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PriceFinding
{
    class ODBCPriceListReader:IPriceListReader
    {
        #region Variables

        private SageTableSettings set = SageTableSettings.Default;

        private MyDictionary<Customer> customerMap;
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
            string queryStringPLNames = "SELECT "
                                  + set.tblCust + "." + set.colPrcRef + ", "
                                  + set.tblCust + "." + set.colCusCode + ", "
                                  + " FROM "
                                  + set.tblCust
                                  + " WHERE "
                                  + set.colPrcRef + " <> " + String.Empty
                                  + " ORDER BY "
                                  + set.colPrcRef
                                  ;
            #endregion

            string cusPriceListRef = String.Empty;
            string cusCode = String.Empty;

            //Create a dictionary of PL's that are acually being referenced v's Empty ProductActivities.
            //Single Price Lists can be used for multiple Customers
            try
            {
                DataTable dataTable = fillTableFromODBC(queryStringPLNames);

                foreach (DataRow row in dataTable.Rows)
                {
                    cusPriceListRef = row[set.colPrcRef].ToString();
                    cusCode = row[set.colCusCode].ToString();

                    //If pLists already contains PL then add customer to it's entry. Else make new entry if not empty string.
                    if (plUsers.ContainsKey(cusPriceListRef))
                    {
                        plUsers[cusPriceListRef].Add(cusCode);
                    }
                    else if (!cusPriceListRef.Equals(String.Empty))
                    {
                        plUsers[cusPriceListRef] = new List<string>();
                        plUsers[cusPriceListRef].Add(cusCode);

                        //Add new entry for each priceList.
                        miniPLActivity[cusPriceListRef] = new MyDictionary<double>();
                    }//Else

                }//ForEach


                #region queryStringPLData
                string queryStringPLData = "SELECT "
                                     + set.tblPrc + "." + set.colPrcRef + ", "
                                     + set.tblPrc + "." + set.colStockCode + ", "
                                     + set.tblPrc + "." + set.colCalcMeth + ", "
                                     + set.tblPrc + "." + set.colCalcVal + ", "
                                     + set.tblCurr + "." + set.colXRate + ", "
                                     + set.tblPrcLst + "." + set.colCurrCode + ", "
                                     + set.tblStock + "." + set.colLstPurchPrice + ", "
                                     + set.tblStock + "." + set.colSalePrc

                                     + " FROM "
                                     + "("
                                         + "("
                                             + "("
                                                 + set.tblPrcLst
                                                 + " INNER JOIN "
                                                 + set.tblPrc
                                                 + " ON "
                                                 + set.tblPrc + "." + set.colPrcRef + " =  " + set.tblPrcLst + "." + set.colPrcRef
                                             + ")"
                                             + " INNER JOIN "
                                             + set.tblCurr
                                             + " ON "
                                             + set.tblPrcLst + "." + set.colCurrCode + " =  " + set.tblCurr + "." + set.colCurrNum
                                         + ")"
                                         + " INNER JOIN "
                                             + set.tblStock
                                             + " ON "
                                             + set.tblStock + "." + set.colStockCode + " =  " + set.tblPrc + "." + set.colStockCode
                                     + ")"
                                     + " ORDER BY "
                                     + set.colPrcRef
                                     ;
                #endregion

                dataTable = fillTableFromODBC(queryStringPLData);

                Dictionary<string, double> productActivity = null;
                string plName = String.Empty;
                string plNamePrev = String.Empty;
                string stockCode;
                double calcValue;
                int calcMeth;
                double xRate;
                double costPrice;
                double salePrice;
                double listPrice;

                foreach (DataRow row in dataTable.Rows)
                {
                    plName = row[set.colPrcRef].ToString().Trim();
                    stockCode = row[set.colStockCode].ToString();
                    calcValue = (double)row[set.colCalcVal];
                    calcMeth = (int)row[set.colCalcMeth];
                    xRate = (double)row[set.colXRate];
                    costPrice = (double)row[set.colLstPurchPrice];
                    salePrice = (double)row[set.colSalePrc];

                    if (plName.Equals(plNamePrev))
                    {
                        //Old plName: Check stock, if found get it's list price & add to productActivity from PLACtivity.
                        if (productMap.ContainsKey(stockCode))
                        {
                            listPrice = calculateListPrice(calcValue, calcMeth, costPrice, salePrice, xRate);
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
                            listPrice = calculateListPrice(calcValue, calcMeth, costPrice, salePrice, xRate);
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

        private double calculateListPrice(double calcValue, int calcMeth, double costPrice, double salePrice, double xRate)
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

        //-------------------------------------------------------------------------------------------------------//

        /// <summary>
        /// Connects to File/Sheet and fills and returns a DataTable of the values.
        /// Table will be empty if an exception occured.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        private DataTable fillTableFromODBC(string queryString)
        {
            DataTable table = new DataTable();
            using (OdbcConnection connection = new OdbcConnection(DataManager.connString))
            {
                using (OdbcDataAdapter adapter = new OdbcDataAdapter(queryString, connection))
                {
                    connection.Open();
                    adapter.Fill(table);
                }//Using
            }//Using

            return table;
        }//fillTable

        //-------------------------------------------------------------------------------------------------------//

        private void parseExMsg(string val, string invNum, string prodCode, string defaultVal)
        {
            string exInfo = "Problem parsing " + val + " in invoice: " + invNum + ", product: " + prodCode
                            + "\n Date has been set to " + defaultVal + ".";
            //MessageBox.Show(exInfo);
            MessageBox.Show(exInfo); ;
        }//parseExMsg

        //-------------------------------------------------------------------------------------------------------//


    }//Cls
}//NS