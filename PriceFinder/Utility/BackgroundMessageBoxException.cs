using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceFinding
{
   class BackgroundMessageBoxException : Exception
   {

      public string Title { get; set; }

      //---------------------------------------------------------------------------------//

      public BackgroundMessageBoxException(string title, string message) : base(message)
      {
         Title = title;
      }//ctor

      //---------------------------------------------------------------------------------//

   }//Cls
}//NS
