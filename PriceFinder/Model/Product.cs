using PriceFinding.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace PriceFinding.Models
{
   [Serializable()]
   public class Product : IComparable<Product>, ISerializable
   {
      #region Constructors
      public Product()
      {
      }//ctor

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - //

      /// <summary>
      /// Constructor for order products
      /// </summary>
      /// <param name="code"></param>
      /// <param name="description"></param>
      public Product(string code, string description)
      {
         this.Code = code;
         this.Description = description;
      }//ctor

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - //

      /// <summary>
      /// Constructor for order products
      /// </summary>
      /// <param name="code"></param>
      /// <param name="salePrice"></param>
      /// <param name="qty"></param>
      public Product(string code, double salePrice, int qty)
      {
         this.Code = code;
         this.SalePrice = salePrice;
         this.Qty = qty;
      }//ctor

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - //

      /// <summary>
      /// Constructor for order products
      /// </summary>
      /// <param name="code"></param>
      /// <param name="salePrice"></param>
      /// <param name="qty"></param>
      public Product(string code, string description, double salePrice, int qty)
      {
         this.Code = code;
         this.Description = description;
         this.SalePrice = salePrice;
         this.Qty = qty;
      }//ctor

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - //

      /// <summary>
      /// Constructor for notInStock products
      /// </summary>
      /// <param name="code"></param>
      /// <param name="salePrice"></param>
      /// <param name="qty"></param>
      public Product(string code, string description, int qty)
      {
         this.Code = code;
         this.Description = description;
         this.Qty = qty;
      }//ctor

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - //

      /// <summary>
      /// Constructor for stock products
      /// </summary>
      /// <param name="code"></param>
      /// <param name="salePrice"></param>
      /// <param name="qty"></param>
      public Product(string code, string description, int qty, double costPrice)
      {
         this.Code = code;
         this.Description = description;
         this.CostPrice = costPrice;
         this.Qty = qty;
      }//ctor

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - //

      public Product(string code, string description, double costPrice)
      {
         this.Code = code;
         this.Description = description;
         this.CostPrice = costPrice;
      }//ctor

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - //

      /// <summary>
      /// Constructor for building products that are used in the ProductList
      /// </summary>
      /// <param name="code"></param>
      /// <param name="description"></param>
      /// <param name="costPrice"></param>
      /// <param name="salePrice"></param>
      public Product(string code, string description, double costPrice, double salePrice)
      {
         this.Code = code;
         this.Description = description;
         this.CostPrice = costPrice;
         this.SalePrice = salePrice;
      }//ctor

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - //

      /// <summary>
      /// Constructor for stock products
      /// </summary>
      /// <param name="code"></param>
      /// <param name="salePrice"></param>
      /// <param name="qty"></param>
      public Product(string code, int qty, double costPrice)
      {
         this.Code = code;
         this.CostPrice = costPrice;
         this.Qty = qty;
      }//ctor 

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - //

      /// <summary>
      /// Constructor for copying products
      /// </summary>
      /// <param name="code"></param>
      /// <param name="salePrice"></param>
      /// <param name="qty"></param>
      public Product(Product product)
      {
         this.Code = product.Code;
         this.Description = product.Description;
         this.CostPrice = product.CostPrice;
         this.SalePrice = product.SalePrice;
         this.UsingPriceListPrice = product.UsingPriceListPrice;
         this.Qty = product.Qty;
      }//ctor

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - //

      /// <summary>
      /// Deserialization constructor
      /// </summary>
      /// <param name="info"></param>
      /// <param name="ctxt"></param>
      public Product(SerializationInfo info, StreamingContext ctxt)
      {
         this.Code = (string)info.GetValue("code", typeof(string));
         this.Description = (string)info.GetValue("description", typeof(string));
         this.CostPrice = (double)info.GetValue("costPrice", typeof(double));
         this.SalePrice = (double)info.GetValue("salePrice", typeof(double));
         this.Qty = (int)info.GetValue("qty", typeof(int));
         this.UsingPriceListPrice = (bool)info.GetValue("usingPriceListPrice", typeof(bool));
      }//ctor


      #endregion

      //----------------------------------------------------------------------------------------------//

      #region Properties
      public string Code { get; } = Settings.Default.NOT_FOUND;
      public string Description { get; } = Settings.Default.NOT_FOUND;
      public int Qty { get; set; }
      public double CostPrice { get; }
      public double SalePrice { get; set; }
      public bool UsingPriceListPrice { get; private set; }
      #endregion

      //----------------------------------------------------------------------------------------------//

      public override string ToString()
      {
         return "code: " + Code + " \r\n"
                + "description: " + Description + " \r\n"
                + "costPrice: " + CostPrice + " \r\n"
                + "salePrice: " + SalePrice + " \r\n"
                + "qty: " + Qty + "\r\n"
                + "Use list price: " + UsingPriceListPrice + "\r\n";
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
         info.AddValue("code", this.Code);
         info.AddValue("description", this.Description);
         info.AddValue("costPrice", this.CostPrice);
         info.AddValue("salePrice", this.SalePrice);
         info.AddValue("qty", this.Qty);
         info.AddValue("usingPriceListPrice", this.UsingPriceListPrice);
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
