using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace PriceFinder
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window
   {

      private const int INITIAL_ROW_COUNT = 10;

      //-------------------------------------------------------------------------------------------------------//

      public MainWindow()
      {
         InitializeComponent();

       
         SetInitialProductRows();
      }//ctor

      //-------------------------------------------------------------------------------------------------------//

      private void ButtOrder_Click(object sender, RoutedEventArgs e)
      {
       

      }//ButtOrder_Click

      //-------------------------------------------------------------------------------------------------------//

      private void SetInitialProductRows()
      {
         // "INITIAL_ROW_COUNT - 1" because we're starting with on row already built
         for (int i = 0; i < INITIAL_ROW_COUNT - 1; i++)
         {
            AddProductRow();
         }//for

      }//SetProductRows

       //-------------------------------------------------------------------------------------------------------//

      private void AddProductRow()
      {
            var rows = grdProducts.RowDefinitions;
            rows.Add(new RowDefinition());
            var gridRow = rows.Count - 1;
            //  var newControl = new Grid { DataContext = grdProductsRow.DataContext };

            string gridXaml = XamlWriter.Save(grdProductsRow);
            // Load it into a new object:

            StringReader stringReader = new StringReader(gridXaml);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            var newControl = (Grid)XamlReader.Load(xmlReader);

            Grid.SetRow(newControl, gridRow);
            grdProducts.Children.Add(newControl);

      }//SetProductRows

      //-------------------------------------------------------------------------------------------------------//

      private void RemoveProductRow()
      {
         var rows = grdProducts.RowDefinitions;
         if (rows.Count < 2)
            return;

         var lastRowIdx = rows.Count - 1;
        // grdProducts.Children.RemoveAt(3);

         rows.RemoveAt(rows.Count - 1);

      }//SetProductRows

      //-------------------------------------------------------------------------------------------------------//

      private void FabAddProductrow_Click(object sender, RoutedEventArgs e)
      {
         AddProductRow();
      }//FabAddProductrow_Click

      //-------------------------------------------------------------------------------------------------------//

      private void FabRemoveProductRow_Click(object sender, RoutedEventArgs e)
      {
         RemoveProductRow();
      }//FabRemoveProductRow_Click

      //-------------------------------------------------------------------------------------------------------//
   }//Cls
}//NS
