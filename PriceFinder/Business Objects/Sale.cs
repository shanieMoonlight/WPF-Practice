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
        private DateTime date;
        private double salePrice;
        private string code = Settings.Default.NOT_FOUND;
        //----------------------------------------------------------------------------------------------------------//

        #region Properties
        public DateTime Date
        {
            get { return date; }
        }//Date
        public double SalePrice
        {
            get { return salePrice; }
        }//Price 
        public string Code
        {
            get { return code; }
        }//Code 
        #endregion

        //----------------------------------------------------------------------------------------------------------//

        #region Constructors
        public Sale()
        {
        }//ctor
        public Sale(DateTime date, double price)
        {
            this.date = date;
            this.salePrice = price;
        }//ctor 
        public Sale(DateTime date, double price, string code)
        {
            this.date = date;
            this.salePrice = price;
            this.code = code;
        }//ctor 

        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="ctxt"></param>
        public Sale(SerializationInfo info, StreamingContext ctxt)
        {
            this.date = (DateTime)info.GetValue("date", typeof(DateTime));
            this.salePrice = (double)info.GetValue("salePrice", typeof(double));
            this.code = (string)info.GetValue("code", typeof(string));
        }//ctor
        #endregion

        //----------------------------------------------------------------------------------------------------------//

        public int CompareTo(Sale sale)
        {
            DateTime thatDate = (DateTime)sale.Date;
            return -date.CompareTo(thatDate);             // reverse the natural order so newest comes first
        }//CompareTo
        
        //----------------------------------------------------------------------------------------------------------//

        public override String ToString()
        {
            return date.ToString("yyyy-MMM-dd") + ", " + salePrice;
        }//ToString

        //----------------------------------------------------------------------------------------------------------//

        /// <summary>
        /// Used for serialization.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("date", this.date);
            info.AddValue("salePrice", this.salePrice);
            info.AddValue("code", this.code);
        }//GetObjectData

    }//Class
}//NS
