using PriceFinding.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceFinding.Models
{
   public class Order
   {
      public Customer customer;
      public List<Product> productList = new List<Product>();
      public OrderType type;

      #region Constructors
      public Order()
      {
      }//ctor
      public Order(Customer customer, List<Product> productList)
      {
         this.customer = customer;
         this.productList = productList;
      }//ctor 
      #endregion

      
   }//Cls Order

   //=====================================================================================================================================//

   public enum OrderType
   {
      ORDER, QUOTE
   }//Enum


}//NS