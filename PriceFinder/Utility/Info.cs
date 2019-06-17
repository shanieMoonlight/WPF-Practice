
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceFinding.Utility
{
   class Info
   {
      public string Title { get; set; }
      public string Message { get; set; }

      //--------------------------------------------------------------------------//

      public Info(string title, string message)
      {
         Title = title;
         Message = message;
      }//ctor
      
      //--------------------------------------------------------------------------//

   }//Cls
}//NS
