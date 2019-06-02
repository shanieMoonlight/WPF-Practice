using IntroTutorial.Models;
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

namespace IntroTutorial
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window
   {
      public MainWindow()
      {
         InitializeComponent();
         MyModelObject butt1DataContext = new MyModelObject() { Name = "I'm button 1" };
         MyModelObject butt2DataContext = new MyModelObject() { Name = "I'm button 2" };

         butt1.DataContext = butt1DataContext;
         butt2.DataContext = butt2DataContext;
      }//ctor

      //------------------------------------------------------------------------------------------//

      private void Window_Loaded(object sender, RoutedEventArgs e)
      {
         gpMain.Background = Brushes.Fuchsia;
         gpMain.Margin = new Thickness(10,0,10,10);
      }//Window_Loaded

      //------------------------------------------------------------------------------------------//

   }//Cls
}//NS
