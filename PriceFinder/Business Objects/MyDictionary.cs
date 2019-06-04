using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceFinding
{
    /// <summary>
    /// A Dictionary thatis always case invariant (For use with JSONDeserializer).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MyDictionary<T> : Dictionary<string, T>
    {

        public MyDictionary(IEqualityComparer<string> comparer)
            : base(comparer)
        {

        }//CTOR

        public MyDictionary()
            : base(StringComparer.InvariantCultureIgnoreCase)
        {

        }//CTOR

    }//Class
}//NS
