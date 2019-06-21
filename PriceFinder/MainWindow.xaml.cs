using PriceFinding.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PriceFinding
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window
   {

      private MainViewModel _mainViewModel;


      //-------------------------------------------------------------------------------------------------------//

      public MainWindow()
      {

         InitializeComponent();

         try
         {
            _mainViewModel = new MainViewModel();
            DataContext = _mainViewModel;

         }
         catch (BackgroundMessageBoxException mbe)
         {
            MyMessageBox.ShowOk(mbe.Title, mbe.Message);
         }
         catch (Exception e)
         {
            MyMessageBox.ShowOk("Error", e.Message);
         }//catch

      }//ctor



      //-------------------------------------------------------------------------------------------------------//


   }//Cls
}//NS
