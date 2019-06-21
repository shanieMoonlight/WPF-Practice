using PriceFinding.Models;
using PriceFinding.Properties;
using PriceFinding.Utility;
using PriceFinding.Utility.Binding;
using PriceFinding.ViewModels.Products;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace PriceFinding.ViewModels
{
   public class ProductViewModel : OrderItemViewModel<Product>
   {
      private readonly double _defaultMargin = Settings.Default.defaultMargin;

      private double? _last;
      private double? _cost;
      private double? _priceList;
      private double? _margin;
      private string _type;
      private DateTime? _date;
      private int? _quantity;
      private int _index;

      public ObservableCollection<string> Types { get; private set; }
      public int QtyTabIndex { get; set; }
      public int CodeTabIndex { get; set; }
      public ResultViewModel Result { get; set; }

      //-------------------------------------------------------------------------------//

      public ProductViewModel(MyDictionary<Product> productMap, IEnumerable<Product> productList) : base(productMap, productList)
      {
         Types = new ObservableCollection<string>(PriceTypes.GetPriceTypes());
         Result = new ResultViewModel();

      }//ctor

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -//

      public ProductViewModel(MyDictionary<Product> productMap, IEnumerable<Product> productList, int index) : this(productMap, productList)
      {
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
            Result.Value = GetMarginPrice();
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

            if (_type == PriceTypes.MANUAL)
            {
               Result.Readonly = false;
               Result.Focusable = true;
               Result.Caret.Color = (Brush)Application.Current.FindResource(MyResources.Brushes.PrimaryDark);
               Result.Background.Color = (Brush)Application.Current.FindResource(MyResources.Brushes.PrimaryLight);
            }
            else
            {
               Result.Readonly = true;
               Result.Focusable = true;
               Result.Caret.Color = (Brush)Application.Current.FindResource(MyResources.Brushes.Primary);
               Result.Background.Color = (Brush)Application.Current.FindResource(MyResources.Brushes.Primary);
            }//If
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
            _index = value;
            CodeTabIndex = _index + 1;
            QtyTabIndex = CodeTabIndex + 1;
         }
      }//Index

      //-------------------------------------------------------------------------------//

      public override void Clear()
      {
         base.Clear();
         Quantity = null;
         Date = null;
         Last = null;
         Cost = null;
         PriceList = null;
         Margin = null;
         Result.Clear();
      }//Clear

      //-------------------------------------------------------------------------------//

      public void SetResult()
      {
         switch (Type)
         {
            case PriceTypes.LAST_PRICE:
               Result.Value = GetLastPrice();
               break;
            case PriceTypes.MARGIN:
               Result.Value = GetMarginPrice();
               break;
            case PriceTypes.PRICE_LIST_PRICE:
               Result.Value = GetPriceListPrice();
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