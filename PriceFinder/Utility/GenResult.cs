using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceFinding.Utility
{
   /// <summary>
   /// Class for encapsulating the results of auth maneuvers. 
   /// </summary>
   /// <typeparam name="T">The payload on a successful result</typeparam>
   public class GenResult<T>
   {
      //-------------------------------------------------------------------------------------------------------//

      /// <summary>
      /// Create empty result
      /// </summary>
      public GenResult() { } //Ctor

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -//

      /// <summary>
      /// Create successful result
      /// </summary>
      /// <param name="value">Payload</param>
      public GenResult(T value)
      {
         this.Value = value;
         this.Succeeded = true;
      } //Ctor

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -//

      /// <summary>
      /// Create successful result
      /// </summary>
      /// <param name="value">Payload</param>
      /// <param name="info">Extra Info</param>
      public GenResult(T value, string info)
      {
         this.Value = value;
         this.Succeeded = true;
         this.Info = info;
      } //Ctor

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -//

      /// <summary>
      /// Create failed result
      /// </summary>
      /// <param name="exception">Exception detailing failure</param>
      public GenResult(Exception exception)
      {
         this.Succeeded = false;
         this.Exception = exception;
      } //Ctor

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -//

      /// <summary>
      /// Create result 
      /// </summary>
      /// <param name="succeeded"></param>
      /// <param name="info"></param>
      public GenResult(bool succeeded, string info)
      {
         this.Succeeded = succeeded;
         this.Info = info;
      } //Ctor

      //-------------------------------------------------------------------------------------------------------//

      /// <summary>
      /// Whether the Validation succeeded or failed.
      /// </summary>
      public bool Succeeded { get; protected set; }

      /// <summary>
      /// Details of validatin failure.
      /// </summary>
      public Exception Exception { get; private set; }

      /// <summary>
      /// Any extra info needed.
      /// </summary>
      public string Info { get; private set; }

      /// <summary>
      /// The payload
      /// </summary>
      public T Value { get; private set; }

      //-------------------------------------------------------------------------------------------------------//


   } //Cls

} //NS