using PriceFinding.Models;
using System.Collections.Generic;

namespace PriceFinding.ViewModels
{
   public class CustomerViewModel : OrderItemViewModel<Customer>
   {



      //-------------------------------------------------------------------------------//

      public CustomerViewModel(MyDictionary<Customer> customerMap, IEnumerable<Customer> customerList):base(customerMap, customerList)
      {
      }//ctor

      //-------------------------------------------------------------------------------//

   

   }//Cls
}//NS
