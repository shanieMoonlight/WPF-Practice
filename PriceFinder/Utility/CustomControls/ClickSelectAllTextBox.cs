using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PriceFinding.Utility.CustomControls
{
   class ClickSelectAllTextBox : TextBox
   {
      public ClickSelectAllTextBox()
      {
         AddHandler(PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(SelectivelyIgnoreMouseButton), true);
         AddHandler(GotKeyboardFocusEvent, new RoutedEventHandler(SelectAllText), true);
         AddHandler(MouseDoubleClickEvent, new RoutedEventHandler(SelectAllText), true);
      }//ctor

      //---------------------------------------------------------------------------//

      private static void SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e)
      {
         // Find the TextBox
         DependencyObject parent = e.OriginalSource as UIElement;
         while (parent != null && !(parent is TextBox))
            parent = VisualTreeHelper.GetParent(parent);

         if (parent == null)
            return;

         var textBox = (TextBox)parent;
         if (!textBox.IsKeyboardFocusWithin)
         {
            // If the text box is not yet focussed, give it the focus and
            // stop further processing of this click event.
            textBox.Focus();
            e.Handled = true;
         }

      }//SelectivelyIgnoreMouseButton

      //---------------------------------------------------------------------------//

      private static void SelectAllText(object sender, RoutedEventArgs e)
      {
         if (e.OriginalSource is TextBox textBox)
            textBox.SelectAll();
      }//SelectAllText
      
      //---------------------------------------------------------------------------//

   }//Cls
}//NS
