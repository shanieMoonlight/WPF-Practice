using PriceFinding.Models;
using PriceFinding.Utility.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceFinding.ViewModels
{
   public class CustomerViewModel : ObservableObject
   {
      private IEnumerable<string> _codeList;
      private MyDictionary<Customer> _map;
      private string _code;
      private string _description;


      //-------------------------------------------------------------------------------//

      public CustomerViewModel(MyDictionary<Customer> customerMap)
      {
         _map = customerMap;
         CodeList = _map.Keys;
      }//ctor

      //-------------------------------------------------------------------------------//

      public IEnumerable<string> CodeList
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
            //Set description.
            if (_map.TryGetValue(_code, out Customer customer))
               Description = customer.Description;

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

   }//Cls
}//NS
