using System;
using System.Text.RegularExpressions;
using System.Windows.Controls;
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

         e.Handled = !(char.IsDigit(e.Text, e.Text.Length - 1));
         Console.WriteLine(e.Handled);
      }//NumberValidationTextBox

      //---------------------------------------------------------------------------//

      private static bool AlreadyHasDot(string text)
      {
         return text.Contains(".");
      }//AlreadyHasDot

      //---------------------------------------------------------------------------//

   }//Cls
}//NS
