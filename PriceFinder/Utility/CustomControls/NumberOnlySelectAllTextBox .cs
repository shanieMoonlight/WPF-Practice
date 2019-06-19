using System.Text.RegularExpressions;
using System.Windows.Input;

namespace PriceFinding.Utility.CustomControls
{
   public class NumberOnlySelectAllTextBox : ClickSelectAllTextBox
   {
      public NumberOnlySelectAllTextBox()
      {
         AddHandler(PreviewTextInputEvent, new TextCompositionEventHandler(NumberValidationTextBox), true);
      }//ctor

      //---------------------------------------------------------------------------//

      private static void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
      {
         Regex regex = new Regex("[^0-9]+");
         e.Handled = regex.IsMatch(e.Text);
      }//NumberValidationTextBox


      //---------------------------------------------------------------------------//

   }//Cls
}//NS
