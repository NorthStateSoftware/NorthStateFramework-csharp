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

namespace NorthStateSoftware.Examples.CommandProcessorWithResetStateMachineExample
{
    public class CommandProcessorWithResetStateMachine : NSFStateMachine
    {
        #region Fields

        protected Queue<String> commandQueue = new Queue<String>();
        protected long responseTimeout = 1000;

        // State Machine Components
        // Define and initialize in the order:
        //   1) Events
        //   2) Regions and states, from outer to inner
        //   3) Transitions, ordered internal, local, external
        //   4) Group states and transitions within a region together.

        // Events
        protected NSFDataEvent<String> newCommandEvent;
        protected NSFDataEvent<String> newResponseEvent;
        protected NSFEvent responseTimeoutEvent;
        protected NSFEvent resetEvent;

        // Regions and states, from outer to inner
        protected NSFInitialState initialCommandProcessorState;
        protected NSFCompositeState waitForCommandState;
        protected NSFCompositeState waitForResponseState;
        protected NSFCompositeState errorState;
        protected ResetStrategy resetState;

        // Transitions, ordered internal, local, external
        protected NSFInternalTransition reactionToNewCommand;
        protected NSFExternalTransition initialCommandProcessorToWaitForCommandTransition;
        protected NSFExternalTransition waitForCommandToWaitForResponseTransition;
        protected NSFExternalTransition waitForResponseToWaitForCommandTransition;
        protected NSFExternalTransition waitForResponseToErrorTransition;
        protected NSFExternalTransition errorToResetTransition;
        protected NSFExternalTransition resetToWaitForCommandTransition;

        #endregion Fields

        #region Properties

        //States
        public NSFCompositeState WaitForCommandState { get { return waitForCommandState; } }
        public NSFCompositeState WaitForResponseState { get { return waitForResponseState; } }
        public NSFCompositeState ErrorState { get { return errorState; } }
        public NSFCompositeState ResetState { get { return resetState; } }

        // Transitions
        public NSFExternalTransition WaitForCommandToWaitForResponseTransition { get { return waitForCommandToWaitForResponseTransition; } }
        public NSFExternalTransition WaitForResponseToWaitForCommandTransition { get { return waitForResponseToWaitForCommandTransition; } }
        public NSFExternalTransition WaitForResponseToErrorTransition { get { return waitForResponseToErrorTransition; } }
        public NSFExternalTransition ErrorToResetTransition { get { return errorToResetTransition; } }
        public NSFExternalTransition ResetToWaitForCommandTransition { get { return resetToWaitForCommandTransition; } }

        #endregion Properties

        #region Constructors

        public CommandProcessorWithResetStateMachine(String name)
            : base(name, new NSFEventThread(name))
        {
            createStateMachine();
        }

        private void createStateMachine()
        {
            // State Machine Components
            // Define and initialize in the order:
            //   1) Events
            //   2) Regions and states, from outer to inner
            //   3) Transitions, ordered internal, local, external
            //   4) Group states and transitions within a region together.

            // Events
            // Event constructors take the form (name, parent)
            // Data event constructors take the form (name, parent, data payload)
            newCommandEvent = new NSFDataEvent<String>("NewCommand", this, "CommandPayload");
            newResponseEvent = new NSFDataEvent<String>("NewResponse", this, "ResponsePayload");
            responseTimeoutEvent = new NSFEvent("ResponseTimeout", this);
            resetEvent = new NSFEvent("Reset", this);

            // Regions and states, from outer to inner 
            // Initial state construtors take the form (name, parent)
            initialCommandProcessorState = new NSFInitialState("InitialCommandProcessor", this);
            // Composite state construtors take the form (name, parent, entry action, exit action)
            waitForCommandState = new NSFCompositeState("WaitForCommand", this, null, null);
            waitForResponseState = new NSFCompositeState("WaitForResponse", this, waitForResponseEntryActions, waitForResponseExitActions);
            errorState = new NSFCompositeState("Error", this, errorEntryActions, null);
            resetState = new ResetStrategy("Reset", this);

            // Transitions, ordered internal, local, external
            // Internal transition construtors take the form (name, state, trigger, guard, action)
            reactionToNewCommand = new NSFInternalTransition("ReactionToNewCommand", this, newCommandEvent, null, queueCommand);
            // External transition construtors take the form (name, source, target, trigger, guard, action)
            initialCommandProcessorToWaitForCommandTransition = new NSFExternalTransition("InitialToWaitForCommand", initialCommandProcessorState, waitForCommandState, null, null, null);
            waitForCommandToWaitForResponseTransition = new NSFExternalTransition("WaitForCommandToWaitForResponse", waitForCommandState, waitForResponseState, null, hasCommand, sendCommand);
            waitForResponseToWaitForCommandTransition = new NSFExternalTransition("WaitForResponseToWaitForCommand", waitForResponseState, waitForCommandState, newResponseEvent, isResponse, handleResponse);
            waitForResponseToErrorTransition = new NSFExternalTransition("WaitForResponseToError", waitForResponseState, errorState, responseTimeoutEvent, null, null);
            errorToResetTransition = new NSFExternalTransition("ErrorToReset", errorState, resetState, resetEvent, null, null);
            resetToWaitForCommandTransition = new NSFExternalTransition("ResetToWaitForCommand", resetState, waitForCommandState, null, isReady, null);
        }

        #endregion Constructors

        #region Methods

        public void addCommand(String newCommand)
        {
            // Queue the event and return so as to not block the calling thread.
            // The event will be picked up by handleNewCommand() and added to a queue.
            // This approach eliminates the need to mutex the command queue,
            // because all queue manipulation occurs on the state machine thread.

            // It is not always necesary to copy an event before it is queued. 
            // However, in this case, multiple copies of the event can be queued,
            // with each event carrying a unique data payload.

            // Also note that event is being tagged as "deleteAfterHandling,"  that 
            // lets the framework know to delete this copy once processing is complete.
            queueEvent(newCommandEvent.copy(newCommand));
        }

        public void addResponse(String newResponse)
        {
            // Queue the event and return so as to not block the calling thread.
            queueEvent(newResponseEvent.copy(newResponse));
        }

        public void resetError()
        {
            queueEvent(resetEvent);
        }

        private void queueCommand(NSFStateMachineContext context)
        {
            // Add data to command queue.
            // If the state machine is in the waitForCommandState, then the
            // run to completion step will result in transitioning to the
            // waitForResponse state, sending the command during the transtion.
            commandQueue.Enqueue(((NSFDataEvent<String>)(context.Trigger)).Data);
        }

        private bool hasCommand(NSFStateMachineContext context)
        {
            return (commandQueue.Count != 0);
        }

        private void sendCommand(NSFStateMachineContext context)
        {
            String commandString = commandQueue.Peek();

            // Code to send the command goes here
            // ...

            // Log trace of command sent
            // Trace Format:
            // <MessageSent>
            //   <Source>name</Source>
            //   <Message>message</Message>
            // </MessageSent>
            NSFTraceLog.PrimaryTraceLog.addTrace(NSFTraceTags.MessageSentTag, NSFTraceTags.SourceTag, Name, NSFTraceTags.MessageTag, commandString);
        }

        private void waitForResponseEntryActions(NSFStateMachineContext context)
        {
            // Schedule timeout event, in case no response received           
            responseTimeoutEvent.schedule(responseTimeout, 0);
        }

        private void waitForResponseExitActions(NSFStateMachineContext context)
        {
            // Unschedule the timeout event
            responseTimeoutEvent.unschedule();

            commandQueue.Dequeue();
        }

        private bool isResponse(NSFStateMachineContext context)
        {
            // Code to verify that the resposne is correct for the command goes here.
            // ...

            return true;
        }

        private void handleResponse(NSFStateMachineContext context)
        {
            // Code to handle the response goes here
            // ...

            // Log trace of response received
            // Trace Format:
            // <MessageReceived>
            //   <Source>name</Source>
            //   <Message>message</Message>
            // </MessageReceived>
            NSFTraceLog.PrimaryTraceLog.addTrace(NSFTraceTags.MessageReceivedTag, NSFTraceTags.SourceTag, Name, NSFTraceTags.MessageTag, ((NSFDataEvent<String>)(context.Trigger)).Data);
        }

        private void errorEntryActions(NSFStateMachineContext context)
        {
            // Code to handle the error goes here
            // ...
        }

        private bool isReady(NSFStateMachineContext context)
        {
            return resetState.ReadyState.isActive();
        }

        #endregion Methods
    }
}
