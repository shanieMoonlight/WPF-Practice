using PriceFinding.Managing_Data.ODBC_Readers;
using PriceFinding.Models;
using PriceFinding.Properties;
using PriceFinding.Utility;
using PriceFinding.Utility.Binding;
using PriceFinding.Writing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PriceFinding.ViewModels
{
   class OrderViewModel : ObservableObject
   {
      private const int INITIAL_ROW_COUNT = 1;
      public CustomerViewModel Customer { get; private set; }
      public ObservableCollection<ProductViewModel> Products { get; private set; }
      private DataManager _dataManager;
      public RelayCommand AddProductCommand { get; private set; }
      public RelayCommand RemoveProductCommand { get; private set; }
      private readonly double _defaultMargin = Settings.Default.defaultMargin;
      private readonly string NOT_FOUND = Settings.Default.NOT_FOUND;
      private MyDictionary<Product> _prodMap;
      private IEnumerable<Product> _prodList;
      private MyDictionary<Customer> _customerMap;
      private IEnumerable<Customer> _customerList;

      //-------------------------------------------------------------------------------//

      public OrderViewModel()
      {
         _dataManager = new DataManager();
         var updateResult = UpdateDataFromBackUp();

         Customer = new CustomerViewModel(_dataManager.CustomerMap, _dataManager.CustomerMap.ToList().Select(kvp => kvp.Value));
         Products = new ObservableCollection<ProductViewModel>();
         for (int i = 0; i < INITIAL_ROW_COUNT; i++)
         {
            AddProduct();
         }//for

         AddProductCommand = new RelayCommand(AddProduct);
         RemoveProductCommand = new RelayCommand(RemoveProduct, CanUseRemoveProduct);

         //If daya was not updated inform user.
         if (!updateResult.Succeeded)
            MyMessageBox.ShowOk("Error", updateResult.Info);
      }//ctor

      //-------------------------------------------------------------------------------//

      public void SetResult()
      {
         foreach (var product in Products)
            product.SetResult();
      }//SetResult

      //-------------------------------------------------------------------------------//

      public void Clear()
      {
         Customer.Clear();
         foreach (var product in Products)
            product.Clear();
      }//Clear

      //-------------------------------------------------------------------------------//

      public bool IsReadyToFindPrices()
      {
         if (!Customer.IsValid)
            return false;

         foreach (var product in Products)
         {
            if (product.IsValid)
               return true;
         }//foreach

         return false;
      }//IsReadyToFindPrices

      //-------------------------------------------------------------------------------//

      public bool IsReadyToPlaceOrder()
      {
         if (!Customer.IsValid)
            return false;

         bool atLeastOne = false;

         foreach (var product in Products)
         {
            if (product.IsValid && (product.Quantity == null || product.Result == null))
               return false;

            if (product.IsValid)
               atLeastOne = true;
         }//foreach

         return atLeastOne;

      }//IsReadyToPlaceOrder

      //-------------------------------------------------------------------------------//

      private void AddProduct()
      {
         Products.Add(new ProductViewModel(_prodMap, _prodList, Products.Count));
      }//AddProduct

      //-------------------------------------------------------------------------------//

      private void RemoveProduct()
      {
         var count = Products.Count;
         if (count < 2)
            return;

         Products.RemoveAt(count - 1);
      }//RemoveProduct

      //-------------------------------------------------------------------------------//

      public bool CanUseRemoveProduct(object message)
      {
         if (Products.Count < 2)
            return false;

         return true;
      }//CanUseRemoveProduct

      //-------------------------------------------------------------------------------//

      public void UpdateData()
      {
         _dataManager.Update();
         _prodMap = _dataManager.ProductMap;
         _prodList = _prodMap.ToList().Select(kvp => kvp.Value).ToList();

         _customerMap = _dataManager.CustomerMap;
         _customerList = _dataManager.CustomerMap.ToList().Select(kvp => kvp.Value);

         Customer.UpdateData(_customerMap, _customerList);


         foreach (var product in Products)
            product.UpdateData(_prodMap, _prodList);

      }//UpdateData

      //-------------------------------------------------------------------------------//

      public GenResult<bool> UpdateDataFromBackUp()
      {
         var updateResult = _dataManager.UpdateFromBackup();

         _prodMap = _dataManager.ProductMap;
         _prodList = _prodMap.ToList().Select(kvp => kvp.Value);

         _customerMap = _dataManager.CustomerMap;
         _customerList = _dataManager.CustomerMap.ToList().Select(kvp => kvp.Value);

         return updateResult;
      }//UpdateData

      //-------------------------------------------------------------------------------//

      public Info PlaceOrder()
      {
         var products = new List<Product>();
         string cusCode = Customer.Code;

         if (cusCode.Equals(""))
            return new Info("Error", "Customer not found");

         Customer customer = _dataManager.CheckCustomer(cusCode);

         foreach (var productVM in Products)
         {
            string prodCode = productVM.Code;

            if (prodCode.Equals(""))
               continue;

            Product product = _dataManager.CheckProduct(prodCode);
            if (product.Code != Settings.Default.NOT_FOUND && productVM.Result != null && productVM.Quantity != null)
            {
               product.SalePrice = productVM.Result.Value.Value;
               product.Qty = productVM.Quantity.Value;
               products.Add(product);
            }//if

         }//foreach

         PostOrderWriter posrOrderWriter = null;

         Order order = new Order(customer, products);
         string username = "Shanie Moonlight";
         posrOrderWriter = new PostOrderWriter(order, username);

         if (order == null)
         {
            return new Info("Error", "Something went wrong. Order not in correct format. Please re-try");
         }//Else

         //Make sure order has customer and products.
         if (order.customer == null)
         {
            return new Info("Error", "You must enter a valid Customer");
         }
         if (order.productList.Count <= 0)
         {
            return new Info("Error", "You must enter at leeast one Product");
         }//If

         try
         {
            //Let rep know if order was posted.
            if (posrOrderWriter.Post())
               return new Info("Success", "You're order has been created.");
            else
               return new Info("Error", "Order not posted. Contact administrator.");
         }
         catch (Exception ex)
         {

            return new Info("Error", "WTF." + Environment.NewLine + ex.Message);
         }//catch

      }//PlaceOrder

      //-------------------------------------------------------------------------------//

      /// <summary>
      /// Try to find prices for customer and products.
      /// </summary>
      public GenResult<bool> FindPrices()
      {

         var productReader = new ODBCProductReader();

         try
         {
            string cusCode = Customer.Code;

            if (cusCode.Equals(""))
            {
               return new GenResult<bool>(false, "You must enter a customer.");
            }//If

            //Remove empty rows
            var enteredProducts = Products
               .Where(s => !string.IsNullOrWhiteSpace(s.Code));

            var prodCodes = enteredProducts
               .Select(s => s.Code);


            var sales = _dataManager.CheckSales(cusCode, prodCodes);
            var costs = _dataManager.CheckCostPrices(prodCodes);
            var listPrices = _dataManager.CheckListPrices(cusCode, prodCodes);

            //Fill in all queried rows.
            foreach (var product in enteredProducts)
            {
               string prodCode = product.Code;
               

               var customer = _dataManager.CheckCustomer(Customer.Code);

               //If Product exists get Cost Price
               product.Cost = costs[prodCode];

               //Check last sale
               if (!costs.TryGetValue(prodCode, out double cost))
                  product.Cost = null;
               else
                  product.Cost = cost;



               //Check last sale
               if (!sales.TryGetValue(prodCode, out Sale sale))
               {
                  product.Date = null;
                  product.Last = null;
                  //product.Quantity = null;
               }
               else
               {
                  product.Date = sale.Date;
                  product.Last = sale.SalePrice;
                  //product.Quantity = sale.Qty;
               }//Else


               //Check Price List
               if (!listPrices.TryGetValue(prodCode, out double priceListPrice))
                  product.PriceList = null;
               else
                  product.PriceList = priceListPrice;

               product.Margin = Settings.Default.defaultMargin;

               if (product.Last != null)
                  product.Type = PriceTypes.LAST_PRICE;
               else
                  product.Type = PriceTypes.MARGIN;
            }//ForEach

            return new GenResult<bool>(true);
         }
         catch (Exception ex)
         {
            string exInfo = ex.GetType().ToString() + "\r\n" + ex.Message;
            return new GenResult<bool>(false, exInfo);
         }//Catch

      }//FindPrices

   }//Cls
}//NS
