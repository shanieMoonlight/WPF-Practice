using PriceFinding.Models;
using PriceFinding.Utility.Binding;

namespace PriceFinding.ViewModels
{
   public class DeliveryAddressViewModel : ObservableObject
   {
      private string _line1;
      private string _line2;
      private string _line3;
      private string _line4;
      private string _line5;

      //----------------------------------------------------------------------//

      public DeliveryAddressViewModel()
      {

      }//ctor

      //----------------------------------------------------------------------//

      public string Line1
      {
         get
         {
            return _line1;
         }
         set
         {
            _line1 = value;
            //Tell the view.
            OnPropertyChanged(nameof(Line1));
         }
      }//Line1

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - //

      public string Line2
      {
         get
         {
            return _line2;
         }
         set
         {
            _line2 = value;
            //Tell the view.
            OnPropertyChanged(nameof(Line2));
         }
      }//Line2

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - //

      public string Line3
      {
         get
         {
            return _line3;
         }
         set
         {
            _line3 = value;
            //Tell the view.
            OnPropertyChanged(nameof(Line3));
         }
      }//Line3

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - //

      public string Line4
      {
         get
         {
            return _line4;
         }
         set
         {
            _line4 = value;
            //Tell the view.
            OnPropertyChanged(nameof(Line4));
         }
      }//Line4

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - //

      public string Line5
      {
         get
         {
            return _line5;
         }
         set
         {
            _line5 = value;
            //Tell the view.
            OnPropertyChanged(nameof(Line5));
         }
      }//Line5

      //----------------------------------------------------------------------//

      public void Clear()
      {
         Line1 = null;
         Line2 = null;
         Line3 = null;
         Line4 = null;
         Line5 = null;
      }//Clear

      //----------------------------------------------------------------------//

      public Address ConvertToOrderDeliveryAddress()
      {
         return new Address()
         {
            Line1 = Line1,
            Line2 = Line2,
            Line3 = Line3,
            Line4 = Line4,
            Line5 = Line5,
         };

      }//ConvertToOrderDeliveryAddress

      //----------------------------------------------------------------------//

   }//Cls
}//NS
