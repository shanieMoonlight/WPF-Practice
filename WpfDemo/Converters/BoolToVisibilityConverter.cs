using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ValueConverters.Converters
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
         throw new NotImplementedException();
      }//ConvertBack

      //---------------------------------------------------------------------------//

   }//Cls
}//NS
