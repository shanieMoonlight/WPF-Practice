using SageDataObject240;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;


namespace PriceFinding
{
   public static class SDOHelper
   {

      /// <summary>
      /// Writes to an SDO field
      /// </summary>
      /// <param name="oObject">The object to write to</param>
      /// <param name="fname">Name of the required field</param>
      /// <param name="value">Value you wish to write</param>
      public static void Write(object oObject, string fname, object value)
      {
         //Stores the required field name in an object
         Fields fields = GetFields(oObject);
         Field field = GetField(fields, fname);
         field.Value = value;
         Marshal.FinalReleaseComObject(field);
         Marshal.FinalReleaseComObject(fields);
      }//Write

      //------------------------------------------------------------------------------------------------------------------//

      /// <summary>
      /// Reads and SDO field and returns its value as an object
      /// </summary>
      /// <param name="oObject">The object to read from</param>
      /// <param name="fname">Name of the required field</param>
      /// <returns>Returns an object containing the value from the field</returns>
      public static object Read(object oObject, string fname)
      {
         //Stores the required field name in an object
         Fields fields = GetFields(oObject);
         Field field = GetField(fields, fname);

         object value = field.Value;
         Marshal.FinalReleaseComObject(field);
         Marshal.FinalReleaseComObject(fields);

         return value;
      }//Read

      //------------------------------------------------------------------------------------------------------------------//

      public static Field GetField(Fields fields, string fName)
      {
         object fieldName = fName;
         return fields.Item(ref fieldName);
      }//GetField

      //------------------------------------------------------------------------------------------------------------------//

      public static Fields GetFields(object oObject)
      {
         return (Fields)oObject.GetType().InvokeMember("Fields", System.Reflection.BindingFlags.GetProperty, null, oObject, null);
      }//GetFields

      //------------------------------------------------------------------------------------------------------------------//

      /// <summary>
      /// Invokes the Add() method of an items collection
      /// </summary>
      /// <param name="oObject">The items collection you wish to invoke the Add() method on</param>
      /// <returns></returns>
      public static object Add(Object oObject)
      {
         //Uses reflection to invoke the Add() Method on the required object
         return oObject.GetType().InvokeMember("Add", System.Reflection.BindingFlags.InvokeMethod, null, oObject, null);
      } //Add

   }//Cls
}//NS