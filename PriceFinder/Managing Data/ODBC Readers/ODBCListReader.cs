using PriceFinding.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PriceFinding
{
    class ODBCListReader : IListReader
    {
        private SageTableSettings set = SageTableSettings.Default;
                
        //-------------------------------------------------------------------------------------------------------//

        /// <summary>
        /// Reads all customers and stores it's values in a List.
        /// </summary>
        /// <param name="customersFileName"></param>
        public MyDictionary<Customer> ReadCustomerData()
        {
            MyDictionary<Customer> customerMap = new MyDictionary<Customer>();
            
            #region queryString
            string queryString = "SELECT "
                                 + set.tblCust + "." + set.colCusCode + ", "
                                 + set.tblCust + "." + set.colCusName + ", "
                                 + set.tblCurr + "." + set.colXRate + ", "
                                 + " FROM "
                                 + set.tblCust + " JOIN " + set.tblCurr
                                 + " ON "
                                 + set.tblCurr + "." + set.colCurrNum + " = " + set.tblCust + "." + set.colCurrCode
                                 + " ORDER BY "
                                 + set.tblCust + "." + set.colCusCode
                                 ;
            #endregion

            
            string code = String.Empty;
            string desc = String.Empty;
            double xRate = 1;

            try
            {
                DataTable dataTable = fillTableFromODBC(queryString);
                foreach (DataRow row in dataTable.Rows)
                {

                    code = row[set.colCusCode].ToString();
                    desc = row[set.colCusName].ToString();
                    xRate = (double)row[set.colXRate];

                    //Create customer
                    Customer customer = new Customer(code, desc, xRate);

                    customerMap.Add(code, customer);

                }//ForEach
            }
            catch (Exception e)
            {
                string eString = "Problem reading Customer Data"
                                  + "\r\n    -----------------     \r\n"
                                  + e.GetType() + "\r\n" + e.Message
                                  + "\r\n    -----------------     \r\n"
                                  + "Query: " + queryString
                                  + "\r\nCustomer: " + code + ", " + desc + "; XRate: " + xRate;
                throw new Exception(eString);
            }//Catch

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
            #region queryString
            string queryString = "SELECT "
                                 + set.tblStock + "." + set.colStockCode + ", "
                                 + set.tblStock + "." + set.colProdDesc + ", "
                                 + set.tblStock + "." + set.colLstPurchPrice + ", "
                                 + set.tblStock + "." + set.colSalePrc + ", "
                                 + " FROM "
                                 + set.tblStock
                                 + " ORDER BY "
                                 + set.tblStock + "." + set.colStockCode
                                 ;
            #endregion

            
            string code = String.Empty;
            string desc = String.Empty;
            double costPrice = 0;
            //double salePrice = 0;

            try
            {
                DataTable dataTable = fillTableFromODBC(queryString);
                foreach (DataRow row in dataTable.Rows)
                {

                    code = row[set.colStockCode].ToString();
                    desc = row[set.colProdDesc].ToString();
                    costPrice = (double)row[set.colLstPurchPrice];
                    //salePrice = (double)row[set.colSalePrc];

                    //Create customer
                    Product product = new Product(code, desc, costPrice);

                    productMap.Add(code, product);

                }//ForEach
            }
            catch (Exception e)
            {
                string eString = "Problem reading Product Data"
                                  + "\r\n    -----------------     \r\n"
                                  + e.GetType() + "\r\n" + e.Message
                                  + "\r\n    -----------------     \r\n"
                                  + "Query: " + queryString
                                  + "\r\nProduct: " + code + ", " + desc + ": Cost Price: " + costPrice;
                throw new Exception(eString);
            }//Catch

            return productMap;

        }//readProductData

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


    }//Cls
}//NS