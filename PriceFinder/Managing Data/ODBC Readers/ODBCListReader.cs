using PriceFinding.Managing_Data.ReaderInterfaces;
using PriceFinding.Properties;
using PriceFinding.Utility.Sage;
using System;
using System.Data;
using System.Data.Odbc;

namespace PriceFinding.Managing_Data.ODBC_Readers
{
   class ODBCListReader : AODBCReader, IListReader
   {
      private SageTables tbls = new SageTables();
      private SageColumns cols = new SageColumns();

      //-------------------------------------------------------------------------------------------------------//

      /// <summary>
      /// Reads all customers and stores it's values in a List.
      /// </summary>
      /// <param name="customersFileName"></param>
      public MyDictionary<Customer> ReadCustomerData()
      {
         MyDictionary<Customer> customerMap = new MyDictionary<Customer>();

         #region queryString
         string queryString = $@"SELECT 
                                    {tbls.Cust}.{cols.CusCode},
                                    {tbls.Cust}.{cols.CusName},
                                    {tbls.Curr}.{ cols.XRate },
                                   FROM
                                    { tbls.Cust}
                                   JOIN 
                                    { tbls.Curr}
                                   ON
                                    { tbls.Curr}.{ cols.CurrNum} = {tbls.Cust}.{ cols.CurrCode}
                                   ORDER BY
                                    { tbls.Cust}.{ cols.CusCode}"
                              ;
         #endregion


         string code = string.Empty;
         string desc = string.Empty;
         double xRate = 1;

         try
         {
            DataTable dataTable = FillTableFromODBC(queryString);
            foreach (DataRow row in dataTable.Rows)
            {

               code = row[cols.CusCode].ToString();
               desc = row[cols.CusName].ToString();
               xRate = (double)row[cols.XRate];

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
         string queryString = $@"SELECT *
                     
                                 FROM 
                                    {tbls.Stock}
                                 ORDER BY 
                                    {tbls.Stock}.{cols.StockCode}"
                              ;
         #endregion


         string code = string.Empty;
         string desc = string.Empty;

         try
         {
            DataTable dataTable = FillTableFromODBC(queryString);
            foreach (DataRow row in dataTable.Rows)
            {

               code = row[cols.StockCode].ToString();
               desc = row[cols.ProdDesc].ToString();

               //Create customer
               Product product = new Product(code, desc);

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
                              + "\r\nProduct: " + code + ", " + desc;
            throw new Exception(eString);
         }//Catch

         return productMap;

      }//readProductData

      //-------------------------------------------------------------------------------------------------------//


   }//Cls
}//NS