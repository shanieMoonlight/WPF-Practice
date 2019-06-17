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
      private MainViewModel _mainViewModel;



      //-------------------------------------------------------------------------------------------------------//

      public MainWindow()
      {

         InitializeComponent();


         defaultMargin = Settings.Default.defaultMargin;
         try
         {
            _mainViewModel = new MainViewModel();
            DataContext = _mainViewModel;

            MyMessageBox.ShowOk("Error","55555555555555");

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

     
      private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
      {
         Regex regex = new Regex("[^0-9]+");
         e.Handled = regex.IsMatch(e.Text);
      }//NumberValidationTextBox


      //-------------------------------------------------------------------------------------------------------//

     
   
   }//Cls
}//NS
