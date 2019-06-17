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
         //var products = new List<Product>();
         //string cusCode = cbCustomerCode.Text;

         //if (cusCode.Equals(""))
         //   return;

         //Customer customer = _dataManager.CheckCustomer(cusCode);

         //foreach (ProductStrip prodStrip in strips)
         //{
         //   string prodCode = prodStrip.cbCode.Text;

         //   if (prodCode.Equals(""))
         //      continue;

         //   Product product = _dataManager.CheckProduct(prodCode);
         //   if (product.Code != Settings.Default.NOT_FOUND && !string.IsNullOrWhiteSpace(prodStrip.tbLast.Text))
         //   {
         //      product.SalePrice = double.Parse(prodStrip.tbResult.Text);
         //      product.Qty = int.Parse(prodStrip.tbQty.Text);
         //      products.Add(product);
         //   }//if

         //}//foreach

         //PostOrderWriter posrOrderWriter = null;

         //Order order = new Order(customer, products);
         //string username = "Shanie Moonlight";
         //posrOrderWriter = new PostOrderWriter(order, username);

         //if (order != null)
         //{

         //   //Make sure order has customer and products.
         //   if (order.customer == null)
         //   {
         //      MyMessageBox.ShowOk("Error", "402 Reading order error: No Customer found in order request. Please re-send");
         //   }
         //   if (order.productList.Count <= 0)
         //   {
         //      MyMessageBox.ShowOk("Error", "402 Reading order error. No productList found in order request. Please re-send");
         //   }//If

         //   try
         //   {
         //      //Let rep know if order was posted.
         //      if (posrOrderWriter.Post())
         //      {
         //         MyMessageBox.ShowOk("Error", "Success, order received.");
         //      }
         //      else
         //      {
         //         MyMessageBox.ShowOk("Error", "Order not posted. Contact administrator.");
         //      }//Else
         //   }
         //   catch (Exception ex)
         //   {

         //      MyMessageBox.ShowOk("Error", "WTF." + Environment.NewLine + ex.Message);
         //   }

         //}
         //else
         //{
         //   MyMessageBox.ShowOk("Error", "402 Reading order error. Order not in correct format. Please re-send");
         //}//Else

      }//ButtOrder_Click

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
         //try
         //{
         //   prgDisplay.Visibility = Visibility.Visible;

         //   if (_dataManager == null)
         //      _dataManager = new DataManager(false);


         //   await Task.Run(() =>
         //   {
         //      _dataManager.Update();
         //   });

         //   cbCustomerCode.ItemsSource = _dataManager.CustomerMap.Keys;
         //   foreach (var strip in strips)
         //      strip.UpdateProductList(_dataManager.ProductMap.Keys);

         //   MyMessageBox.ShowOk("Result", "Ready to go.");
         //}
         //catch (BackgroundMessageBoxException mbe)
         //{
         //   MyMessageBox.ShowOk(mbe.Title, mbe.Message);
         //}
         //finally
         //{
         //   prgDisplay.Visibility = Visibility.Hidden;
         //}//finally


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
      private void FindPrices2()
      {

         //var productReader = new ODBCProductReader();

         //try
         //{
         //   string cusCode = cbCustomerCode.Text;

         //   if (cusCode.Equals(""))
         //   {
         //      MyMessageBox.ShowOk("Error", "You must enter a customer.");
         //      return;
         //   }//If


         //   var prodCodes = strips
         //      .Where(s => !string.IsNullOrWhiteSpace(s.cbCode.Text))
         //      .Select(s => s.cbCode.Text);

         //   var sales = _dataManager.CheckSales(cusCode, prodCodes);
         //   var costs = _dataManager.CheckCostPrices(prodCodes);
         //   var listPrices = _dataManager.CheckListPrices(cusCode, prodCodes);

         //   //Fill in all queried rows.
         //   foreach (ProductStrip prodStrip in strips)
         //   {
         //      string prodCode = prodStrip.cbCode.Text;

         //      //Skip blanks
         //      if (prodCode.Equals(""))
         //         continue;



         //      var customer = _dataManager.CheckCustomer(cbCustomerCode.Text);

         //      //If Product exists get Cost Price
         //      prodStrip.tbCost.Text = costs[prodCode].ToString();

         //      //Check last sale
         //      if (!costs.TryGetValue(prodCode, out double cost))
         //         prodStrip.tbCost.Text = NOT_FOUND;
         //      else
         //         prodStrip.tbCost.Text = cost.ToString();



         //      //Check last sale
         //      if (!sales.TryGetValue(prodCode, out Sale sale))
         //      {
         //         prodStrip.tbDate.Text = NOT_FOUND;
         //         prodStrip.tbLast.Text = NOT_FOUND;
         //         prodStrip.tbQty.Text = "";
         //      }
         //      else
         //      {
         //         prodStrip.tbDate.Text = sale.Date.ToString("dd-MMM-yy");
         //         prodStrip.tbLast.Text = sale.SalePrice.ToString();
         //         prodStrip.tbQty.Text = sale.Qty.ToString();
         //      }//Else


         //      //Check Price List
         //      if (!listPrices.TryGetValue(prodCode, out double priceListPrice))
         //         prodStrip.tbPriceList.Text = NOT_FOUND;
         //      else
         //         prodStrip.tbPriceList.Text = priceListPrice.ToString();

         //      //tbMargin.Text = Settings.Default.defaultMargin.ToString();
         //   }//ForEach

         //}
         //catch (Exception ex)
         //{
         //   string exInfo = ex.GetType().ToString() + "\r\n" + ex.Message;
         //   MyMessageBox.ShowOk("Error", exInfo);
         //}//Catch

      }//FindPrices2

      //-------------------------------------------------------------------------------------------------------//

      private void ButtFind_Click(object sender, RoutedEventArgs e)
      {
         FindPrices2();

         _mainViewModel.SetResult();

      }//ButtFind_Click

      //-------------------------------------------------------------------------------------------------------//

      private void ButtClear_Click(object sender, RoutedEventArgs e)
      {
         _mainViewModel.Clear();

      }//ButtClear_Click

      //-------------------------------------------------------------------------------------------------------//

   }//Cls
}//NS
