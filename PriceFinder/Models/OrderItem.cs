using PriceFinding.Properties;

namespace PriceFinding.Models
{
   /// <summary>
   /// Base class for Customer & Product
   /// </summary>
   public class OrderItem
   {

      //-------------------------------------------------------------------------------------------------------//

      public OrderItem(string code, string description)
      {
         this.Code = code;
         this.Description = description;
      }//ctor 


      //-------------------------------------------------------------------------------------------------------//

      #region Properties
      public string Code { get; } = Settings.Default.NOT_FOUND;
      public string Description { get; } = Settings.Default.NOT_FOUND;
      #endregion

      //-------------------------------------------------------------------------------------------------------//

      public override string ToString()
      {
         return Code + " - " + Description;
      }//ToString

      //-------------------------------------------------------------------------------------------------------//


   }//Cls
}//NS
