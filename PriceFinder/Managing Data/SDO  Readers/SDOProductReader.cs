using PriceFinding.Properties;
using SageDataObject240;
using System.Runtime.InteropServices;

namespace PriceFinding.Managing_Data
{
   public class SDOProductReader
   {

      private static UserSettings sageUsrSet = UserSettings.Default;

      //Declare Sage Objects
      private SDOEngine sdo;
      private WorkSpace ws;
      private SalesRecord salesRecord;
      private StockRecord stockRecord;
      private CurrencyData currencyData;

      private readonly double baseCurrencyCode = (double)sageUsrSet.baseCurrCode;

      //-------------------------------------------------------------------------------------------------------//

      public SDOProductReader()
      {
         sdo = new SDOEngine();
         ws = (WorkSpace)sdo.Workspaces.Add("App Server Update");
         ws.Disconnect();
         ws.Connect(sageUsrSet.sageDBDir, sageUsrSet.sageUsername, sageUsrSet.sagePassword, "UniqueUpdater");
      }//ctor

      //-------------------------------------------------------------------------------------------------------//

      public double GetCostPrice(string prodCode)
      {
         //Create product with prodCode
         stockRecord = (StockRecord)ws.CreateObject("StockRecord");
         SDOHelper.Write(stockRecord, "STOCK_CODE", prodCode);

         var result = stockRecord.Find(false);

         return (double)SDOHelper.Read(stockRecord, "LAST_PURCHASE_PRICE");

      }//GetCostPrice

      //-------------------------------------------------------------------------------------------------------//

      /// <summary>
      /// Make sure all objects are released for garbage collection.
      /// </summary>
      public void DestroyAllObjects()
      {
         if (salesRecord != null)
         {
            Marshal.FinalReleaseComObject(salesRecord);
            salesRecord = null;
         }//If
         if (currencyData != null)
         {
            Marshal.FinalReleaseComObject(currencyData);
            currencyData = null;
         }//If
         if (stockRecord != null)
         {
            Marshal.FinalReleaseComObject(stockRecord);
            stockRecord = null;
         }//If

         if (ws != null)
         {
            ws.Disconnect();
            Marshal.FinalReleaseComObject(ws);
            ws = null;
         }//If

         if (sdo != null)
         {
            Marshal.FinalReleaseComObject(sdo);
            sdo = null;
         }//If

      }//DestroyAllObjects

      //-------------------------------------------------------------------------------------------------------//
   }//Cls
}//NS
