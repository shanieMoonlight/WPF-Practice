
namespace PriceFinding
{
    interface IListReader
    {
        MyDictionary<Customer> ReadCustomerData();
        MyDictionary<Product> ReadProductData();
    }//Int
}//NS