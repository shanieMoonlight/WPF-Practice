﻿using PriceFinding.Models;
using PriceFinding.Utility.Binding;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceFinding.ViewModels
{
   class ProductViewModel : ObservableObject
   {

      public ObservableCollection<string> CodeList { get; private set; }
      private MyDictionary<Product> _map;
      private string _code;
      private string _description;
      private double? _last;
      private double? _cost;
      private double? _priceList;
      private double? _margin;
      private string _type;
      private DateTime? _date;
      private double? _result;
      private int? _quantity;
      public ObservableCollection<string> Types { get; private set; }


      //-------------------------------------------------------------------------------//

      public ProductViewModel(MyDictionary<Product> productMap)
      {
         _map = productMap;
         CodeList = new ObservableCollection<string>(_map.Keys);
         Types = new ObservableCollection<string>(PriceTypes.GetPriceTypes());
      }//ctor

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -//

      public string Code
      {
         get
         {
            if (_code == null)
               return "";

            return _code;
         }
         set
         {
            _code = value ?? "";
            //Set description.
            if (_map.TryGetValue(_code, out Product customer))
               Description = customer.Description;

            //Tell the view.
            OnPropertyChanged(nameof(Code));
         }
      }//Code

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -//

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
            //Tell the View about it.
            OnPropertyChanged(nameof(Description));
         }
      }//Description

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -//

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

      //-------------------------------------------------------------------------------//

      public void Clear()
      {
         Code = null;
         Description = null;
      }//Clear

      //-------------------------------------------------------------------------------//

      public void SetResult()
      {
         Result = new Random().Next();
         Console.WriteLine("SetResult");
      }//SetResult

      //-------------------------------------------------------------------------------//

   }//Cls
}//NS