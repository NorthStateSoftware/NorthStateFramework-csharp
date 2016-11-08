// Copyright 2004-2016, North State Software, LLC.  All rights reserved.

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

namespace NorthStateSoftware.Examples.CommandProcessorWithRetryExample
{
    public class CommandProcessorWithRetry : CommandProcessor
    {
        #region Fields

        private int retries = 0;
        private int maxRetries = 1;

        // State Machine Components
        // Define and initialize in the order:
        //   1) Events
        //   2) Regions and states, from outer to inner
        //   3) Transitions, ordered internal, local, external
        //   4) Group states and transitions within a region together.

        // Transitions, ordered internal, local, external
#pragma warning disable 0414
        private NSFInternalTransition waitForResponseReactionToResponseTimeout;
#pragma warning restore 0414

        #endregion Fields

        #region Constructors

        public CommandProcessorWithRetry(String name)
            : base(name)
        {
            createStateMachine();
        }

        private void createStateMachine()
        {
            waitForResponseReactionToResponseTimeout = new NSFInternalTransition("WaitForResponseReactionToResponseTimeout", waitForResponseState, responseTimeoutEvent, canRetry, retrySend);
            waitForResponseState.EntryActions += waitForResponseEntryActions;
        }

        #endregion Constructors

        #region Methods

        private void retrySend(NSFStateMachineContext context)
        {
            NSFDebugUtility.PrimaryDebugUtility.writeLineToConsole("Retry send ...");
            ++retries;
            // Code to re-send the command goes here
            // ...

            // Schedule timeout event, in case no response received           
            responseTimeoutEvent.schedule(responseTimeout, 0);
        }

        private void waitForResponseEntryActions(NSFStateMachineContext context)
        {
            retries = 0;
        }

        private bool canRetry(NSFStateMachineContext context)
        {
            return (retries < maxRetries);
        }

        #endregion Methods
    }
}
