using BespokeFusion;
using PriceFinding.Managing_Data;
using PriceFinding.Managing_Data.ODBC_Readers;
using PriceFinding.Models;
using PriceFinding.Properties;
using PriceFinding.ViewModels;
using PriceFinding.Writing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;

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

      private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
      {
         Regex regex = new Regex("[^0-9]+");
         e.Handled = regex.IsMatch(e.Text);
      }//NumberValidationTextBox


      //-------------------------------------------------------------------------------------------------------//

     
   
   }//Cls
}//NS
