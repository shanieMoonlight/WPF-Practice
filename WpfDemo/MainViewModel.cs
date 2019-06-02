using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ValueConverters
{
   class MainViewModel : ObservableObject
   {
      private bool _isVisible;
      public bool IsVisible
      {
         get { return _isVisible; }
         set
         {
            _isVisible = value;
            UpdateVisibility(value);
            OnPropertyChanged("IsVisible");

         }
      }

      //- - - - - - - - - - - - - - - - - - - - - - - - - - //

      private Visibility _visibility;
      public Visibility Visibility
      {
         get { return _visibility; }
         set
         {
            _visibility = value;
            OnPropertyChanged("Visibility");
         }
      }

      //----------------------------------------------------//

      private void UpdateVisibility(bool isVisible)
      {
         if (isVisible)
            Visibility = Visibility.Visible;
         else
            Visibility = Visibility.Hidden;
      }//UpdateVisibility


      //----------------------------------------------------//
      
   }//Cls
}//NS
