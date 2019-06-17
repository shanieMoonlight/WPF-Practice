using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PriceFinding.Utility.Binding.Converters
{
   class BoolToVisibilityConverter : IValueConverter
   {

      //---------------------------------------------------------------------------//

      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
         var boolVal = (bool)value;
         if (boolVal)
            return Visibility.Visible;
         else
            return Visibility.Hidden;
      }//Convert

      //---------------------------------------------------------------------------//

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
      {
         var visibilityVal = (Visibility)value;
         if (visibilityVal == Visibility.Visible)
            return true;
         else
            return false;
      }//ConvertBack

      //---------------------------------------------------------------------------//

   }//Cls
}//NS
