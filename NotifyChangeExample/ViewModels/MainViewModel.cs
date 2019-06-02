using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace NotifyChangeExample.ViewModels
{
   class MainViewModel : ObservableObject
   {
      public MainViewModel DataContext { get; }
      public PersonViewModel Person { get; private set; }
      public BackgroundViewModel Background { get; private set; }

      //----------------------------------------------------------------------//

      public MainViewModel()
      {
         Person = new PersonViewModel();
         Background = new BackgroundViewModel();
      }//ctor

      //----------------------------------------------------------------------//

      public void SetBackground(Brush brushColor)
      {
         Background.Color = brushColor;
      }//SetBackground

      //----------------------------------------------------------------------//

   }//Cls
}//NS
