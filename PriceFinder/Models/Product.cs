using PriceFinding.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace PriceFinding.Models
{
   [Serializable()]
   public class Product : OrderItem, IComparable<Product>, ISerializable
   {
      #region Constructors
      public Product() : base(null, null)
      {
      }//ctor

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - //

      /// <summary>
      /// Constructor for order products
      /// </summary>
      /// <param name="code"></param>
      /// <param name="description"></param>
      public Product(string code, string description) : base(code, description)
      {
      }//ctor

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - //

      /// <summary>
      /// Constructor for order products
      /// </summary>
      /// <param name="code"></param>
      /// <param name="salePrice"></param>
      /// <param name="qty"></param>
      public Product(string code, double salePrice, int qty) : base(null, null)
      {
         SalePrice = salePrice;
         Qty = qty;
      }//ctor

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - //

      /// <summary>
      /// Constructor for order products
      /// </summary>
      /// <param name="code"></param>
      /// <param name="salePrice"></param>
      /// <param name="qty"></param>
      public Product(string code, string description, double salePrice, int qty) : base(code, description)
      {
         SalePrice = salePrice;
         Qty = qty;
      }//ctor

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - //

      /// <summary>
      /// Constructor for notInStock products
      /// </summary>
      /// <param name="code"></param>
      /// <param name="salePrice"></param>
      /// <param name="qty"></param>
      public Product(string code, string description, int qty) : base(code, description)
      {
         Qty = qty;
      }//ctor

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - //

      /// <summary>
      /// Constructor for stock products
      /// </summary>
      /// <param name="code"></param>
      /// <param name="salePrice"></param>
      /// <param name="qty"></param>
      public Product(string code, string description, int qty, double costPrice) : base(code, description)
      {
         CostPrice = costPrice;
         Qty = qty;
      }//ctor

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - //

      public Product(string code, string description, double costPrice) : base(code, description)
      {
         CostPrice = costPrice;
      }//ctor

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - //

      /// <summary>
      /// Constructor for building products that are used in the ProductList
      /// </summary>
      /// <param name="code"></param>
      /// <param name="description"></param>
      /// <param name="costPrice"></param>
      /// <param name="salePrice"></param>
      public Product(string code, string description, double costPrice, double salePrice) : base(code, description)
      {
         CostPrice = costPrice;
         SalePrice = salePrice;
      }//ctor

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - //

      /// <summary>
      /// Constructor for stock products
      /// </summary>
      /// <param name="code"></param>
      /// <param name="salePrice"></param>
      /// <param name="qty"></param>
      public Product(string code, int qty, double costPrice) : base(code, null)
      {
         CostPrice = costPrice;
         Qty = qty;
      }//ctor 

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - //

      /// <summary>
      /// Constructor for copying products
      /// </summary>
      /// <param name="code"></param>
      /// <param name="salePrice"></param>
      /// <param name="qty"></param>
      public Product(Product product) : base(product.Code, product.Description)
      {
         CostPrice = product.CostPrice;
         SalePrice = product.SalePrice;
         UsingPriceListPrice = product.UsingPriceListPrice;
         Qty = product.Qty;
      }//ctor

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - //

      /// <summary>
      /// Deserialization constructor
      /// </summary>
      /// <param name="info"></param>
      /// <param name="ctxt"></param>
      public Product(SerializationInfo info, StreamingContext ctxt) : base((string)info.GetValue("code", typeof(string)), (string)info.GetValue("description", typeof(string)))
      {
         CostPrice = (double)info.GetValue("costPrice", typeof(double));
         SalePrice = (double)info.GetValue("salePrice", typeof(double));
         Qty = (int)info.GetValue("qty", typeof(int));
         UsingPriceListPrice = (bool)info.GetValue("usingPriceListPrice", typeof(bool));
      }//ctor


      #endregion

      //----------------------------------------------------------------------------------------------//

      #region Properties
      public int Qty { get; set; }
      public double CostPrice { get; }
      public double SalePrice { get; set; }
      public bool UsingPriceListPrice { get; private set; }
      #endregion

      //----------------------------------------------------------------------------------------------//

      public override string ToString()
      {
         return Code + " - " + Description;
      }//ToString

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - //

      public string ToStringStockLevel()
      {
         return "code: " + Code + " \r\n"
                + "description: " + Description + " \r\n"
                + "qty: " + Qty + "\r\n";
      }//ToString

      //----------------------------------------------------------------------------------------------//

      /// <summary>
      /// Used for serialization.
      /// </summary>
      /// <param name="info"></param>
      /// <param name="context"></param>
      public void GetObjectData(SerializationInfo info, StreamingContext context)
      {
         info.AddValue("code", Code);
         info.AddValue("description", Description);
         info.AddValue("costPrice", CostPrice);
         info.AddValue("salePrice", SalePrice);
         info.AddValue("qty", Qty);
         info.AddValue("usingPriceListPrice", UsingPriceListPrice);
      }//GetObjectData

      //----------------------------------------------------------------------------------------------//

      /// <summary>
      /// Used when sorting a list of products.
      /// </summary>
      /// <param name="product"></param>
      /// <returns></returns>
      public int CompareTo(Product product)
      {
         string thatCode = product.Code;
         return Code.CompareTo(thatCode);
      }//CompareTo

      //----------------------------------------------------------------------------------------------//

   }//Cls Product

}//NS
