using PriceFinding.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using Newtonsoft.Json;

namespace PriceFinding
{
   /// <summary>
   /// Class for collecting and sorting data. Contains methods for querying sorted data.
   /// </summary>
   class DataManager
   {
      #region Fields

      private static SageUserSettings sageUsrSet = SageUserSettings.Default;
      private Settings set = Settings.Default;
      public static string connString = String.Format("DSN={0};Uid={1};Pwd={2}", sageUsrSet.sageAccDSN, sageUsrSet.sageUsername, sageUsrSet.sagePassword);

      /// <summary>
      /// For storing all the PriceFinder data 
      /// </summary>
      private string pfStorageFilePath;
      /// <summary>
      /// For retreiving app server data so we don't need to update ours.
      /// </summary>                       
      private string appServerStorageFilePath;

      private MyDictionary<Customer> customerMap;
      private MyDictionary<Product> productMap;
      private MyDictionary<MyDictionary<List<Sale>>> customerActivity;
      private MyDictionary<MyDictionary<double>> priceListActivity;

      private bool isSerializing = false;
      private bool isUpdated = false;

      private DataStorage dataStore;
      #endregion

      //-------------------------------------------------------------------------------------------------------//

      #region Properties

      public MyDictionary<Customer> CustomerMap
      {
         get { return customerMap; }
      }
      public MyDictionary<Product> ProductMap
      {
         get { return productMap; }
      }

      public bool IsSerializing
      {
         get { return isSerializing; }
      }

      public bool IsUpdated
      {
         get { return isUpdated; }
      }

      #endregion

      //-------------------------------------------------------------------------------------------------------//

      public DataManager()
      {
         customerMap = new MyDictionary<Customer>();
         productMap = new MyDictionary<Product>();
         customerActivity = new MyDictionary<MyDictionary<List<Sale>>>();
         priceListActivity = new MyDictionary<MyDictionary<double>>();

         string baseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), set.programDir);
         string appServerBaseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), set.appProgramDir);
         string appServerPriceFinderDir = Path.Combine(appServerBaseDir, set.appPFUpdaterDir);
         pfStorageFilePath = Path.Combine(baseDir, set.storageFileName);
         appServerStorageFilePath = Path.Combine(appServerPriceFinderDir, set.storageFileName);

         Directory.CreateDirectory(baseDir);
         UpdateFromBackup();
      }//CTOR

      //-------------------------------------------------------------------------------------------------------//

      /// <summary>
      /// Update data by querying database
      /// <summary>
      /// Update data by querying database
      /// </summary>
      public void Update()
      {
         isUpdated = false;

         IListReader ListReader;
         IInvoiceReader InvoiceReader;
         IPriceListReader PriceListReader;

         try
         {
            ListReader = new SDOListReader();

            //Lists must be read first becaause InvoiceReader and PriceListReader will use them.
            //Read Customer data.
            customerMap = ListReader.ReadCustomerData();
            //Read Product data.
            productMap = ListReader.ReadProductData();


            //if (set.sageAccess == SageAccessType.ODBC)
            //{
            //   //At the moment pass empty Map references which will be filled later
            //   InvoiceReader = new ODBCInvoiceReader(customerMap, productMap);
            //   PriceListReader = new ODBCPriceListReader(customerMap, productMap);
            //}
            //else
            //{
            //   //At the moment pass empty Map references which will be filled later
            //   InvoiceReader = new SDOInvoiceReader(customerMap, productMap);
            //   PriceListReader = new SDOPriceListReader(customerMap, productMap);
            //}//Else


            ////Read Invoice data.
            //customerActivity = InvoiceReader.ReadInvoiceData();
            ////Read PriceList data.
            //priceListActivity = PriceListReader.ReadPriceListData();

            isUpdated = true;
         }
         catch (Exception e)
         {
            MyMessageBox.ShowOk("Error", "Data not updated" + e.Message);
         }//Catch



         //Package app data for storage.
         List<Customer> customerList = customerMap.Values.ToList<Customer>();
         List<Product> productList = productMap.Values.ToList<Product>();
         dataStore = new DataStorage(customerMap, productMap, customerActivity, priceListActivity);


         //Store data.
         try
         {
            isSerializing = true;
            SerializeToJSON();
         }
         catch (Exception e)
         {
            MyMessageBox.Show("Error", "Data updates have not been stored. You will have to update again on next program start up\r\n\r\n" + e.Message);
         }
         finally
         {
            isSerializing = false;
         }//Finally



      }//Update

      //------------------------------------------------------------------------------------------------------------------//

      /// <summary>
      /// For using PriceFinder immediately without checking database.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      public void UpdateFromBackup()
      {
         //Last update must be sooner than minDataFileAgeInDays ago.
         DateTime checkDate = DateTime.Now.AddDays(-(int)set.minDataFileAgeInDays);
         //Will return 1601/00/00 if file not found.
         DateTime lastModifiedDateAppFile = new FileInfo(appServerStorageFilePath).LastWriteTime;
         DateTime lastModifiedDatePFFile = new FileInfo(pfStorageFilePath).LastWriteTime;

         //Deserialize stored data.
         //First check if the server has left one for us. If check if PF left one otherwise inform user.
         dataStore = new DataStorage();
         try
         {
            //Make sure that file is recent 
            if (checkDate < lastModifiedDateAppFile)
               dataStore = DeserializeFromJSON<DataStorage>(appServerStorageFilePath);
            else
               throw new Exception("Pricing Data file is too old");
         }
         catch (Exception)
         {
            //Just catch exception and try with PF file instead.
            try
            {
               //Make sure that file is recent 
               if (checkDate < lastModifiedDatePFFile)
                  dataStore = DeserializeFromJSON<DataStorage>(pfStorageFilePath);
               else
                  MyMessageBox.Show("Error",
                     "Pricing Data has not been uploaded."
                     + "\n    -----------------     \n"
                     + "You need to update the data.");
            }
            catch (Exception ex)
            {
               MyMessageBox.Show("Error",
                  "Pricing Data has not been uploaded."
                     + "\n    -----------------     \n"
                     + "You need to update the data."
                     + "\n    -----------------     \n"
                     + ex.Message);
            }//Catch
         }//Catch

         customerMap = dataStore.CustomerMap;

         productMap = dataStore.ProductMap;

         customerActivity = dataStore.CustomerActivity;
         priceListActivity = dataStore.PriceListActivity;

      }//updateFromBackup

      //-------------------------------------------------------------------------------------------------------//

      /// <summary>
      /// Checks for the most recent sale of product to customer.
      /// </summary>
      /// <param name="cusCode">Customer Code</param>
      /// <param name="prodCode">Product Code</param>
      /// <returns>Most recent sale or blank sale.</returns>
      public Sale CheckSale(string cusCode, string prodCode)
      {
         Sale sale = new Sale();

         //If CusCode & ProdCode are in customerActivity return most recent Sale else return blank Sale
         if (customerActivity.ContainsKey(cusCode))
         {
            MyDictionary<List<Sale>> prodActivity = customerActivity[cusCode];
            if (prodActivity.ContainsKey(prodCode))
            {
               List<Sale> sales = prodActivity[prodCode];
               sale = sales[0];
            }//If
         }//If

         return sale;
      }//CheckSale

      //-------------------------------------------------------------------------------------------------------//

      /// <summary>
      /// Checks for the most recent sale of product to customer.
      /// </summary>
      /// <param name="cusCode">Customer Code</param>
      /// <param name="prodCode">Product Code</param>
      /// <returns>Most recent sale or blank sale.</returns>
      public double CheckListPrice(string cusCode, string prodCode)
      {
         double listPrice = -1;

         //If CusCode & ProdCode are in customerActivity return most recent Sale else return blank Sale
         if (priceListActivity.ContainsKey(cusCode))
         {
            MyDictionary<double> prodActivity = priceListActivity[cusCode];
            if (prodActivity.ContainsKey(prodCode))
            {
               listPrice = prodActivity[prodCode];

            }//If
         }//If

         return listPrice;
      }//CheckListPrice

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
            MyMessageBox.Show("Error: ",
               parseFileName(filePath) + " has not been stored."
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
            MyMessageBox.Show("Error: ",
               parseFileName(pfStorageFilePath) + " has not been stored."
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

      private string parseFileName(string filePath)
      {
         int index = filePath.LastIndexOf("\\") + 1;
         return filePath.Substring(index);
      }//parseExMsg

      //-------------------------------------------------------------------------------------------------------//

   }//Class
}//NS
