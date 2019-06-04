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
    class ODBCInvoiceReader : IInvoiceReader
    {
        #region Variables

        private SageTableSettings sageTblSet = SageTableSettings.Default;
        private SageUserSettings sageUsrSet = SageUserSettings.Default;
        private static string salePriceCol = "SALE_PRICE";
        private static string foreignSalePriceCol = "FOREIGN_SALE_PRICE";

        private MyDictionary<Customer> customerMap;
        private MyDictionary<Product> productMap;

        #endregion

        //-------------------------------------------------------------------------------------------------------//

        public ODBCInvoiceReader(MyDictionary<Customer> customerMap, MyDictionary<Product> productMap)
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

           //Start date is lookBackYrs year ago.
            int lookBackYrs = (int)Settings.Default.invoiceLookBackYrs;
            string startDate = "'" + (DateTime.Now.Year - lookBackYrs) + "-" + DateTime.Now.Month + "-01'";


            #region queryString
            string queryString = "SELECT "
                                 + sageTblSet.tblInv + "." + sageTblSet.colInvNum + ", "
                                 + sageTblSet.tblInv + "." + sageTblSet.colCusCode + ", "
                                 + sageTblSet.tblInv + "." + sageTblSet.colInvDate + ", "
                                 + sageTblSet.tblInv + "." + sageTblSet.colCurrCode + ", "
                                 + sageTblSet.tblInvItm + "." + sageTblSet.colStockCode + ", "
                                 + "( " + (sageTblSet.colNetAmt + "/" + sageTblSet.colQty) + ") AS " + salePriceCol + ", "
                                 + "( " + (sageTblSet.colFrgnNetAmt + "/" + sageTblSet.colQty) + ") AS " + foreignSalePriceCol
                                 + " FROM "
                                 + sageTblSet.tblInv + " JOIN " + sageTblSet.tblInvItm
                                 + " ON "
                                 + sageTblSet.tblInv + "." + sageTblSet.colInvNum + " = " + sageTblSet.tblInvItm + "." + sageTblSet.colInvNum
                                 + " WHERE "
                                 + sageTblSet.tblInvItm + "." + sageTblSet.colQty + " IS NOT NULL "
                                 + " AND "
                                 + startDate + " <= " + sageTblSet.colInvDate
                                 + " ORDER BY "
                                 + sageTblSet.colCusCode + ", "
                                 + sageTblSet.colStockCode + ", "
                                 + sageTblSet.colInvDate
                                 ;
            #endregion


            string invNum = String.Empty;
            string cusCode = String.Empty;
            string prodCode = String.Empty;
            string prevCusCode = String.Empty;
            string prevProdCode = String.Empty;
            double salePrice = 0;
            DateTime date = new DateTime();
            MyDictionary<List<Sale>> productActivity = new MyDictionary<List<Sale>>(StringComparer.InvariantCultureIgnoreCase);

            List<Sale> salesList = new List<Sale>();

            try
            {
                DataTable dataTable = fillTableFromODBC(queryString);

                foreach (DataRow row in dataTable.Rows)
                {
                    invNum = row[sageTblSet.colInvNum].ToString();
                    cusCode = row[sageTblSet.colCusCode].ToString();
                    prodCode = row[sageTblSet.colStockCode].ToString();
                    if (!DateTime.TryParse(row[sageTblSet.colInvDate].ToString(), out date))
                        parseExMsg(sageTblSet.colInvDate, invNum, prodCode, "1900/0/0");
                    if (!Double.TryParse(row[salePriceCol].ToString(), out salePrice))
                    {
                        parseExMsg(salePriceCol, invNum, prodCode, " 0. So is sale price");
                    }//If


                    //Check if it's a foreign customer.
                    int currCode = 0;
                    Int32.TryParse(row[sageTblSet.colCurrCode].ToString(), out currCode);
                    if (currCode != (int)sageUsrSet.baseCurrCode)
                    {
                        if (!Double.TryParse(row[foreignSalePriceCol].ToString(), out salePrice))
                        {
                            parseExMsg(foreignSalePriceCol, invNum, prodCode, " 0. So is sale price");
                        }//If
                    }//If


                    //IfTravellingCustomer or Product were not on lists then skip row.
                    if (!productMap.ContainsKey(prodCode) || !customerMap.ContainsKey(cusCode))
                        continue;

                    //If row contains new customer add it to CAs with new PAs else use prev one.
                    if (cusCode != prevCusCode)
                    {
                        productActivity = new MyDictionary<List<Sale>>(StringComparer.InvariantCultureIgnoreCase);
                        customerActivity[cusCode] = productActivity;

                        //reset prevCodes
                        prevCusCode = cusCode;
                        prevProdCode = String.Empty;
                    }//If

                    //If row contains new Product make new salesList otherwise use current one
                    if (prodCode != prevProdCode)
                    {
                        salesList = new List<Sale>();
                        productActivity[prodCode] = salesList;

                        //reset prevCodes
                        prevProdCode = prodCode;
                    }//If


                    //Retrieve sale and add it to salesList.
                    Sale sale = new Sale(date, salePrice, prodCode);
                    salesList.Add(sale);
                    salesList.Sort();

                }//ForEach
            }
            catch (Exception e)
            {
                string eString = "Problem reading Invoice Data"
                                  + "\r\n    -----------------     \r\n"
                                  + e.GetType() + "\r\n" + e.Message
                                  + "\r\n    -----------------     \r\n"
                                  + "Query: " + queryString
                                  + "\r\nInvoice No.: " + invNum + ", Product: " + prodCode + "Customer : " + cusCode
                                  + "\r\nSale Price : " + salePrice + "\r\n";
                // server.TbInfo = eString;
                throw new Exception(eString);
            }//Catch


            return customerActivity;

        }//readInvoiceData

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

        //------------------------------------------------------------------------------------------------------------------//
        

    }//Cls
}//NS