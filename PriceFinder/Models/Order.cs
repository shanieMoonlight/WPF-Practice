using System.Collections.Generic;

namespace PriceFinding.Models
{
   public class Order
   {
      public Customer Customer;
      public List<Product> ProductList = new List<Product>();
      public OrderType type;

      public string Notes;
      public string TakenBy;
      public string CustomerOrderNumber;
      public double Carriage;
      public Address DeliveryAddress;

      //--------------------------------------------------------------------------------------------------//

      #region Constructors
      public Order()
      {
      }//ctor
      public Order(Customer customer, List<Product> productList)
      {
         Customer = customer;
         ProductList = productList;
         DeliveryAddress = new Address();
      }//ctor 
      #endregion

      //--------------------------------------------------------------------------------------------------//


   }//Cls Order

   //=====================================================================================================================================//

   public enum OrderType
   {
      ORDER, QUOTE
   }//Enum

   //=====================================================================================================================================//

}//NS