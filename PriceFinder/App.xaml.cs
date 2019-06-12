using PriceFinding.Managing_Data.ODBC_Readers;
using PriceFinding.Managing_Data.ReaderInterfaces;
using System.Windows;
using Unity;

namespace PriceFinding
{
   /// <summary>
   /// Interaction logic for App.xaml
   /// </summary>
   public partial class App : Application
   {


      protected override void OnStartup(StartupEventArgs e)
      {
         IUnityContainer container = new UnityContainer();
         container.RegisterType<IListReader, ODBCListReader>();
         container.RegisterType<IInvoiceReader, ODBCInvoiceReader>();
         container.RegisterType<IPriceListReader, ODBCPriceListReader>();
      }//OnStartup


   }//Cls
}//NS
