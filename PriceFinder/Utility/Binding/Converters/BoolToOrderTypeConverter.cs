using PriceFinding.Models;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PriceFinding.Utility.Binding.Converters
{
   class BoolToOrderTypeConverter : IValueConverter
   {

      //---------------------------------------------------------------------------//

      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {

         var orderTypeVal = (OrderType)value;
         if (orderTypeVal == OrderType.QUOTE)
            return true;
         else
            return false;
      }//Convert

      //---------------------------------------------------------------------------//

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
      {
         var boolVal = (bool)value;
         if (boolVal)
            return OrderType.QUOTE;
         else
            return OrderType.ORDER;
      }//ConvertBack

      //---------------------------------------------------------------------------//

   }//Cls
}//NS
