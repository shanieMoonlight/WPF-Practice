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

      private void LeftAlignedComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
      {
         var cb = sender as ComboBox;
         //var Prodrow = cb.Parent as 
         Console.WriteLine("Sup?  " + cb.IsDropDownOpen);
         if (cb.IsDropDownOpen)
            cb.MoveFocus(new TraversalRequest(FocusNavigationDirection.Right));
      }

      private void CbCustomerCode_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
      {
         var cb = sender as ComboBox;
         if (cb.IsDropDownOpen)
         {
            Console.WriteLine("Hello?  " + cb.IsDropDownOpen);
            e.Handled = true;
            //cb.MoveFocus(new TraversalRequest(FocusNavigationDirection.Right));
         }
      }

      //-------------------------------------------------------------------------------------------------------//


   }//Cls
}//NS
