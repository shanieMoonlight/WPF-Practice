using BespokeFusion;
using PriceFinding;
using PriceFinding.Managing_Data;
using PriceFinding.Properties;
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

      private const int INITIAL_ROW_COUNT = 10;
      private List<ProductStrip> strips = new List<ProductStrip>();
      private double defaultMargin;

      private DataManager _dataManager;

      //-------------------------------------------------------------------------------------------------------//

      public MainWindow()
      {

         InitializeComponent();


         defaultMargin = Settings.Default.defaultMargin;
         try
         {


            _dataManager = new DataManager();
            _dataManager.UpdateFromBackup();
            cbCustomerCode.ItemsSource = _dataManager.CustomerMap.Keys;

            SetInitialProductRows();
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
         ToggleProgressBarVisibility();

      }//ButtOrder_Click

      //-------------------------------------------------------------------------------------------------------//

      private void SetInitialProductRows()
      {
         //Add First row
         strips.Add(new ProductStrip(grdProductsRow, _dataManager));


         // "INITIAL_ROW_COUNT - 1" because we're starting with on row already built
         for (int i = 0; i < INITIAL_ROW_COUNT - 1; i++)
         {
            AddProductRow();
         }//for

      }//SetProductRows

      //-------------------------------------------------------------------------------------------------------//

      private void AddProductRow()
      {
         //Make a spot for the new row to go.
         var rows = grdProducts.RowDefinitions;
         rows.Add(new RowDefinition());
         var gridRowIdx = rows.Count - 1;

         var newProductStrip = MakeProductRow();

         Grid.SetRow(newProductStrip, gridRowIdx);
         grdProducts.Children.Add(newProductStrip);
         strips.Add(new ProductStrip(newProductStrip, _dataManager));

      }//SetProductRows

      //-------------------------------------------------------------------------------------------------------//

      private Grid MakeProductRow()
      {
         string gridXaml = XamlWriter.Save(grdProductsRow);

         // Load it into a new object:
         StringReader stringReader = new StringReader(gridXaml);
         XmlReader xmlReader = XmlReader.Create(stringReader);
         return (Grid)XamlReader.Load(xmlReader);

      }//MakeProductRow

      //-------------------------------------------------------------------------------------------------------//

      private void RemoveProductRow()
      {
         var rows = grdProducts.RowDefinitions;
         if (rows.Count < 2)
            return;

         var lastRowIdx = rows.Count - 1;

         rows.RemoveAt(rows.Count - 1);

      }//SetProductRows

      //-------------------------------------------------------------------------------------------------------//

      private void FabAddProductrow_Click(object sender, RoutedEventArgs e)
      {
         AddProductRow();
      }//FabAddProductrow_Click

      //-------------------------------------------------------------------------------------------------------//

      private void FabRemoveProductRow_Click(object sender, RoutedEventArgs e)
      {
         RemoveProductRow();
      }//FabRemoveProductRow_Click

      //-------------------------------------------------------------------------------------------------------//

      private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
      {
         Regex regex = new Regex("[^0-9]+");
         e.Handled = regex.IsMatch(e.Text);
      }//NumberValidationTextBox

      //-------------------------------------------------------------------------------------------------------//

      private void TbCustomerCode_TextChanged(object sender, TextChangedEventArgs e)
      {
         var desc = "Change_" + new Random().Next();// dataMgr.ProductMap.ContainsKey(code) ? dataMgr.ProductMap[code].Description : "";
         tbCustomerDesc.Text = desc;
      }//TbCustomerCode_TextChanged

      //-------------------------------------------------------------------------------------------------------//

      private async void ButtUpdate_Click(object sender, RoutedEventArgs e)
      {
         if (_dataManager == null)
            _dataManager = new DataManager(false);

         prgDisplay.Visibility = Visibility.Visible;
         try
         {

            await Task.Run(() =>
            {
               _dataManager.Update();
               cbCustomerCode.ItemsSource = _dataManager.CustomerMap.Keys;
               foreach (var strip in strips)
               {
                  strip.UpdateProductList(_dataManager.ProductMap.Keys);
               }

            });
         }
         catch (BackgroundMessageBoxException mbe)
         {
            MyMessageBox.ShowOk(mbe.Title, mbe.Message);
         }
         prgDisplay.Visibility = Visibility.Hidden;


         // Write result.
         MyMessageBox.ShowOk("Result", "Ready to go.");


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

      private void CbCustomerCode_KeyUp(object sender, KeyEventArgs e)
      {
         SetCustomerDescription(cbCustomerCode.Text);

      }//CbCustomerCode_KeyUp

      //-------------------------------------------------------------------------------------------------------//

      private void CbCustomerCode_SelectionChanged(object sender, SelectionChangedEventArgs e)
      {

         if (cbCustomerCode.SelectedValue != null)
            SetCustomerDescription((string)cbCustomerCode.SelectedValue);
      }//CbCustomerCode_SelectionChanged

      //-------------------------------------------------------------------------------------------------------//

      /// <summary>
      /// Sets the Customere description if the code matches a customer.
      /// </summary>
      /// <param name="code">Customer Code</param>
      private void SetCustomerDescription(string code)
      {

         var cus = _dataManager.CheckCustomer(code);

         if (cus.Code != Settings.Default.NOT_FOUND)
         {
            tbCustomerDesc.Text = cus.Description;
            cbCustomerCode.ClearValue(TextBox.BackgroundProperty);
         }
         else
         {
            tbCustomerDesc.Text = "";
            cbCustomerCode.Background = Brushes.Salmon;
         }//else
      }//SetCustomerDescription

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
            tbCustomerDesc.Text = customer.Description;

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
            tbCustomerDesc.Text = customer.Description;

            var codes = strips
               .Where(s => !string.IsNullOrWhiteSpace(s.cbCode.Text))
               .Select(s => s.cbCode.Text);

            _dataManager.CheckSale(cusCode, codes);

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

      private void ButtFind_Click(object sender, RoutedEventArgs e)
      {
         FindPrices2();
         foreach (var strip in strips)
            strip.SetResult();

      }//ButtFind_Click

   }//Cls
}//NS
