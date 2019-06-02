using SingularCommand.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SingularCommand
{
   class MessageViewModel
   {

      public string MessageText { get; set; }
      public MessageCommand DisplayMessageCommand { get; private set; }

      //-------------------------------------------------------------------------//

      public MessageViewModel()
      {
         DisplayMessageCommand = new MessageCommand(DisplayMessage); 
      }//ctor

      //-------------------------------------------------------------------------//

      public void DisplayMessage()
      {
         MessageBox.Show(MessageText);
      }//DisplayMessage

      //-------------------------------------------------------------------------/

   }//Cls
}//NS
