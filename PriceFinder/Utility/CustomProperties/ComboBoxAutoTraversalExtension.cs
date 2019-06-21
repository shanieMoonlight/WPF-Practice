using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PriceFinding.Utility.CustomProperties
{
   /// <summary>
   /// DON'T USE
   /// Automatically move focus after selecting an item with the Mouse or hitting the Enter Key
   /// </summary>
   public class ComboBoxAutoTraversalExtension
   {
      //----------------------------------------------------------------------------------------------------//

      public static bool GetIsEnabled(DependencyObject obj)
      {
         return (bool)obj.GetValue(IsEnabledProperty);
      }//GetIsEnabled

      //----------------------------------------------------------------------------------------------------//

      public static void SetIsEnabled(DependencyObject obj, bool value)
      {
         obj.SetValue(IsEnabledProperty, value);
      }//SetIsEnabled

      //----------------------------------------------------------------------------------------------------//

      /// <summary>
      /// Moves focus after Enter Key has been pressed
      /// </summary>
      /// <param name="sender">TextBox</param>
      /// <param name="e"></param>
      static void PreviewKeyDown(object sender, KeyEventArgs e)
      {
         //This will be acting on the TextBox inside the ComboBox
         var fe = e.OriginalSource as FrameworkElement;

         if (e.Key == Key.Enter)
         {
            e.Handled = true;
            fe.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
         }
      }//PreviewKeyDown

      //----------------------------------------------------------------------------------------------------//

      /// <summary>
      /// Moves focus after Enter Key has been pressed
      /// </summary>
      /// <param name="sender">TextBox</param>
      /// <param name="e"></param>
      static void PreviewKeyUp(object sender, KeyEventArgs e)
      {
         //This will be acting on the TextBox inside the ComboBox
        var fe = e.OriginalSource as FrameworkElement;

         if (e.Key == Key.Enter)
         {
            e.Handled = true;
            fe.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
         }
      }//PreviewKeyUp

      //----------------------------------------------------------------------------------------------------//

      /// <summary>
      /// Moves focus after selection has been changed with a mouse click
      /// </summary>
      /// <param name="sender">ComboBox</param>
      /// <param name="e"></param>
      static void PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
      {
         var cb = sender as ComboBox;
         Console.WriteLine("SC");
         if (cb.IsDropDownOpen)
         {
            // e.Handled = true;
             cb.MoveFocus(new TraversalRequest(FocusNavigationDirection.Right));
         }//if

      }//SelectionChanged

      //----------------------------------------------------------------------------------------------------//

      private static void Unloaded(object sender, RoutedEventArgs e)
      {
         if (!(sender is ComboBox ue)) return;

         ue.Unloaded -= Unloaded;
         ue.PreviewKeyDown -= PreviewKeyDown;
      }//Unloaded

      //----------------------------------------------------------------------------------------------------//

      public static readonly DependencyProperty IsEnabledProperty =
          DependencyProperty.RegisterAttached("IsEnabled", typeof(bool),
          typeof(ComboBoxAutoTraversalExtension), new UIPropertyMetadata(false, IsEnabledChanged));

      //----------------------------------------------------------------------------------------------------//

      /// <summary>
      /// Subscribe/Unsubscribe
      /// </summary>
      /// <param name="d"></param>
      /// <param name="e"></param>
      static void IsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
      {
         if (!(d is ComboBox ue)) return;

         if ((bool)e.NewValue)
         {
            ue.Unloaded += Unloaded;
            //ue.PreviewKeyDown += PreviewKeyDown;
            //ue.KeyDown += KeyDown;
            //ue.PreviewKeyUp += PreviewKeyUp;
           
            ue.PreviewMouseLeftButtonDown += PreviewMouseLeftButtonDown;
         }
         else
         {
            //ue.PreviewKeyDown -= PreviewKeyDown;
            //ue.PreviewKeyUp -= PreviewKeyUp;
            ue.PreviewMouseLeftButtonDown += PreviewMouseLeftButtonDown;
         }//else

      }//IsEnabledChanged

      private static void KeyDown(object sender, KeyEventArgs e)
      {
         Console.WriteLine("-->" + e.Key);
      }

      //----------------------------------------------------------------------------------------------------//

   }//Cls
}//NS
