using System.ComponentModel;

namespace PriceFinding.Utility.Binding
{
   public class ObservableObject : INotifyPropertyChanged
   {

      public event PropertyChangedEventHandler PropertyChanged;

      //--------------------------------------------------------------------//

      protected void OnPropertyChanged(string property)
      {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
      }//OnPropertyChanged

      //--------------------------------------------------------------------//

   }//Cls
}//NS
