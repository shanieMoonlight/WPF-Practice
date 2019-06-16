using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NotifyChangeExample.ViewModels
{
   class BackgroundViewModel : ObservableObject
   {

      private Brush _color;

      public Brush Color
      {
         get
         {
            if (_color == null)
               return Brushes.Red;
            else
               return _color;
         }
         set
         {
            _color = value;
            //"Color" is the name of the Property that just got changed. This will tell the View about it.
            OnPropertyChanged("Color");
         }
      }//Color


   }//Cls
}//NS
