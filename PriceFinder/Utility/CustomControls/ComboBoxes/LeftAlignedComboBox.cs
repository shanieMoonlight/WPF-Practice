using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PriceFinding.Utility.CustomControls.ComboBoxes
{
   class LeftAlignedComboBox : ComboBox
   {
      public LeftAlignedComboBox()
      {
         AddHandler(LostFocusEvent, new RoutedEventHandler(OnLostFocus), true);
      }//ctor

      //---------------------------------------------------------------------------//

      private static void OnLostFocus(object sender, RoutedEventArgs e)
      {
         var cb = (ComboBox)sender;
         cb.HorizontalContentAlignment = HorizontalAlignment.Left;
         var cmbTextBox = (TextBox)cb.Template.FindName("PART_EditableTextBox", cb);
         cmbTextBox.CaretIndex = 0;
      }//SelectAllText

      //---------------------------------------------------------------------------//
   }//Cls
}//NS
