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

         MyMessageBox.ShowOk("Your cool message here", "The awesome message title");

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
         await Task.Run(async () =>
         {
            await dm.Update();
         });
      }//ButtUpdate_Click

      //-------------------------------------------------------------------------------------------------------//

      //-------------------------------------------------------------------------------------------------------//

      ///// <summary>
      ///// Enable/Disable all controls.
      ///// </summary>
      ///// <param name="enabled"></param>
      //private void ChangeEnabledAllControls(bool enabled)
      //{
      //   //Do all this through ChangeEnabled so that it's thread safe.
      //   foreach (Control c in this.Controls)
      //      ChangeEnabled(c, enabled);

      //}//ChangeEnabled

      ////-------------------------------------------------------------------------------------------------------//

      ///// <summary>
      ///// Changes enabled settings from thread that created control.
      ///// </summary>
      ///// <param name="tb"></param>
      ///// <param name="text"></param>
      //private void ChangeEnabled(Control c, bool enabled)
      //{
      //   // InvokeRequired required compares the thread ID of the
      //   // calling thread to the thread ID of the creating thread.
      //   // If these threads are different, it returns true.
      //   if (c.InvokeRequired)
      //   {
      //      Action<Control, bool> callback = new Action<Control, bool>(ChangeEnabled);
      //      c.Invoke(callback, new object[] { c, enabled });
      //   }
      //   else
      //   {
      //      c.Enabled = enabled;
      //   }//Else
      //}//addText

      ////-------------------------------------------------------------------------------------------------------//

      ///// <summary>
      ///// Updates AutoCompletes from thread that created control.
      ///// </summary>
      ///// <param name="tb"></param>
      ///// <param name="text"></param>
      //private void UpdateAutoComplete<T>(TextBox tb, MyDictionary<T> myDic)
      //{
      //   // InvokeRequired required compares the thread ID of the
      //   // calling thread to the thread ID of the creating thread.
      //   // If these threads are different, it returns true.
      //   if (tb.InvokeRequired)
      //   {
      //      Action<TextBox, MyDictionary<T>> callback = new Action<TextBox, MyDictionary<T>>(UpdateAutoComplete);
      //      tb.Invoke(callback, new object[] { tb, myDic });
      //   }
      //   else
      //   {
      //      //Add codes to AutoComplete
      //      string[] codes = myDic.Keys.ToArray();
      //      //Create new AutoCompleteStringCollection otherwise we will just add to the existing one and have duplicate entries.
      //      AutoCompleteStringCollection acStringCollection = new AutoCompleteStringCollection();
      //      acStringCollection.AddRange(codes);
      //      tb.AutoCompleteCustomSource = acStringCollection;
      //   }//Else
      //}//UpdateAutoComplete

      //-------------------------------------------------------------------------------------------------------//

   }//Cls
}//NS
