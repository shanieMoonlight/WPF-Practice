using NotifyChangeExample.ViewModels;
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

namespace NotifyChangeExample
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window
   {
      private MainViewModel _main = new MainViewModel();

      //----------------------------------------------------------------------//

      public MainWindow()
      {
         InitializeComponent();
         //Hook up the ViewModel
         DataContext = _main;
      }//ctor

      //----------------------------------------------------------------------//

      private void Green_Clicked(object sender, RoutedEventArgs e)
      {
         _main.SetBackground(new SolidColorBrush(Color.FromRgb(73,251,53)));
      }//Red_Clicked

      //----------------------------------------------------------------------//

      private void Blue_Clicked(object sender, RoutedEventArgs e)
      {
         _main.SetBackground(Brushes.DodgerBlue);
      }//Blue_Clicked

      //----------------------------------------------------------------------//

      private void Yellow_Clicked(object sender, RoutedEventArgs e)
      {
         _main.SetBackground(Brushes.Yellow);
      }//Yellow_Clicked

      //----------------------------------------------------------------------//

   }//Cls
}//NS
