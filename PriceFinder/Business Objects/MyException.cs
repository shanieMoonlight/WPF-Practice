using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceFinding
{
    class MyException : Exception
    {
        public MyException(string message)
            : base(message)
        {

        }//CTOR

        public MyException(Exception ex)
            : base("500 Internal Server Error. " + ex.Message, ex)
        {
        }//CTOR

        public MyException(string message, Exception ex)
            : base(message, ex)
        {
        }//CTOR


    }//Cls MyException


    class NotFromAppException : Exception
    {
        public NotFromAppException(string message)
            : base(message)
        {

        }//CTOR
    }//Cls NotFromAppException

}//NS
