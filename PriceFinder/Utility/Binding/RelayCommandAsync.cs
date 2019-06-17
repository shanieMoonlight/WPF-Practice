using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PriceFinding.Utility.Binding
{
   class RelayCommandAsync : ICommand
   {
      private readonly Func<object, Task> _execute;
      private readonly Predicate<object> _canExecute;

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -//

      public event EventHandler CanExecuteChanged
      {
         add { CommandManager.RequerySuggested += value; }
         remove { CommandManager.RequerySuggested -= value; }
      }//CanExecuteChanged

      //---------------------------------------------------------------//

      public RelayCommandAsync(Func<object, Task> execute, Predicate<object> canExecute)
      {
         _execute = execute ?? throw new NullReferenceException("execute");

         _canExecute = canExecute;
      }//ctor

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -//

      public RelayCommandAsync(Func<object, Task> execute) : this(execute, null)
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