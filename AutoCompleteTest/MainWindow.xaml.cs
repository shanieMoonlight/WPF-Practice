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
using System.Xml;

namespace AutoCompleteTest
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window
   {
      public MainWindow()
      {
         InitializeComponent();
      }//ctor

      List<string> TestItems = new List<string>()
      {
            "The badger knows something",
            "Your head looks something like a pineapple",
            "Crazy like a box of green frogs",
            "The billiard table has green cloth",
            "The sky is blue",
            "We're going to need some golf shoes",
            "This is going straight to the pool room",
            "We're going to  Bonnie Doon",
            "Spring forward - Fall back",
             "Gerry had a plan which involved telling all",
             "When is the summer coming",
             "Take you time and tell me what you saw",
             "All hands on deck"
      };


   }//Cls
}//NS
