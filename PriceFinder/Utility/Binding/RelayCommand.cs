using System;
using System.Windows.Input;

namespace PriceFinding.Utility.Binding
{
   public class RelayCommand : ICommand
   {
      private readonly Action _execute;
      private readonly Predicate<object> _canExecute;

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -//

      public event EventHandler CanExecuteChanged
      {
         add { CommandManager.RequerySuggested += value; }
         remove { CommandManager.RequerySuggested -= value; }
      }//CanExecuteChanged

      //---------------------------------------------------------------//

      public RelayCommand(Action execute, Predicate<object> canExecute)
      {
         _execute = execute ?? throw new NullReferenceException("execute");

         _canExecute = canExecute;
      }//ctor

      //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -//

      public RelayCommand(Action execute) : this(execute, null)
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
         _execute.Invoke();
      }//Execute

      //---------------------------------------------------------------//

   }//Cls
}//NS