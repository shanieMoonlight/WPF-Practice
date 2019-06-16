using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using PriceFinding.Properties;

namespace PriceFinding.Models
{
   [Serializable()]
   public class Customer : ISerializable
   {
      public const string DEF_ADDRESS = "Using Default Address";


      //-------------------------------------------------------------------------------------------------------//

      #region Constructors
      public Customer()
      {
      }//ctor

      public Customer(string code, string description)
      {
         this.Code = code;
         this.Description = description;
      }//ctor 

      public Customer(string code, string description, double xRate)
      {
         this.Code = code;
         this.Description = description;
         this.XRate = xRate;
      }//ctor 

      /// <summary>
      /// Deserialization constructor
      /// </summary>
      /// <param name="info"></param>
      /// <param name="ctxt"></param>
      public Customer(SerializationInfo info, StreamingContext ctxt)
      {
         this.Code = (string)info.GetValue("code", typeof(string));
         this.Description = (string)info.GetValue("description", typeof(string));
         this.XRate = (double)info.GetValue("xRate", typeof(double));
      }//ctor
      #endregion

      //-------------------------------------------------------------------------------------------------------//

      #region Properties
      public string Code { get; } = Settings.Default.NOT_FOUND;
      public string Description { get; } = Settings.Default.NOT_FOUND;
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
         info.AddValue("code", this.Code);
         info.AddValue("description", this.Description);
         info.AddValue("xRate", this.XRate);
      }//GetObjectData

   }//Cls
}//NS
