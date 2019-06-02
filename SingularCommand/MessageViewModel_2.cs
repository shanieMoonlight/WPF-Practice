using SingularCommand.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SingularCommand
{
   /// <summary>
   /// This on e doesn't use a MessageText string property. We'll pass it in from the XAML
   /// </summary>
   class MessageViewModel_2
   {
      public MessageCommand2 DisplayMessageCommand { get; private set; }

      //-------------------------------------------------------------------------//

      public MessageViewModel_2()
      {
         DisplayMessageCommand = new MessageCommand2(DisplayMessage);
      }//ctor

      //-------------------------------------------------------------------------//

      public void DisplayMessage(string message)
      {
         MessageBox.Show(message);
      }//DisplayMessage

      //-------------------------------------------------------------------------/

   }//Cls
}//NS
