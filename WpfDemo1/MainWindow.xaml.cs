using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PriceFinder
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window
   {

      List<Person> people;

      //----------------------------------------------------------------------------------//

      public MainWindow()
      {
         InitializeComponent();

         people = new List<Person>()
         {
            new Person("Tim", "Corey"),
            new Person("Joe", "Smith"),
            new Person("Sue", "Storm"),
         };

         cbPeople.ItemsSource = people;

      }//MainWindow

      //----------------------------------------------------------------------------------//

      private void ButtSubmit_Click(object sender, RoutedEventArgs e)
      {
         MessageBox.Show($"Hello {tbFirstName.Text} {tbLastName.Text}");
      }//ButtSubmit_Click

      //----------------------------------------------------------------------------------//

   }//Cls
}//NS
