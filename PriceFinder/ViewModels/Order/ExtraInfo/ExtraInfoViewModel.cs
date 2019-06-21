using PriceFinding.Utility.Binding;

namespace PriceFinding.ViewModels
{
   public class ExtraInfoViewModel : ObservableObject
   {
      public DeliveryAddressViewModel DeliveryAddress { get; private set; }
      private string _takenBy;
      private string _customerOrderNumber;
      private string _notes;
      private double? _carriage;

      //----------------------------------------------------------------------//

      public ExtraInfoViewModel()
      {
         DeliveryAddress = new DeliveryAddressViewModel();
      }//ctor

      //----------------------------------------------------------------------//

      public string TakenBy
      {
         get
         {
            return _takenBy;
         }
         set
         {
            _takenBy = value;
            //Tell the view.
            OnPropertyChanged(nameof(TakenBy));
         }
      }//TakenBy

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - //

      public string CustomerOrderNumber
      {
         get
         {
            return _customerOrderNumber;
         }
         set
         {
            _customerOrderNumber = value;
            //Tell the view.
            OnPropertyChanged(nameof(CustomerOrderNumber));
         }
      }//CustomerOrderNumber

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - //

      public string Notes
      {
         get
         {
            return _notes;
         }
         set
         {
            _notes = value;
            //Tell the view.
            OnPropertyChanged(nameof(Notes));
         }
      }//Notes

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - //

      public double? Carriage
      {
         get
         {
            return _carriage;
         }
         set
         {
            _carriage = value;
            //Tell the view.
            OnPropertyChanged(nameof(Carriage));
         }
      }//Carriage

      //----------------------------------------------------------------------//

      public void Clear()
      {
         DeliveryAddress.Clear();
         TakenBy = null;
         CustomerOrderNumber = null;
         Carriage = null;
      }//Clear

   }//Cls
}//NS
