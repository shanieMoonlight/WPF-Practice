using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using PriceFinding.Properties;

namespace PriceFinding
{
   [Serializable()]
   public class Customer : ISerializable
   {
      private string code = Settings.Default.NOT_FOUND;
      private string description = Settings.Default.NOT_FOUND;
      private string poNumber;
      private string address;
      public const string DEF_ADDRESS = "Using Default Address";

      private double xRate = 1;


      //-------------------------------------------------------------------------------------------------------//

      #region Constructors
      public Customer()
      {
      }//ctor

      public Customer(string code, string description)
      {
         this.code = code;
         this.description = description;
      }//ctor 

      public Customer(string code, string description, double xRate)
      {
         this.code = code;
         this.description = description;
         this.xRate = xRate;
      }//ctor 

      /// <summary>
      /// Deserialization constructor
      /// </summary>
      /// <param name="info"></param>
      /// <param name="ctxt"></param>
      public Customer(SerializationInfo info, StreamingContext ctxt)
      {
         this.code = (string)info.GetValue("code", typeof(string));
         this.description = (string)info.GetValue("description", typeof(string));
         this.xRate = (double)info.GetValue("xRate", typeof(double));
      }//ctor
      #endregion

      //-------------------------------------------------------------------------------------------------------//

      #region Properties
      public string Code
      {
         get { return code; }
      }//Code 
      public string Description
      {
         get { return description; }
      }//Description 
      public string PoNumber
      {
         get { return poNumber; }
      }//PoNumber 
      public string Address
      {
         get { return address; }
      }//Address 
      public double XRate
      {
         get { return xRate; }
         set { xRate = value; }
      }//XRate
      #endregion

      //-------------------------------------------------------------------------------------------------------//

      public override string ToString()
      {
         return "code: " + code + " \r\n"
                + "description: " + description + " \r\n";
      }//ToString

      //-------------------------------------------------------------------------------------------------------//

      /// <summary>
      /// Used for serialization.
      /// </summary>
      /// <param name="info"></param>
      /// <param name="context"></param>
      public void GetObjectData(SerializationInfo info, StreamingContext context)
      {
         info.AddValue("code", this.code);
         info.AddValue("description", this.description);
         info.AddValue("xRate", this.xRate);
      }//GetObjectData

   }//Cls
}//NS
