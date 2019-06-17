using PriceFinding.Models;
using PriceFinding.Utility.Binding;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceFinding.ViewModels
{
   class OrderViewModel : ObservableObject
   {
      private const int INITIAL_ROW_COUNT = 10;
      public CustomerViewModel Customer { get; private set; }
      public ObservableCollection<ProductViewModel> Products { get; private set; }
      private DataManager _dataManager;


      //-------------------------------------------------------------------------------//

      public OrderViewModel(DataManager dataManager)
      {
         _dataManager = dataManager;
         Customer = new CustomerViewModel(_dataManager.CustomerMap);
         Products = new ObservableCollection<ProductViewModel>();
         for (int i = 0; i < INITIAL_ROW_COUNT; i++)
         {
            Products.Add(new ProductViewModel(_dataManager.ProductMap));
         }//for

      }//ctor

      //-------------------------------------------------------------------------------//

      public void SetResult()
      {
         foreach (var product in Products)
            product.SetResult();
      }//SetResult

      //-------------------------------------------------------------------------------//

      public void Clear()
      {
         Customer.Clear();
         foreach (var product in Products)
            product.Clear();
      }//Clear

      //-------------------------------------------------------------------------------//



   }//Cls
}//NS
