using PriceFinding.Managing_Data.ODBC_Readers;
using PriceFinding.Utility.Binding;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PriceFinding.ViewModels
{
   class MainViewModel : ObservableObject
   {

      public OrderViewModel Order { get; private set; }
      private bool _showProgressSpinner;
      private bool _disableButtons;
      public RelayCommandAsync FindCommand { get; private set; }
      public RelayCommandAsync UpdateCommand { get; private set; }
      public RelayCommandAsync PlaceOrderCommand { get; private set; }
      public RelayCommand ClearCommand { get; private set; }


      //-------------------------------------------------------------------------------//

      public MainViewModel()
      {
         Order = new OrderViewModel();
         FindCommand = new RelayCommandAsync(FindPricesAsync, CanUseFindPrices);
         UpdateCommand = new RelayCommandAsync(UpdateAsync, CanUseUpdate);
         PlaceOrderCommand = new RelayCommandAsync(OrderAsync, CanUseOrder);
         ClearCommand = new RelayCommand(Clear, CanUseClear);


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

      private bool CanUseClear(object message) => _disableButtons ? false : true;

      //-------------------------------------------------------------------------------//

      private void SetResult()
      {
         Order.SetResult();
      }//SetResult

      //-------------------------------------------------------------------------------//

      private async Task FindPricesAsync(object obj)
      {
         ShowProgressSpinner = true;
         _disableButtons = true;

         await Task.Run(() => Order.FindPrices());

         ShowProgressSpinner = false;
         _disableButtons = false;


         DisplayMessage("Order ready", "Don't forget to set the quantities, and pricing types.");
      }//FindPrices

      //-------------------------------------------------------------------------------//

      private bool CanUseFindPrices(object message)
      {
         if (_disableButtons)
            return false;

         return Order.IsReadyToFindPrices();
      }//ConsoleCanUse

      //-------------------------------------------------------------------------------//

      private async Task UpdateAsync(object obj)
      {
         ShowProgressSpinner = true;
         _disableButtons = true;

         await Task.Run(() => Order.UpdateData());

         ShowProgressSpinner = false;
         _disableButtons = false;

         DisplayMessage("Updated", "Ready to go!");

      }//Update

      //-------------------------------------------------------------------------------//

      private bool CanUseUpdate(object message) => _disableButtons ? false : true;

      //-------------------------------------------------------------------------------//

      private async Task OrderAsync(object obj)
      {
         ShowProgressSpinner = true;
         _disableButtons = true;

         await Task.Run(() => Thread.Sleep(2500));

         ShowProgressSpinner = false;
         _disableButtons = false;
      }//Update

      //-------------------------------------------------------------------------------//

      private bool CanUseOrder(object message)
      {
         if (_disableButtons)
            return false;

         return Order.IsReadyToPlaceOrder();
      }//CanUseOrder

      //-------------------------------------------------------------------------------//

      private void DisplayMessage(string title, string message)
      {
         MyMessageBox.ShowOk(title, message);
      }//DisplayMessage

      //-------------------------------------------------------------------------------//

   }//Cls
}//NS
