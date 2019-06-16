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
      private IEnumerable<string> _list;

      //-----------------------------------------------------------------------//

      public PersonViewModel()
      {
         List = new List<string> { "Guitar", "Violin", "Maths", "Running" };

      }

      //-----------------------------------------------------------------------//


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
            //"Name" is the name of the Property that just got changed. This will tell the View about it.
            OnPropertyChanged("Name");
         }
      }//Name

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -//

      public IEnumerable<string> List
      {
         get
         {
               return _list;
         }
         set
         {
            _list = value;
            //"List" is the name of the Property that just got changed. This will tell the View about it.
            OnPropertyChanged("List");
         }
      }//List

      //-----------------------------------------------------------------------//

   }//Cls
}//NS
