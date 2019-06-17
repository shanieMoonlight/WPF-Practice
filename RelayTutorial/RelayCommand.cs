using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RelayTutorial
{
   public class RelayCommand : ICommand
   {
      private readonly Action<object> _execute;
      private readonly Predicate<object> _canExecute;

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -//

      public event EventHandler CanExecuteChanged
      {
         add { CommandManager.RequerySuggested += value; }
         remove { CommandManager.RequerySuggested -= value; }
      }//CanExecuteChanged

      //---------------------------------------------------------------//

      public RelayCommand(Action<object> execute, Predicate<object> canExecute)
      {
         _execute = execute ?? throw new NullReferenceException("execute");

         _canExecute = canExecute;
      }//ctor

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -//

      public RelayCommand(Action<object> execute) : this(execute, null)
      {
      }//ctor

      //---------------------------------------------------------------//

      public bool CanExecute(object parameter)
      {
         return _canExecute == null ? true : _canExecute(parameter);
      }//CanExecute

      //---------------------------------------------------------------//

      public void Execute(object parameter)
      {
         _execute.Invoke(parameter);
      }//Execute

      //---------------------------------------------------------------//

   }//Cls
}//NS
