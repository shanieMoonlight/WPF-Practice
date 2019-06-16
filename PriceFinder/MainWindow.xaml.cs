using BespokeFusion;
using PriceFinding.Managing_Data;
using PriceFinding.Managing_Data.ODBC_Readers;
using PriceFinding.Models;
using PriceFinding.Properties;
using PriceFinding.ViewModels;
using PriceFinding.Writing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;

namespace PriceFinding
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window
   {

      private const int INITIAL_ROW_COUNT = 2;
      private List<ProductStrip> strips = new List<ProductStrip>();
      private double defaultMargin;
      private readonly string NOT_FOUND = Settings.Default.NOT_FOUND;
      private DataManager _dataManager;
      private MainViewModel _mainViewModel;



      //-------------------------------------------------------------------------------------------------------//

      public MainWindow()
      {

         InitializeComponent();


         defaultMargin = Settings.Default.defaultMargin;
         try
         {
            _dataManager = new DataManager();
            _dataManager.UpdateFromBackup();
            _mainViewModel = new MainViewModel(_dataManager);
            DataContext = _mainViewModel;

           //SetInitialProductRows();
         }
         catch (BackgroundMessageBoxException mbe)
         {
            MyMessageBox.ShowOk(mbe.Title, mbe.Message);
         }
         catch (Exception e)
         {
            MyMessageBox.ShowOk("Error", e.Message);
         }//catch

      }//ctor

      //-------------------------------------------------------------------------------------------------------//

      private void ButtOrder_Click(object sender, RoutedEventArgs e)
      {
         var products = new List<Product>();
         string cusCode = cbCustomerCode.Text;

         if (cusCode.Equals(""))
            return;

         Customer customer = _dataManager.CheckCustomer(cusCode);

         foreach (ProductStrip prodStrip in strips)
         {
            string prodCode = prodStrip.cbCode.Text;

            if (prodCode.Equals(""))
               continue;

            Product product = _dataManager.CheckProduct(prodCode);
            if (product.Code != Settings.Default.NOT_FOUND && !string.IsNullOrWhiteSpace(prodStrip.tbLast.Text))
            {
               product.SalePrice = double.Parse(prodStrip.tbResult.Text);
               product.Qty = int.Parse(prodStrip.tbQty.Text);
               products.Add(product);
            }//if

         }//foreach

         PostOrderWriter posrOrderWriter = null;

         Order order = new Order(customer, products);
         string username = "Shanie Moonlight";
         posrOrderWriter = new PostOrderWriter(order, username);

         if (order != null)
         {

            //Make sure order has customer and products.
            if (order.customer == null)
            {
               MyMessageBox.ShowOk("Error", "402 Reading order error: No Customer found in order request. Please re-send");
            }
            if (order.productList.Count <= 0)
            {
               MyMessageBox.ShowOk("Error", "402 Reading order error. No productList found in order request. Please re-send");
            }//If

            try
            {
               //Let rep know if order was posted.
               if (posrOrderWriter.Post())
               {
                  MyMessageBox.ShowOk("Error", "Success, order received.");
               }
               else
               {
                  MyMessageBox.ShowOk("Error", "Order not posted. Contact administrator.");
               }//Else
            }
            catch (Exception ex)
            {

               MyMessageBox.ShowOk("Error", "WTF." + Environment.NewLine + ex.Message);
            }

         }
         else
         {
            MyMessageBox.ShowOk("Error", "402 Reading order error. Order not in correct format. Please re-send");
         }//Else

      }//ButtOrder_Click

      //-------------------------------------------------------------------------------------------------------//

      //private void SetInitialProductRows()
      //{
      //   //Add First row
      //   strips.Add(new ProductStrip(grdProductsRow, _dataManager));


      //   // "INITIAL_ROW_COUNT - 1" because we're starting with on row already built
      //   for (int i = 0; i < INITIAL_ROW_COUNT - 1; i++)
      //   {
      //      AddProductRow();
      //   }//for

      //}//SetProductRows

      ////-------------------------------------------------------------------------------------------------------//

      //private void AddProductRow()
      //{
      //   //Make a spot for the new row to go.
      //   var rows = grdProducts.RowDefinitions;
      //   rows.Add(new RowDefinition());
      //   var gridRowIdx = rows.Count - 1;

      //   var newProductStrip = MakeProductRow();

      //   Grid.SetRow(newProductStrip, gridRowIdx);
      //   grdProducts.Children.Add(newProductStrip);
      //   strips.Add(new ProductStrip(newProductStrip, _dataManager, strips.Count + 1));

      //}//SetProductRows

      ////-------------------------------------------------------------------------------------------------------//

      //private Grid MakeProductRow()
      //{
      //   string gridXaml = XamlWriter.Save(grdProductsRow);

      //   // Load it into a new object:
      //   StringReader stringReader = new StringReader(gridXaml);
      //   XmlReader xmlReader = XmlReader.Create(stringReader);
      //   return (Grid)XamlReader.Load(xmlReader);

      //}//MakeProductRow

      ////-------------------------------------------------------------------------------------------------------//

      //private void RemoveProductRow()
      //{
      //   var rows = grdProducts.RowDefinitions;
      //   if (rows.Count < 2)
      //      return;

      //   var lastRowIdx = rows.Count - 1;

      //   rows.RemoveAt(rows.Count - 1);

      //}//SetProductRows

      //-------------------------------------------------------------------------------------------------------//

      private void FabAddProductrow_Click(object sender, RoutedEventArgs e)
      {
       //  AddProductRow();
      }//FabAddProductrow_Click

      //-------------------------------------------------------------------------------------------------------//

      private void FabRemoveProductRow_Click(object sender, RoutedEventArgs e)
      {
        // RemoveProductRow();
      }//FabRemoveProductRow_Click

      //-------------------------------------------------------------------------------------------------------//

      private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
      {
         Regex regex = new Regex("[^0-9]+");
         e.Handled = regex.IsMatch(e.Text);
      }//NumberValidationTextBox
      
      //-------------------------------------------------------------------------------------------------------//

      private async void ButtUpdate_Click(object sender, RoutedEventArgs e)
      {
         try
         {
            prgDisplay.Visibility = Visibility.Visible;

            if (_dataManager == null)
               _dataManager = new DataManager(false);


            await Task.Run(() =>
            {
               _dataManager.Update();
            });

            cbCustomerCode.ItemsSource = _dataManager.CustomerMap.Keys;
            foreach (var strip in strips)
               strip.UpdateProductList(_dataManager.ProductMap.Keys);

            MyMessageBox.ShowOk("Result", "Ready to go.");
         }
         catch (BackgroundMessageBoxException mbe)
         {
            MyMessageBox.ShowOk(mbe.Title, mbe.Message);
         }
         finally
         {
            prgDisplay.Visibility = Visibility.Hidden;
         }//finally


      }//ButtUpdate_Click

      //-------------------------------------------------------------------------------------------------------//

      private void ToggleProgressBarVisibility()
      {
         if (prgDisplay.Visibility == Visibility.Visible)
            prgDisplay.Visibility = Visibility.Hidden;
         else
            prgDisplay.Visibility = Visibility.Visible;
      }//ToggleProgressBarVisibility

      //-------------------------------------------------------------------------------------------------------//

      /// <summary>
      /// Try to find prices for customer and products.
      /// </summary>
      private void FindPrices()
      {

         var productReader = new SDOProductReader();

         string NOT_FOUND = Settings.Default.NOT_FOUND;
         try
         {
            string cusCode = cbCustomerCode.Text;

            if (cusCode.Equals(""))
            {
               MyMessageBox.Show("Error", "You must enter a customer.");
               return;
            }//If

            Customer customer = _dataManager.CheckCustomer(cusCode);

            //Fill in all queried rows.
            foreach (ProductStrip prodStrip in strips)
            {
               string prodCode = prodStrip.cbCode.Text;

               if (prodCode.Equals(""))
                  continue;

               Product product = _dataManager.CheckProduct(prodCode);

               //If Product exists get Cost Price
               if (product.Code == NOT_FOUND)
               {
                  prodStrip.tbDesc.Text = NOT_FOUND;
                  prodStrip.tbCost.Text = NOT_FOUND;
               }
               else
               {
                  prodStrip.tbDesc.Text = product.Description;

                  var costPrice = productReader.GetCostPrice(prodCode) * customer.XRate;
                  prodStrip.tbCost.Text = costPrice.ToString();
               }//Else

               //Check last sale
               Sale sale = _dataManager.CheckSale(cusCode, prodCode);
               if (sale.Code == NOT_FOUND)
               {
                  prodStrip.tbDate.Text = NOT_FOUND;
                  prodStrip.tbLast.Text = NOT_FOUND;
               }
               else
               {
                  prodStrip.tbDate.Text = sale.Date.ToString("dd-MMM-yy");
                  prodStrip.tbLast.Text = sale.SalePrice.ToString();
               }//Else

               //Check Price List
               double price = _dataManager.CheckListPrice(cusCode, prodCode);
               if (price == -1)
               {
                  prodStrip.tbPriceList.Text = NOT_FOUND;
               }
               else
               {
                  prodStrip.tbPriceList.Text = price.ToString();
               }//Else 



            }//ForEach



         }
         catch (Exception ex)
         {
            string exInfo = ex.GetType().ToString() + "\r\n" + ex.Message;
            MyMessageBox.Show("Error", exInfo);
         }//Catch
      }//FindPrices

      //-------------------------------------------------------------------------------------------------------//

      /// <summary>
      /// Try to find prices for customer and products.
      /// </summary>
      private void FindPrices2()
      {

         var productReader = new ODBCProductReader();

         try
         {
            string cusCode = cbCustomerCode.Text;

            if (cusCode.Equals(""))
            {
               MyMessageBox.ShowOk("Error", "You must enter a customer.");
               return;
            }//If


            var prodCodes = strips
               .Where(s => !string.IsNullOrWhiteSpace(s.cbCode.Text))
               .Select(s => s.cbCode.Text);

            var sales = _dataManager.CheckSales(cusCode, prodCodes);
            var costs = _dataManager.CheckCostPrices(prodCodes);
            var listPrices = _dataManager.CheckListPrices(cusCode, prodCodes);

            //Fill in all queried rows.
            foreach (ProductStrip prodStrip in strips)
            {
               string prodCode = prodStrip.cbCode.Text;

               //Skip blanks
               if (prodCode.Equals(""))
                  continue;



               var customer = _dataManager.CheckCustomer(cbCustomerCode.Text);

               //If Product exists get Cost Price
               prodStrip.tbCost.Text = costs[prodCode].ToString();

               //Check last sale
               if (!costs.TryGetValue(prodCode, out double cost))
                  prodStrip.tbCost.Text = NOT_FOUND;
               else
                  prodStrip.tbCost.Text = cost.ToString();



               //Check last sale
               if (!sales.TryGetValue(prodCode, out Sale sale))
               {
                  prodStrip.tbDate.Text = NOT_FOUND;
                  prodStrip.tbLast.Text = NOT_FOUND;
                  prodStrip.tbQty.Text = "";
               }
               else
               {
                  prodStrip.tbDate.Text = sale.Date.ToString("dd-MMM-yy");
                  prodStrip.tbLast.Text = sale.SalePrice.ToString();
                  prodStrip.tbQty.Text = sale.Qty.ToString();
               }//Else


               //Check Price List
               if (!listPrices.TryGetValue(prodCode, out double priceListPrice))
                  prodStrip.tbPriceList.Text = NOT_FOUND;
               else
                  prodStrip.tbPriceList.Text = priceListPrice.ToString();

               //tbMargin.Text = Settings.Default.defaultMargin.ToString();
            }//ForEach

         }
         catch (Exception ex)
         {
            string exInfo = ex.GetType().ToString() + "\r\n" + ex.Message;
            MyMessageBox.ShowOk("Error", exInfo);
         }//Catch
      }//FindPrices2

      //-------------------------------------------------------------------------------------------------------//

      private void ButtFind_Click(object sender, RoutedEventArgs e)
      {
         FindPrices2();
         foreach (var strip in strips)
            strip.SetResult();

      }//ButtFind_Click

      //-------------------------------------------------------------------------------------------------------//

      private void ButtClear_Click(object sender, RoutedEventArgs e)
      {
         _mainViewModel.Clear();


         foreach (var strip in strips)
            strip.Clear();

      }//ButtClear_Click

      //-------------------------------------------------------------------------------------------------------//

   }//Cls
}//NS
