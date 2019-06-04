using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace PriceFinding
{
    /// <summary>
    /// Class for storing All the data in one place.
    /// </summary>
    [Serializable()]
    class DataStorage : ISerializable
    {
        public MyDictionary<Customer> CustomerMap;
        public MyDictionary<Product> ProductMap;
        public MyDictionary<MyDictionary<List<Sale>>> CustomerActivity;
        public MyDictionary<MyDictionary<double>> PriceListActivity;

        //------------------------------------------------------------------------------------------------------------------//

        #region constructors
        public DataStorage()
        {
            CustomerMap = new MyDictionary<Customer>();
            ProductMap = new MyDictionary<Product>();
            CustomerActivity = new MyDictionary<MyDictionary<List<Sale>>>();
            PriceListActivity = new MyDictionary<MyDictionary<double>>();
        }//ctor
        public DataStorage(MyDictionary<Customer> CustomerMap, MyDictionary<Product> ProductMap, MyDictionary<MyDictionary<List<Sale>>> customerActivity, MyDictionary<MyDictionary<double>> priceListActivity)
        {
            this.CustomerMap = CustomerMap;
            this.ProductMap = ProductMap;
            this.CustomerActivity = customerActivity;
            this.PriceListActivity = priceListActivity;
        }//ctor
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="ctxt"></param>
        public DataStorage(SerializationInfo info, StreamingContext ctxt)
        {
            this.CustomerMap = (MyDictionary<Customer>)info.GetValue("CustomerMap", typeof(MyDictionary<Customer>));
            this.ProductMap = (MyDictionary<Product>)info.GetValue("ProductMap", typeof(MyDictionary<Product>));
            this.CustomerActivity = (MyDictionary<MyDictionary<List<Sale>>>)info.GetValue("CustomerActivity", typeof(MyDictionary<MyDictionary<List<Sale>>>));
            this.PriceListActivity = (MyDictionary<MyDictionary<double>>)info.GetValue("PriceListActivity", typeof(MyDictionary<MyDictionary<double>>));

        }//ctor 
        #endregion

        //------------------------------------------------------------------------------------------------------------------//

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("CustomerMap", this.CustomerMap);
            info.AddValue("ProductMap", this.ProductMap);
            info.AddValue("CustomerActivity", this.CustomerActivity);
            info.AddValue("PriceListActivity", this.PriceListActivity);
        }//GetObjectData
    }//Cls DataStorage
}//NS
