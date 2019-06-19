using PriceFinding.Utility.Binding;
using PriceFinding.ViewModels.Common;

namespace PriceFinding.ViewModels.Products
{
   public class ResultViewModel : ObservableObject
   {

      private double? _value;
      private bool _readonly = true;
      private bool _focusable = false;



      //-------------------------------------------------------------------------------//

      public ResultViewModel()
      {
         Background = new BackgroundViewModel();
         Caret = new CaretViewModel();
      }//ctor

      //-------------------------------------------------------------------------------//
      
      public BackgroundViewModel Background { get; set; }

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -//

      public CaretViewModel Caret { get; set; }

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -//

      public double? Value
      {
         get
         {
            return _value;
         }
         set
         {
            _value = value;
            //Tell the view.
            OnPropertyChanged(nameof(Value));
         }

      }//Value

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -//

      public bool Readonly
      {
         get
         {
            return _readonly;
         }
         set
         {
            _readonly = value;
            //Tell the View about it.
            OnPropertyChanged(nameof(Readonly));
         }
      }//Result

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -//

      public bool Focusable
      {
         get
         {
            return _focusable;
         }
         set
         {
            _focusable = value;
            //Tell the View about it.
            OnPropertyChanged(nameof(Focusable));
         }
      }//Focusable

      //-------------------------------------------------------------------------------//



   }//Cls
}//NS
