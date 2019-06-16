
using PriceFinding.Models;

namespace PriceFinding.Managing_Data.ReaderInterfaces
{
  public  interface IListReader 
    {
        MyDictionary<Customer> ReadCustomerData();
        MyDictionary<Product> ReadProductData();
    }//Int
}//NS