// Copyright 2004-2014, North State Software, LLC.  All rights reserved.

// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using NorthStateSoftware.NorthStateFramework;

namespace NorthStateSoftware.Examples.CommandProcessorExample
{
    public class CommandProcessorObserver
    {
        #region Constructors

        public CommandProcessorObserver(CommandProcessor commandProcessor)
        {
            // Register handleStateChange to be called on state entry.
            commandProcessor.WaitForCommandState.EntryActions += handleStateEntered;
            commandProcessor.WaitForResponseState.EntryActions += handleStateEntered;
            commandProcessor.ErrorState.EntryActions += handleStateEntered;
            commandProcessor.ResetState.EntryActions += handleStateEntered;

            // Register handleStateChange to be called on state exit.
            commandProcessor.WaitForCommandState.ExitActions += handleStateExited;
            commandProcessor.WaitForResponseState.ExitActions += handleStateExited;
            commandProcessor.ErrorState.ExitActions += handleStateExited;
            commandProcessor.ResetState.ExitActions += handleStateExited;

            // Register transition actions to be called on transition from one state to another.
            commandProcessor.WaitForCommandToWaitForResponseTransition.Actions += handleTransition;
            commandProcessor.WaitForResponseToWaitForCommandTransition.Actions += handleTransition;
            commandProcessor.WaitForResponseToErrorTransition.Actions += handleTransition;
            commandProcessor.ErrorToResetTransition.Actions += handleTransition;
            commandProcessor.ResetToWaitForCommandTransition.Actions += handleTransition;
        }

        #endregion Constructors

        #region Methods

        private void handleStateEntered(NSFStateMachineContext context)
        {
            NSFDebugUtility.PrimaryDebugUtility.writeLineToConsole("Entering " + context.EnteringState.Name);
        }

        private void handleTransition(NSFStateMachineContext context)
        {
            NSFDebugUtility.PrimaryDebugUtility.writeLineToConsole("Transitioning from " + context.Transition.Source.Name
                + " to " + context.Transition.Target.Name);
        }

        private void handleStateExited(NSFStateMachineContext context)
        {
            NSFDebugUtility.PrimaryDebugUtility.writeLineToConsole("Exiting " + context.ExitingState.Name);
        }

        #endregion Methods
    }
}
