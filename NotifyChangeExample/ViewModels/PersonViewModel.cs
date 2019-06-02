using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotifyChangeExample.ViewModels
{
   class PersonViewModel : ObservableObject
   {
      private string _name;

      public string Name
      {
         get
         {
            if (string.IsNullOrWhiteSpace(_name))
               return "Unknown";
            else
               return _name;
         }
         set
         {
            _name = value;
            OnPropertyChanged("Name"); 
         }
      }//Name

   }//Cls
}//NS
