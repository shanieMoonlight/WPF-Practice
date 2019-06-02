using System.ComponentModel;

namespace ValueConverters
{
   internal class ObservableObject : INotifyPropertyChanged
   {
      public event PropertyChangedEventHandler PropertyChanged;

      public void OnPropertyChanged(string name)
      {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

      }//OnPropertyChanged

   }//Cls
}//NS
