using PriceFinding.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace PriceFinding
{
   [Serializable()]
   public class Product : IComparable<Product>, ISerializable
   {
      private string code = Settings.Default.NOT_FOUND;
      private string description = Settings.Default.NOT_FOUND;
      private double costPrice;
      private double salePrice;
      private bool usingPriceListPrice;
      private int qty;

      #region Constructors
      public Product()
      {
      }//ctor
       /// <summary>
       /// Constructor for order products
       /// </summary>
       /// <param name="code"></param>
       /// <param name="description"></param>
      public Product(string code, string description)
      {
         this.code = code;
         this.description = description;
      }//ctor
       /// <summary>
       /// Constructor for order products
       /// </summary>
       /// <param name="code"></param>
       /// <param name="salePrice"></param>
       /// <param name="qty"></param>
      public Product(string code, double salePrice, int qty)
      {
         this.code = code;
         this.salePrice = salePrice;
         this.qty = qty;
      }//ctor
       /// <summary>
       /// Constructor for order products
       /// </summary>
       /// <param name="code"></param>
       /// <param name="salePrice"></param>
       /// <param name="qty"></param>
      public Product(string code, string description, double salePrice, int qty)
      {
         this.code = code;
         this.description = description;
         this.salePrice = salePrice;
         this.qty = qty;
      }//ctor
       /// <summary>
       /// Constructor for notInStock products
       /// </summary>
       /// <param name="code"></param>
       /// <param name="salePrice"></param>
       /// <param name="qty"></param>
      public Product(string code, string description, int qty)
      {
         this.code = code;
         this.description = description;
         this.qty = qty;
      }//ctor
       /// <summary>
       /// Constructor for stock products
       /// </summary>
       /// <param name="code"></param>
       /// <param name="salePrice"></param>
       /// <param name="qty"></param>
      public Product(string code, string description, int qty, double costPrice)
      {
         this.code = code;
         this.description = description;
         this.costPrice = costPrice;
         this.qty = qty;
      }//ctor
      public Product(string code, string description, double costPrice)
      {
         this.code = code;
         this.description = description;
         this.costPrice = costPrice;
      }//ctor
       /// <summary>
       /// Constructor for building products that are used in the ProductList
       /// </summary>
       /// <param name="code"></param>
       /// <param name="description"></param>
       /// <param name="costPrice"></param>
       /// <param name="salePrice"></param>
      public Product(string code, string description, double costPrice, double salePrice)
      {
         this.code = code;
         this.description = description;
         this.costPrice = costPrice;
         this.salePrice = salePrice;
      }//ctor
       /// <summary>
       /// Constructor for stock products
       /// </summary>
       /// <param name="code"></param>
       /// <param name="salePrice"></param>
       /// <param name="qty"></param>
      public Product(string code, int qty, double costPrice)
      {
         this.code = code;
         this.costPrice = costPrice;
         this.qty = qty;
      }//ctor 
       /// <summary>
       /// Constructor for copying products
       /// </summary>
       /// <param name="code"></param>
       /// <param name="salePrice"></param>
       /// <param name="qty"></param>
      public Product(Product product)
      {
         this.code = product.code;
         this.description = product.description;
         this.costPrice = product.costPrice;
         this.salePrice = product.salePrice;
         this.usingPriceListPrice = product.usingPriceListPrice;
         this.qty = product.qty;
      }//ctor

      /// <summary>
      /// Deserialization constructor
      /// </summary>
      /// <param name="info"></param>
      /// <param name="ctxt"></param>
      public Product(SerializationInfo info, StreamingContext ctxt)
      {
         this.code = (string)info.GetValue("code", typeof(string));
         this.description = (string)info.GetValue("description", typeof(string));
         this.costPrice = (double)info.GetValue("costPrice", typeof(double));
         this.salePrice = (double)info.GetValue("salePrice", typeof(double));
         this.qty = (int)info.GetValue("qty", typeof(int));
         this.usingPriceListPrice = (bool)info.GetValue("usingPriceListPrice", typeof(bool));
      }//ctor


      #endregion

      #region Properties
      public string Code
      {
         get { return code; }
      }//Code 
      public string Description
      {
         get { return description; }
      }//Description 
      public int Qty
      {
         get { return qty; }
         set { qty = value; }
      }//Qty
      public double CostPrice
      {
         get { return costPrice; }
      }//CostPrice 
      public double SalePrice
      {
         get { return salePrice; }
         set { salePrice = value; }
      }//SalePrice 
      public bool UsingPriceListPrice
      {
         get { return usingPriceListPrice; }
      }//SalePrice 
      #endregion

      public override string ToString()
      {
         return "code: " + code + " \r\n"
                + "description: " + description + " \r\n"
                + "costPrice: " + costPrice + " \r\n"
                + "salePrice: " + salePrice + " \r\n"
                + "qty: " + qty + "\r\n"
                + "Use list price: " + usingPriceListPrice + "\r\n";
      }//ToString

      public string ToStringStockLevel()
      {
         return "code: " + code + " \r\n"
                + "description: " + description + " \r\n"
                + "qty: " + qty + "\r\n";
      }//ToString

      /// <summary>
      /// Used for serialization.
      /// </summary>
      /// <param name="info"></param>
      /// <param name="context"></param>
      public void GetObjectData(SerializationInfo info, StreamingContext context)
      {
         info.AddValue("code", this.code);
         info.AddValue("description", this.description);
         info.AddValue("costPrice", this.costPrice);
         info.AddValue("salePrice", this.salePrice);
         info.AddValue("qty", this.qty);
         info.AddValue("usingPriceListPrice", this.usingPriceListPrice);
      }//GetObjectData

      /// <summary>
      /// Used when sorting a list of products.
      /// </summary>
      /// <param name="product"></param>
      /// <returns></returns>
      public int CompareTo(Product product)
      {
         string thatCode = product.code;
         return code.CompareTo(thatCode);
      }//CompareTo

   }//Cls Product

}//NS
