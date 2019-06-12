using System.Data;
using System.Data.Odbc;
using System.Linq;

namespace PriceFinding.Managing_Data.ODBC_Readers
{
   public abstract class AODBCReader
   {

      //-------------------------------------------------------------------------------------------------------//

      /// <summary>
      /// Connects to File/Sheet and fills and returns a DataTable of the values.
      /// Table will be empty if an exception occured.
      /// </summary>
      /// <param name="queryString"></param>
      /// <returns></returns>
      protected DataTable FillTableFromODBC(string queryString)
      {
         DataTable table = new DataTable();
         using (OdbcConnection connection = new OdbcConnection(DataManager.connString))
         {
            using (OdbcDataAdapter adapter = new OdbcDataAdapter(queryString, connection))
            {
               connection.Open();
               adapter.Fill(table);
            }//Using
         }//Using

         return table;
      }//FillTableFromODBC

      //-------------------------------------------------------------------------------------------------------//

   }//Cls
}//NS





//Schema queries
//var dtCols = connection.GetSchema("Columns", new[] { connection.DataSource, null, "STKINDEX" });
//var possiblities = dtCols.AsEnumerable().Where(r => r.ItemArray[3].ToString().Contains("LAST"));
