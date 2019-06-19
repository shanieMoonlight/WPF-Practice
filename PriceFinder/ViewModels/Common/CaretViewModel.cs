using PriceFinding.Utility.Binding;
using System.Windows.Media;

namespace PriceFinding.ViewModels.Common
{
   public class CaretViewModel : ObservableObject
   {

      private Brush _color;
      private int _index;
      private readonly Brush _defaultBrush;

      //------------------------------------------------------------------------//

      public CaretViewModel(string hex = null)
      {  
            try
            {
               _defaultBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom(hex));
            }
            catch
            {
               _defaultBrush = Brushes.Transparent;
            }//catch       

      }//ctor

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - //

      public CaretViewModel(SolidColorBrush defaultColor)
      {

         if (defaultColor != null)
            _defaultBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom(defaultColor));
         else
            _defaultBrush = Brushes.Transparent;
         
      }//ctor

      //------------------------------------------------------------------------//

      public Brush Color
      {
         get
         {
            if (_color == null)
               return _defaultBrush;
            else
               return _color;
         }
         set
         {
            _color = value;
            //"Color" is the name of the Property that just got changed. This will tell the View about it.
            OnPropertyChanged(nameof(Color));
         }
      }//Color

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - //

      public int Index
      {
         get
         {
            return _index;
         }
         set
         {
            _index = value;
            //"Color" is the name of the Property that just got changed. This will tell the View about it.
            OnPropertyChanged(nameof(Index));
         }
      }//Color

      //------------------------------------------------------------------------//

   }//Cls
}//NS
