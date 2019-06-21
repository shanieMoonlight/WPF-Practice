using System.Text.RegularExpressions;
using System.Windows.Input;

namespace PriceFinding.Utility.CustomControls.TextBoxes
{
   public class IntegerOnlySelectAllTextBox : ClickSelectAllTextBox
   {
      public IntegerOnlySelectAllTextBox()
      {
         AddHandler(PreviewTextInputEvent, new TextCompositionEventHandler(NumberValidation), true);
      }//ctor

      //---------------------------------------------------------------------------//

      private static void NumberValidation(object sender, TextCompositionEventArgs e)
      {
         Regex regex = new Regex("[^0-9]+");
         e.Handled = regex.IsMatch(e.Text);
      }//NumberValidationTextBox


      //---------------------------------------------------------------------------//

   }//Cls
}//NS
