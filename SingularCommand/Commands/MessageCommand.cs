using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SingularCommand.Commands
{
  public class MessageCommand : ICommand
   {
      public event EventHandler CanExecuteChanged;
      private Action _execute;

      //-------------------------------------------------------------------------//

      public MessageCommand(Action execute)
      {
         _execute = execute;
      }//ctor

      //-------------------------------------------------------------------------//

      public bool CanExecute(object parameter)
      {
         return true;
      }//CanExecute

      //-------------------------------------------------------------------------/

      public void Execute(object parameter)
      {
         _execute.Invoke(); 
      }//Execute

      //-------------------------------------------------------------------------//

   }//Cls
}//NS
