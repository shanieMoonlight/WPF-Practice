using PriceFinding.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace PriceFinding
{
   [Serializable()]
   public class Sale : IComparable<Sale>, ISerializable
   {

      //----------------------------------------------------------------------------------------------------------//

      #region Properties
      public DateTime Date { get; }
      public double SalePrice { get; }
      public string Code { get; private set; } = Settings.Default.NOT_FOUND;
      public int Qty { get; private set; } = 1;
      #endregion

      //----------------------------------------------------------------------------------------------------------//

      #region Constructors
      public Sale()
      {
      }//ctor
      public Sale(DateTime date, double price)
      {
         this.Date = date;
         this.SalePrice = price;
      }//ctor 
      public Sale(DateTime date, double price, string code)
      {
         this.Date = date;
         this.SalePrice = price;
         this.Code = code;
      }//ctor 
      public Sale(DateTime date, double price, string code, int qty)
      {
         this.Date = date;
         this.SalePrice = price;
         this.Code = code;
         this.Qty = qty;
      }//ctor 

      /// <summary>
      /// Deserialization constructor
      /// </summary>
      /// <param name="info"></param>
      /// <param name="ctxt"></param>
      public Sale(SerializationInfo info, StreamingContext ctxt)
      {
         this.Date = (DateTime)info.GetValue("date", typeof(DateTime));
         this.SalePrice = (double)info.GetValue("salePrice", typeof(double));
         this.Code = (string)info.GetValue("code", typeof(string));
      }//ctor
      #endregion

      //----------------------------------------------------------------------------------------------------------//

      public int CompareTo(Sale sale)
      {
         DateTime thatDate = (DateTime)sale.Date;
         return -Date.CompareTo(thatDate);             // reverse the natural order so newest comes first
      }//CompareTo

      //----------------------------------------------------------------------------------------------------------//

      public override String ToString()
      {
         return Date.ToString("yyyy-MMM-dd") + ", " + SalePrice;
      }//ToString

      //----------------------------------------------------------------------------------------------------------//

      /// <summary>
      /// Used for serialization.
      /// </summary>
      /// <param name="info"></param>
      /// <param name="context"></param>
      public void GetObjectData(SerializationInfo info, StreamingContext context)
      {
         info.AddValue("date", this.Date);
         info.AddValue("salePrice", this.SalePrice);
         info.AddValue("code", this.Code);
      }//GetObjectData

   }//Class
}//NS
