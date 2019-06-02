using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ValueConverters.Converters
{
   class BoolToStringConverter : IValueConverter
   {
      private const string YES = "Yes";
      private const string NO = "No";
      private object esle;

      //---------------------------------------------------------------------------//

      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
         var boolAnswer = (bool)value;
         if (boolAnswer)
            return YES;
         else
            return NO;
      }//Convert

      //---------------------------------------------------------------------------//

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
      {
         var answerString = (string)value;
         if (answerString.Equals(YES, StringComparison.InvariantCultureIgnoreCase))
            return true;
         else
            return false;

      }//ConvertBack

      //---------------------------------------------------------------------------//

   }//Cls
}//NS
