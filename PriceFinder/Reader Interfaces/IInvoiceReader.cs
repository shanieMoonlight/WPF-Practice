using System.Collections.Generic;

namespace PriceFinding
{
    interface IInvoiceReader
    {
        MyDictionary<MyDictionary<List<Sale>>> ReadInvoiceData();
    }//Int
}//NS
