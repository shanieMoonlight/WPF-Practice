using PriceFinding.Utility.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PriceFinding.ViewModels
{
   class MainViewModel : ObservableObject
   {

      public OrderViewModel Order { get; private set; }
      private bool _showProgressSpinner;
      private readonly DataManager _dataManager;
      public RelayCommandAsync FindCommand { get; private set; }
      public RelayCommandParametered UpdateCommand { get; private set; }
      public RelayCommandParametered OrderCommand { get; private set; }
      public RelayCommand ClearCommand { get; private set; }


      //-------------------------------------------------------------------------------//

      public MainViewModel()
      {

         _dataManager = new DataManager();
         _dataManager.UpdateFromBackup();
         Order = new OrderViewModel(_dataManager);
         FindCommand = new RelayCommandAsync(FindPricesAsync, CanUseFindPrices);
         ClearCommand = new RelayCommand(Clear);


      }//ctor 

      //-------------------------------------------------------------------------------//

      public bool ShowProgressSpinner
      {
         get
         {
            return _showProgressSpinner;
         }
         set
         {
            _showProgressSpinner = value;
            //Tell the View about it.
            OnPropertyChanged(nameof(ShowProgressSpinner));
         }
      }//ShowProgressSpinner

      //-------------------------------------------------------------------------------//

      private void Clear()
      {
         Order.Clear();
      }//Clear

      //-------------------------------------------------------------------------------//

      public void SetResult()
      {
         Order.SetResult();
      }//SetResult

      //-------------------------------------------------------------------------------//

      private async Task FindPricesAsync(object obj)
      {
         ShowProgressSpinner = true;

          await Task.Run(()=> Thread.Sleep(1500));
         
         ShowProgressSpinner = false;
      }//FindPrices

      //-------------------------------------------------------------------------------//

      public bool CanUseFindPrices(object message)
      {
            return Order.AreItemsSelected();
      }//ConsoleCanUse

      //-------------------------------------------------------------------------------//


   }//Cls
}//NS
