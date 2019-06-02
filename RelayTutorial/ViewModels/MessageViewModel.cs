using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RelayTutorial.ViewModels
{
   public class MessageViewModel
   {
      private const string CONSOLE_MESSAGE = "I'm a console";
      private const string BOX_MESSAGE = "I'm a message box!";
      public ObservableCollection<string> MyMessages { get; private set; }
      public RelayCommand MessageBoxCommand { get; private set; }
      public RelayCommand ConsoleCommand { get; private set; }

      //-------------------------------------------------------------------------------------//

      public MessageViewModel()
      {
         MyMessages = new ObservableCollection<string>()
         {
            "Hello World!",
            "My name is Joe",
            "I love learning Commands",
            BOX_MESSAGE,
            CONSOLE_MESSAGE
         };

         MessageBoxCommand = new RelayCommand(DisplayMessageBox, MessageBoxCanUse);
         ConsoleCommand = new RelayCommand(DisplayInConsole, ConsoleCanUse);
      }//ctor

      //-------------------------------------------------------------------------------------//

      public void DisplayMessageBox(object message)
      {
         MessageBox.Show(message as string);
      }//DisplayMessageBox

      //-------------------------------------------------------------------------------------//

      public void DisplayInConsole(object message)
      {
         Console.WriteLine(message as string);
      }//DisplayInConsole

      //-------------------------------------------------------------------------------------//

      public bool MessageBoxCanUse(object message)
      {
         if ((string)message == CONSOLE_MESSAGE)
            return false;
         else
            return true;
      }//MessageBoxCanUse

      //-------------------------------------------------------------------------------------//

      public bool ConsoleCanUse(object message)
      {
         if ((string)message == BOX_MESSAGE)
            return false;
         else
            return true;
      }//ConsoleCanUse

      //-------------------------------------------------------------------------------------//

   }//Cls
}//NS
