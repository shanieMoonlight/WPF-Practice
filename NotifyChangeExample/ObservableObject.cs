﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotifyChangeExample
{
   class ObservableObject : INotifyPropertyChanged
   {
      public event PropertyChangedEventHandler PropertyChanged;

      public void OnPropertyChanged(string name)
      {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

      }//OnPropertyChanged

   }//Cls
}//NS
