using BespokeFusion;
using PriceFinding;
using PriceFinding.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
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

      //-------------------------------------------------------------------------------------------------------//

      public MainWindow()
      {
         InitializeComponent();

         SetInitialProductRows();

         defaultMargin = Settings.Default.defaultMargin;
      }//ctor

      //-------------------------------------------------------------------------------------------------------//

      private void ButtOrder_Click(object sender, RoutedEventArgs e)
      {
         ToggleProgressBarVisibility();
         //prgBarCard. = !prgBarCard.IsVisible;
         //MyMessageBox.ShowOk("Your cool message here", "The awesome message title");

      }//ButtOrder_Click

      //-------------------------------------------------------------------------------------------------------//

      private void SetInitialProductRows()
      {
         //Add First row
         strips.Add(new ProductStrip(grdProductsRow));


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
         strips.Add(new ProductStrip(newProductStrip));

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
         var dm = new DataManager();
         //await Task.Run(async () =>
         //{
         //   await dm.Update();
         //});

         Application.Current.Dispatcher.Invoke((Action)async delegate
         {
            dm.Update();
         });
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



      //-------------------------------------------------------------------------------------------------------//

   }//Cls
}//NS
