using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using PriceFinding.Properties;

namespace PriceFinding.Models
{
   [Serializable()]
   public class Customer : OrderItem, ISerializable
   {
      public const string DEF_ADDRESS = "Using Default Address";


      //-------------------------------------------------------------------------------------------------------//

      #region Constructors
      public Customer() : base(null, null)
      {
      }//ctor

      public Customer(string code, string description) : base(code, description)
      {
      }//ctor 

      public Customer(string code, string description, double xRate) : base(code, description)
      {
         XRate = xRate;
      }//ctor 

      /// <summary>
      /// Deserialization constructor
      /// </summary>
      /// <param name="info"></param>
      /// <param name="ctxt"></param>
      public Customer(SerializationInfo info, StreamingContext ctxt) : base((string)info.GetValue("code", typeof(string)), (string)info.GetValue("description", typeof(string)))
      {
         XRate = (double)info.GetValue("xRate", typeof(double));
      }//ctor
      #endregion

      //-------------------------------------------------------------------------------------------------------//

      #region Properties
      public string PoNumber { get { return PoNumber1; } }
      public string Address { get; }
      public double XRate { get; set; } = 1;
      public string PoNumber1 { get; }
      #endregion

      //-------------------------------------------------------------------------------------------------------//

      public override string ToString()
      {
         return Code + " - " + Description;
      }//ToString

      //-------------------------------------------------------------------------------------------------------//

      /// <summary>
      /// Used for serialization.
      /// </summary>
      /// <param name="info"></param>
      /// <param name="context"></param>
      public void GetObjectData(SerializationInfo info, StreamingContext context)
      {
         info.AddValue("code", Code);
         info.AddValue("description", Description);
         info.AddValue("xRate", XRate);
      }//GetObjectData

   }//Cls
}//NS
