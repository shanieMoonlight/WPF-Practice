using PriceFinding.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PriceFinding
{
   class ProductStrip
   {

      public const int CODE_IDX = 0;
      public const int DESC_IDX = 1;
      public const int DATE_IDX = 0;
      public const int LAST_IDX = 1;
      public const int PRICE_LIST_IDX = 2;
      public const int COST_IDX = 3;
      public const int MARGIN_IDX = 4;
      public const int TYPE_IDX = 5;
      public const int RESULT_IDX = 6;
      public const int QTY_IDX = 7;

      //--------------------------------------------------------------------------//

      public ComboBox cbCode;
      public TextBox tbDesc;
      public TextBox tbDate;
      public TextBox tbLast;
      public TextBox tbCost;
      public TextBox tbMargin;
      public TextBox tbPriceList;
      public ComboBox cbTypes;
      public TextBox tbResult;
      public TextBox tbQty;

      private DataManager _dataManager;

      //--------------------------------------------------------------------------//

      public ProductStrip(Grid gridRow, DataManager dataManager)
      {
         var dpProductInfo = (DockPanel)gridRow.Children[0];
         cbCode = (ComboBox)dpProductInfo.Children[CODE_IDX];
         tbDesc = (TextBox)dpProductInfo.Children[DESC_IDX];
         cbCode.KeyUp += CbCode_KeyUp;
         cbCode.SelectionChanged += CbCode_SelectionChanged;

         var spProductPricing = (StackPanel)gridRow.Children[1];

         tbDate = (TextBox)spProductPricing.Children[DATE_IDX];
         tbLast = (TextBox)spProductPricing.Children[LAST_IDX];
         tbPriceList = (TextBox)spProductPricing.Children[PRICE_LIST_IDX];
         tbCost = (TextBox)spProductPricing.Children[COST_IDX];
         tbMargin = (TextBox)spProductPricing.Children[MARGIN_IDX];
         cbTypes = (ComboBox)spProductPricing.Children[TYPE_IDX];
         tbResult = (TextBox)spProductPricing.Children[RESULT_IDX];
         tbQty = (TextBox)spProductPricing.Children[QTY_IDX];

         tbMargin.PreviewTextInput += NumberValidationTextBox;
         tbMargin.Text = Settings.Default.defaultMargin.ToString();


         tbQty.PreviewTextInput += NumberValidationTextBox;
         tbQty.Text = "1";

         //if (cbTypes.Items.Count > 0)
         //   cbTypes.Items.Clear();
         //cbTypes.ItemsSource = PriceTypes.GetPriceTypes();
         //cbTypes.SelectedIndex = 0;

         _dataManager = dataManager;

         //if (cbCode.Items.Count > 0)
         //   cbCode.Items.Clear();
         //cbCode.ItemsSource = _dataManager.ProductMap.Keys;

         SetComboBoxItems(cbTypes, PriceTypes.GetPriceTypes(), true);
         SetComboBoxItems(cbCode, _dataManager.ProductMap.Keys);


      }//ctor

      //--------------------------------------------------------------------------//

      /// <summary>
      /// Clears text boxes.
      /// </summary>
      public void Clear()
      {
         cbCode.Text = "";
         tbDesc.Text = "";
         tbDate.Text = "";
         tbLast.Text = "";
         tbCost.Text = "";
         tbPriceList.Text = "";
      }//Clear

      //--------------------------------------------------------------------------//

      private void CbCode_KeyUp(object sender, EventArgs e)
      {
         SetProductDescription(cbCode.Text);
      }//tbCode_TextChanged

      //--------------------------------------------------------------------------//

      private void CbCode_SelectionChanged(object sender, SelectionChangedEventArgs e)
      {

         if (cbCode.SelectedValue != null)
            SetProductDescription((string)cbCode.SelectedValue);
      }//CbCustomerCode_SelectionChanged

      //--------------------------------------------------------------------------//

      /// <summary>
      /// Sets the Customere description if the code matches a customer.
      /// </summary>
      /// <param name="code">Customer Code</param>
      private void SetProductDescription(string code)
      {

         var cus = _dataManager.CheckProduct(code);

         if (cus.Code != Settings.Default.NOT_FOUND)
         {
            tbDesc.Text = cus.Description;
            cbCode.ClearValue(TextBox.BackgroundProperty);
         }
         else
         {
            tbDesc.Text = "";
            cbCode.Background = Brushes.Salmon;
         }//else
      }//SetCustomerDescription

      //--------------------------------------------------------------------------//

      private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
      {
         Regex regex = new Regex("[^0-9]+");
         e.Handled = regex.IsMatch(e.Text);
      }//NumberValidationTextBox

      //--------------------------------------------------------------------------//

     private void SetComboBoxItems(ComboBox cb, IEnumerable<string> dataSet, bool selectFirstItem = false)
      {

         if (cb.Items.Count > 0)
            cb.Items.Clear();
         cb.ItemsSource = dataSet;
         if (selectFirstItem)
            cb.SelectedIndex = 0;
      }//SetComboBoxItems

   }//Cls
}//NS
