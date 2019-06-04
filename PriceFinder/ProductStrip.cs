using PriceFinding.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

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

      public TextBox tbCode;
      public TextBox tbDesc;
      public TextBox tbDate;
      public TextBox tbLast;
      public TextBox tbCost;
      public TextBox tbMargin;
      public TextBox tbPriceList;
      public ComboBox cbTypes;
      public TextBox tbResult;
      public TextBox tbQty;

      //--------------------------------------------------------------------------//

      public ProductStrip(Grid gridRow)
      {
         var dpProductInfo = (DockPanel)gridRow.Children[0];
         tbCode = (TextBox)dpProductInfo.Children[CODE_IDX];
         tbDesc = (TextBox)dpProductInfo.Children[DESC_IDX];
         tbCode.TextChanged += tbCode_TextChanged;

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

         if (cbTypes.Items.Count > 0)
            cbTypes.Items.Clear();
         cbTypes.ItemsSource = PriceTypes.GetPriceTypes();
         cbTypes.SelectedIndex = 0;

      }//ctor

      //--------------------------------------------------------------------------//

      /// <summary>
      /// Clears text boxes.
      /// </summary>
      public void Clear()
      {
         tbCode.Text = "";
         tbDesc.Text = "";
         tbDate.Text = "";
         tbLast.Text = "";
         tbCost.Text = "";
         tbPriceList.Text = "";
      }//Clear

      //--------------------------------------------------------------------------//

      private void tbCode_TextChanged(object sender, EventArgs e)
      {
         var code = tbCode.Text;
         var desc = "Change_" + new Random().Next();// dataMgr.ProductMap.ContainsKey(code) ? dataMgr.ProductMap[code].Description : "";
         tbDesc.Text = desc;


         // tbDesc.Text = "";
         tbDate.Text = "";
         tbLast.Text = "";
         tbCost.Text = "";
         tbPriceList.Text = "";
      }//tbCode_TextChanged

      //--------------------------------------------------------------------------//

      private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
      {
         Regex regex = new Regex("[^0-9]+");
         e.Handled = regex.IsMatch(e.Text);
      }//NumberValidationTextBox

      //--------------------------------------------------------------------------//

   }//Cls
}//NS
