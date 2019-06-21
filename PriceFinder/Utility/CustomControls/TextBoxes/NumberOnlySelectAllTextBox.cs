using System.Text.RegularExpressions;
using System.Windows.Input;

namespace PriceFinding.Utility.CustomControls.TextBoxes
{
   public class NumberOnlySelectAllTextBox : ClickSelectAllTextBox
   {
      public NumberOnlySelectAllTextBox()
      {
         AddHandler(PreviewTextInputEvent, new TextCompositionEventHandler(NumberValidation), true);
      }//ctor

      //---------------------------------------------------------------------------//

      private static void NumberValidation(object sender, TextCompositionEventArgs e)
      {
         Regex regex = new Regex(@"/^\d*\.?\d*$/");
         e.Handled = regex.IsMatch(e.Text);
      }//NumberValidationTextBox


      //---------------------------------------------------------------------------//

   }//Cls
}//NS
