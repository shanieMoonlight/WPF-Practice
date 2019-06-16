using PriceFinding.Models;
using PriceFinding.Utility.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PriceFinding.ViewModels
{
   public class CustomerViewModel : ObservableObject
   {
      private IEnumerable<Customer> _codeList;
      public BackgroundViewModel Background { get; set; }
      private MyDictionary<Customer> _map;
      private string _code;
      private string _description;


      //-------------------------------------------------------------------------------//

      public CustomerViewModel(MyDictionary<Customer> customerMap)
      {
         _map = customerMap;
         //CodeList = _map.Keys;
         CodeList = _map.ToList().Select(kvp => kvp.Value);
         Background = new BackgroundViewModel();
      }//ctor

      //-------------------------------------------------------------------------------//

      public IEnumerable<Customer> CodeList
      {
         get
         {
            return _codeList;
         }
         set
         {
            _codeList = value;
            //"CodeList" is the name of the Property that just got changed. This will tell the View about it.
            OnPropertyChanged(nameof(CodeList));
         }
      }//CodeList

      //-------------------------------------------------------------------------------//

      public string Code
      {
         get
         {
            return _code;
         }
         set
         {
            _code = value ?? "";

            if (_map.TryGetValue(_code, out Customer customer) || string.IsNullOrWhiteSpace(_code))
            {
               Background.Color = Brushes.Transparent;
               //Description = customer.Description;
            }
            else
            {
               Background.Color = Brushes.Salmon;
               //Description = "";
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
