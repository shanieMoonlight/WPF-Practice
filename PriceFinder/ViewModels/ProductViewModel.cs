using PriceFinding.Models;
using PriceFinding.Properties;
using PriceFinding.Utility.Binding;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceFinding.ViewModels
{
   class ProductViewModel : OrderItemViewModel<Product>
   {
      private double? _last;
      private double? _cost;
      private double? _priceList;
      private double? _margin;
      private string _type;
      private DateTime? _date;
      private double? _result;
      private int? _quantity;
      public ObservableCollection<string> Types { get; private set; }
      private readonly double _defaultMargin = Settings.Default.defaultMargin;
      public int QtyTabIndex { get; set; }
      public int CodeTabIndex { get; set; }
      private int _index;

      //-------------------------------------------------------------------------------//

      public ProductViewModel(MyDictionary<Product> productMap, IEnumerable<Product> productList) : base(productMap, productList)
      {
         Types = new ObservableCollection<string>(PriceTypes.GetPriceTypes());
      }//ctor

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -//

      public ProductViewModel(MyDictionary<Product> productMap, IEnumerable<Product> productList, int index) : base(productMap, productList)
      {
         Types = new ObservableCollection<string>(PriceTypes.GetPriceTypes());
         Index = index;
      }//ctor

      //-------------------------------------------------------------------------------//

      public double? Last
      {
         get
         {
            return _last;
         }
         set
         {
            _last = value;

            //Tell the view.
            OnPropertyChanged(nameof(Last));
         }
      }//Last

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -//

      public double? Cost
      {
         get
         {
            return _cost;
         }
         set
         {
            _cost = value;
            //Tell the View about it.
            OnPropertyChanged(nameof(Cost));
         }
      }//Cost

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -//

      public double? PriceList
      {
         get
         {
            return _priceList;
         }
         set
         {
            _priceList = value;

            //Tell the view.
            OnPropertyChanged(nameof(PriceList));
         }
      }//PriceList

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -//

      public double? Margin
      {
         get
         {
            return _margin;
         }
         set
         {
            _margin = value;
            Result = GetMarginPrice();
            //Tell the View about it.
            OnPropertyChanged(nameof(Margin));
         }
      }//Margin

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -//

      public DateTime? Date
      {
         get
         {
            return _date;
         }
         set
         {
            _date = value;
            //Tell the View about it.
            OnPropertyChanged(nameof(Date));
         }
      }//Date

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -//

      public double? Result
      {
         get
         {
            return _result;
         }
         set
         {
            _result = value;
            //Tell the View about it.
            OnPropertyChanged(nameof(Result));
         }
      }//Result

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -//

      public int? Quantity
      {
         get
         {
            return _quantity;
         }
         set
         {
            _quantity = value;
            //Tell the View about it.
            OnPropertyChanged(nameof(Quantity));
         }
      }//Quantity

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -//

      public string Type
      {
         get
         {
            return _type;
         }
         set
         {
            _type = value;
            if (!string.IsNullOrWhiteSpace(_type))
               SetResult();
            //Tell the View about it.
            OnPropertyChanged(nameof(Type));
         }
      }//Type

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -//

      public int Index
      {
         get { return _index; }
         set
         {
            CodeTabIndex = value + 1;
            QtyTabIndex = CodeTabIndex + 1;
         }
      }//Index
      //-------------------------------------------------------------------------------//

      public override void Clear()
      {
         base.Clear();
         Date = null;
         Last = null;
         Cost = null;
         PriceList = null;
         Margin = null;
         Result = null;
         Quantity = null;
      }//Clear

      //-------------------------------------------------------------------------------//

      public void SetResult()
      {
         switch (Type)
         {
            case PriceTypes.LAST_PRICE:
               Result = GetLastPrice();
               break;
            case PriceTypes.MARGIN:
               Result = GetMarginPrice();
               break;
            case PriceTypes.PRICE_LIST_PRICE:
               Result = GetPriceListPrice();
               break;
            default:
               GetLastPrice();
               break;
         }//switch

      }//SetResult

      //-------------------------------------------------------------------------------//

      private double? GetLastPrice()
      {
         return Last;
      }//GetLastPrice

      //-------------------------------------------------------------------------------//

      private double? GetPriceListPrice()
      {
         return PriceList;
      }//GetPriceListPrice

      //-------------------------------------------------------------------------------//

      private double? GetMarginPrice()
      {
         if (Cost == null)
            return null;

         if (Margin == null)
            return null;

         var marginPercent = Margin / 100;
         return Cost / (1 - marginPercent);
      }//GetMarginPrice

      //-------------------------------------------------------------------------------//

   }//Cls
}//NS