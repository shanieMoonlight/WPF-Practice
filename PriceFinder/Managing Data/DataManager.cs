﻿using Newtonsoft.Json;
using PriceFinding.Managing_Data.ODBC_Readers;
using PriceFinding.Managing_Data.ReaderInterfaces;
using PriceFinding.Models;
using PriceFinding.Properties;
using PriceFinding.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace PriceFinding
{
   /// <summary>
   /// Class for collecting and sorting data. Contains methods for querying sorted data.
   /// </summary>
   class DataManager
   {
      #region Fields

      private static UserSettings sageUsrSet = UserSettings.Default;
      private Settings set = Settings.Default;


      public static string connString = $@"Driver={{Sage Line 50 v{sageUsrSet.sageVersion}}};DIR={sageUsrSet.sageDBDir};UseDataPath=No;UID={sageUsrSet.sageUsername};PWD={sageUsrSet.sagePassword}";

      /// <summary>
      /// For storing all the PriceFinder data 
      /// </summary>
      private readonly string pfStorageFilePath;

      private DataStorage dataStore;

      private IProductReader ProductReader;
      private IInvoiceReader InvoiceReader;
      private IPriceListReader PriceListReader;

      #endregion

      //-------------------------------------------------------------------------------------------------------//

      #region Properties

      public MyDictionary<Customer> CustomerMap { get; private set; }
      public MyDictionary<Product> ProductMap { get; private set; }

      public bool IsUpdated { get; private set; } = false;

      #endregion

      //-------------------------------------------------------------------------------------------------------//

      public DataManager(bool update) : this()
      {
         if (update)
            UpdateFromBackup();
      }//CTOR

      //-------------------------------------------------------------------------------------------------------//

      public DataManager()
      {
         CustomerMap = new MyDictionary<Customer>();
         ProductMap = new MyDictionary<Product>();


         string baseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), set.programDir);
         pfStorageFilePath = Path.Combine(baseDir, set.storageFileName);

         Directory.CreateDirectory(baseDir);

         ProductReader = new ODBCProductReader();
         InvoiceReader = new ODBCInvoiceReader();
         PriceListReader = new ODBCPriceListReader();
      }//CTOR

      //-------------------------------------------------------------------------------------------------------//

      /// <summary>
      /// Update data by querying database
      /// <summary>
      /// Update data by querying database
      /// </summary>
      public void Update()
      {
         IsUpdated = false;

         IListReader ListReader;

         try
         {
            ListReader = new ODBCListReader();

            //Read Customer data.
            CustomerMap = ListReader.ReadCustomerData();
            //Read Product data.
            ProductMap = ListReader.ReadProductData();


            IsUpdated = true;

         }
         catch (Exception e)
         {
            throw new BackgroundMessageBoxException("Error", "Data not updated" + e.Message);
         }//Catch


         //Package app data for storage.
         List<Customer> customerList = CustomerMap.Values.ToList<Customer>();
         List<Product> productList = ProductMap.Values.ToList<Product>();
         dataStore = new DataStorage(CustomerMap, ProductMap, null, null);


         //Store data.
         try
         {
            SerializeToJSON();
         }
         catch (Exception e)
         {
            throw new BackgroundMessageBoxException("Error", "Data updates have not been stored. You will have to update again on next program start up\r\n\r\n" + e.Message);
         }//Catch



      }//Update

      //------------------------------------------------------------------------------------------------------------------//

      /// <summary>
      /// For using PriceFinder immediately without checking database.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      public GenResult<bool> UpdateFromBackup()
      {
         //Last update must be sooner than minDataFileAgeInDays ago.
         DateTime checkDate = DateTime.Now.AddDays(-(int)set.minDataFileAgeInDays);
         //Will return 1601/00/00 if file not found.
         DateTime lastModifiedDatePFFile = new FileInfo(pfStorageFilePath).LastWriteTime;

         //Deserialize stored data.
         //First check if the server has left one for us. If check if PF left one otherwise inform user.
         dataStore = new DataStorage();

         //Make sure that file is recent 
         if (checkDate < lastModifiedDatePFFile)
            dataStore = DeserializeFromJSON<DataStorage>(pfStorageFilePath);
         else
            return new GenResult<bool>(false, "Customer & Product Data has not been uploaded.\nYou need to update the data.");


         CustomerMap = dataStore.CustomerMap;
         ProductMap = dataStore.ProductMap;

         return new GenResult<bool>(true);

      }//UpdateFromBackup

      //-------------------------------------------------------------------------------------------------------//

      /// <summary>
      /// Checks for the most recent sale of product to customer.
      /// </summary>
      /// <param name="cusCode">Customer Code</param>
      /// <param name="prodCode">Product Code</param>
      /// <returns>Most recent sale or blank sale.</returns>
      public Dictionary<string, Sale> CheckSales(string cusCode, IEnumerable<string> prodCodes)
      {
         var sales = InvoiceReader.GetLastPriceData(cusCode, prodCodes);


         return sales;

      }//CheckSales

      //-------------------------------------------------------------------------------------------------------//

      /// <summary>
      /// Checks for the most recent sale of product to customer.
      /// </summary>
      /// <param name="cusCode">Customer Code</param>
      /// <param name="prodCode">Product Code</param>
      /// <returns>Most recent sale or blank sale.</returns>
      public Sale CheckSale(string cusCode, string prodCode)
      {
         Sale blankSale = new Sale();


         var lastSale = InvoiceReader.GetLastPriceData(cusCode, prodCode);

         if (lastSale != null)
            return lastSale;
         else
            return blankSale;

      }//CheckSale

      //-------------------------------------------------------------------------------------------------------//

      /// <summary>
      /// Checks for the most recent sale of product to customer.
      /// </summary>
      /// <param name="cusCode">Customer Code</param>
      /// <param name="prodCode">Product Code</param>
      /// <returns>Most recent sale or blank sale.</returns>
      public Dictionary<string, double> CheckCostPrices(IEnumerable<string> prodCodes)
      {


         return ProductReader.GetCostPrices(prodCodes);

      }//CheckCostPrices

      //-------------------------------------------------------------------------------------------------------//

      /// <summary>
      /// Checks for the most recent sale of product to customer.
      /// </summary>
      /// <param name="cusCode">Customer Code</param>
      /// <param name="prodCode">Product Code</param>
      /// <returns>Most recent sale or blank sale.</returns>
      public double CheckCostPrice(string prodCode)
      {
         return ProductReader.GetCostPrice(prodCode);

      }//CheckCostPrice

      //-------------------------------------------------------------------------------------------------------//

      /// <summary>
      /// Checks for the most recent sale of product to customer.
      /// </summary>
      /// <param name="cusCode">Customer Code</param>
      /// <param name="prodCode">Product Code</param>
      /// <returns>Most recent sale or blank sale.</returns>
      public double CheckListPrice(string cusCode, string prodCode)
      {

         return PriceListReader.GetPriceListPrice(cusCode, prodCode);

      }//CheckListPrice

      //-------------------------------------------------------------------------------------------------------//

      public MyDictionary<double> CheckListPrices(string cusCode, IEnumerable<string> prodCodes)
      {
         return PriceListReader.GetPriceListPrices(cusCode, prodCodes);
      }//CheckListPrices

      //-------------------------------------------------------------------------------------------------------//

      /// <summary>
      /// Checks whether customer exists.
      /// </summary>
      /// <param name="prodCode">Customer Code</param>
      /// <returns>This Customer or blank Customer.</returns>
      public Customer CheckCustomer(string cusCode)
      {
         Customer customer;
         if (CustomerMap.ContainsKey(cusCode))
         {
            customer = CustomerMap[cusCode];
         }
         else
         {
            customer = new Customer();
         }//else  

         return customer;
      }//CheckCustomer

      //-------------------------------------------------------------------------------------------------------//

      /// <summary>
      /// Checks whether product exists.
      /// </summary>
      /// <param name="prodCode">Product Code</param>
      /// <returns>This Product or blank Product.</returns>
      public Product CheckProduct(string prodCode)
      {
         Product product;
         if (ProductMap.ContainsKey(prodCode))
         {
            product = ProductMap[prodCode];
         }
         else
         {
            product = new Product();
         }//else  

         return product;
      }//CheckProduct

      //-------------------------------------------------------------------------------------------------------//

      private T DeserializeFromJSON<T>(string filePath) where T : ISerializable
      {

         T dataStore = default(T);
         try
         {
            using (Stream stream = File.OpenRead(filePath))
            {
               using (StreamReader reader = new StreamReader(stream))
               {
                  using (JsonTextReader jsonReader = new JsonTextReader(reader))
                  {
                     JsonSerializer ser = new JsonSerializer();
                     dataStore = ser.Deserialize<T>(jsonReader);
                  }//Using JsonTextWriter
               }//Using StreamWriter
            }//Using Stream
         }
         catch (Exception e)
         {
            throw new BackgroundMessageBoxException("Error: ",
               ParseFileName(filePath) + " has not been stored."
                  + "\n\n    -----------------     \n\n"
                  + e.Message);
         }//Catch

         return dataStore;
      }//DeserializeFromJSON

      //-------------------------------------------------------------------------------------------------------//

      private void SerializeToJSON()
      {
         try
         {
            using (Stream stream = File.Create(pfStorageFilePath))
            {
               using (StreamWriter writer = new StreamWriter(stream))
               {
                  using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
                  {
                     JsonSerializer ser = new JsonSerializer();
                     ser.Serialize(jsonWriter, dataStore);
                     jsonWriter.Flush();
                  }//Using JsonTextWriter
               }//Using StreamWriter
            }//Using Stream
         }
         catch (Exception e)
         {
            throw new BackgroundMessageBoxException("Error: ",
               ParseFileName(pfStorageFilePath) + " has not been stored."
                  + "\n\n    -----------------     \n\n"
                  + e.Message);
         }//Catch
      }//SerializeToJSON

      //-------------------------------------------------------------------------------------------------------//

      public void StoreData()
      {
         SerializeToJSON();
      }//StoreData

      //-------------------------------------------------------------------------------------------------------//

      private string ParseFileName(string filePath)
      {
         int index = filePath.LastIndexOf("\\") + 1;
         return filePath.Substring(index);
      }//parseExMsg

      //-------------------------------------------------------------------------------------------------------//


   }//Class
}//NS
