using PriceFinding.Models;
using PriceFinding.Utility.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceFinding.ViewModels
{
   class OrderViewModel : ObservableObject
   {
      public CustomerViewModel Customer { get; private set; }
      private DataManager _dataManager;


      //-------------------------------------------------------------------------------//

      public OrderViewModel(DataManager dataManager)
      {
         _dataManager = dataManager;
         Customer = new CustomerViewModel(_dataManager.CustomerMap);
      }//ctor

      //-------------------------------------------------------------------------------//



   }//Cls
}//NS
