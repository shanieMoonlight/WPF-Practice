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
         if (e.Text == ".")
         {
            var originalText = ((TextBox)sender).Text;
            if (AlreadyHasDot(originalText))
            {
               e.Handled = true;
               return;
            }
            else
            {
               return;
            }//else
         }//if

         e.Handled = !IsDigit(e.Text);

      }//NumberValidationTextBox

      //---------------------------------------------------------------------------//

      private static bool AlreadyHasDot(string text)
      {
         return text.Contains(".");
      }//AlreadyHasDot

      //---------------------------------------------------------------------------//

      private static bool IsDigit(string text)
      {
         return (char.IsDigit(text, text.Length - 1));
      }//IsDigit

      //---------------------------------------------------------------------------//

   }//Cls
}//NS
