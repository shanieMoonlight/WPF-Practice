using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace PriceFinding.Utility.CustomControls.TextBoxes
{
  public class NumberOnlyTextBox : TextBox
   {
      public NumberOnlyTextBox()
      {
         AddHandler(PreviewTextInputEvent, new TextCompositionEventHandler(NumberValidation), true);
      }//ctor

      //---------------------------------------------------------------------------//
      
      private static void NumberValidation(object sender, TextCompositionEventArgs e)
      {
         Regex regex = new Regex(@"/^\d*\.?\d*$/");
         e.Handled = regex.IsMatch(e.Text);
      }//NumberValidation


      //---------------------------------------------------------------------------//

   }//Cls
}//NS
