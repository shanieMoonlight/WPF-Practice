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
      public const int DESC_IDX = CODE_IDX + 1;
      public const int DATE_IDX = 0;
      public const int LAST_IDX = DATE_IDX + 1;
      public const int PRICE_LIST_IDX = LAST_IDX + 1;
      public const int COST_IDX = PRICE_LIST_IDX + 1;

      public const int STACK_MARGIN_AND_TYPE_IDX = COST_IDX + 1;
      public const int MARGIN_IDX = 0;
      public const int TYPE_IDX = 1;

      public const int RESULT_IDX = STACK_MARGIN_AND_TYPE_IDX + 1;

      public const int STACK_QTY_IDX = RESULT_IDX + 1;
      public const int QTY_IDX = 0;
      public const string NO_RESULT = "---";

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


         var stackMarginAndType = (StackPanel)spProductPricing.Children[STACK_MARGIN_AND_TYPE_IDX];
         tbMargin = (TextBox)stackMarginAndType.Children[MARGIN_IDX];
         cbTypes = (ComboBox)stackMarginAndType.Children[TYPE_IDX];

         tbResult = (TextBox)spProductPricing.Children[RESULT_IDX];

         var stackQty = (StackPanel)spProductPricing.Children[STACK_QTY_IDX];
         tbQty = (TextBox)stackQty.Children[QTY_IDX];

         tbMargin.PreviewTextInput += NumberValidationTextBox;
         //tbMargin.Text = Settings.Default.defaultMargin.ToString();


         tbQty.PreviewTextInput += NumberValidationTextBox;


         cbTypes.SelectionChanged += CbTypes_SelectionChanged;

         tbMargin.KeyUp += TbMargin_KeyUp;

         _dataManager = dataManager;

         SetComboBoxItems(cbTypes, PriceTypes.GetPriceTypes(), true);
         SetComboBoxItems(cbCode, _dataManager.ProductMap.Keys);


      }//ctor

      //--------------------------------------------------------------------------//

      public ProductStrip(Grid gridRow, DataManager dataManager, int tabIndex) : this(gridRow, dataManager)
      {
         cbCode.TabIndex = tabIndex;
         tbMargin.TabIndex = (tabIndex + 100);
         tbQty.TabIndex = (tabIndex + 200);
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
         tbResult.Text = "";
         tbQty.Text = "";
      }//Clear

      //--------------------------------------------------------------------------//

      public void UpdateProductList(IEnumerable<string> dataSet)
      {
         SetComboBoxItems(cbCode, dataSet);
      }//UpdateProductList

      //--------------------------------------------------------------------------//

      private void CbCode_KeyUp(object sender, EventArgs e)
      {
         SetProductInfo(cbCode.Text);
      }//tbCode_TextChanged

      //--------------------------------------------------------------------------//

      private void TbMargin_KeyUp(object sender, KeyEventArgs e)
      {
         if (cbTypes.Text == PriceTypes.MARGIN)
            tbResult.Text = CalculateSaleFromMargin();
      }//TbMargin_KeyUp

      //--------------------------------------------------------------------------//

      private void CbCode_SelectionChanged(object sender, SelectionChangedEventArgs e)
      {

         if (cbCode.SelectedValue != null)
            SetProductInfo((string)cbCode.SelectedValue);
      }//CbCustomerCode_SelectionChanged

      //--------------------------------------------------------------------------//

      private void CbTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
      {

         SetResult();
      }//cbTypes_SelectionChanged

      //--------------------------------------------------------------------------//

      public void SetResult()
      {
         if (cbTypes.SelectedValue == null)
            return;

         switch (cbTypes.SelectedValue.ToString())
         {
            case PriceTypes.LAST_PRICE:
               tbResult.Text = CalculateSaleFromLast();
               break;
            case PriceTypes.PRICE_LIST_PRICE:
               tbResult.Text = CalculateSaleFromPriceList();
               break;
            case PriceTypes.MARGIN:
               tbResult.Text = CalculateSaleFromMargin();
               break;
            default:
               break;
         }//switch
      }//SetResult

      //--------------------------------------------------------------------------//

      private string CalculateSaleFromLast()
      {
         return string.IsNullOrWhiteSpace(tbLast.Text) || tbLast.Text == Settings.Default.NOT_FOUND
            ? NO_RESULT
            : tbLast.Text;

      }//CalculateSaleFromLast

      //--------------------------------------------------------------------------//

      private string CalculateSaleFromPriceList()
      {

         return string.IsNullOrWhiteSpace(tbPriceList.Text) || tbPriceList.Text == Settings.Default.NOT_FOUND
            ? NO_RESULT
            : tbPriceList.Text;

      }//CalculateSaleFromPriceList

      //--------------------------------------------------------------------------//

      private string CalculateSaleFromMargin()
      {

         if (!double.TryParse(tbMargin.Text, out double margin))
            return NO_RESULT;

         if (!double.TryParse(tbCost.Text, out double cost))
            return NO_RESULT;

         var salePrice = cost / (1 - (margin / 100));
         return salePrice.ToString();

      }//CalculateSaleFromMargin

      //--------------------------------------------------------------------------//

      /// <summary>
      /// Sets the Customere description if the code matches a customer.
      /// </summary>
      /// <param name="code">Customer Code</param>
      private void SetProductInfo(string code)
      {

         var product = _dataManager.CheckProduct(code);

         if (product.Code != Settings.Default.NOT_FOUND)
         {
            tbDesc.Text = product.Description;
            cbCode.ClearValue(Control.BackgroundProperty);
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

      //--------------------------------------------------------------------------//

   }//Cls
}//NS
