using PriceFinding.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceFinding.Business_Objects
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

      public override string ToString()
      {
         StringBuilder prodListString = new StringBuilder("\r\n\tCode,\tQty,\tSale Price\tUsing Price List");

         if (productList != null)
         {
            for (int i = 0; i < productList.Count; i++)
            {
               prodListString.Append("\r\n\t" + productList[i].Code + ",\t" + productList[i].Qty + ",\t" + productList[i].SalePrice + ",\t\t" + productList[i].UsingPriceListPrice + ";");
            }//For 
         }//If

         string customerInfo;

         if (customer != null)
         {
            customerInfo = customer.Code + ",\r\n\t"
                           + customer.PoNumber + ",\r\n\t"
                           + customer.Address.Replace("\n", "\r\n");
         }
         else
         {
            customerInfo = Settings.Default.NOT_FOUND;
         }//Else

         return "Customer: \r\n\t" + customerInfo + "\r\n"
                 + "Product List: " + prodListString + "\r\n";
      }//ToString

   }//Cls Order

   //=====================================================================================================================================//

   public enum OrderType
   {
      ORDER, QUOTE
   }//Enum


}//NS