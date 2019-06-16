using PriceFinding.Utility.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceFinding.ViewModels
{
   class MainViewModel : ObservableObject
   {
    
      public OrderViewModel Order { get; private set; }
      private readonly DataManager _dataManager;


      //-------------------------------------------------------------------------------//

      public MainViewModel(DataManager dataManager)
      {
         //_dataManager = new DataManager();
         _dataManager = dataManager;
         Order = new OrderViewModel(_dataManager);
        
      }//ctor

      //-------------------------------------------------------------------------------//

      public void Clear()
      {
         Order.Clear();
      }//Clear

      //-------------------------------------------------------------------------------//

   }//Cls
}//NS
