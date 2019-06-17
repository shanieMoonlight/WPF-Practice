using PriceFinding.Models;
using PriceFinding.Utility.Binding;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PriceFinding.ViewModels
{
   public class CustomerViewModel : ObservableObject
   {
      public ObservableCollection<Customer> CodeList { get; private set; }
      public BackgroundViewModel Background { get; set; }
      public bool IsValid { get; set; } = false;
      private MyDictionary<Customer> _map;
      private string _code;
      private string _description;


      //-------------------------------------------------------------------------------//

      public CustomerViewModel(MyDictionary<Customer> customerMap)
      {
         _map = customerMap;
         CodeList = new ObservableCollection<Customer>(_map.ToList().Select(kvp => kvp.Value));

         Background = new BackgroundViewModel();
      }//ctor

      //-------------------------------------------------------------------------------//

      public string Code
      {
         get
         {
            return _code;
         }
         set
         {
            _code = value;

            if (_code == null)
            {
               _code = "";
               Background.Color = Brushes.Transparent;
               IsValid = false;
            }
            else if (_map.TryGetValue(_code, out Customer customer))
            {
               Background.Color = Brushes.Transparent;
               IsValid = true;
            }
            else
            {
               IsValid = false;
               Background.Color = Brushes.Salmon;
            }//else

            //Tell the view.
            OnPropertyChanged(nameof(Code));
         }
      }//Code

      //-------------------------------------------------------------------------------//

      public string Description
      {
         get
         {
            if (string.IsNullOrWhiteSpace(_description))
               return "";
            else
               return _description;
         }
         set
         {
            _description = value;
            //"Name" is the name of the Property that just got changed. This will tell the View about it.
            OnPropertyChanged(nameof(Description));
         }
      }//Description

      //-------------------------------------------------------------------------------//

      public void Clear()
      {
         Code = null;
         Description = null;
      }//Clear

      //-------------------------------------------------------------------------------//

   }//Cls
}//NS
