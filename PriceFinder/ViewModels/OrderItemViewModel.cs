using PriceFinding.Models;
using PriceFinding.Properties;
using PriceFinding.Utility.Binding;
using PriceFinding.ViewModels.Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace PriceFinding.ViewModels
{
   /// <summary>
   /// Base class for CustomerViewModel & ProductViewModel
   /// </summary>
   /// <typeparam name="T"></typeparam>
   public class OrderItemViewModel<T> : ObservableObject where T : OrderItem
   {
      private ObservableCollection<T> _codeList;
      public BackgroundViewModel Background { get; set; }
      public bool IsValid { get; set; } = false;
      private MyDictionary<T> _map;
      private string _code;
      private string _description;
      protected readonly string NOT_FOUND = Settings.Default.NOT_FOUND;


      //-------------------------------------------------------------------------------//

      public OrderItemViewModel(MyDictionary<T> map, IEnumerable<T> list)
      {
         UpdateData(map, list);
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
            else if (_map.TryGetValue(_code, out T customer))
            {
               Description = customer.Description;
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

      public ObservableCollection<T> CodeList
      {
         get
         {
            return _codeList;
         }
         set
         {
            _codeList = value;
            //Tell the view.
            OnPropertyChanged(nameof(CodeList));
         }
      }//CodeList

      //-------------------------------------------------------------------------------//

      public virtual void Clear()
      {
         Code = null;
         Description = null;
      }//Clear

      //-------------------------------------------------------------------------------//

      public void UpdateData(MyDictionary<T> customerMap, IEnumerable<T> list)
      {
         _map = customerMap;
         CodeList = new ObservableCollection<T>(list);

      }//UpdateData

      //-------------------------------------------------------------------------------//

   }//Cls
}//NS
