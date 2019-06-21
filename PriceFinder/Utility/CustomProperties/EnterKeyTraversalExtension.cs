using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PriceFinding.Utility.CustomProperties
{
   /// <summary>
   /// Automatically move foces after hitting enter.
   /// </summary>
   public class EnterKeyTraversalExtension
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

      static void PreviewKeyDown(object sender, KeyEventArgs e)
      {
         var ue = e.OriginalSource as FrameworkElement;

         if (e.Key == Key.Enter)
         {
            e.Handled = true;
            ue.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
         }
      }//PreviewKeyDown
      //----------------------------------------------------------------------------------------------------//

      private static void Unloaded(object sender, RoutedEventArgs e)
      {
         if (!(sender is FrameworkElement ue)) return;

         ue.Unloaded -= Unloaded;
         ue.PreviewKeyDown -= PreviewKeyDown;
      }//ue_Unloaded

      //----------------------------------------------------------------------------------------------------//

      public static readonly DependencyProperty IsEnabledProperty =
          DependencyProperty.RegisterAttached("IsEnabled", typeof(bool),
          typeof(EnterKeyTraversalExtension), new UIPropertyMetadata(false, IsEnabledChanged));

      //----------------------------------------------------------------------------------------------------//

      static void IsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
      {
         if (!(d is FrameworkElement ue)) return;

         if ((bool)e.NewValue)
         {
            ue.Unloaded += Unloaded;
            ue.PreviewKeyDown += PreviewKeyDown;
         }
         else
         {
            ue.PreviewKeyDown -= PreviewKeyDown;
         }//Else

      }//IsEnabledChanged

      //----------------------------------------------------------------------------------------------------//

   }//Cls
}//NS
