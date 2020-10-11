//-----------------------------------------------------------------------
// <copyright file="Command.cs" company="FH Wiener Neustadt">
//     Copyright (c) Emre Rauhofer. All rights reserved.
// </copyright>
// <author>Emre Rauhofer</author>
// <summary>
// This is a dashboard.
// </summary>
//-----------------------------------------------------------------------
namespace ProcessWatcher.Commands
{
    using System;
    using System.Windows.Input;

    /// <summary>
    /// The <see cref="Command"/> class.
    /// </summary>
    public class Command : ICommand
    {
        /// <summary>
        /// The <see cref="Action"/> object.
        /// </summary>
        private readonly Action<object> action;

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="action"> The method to be executed. </param>
        public Command(Action<object> action)
        {
            this.action = action;
        }

        /// <summary>
        /// The event handler.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// This method checks if the command can be executed.
        /// </summary>
        /// <param name="parameter"> The object parameter. </param>
        /// <returns> It always returns true. </returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// This method executes the command with the given parameter.
        /// </summary>
        /// <param name="parameter"> The object parameter. </param>
        public void Execute(object parameter)
        {
            this.action(parameter);
        }
    }
}
