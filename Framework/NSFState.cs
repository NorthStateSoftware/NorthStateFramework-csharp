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
using System.ComponentModel;

using NSFString = System.String;

namespace NorthStateSoftware.NorthStateFramework
{
    /// <summary>
    /// Represents a state machine state.
    /// </summary>
    /// <remarks>
    /// This class is used by the North State Framework as a base class for all state types.
    /// It is rarely used by external applications.
    /// They should use NSFCompositeState rather than NSFState, for greater flexibility and extensibility.
    /// </remarks>
    public class NSFState : NSFTaggedObject
    {
        #region Public Constructors

        /// <summary>
        /// Creates a state.
        /// </summary>
        /// <param name="name">The name of the state.</param>
        /// <param name="parentRegion">The parent region of the state.</param>
        /// <param name="entryAction">The actions to be performed upon entry to the state.</param>
        /// <param name="exitAction">The actions to be performed upon exit of the state.</param>
        public NSFState(NSFString name, NSFRegion parentRegion, NSFVoidAction<NSFStateMachineContext> entryAction, NSFVoidAction<NSFStateMachineContext> exitAction)
            : base(name)
        {
            this.parentRegion = parentRegion;
            EntryActions += entryAction;
            ExitActions += exitAction;

            if (parentRegion != null)
            {
                parentRegion.addSubstate(this);
            }

            EntryActions.setExceptionAction(handleEntryActionException);
            ExitActions.setExceptionAction(handleExitActionException);
        }

        /// <summary>
        /// Creates a state.
        /// </summary>
        /// <param name="name">The name of the state.</param>
        /// <param name="parentState">The parent state of the state.</param>
        /// <param name="entryAction">The actions to be performed upon entry to the state.</param>
        /// <param name="exitAction">The actions to be performed upon exit of the state.</param>
        public NSFState(NSFString name, NSFCompositeState parentState, NSFVoidAction<NSFStateMachineContext> entryAction, NSFVoidAction<NSFStateMachineContext> exitAction)
            : this(name, parentState.getDefaultRegion(), entryAction, exitAction)
        {
        }

        #endregion Public Constructors

        #region Public Fields, Events, and Properties

        /// <summary>
        /// Actions to be executed whenever the state is entered.
        /// </summary>
        public NSFVoidActions<NSFStateMachineContext> EntryActions = new NSFVoidActions<NSFStateMachineContext>();

        /// <summary>
        /// Actions to be executed whenever the state is exited.
        /// </summary>
        public NSFVoidActions<NSFStateMachineContext> ExitActions = new NSFVoidActions<NSFStateMachineContext>();

        /// <summary>
        /// Gets or sets the flag indicating if trace logging is enabled or disabled for the state.
        /// </summary>
        /// <remarks>
        /// If this flag and the state machine's logging enabled flag are set true,
        /// then entry into the state is logged in the trace log.
        /// </remarks>
        public bool LogEntry
        {
            get { return logEntry; }
            set { logEntry = value; }
        }

        /// <summary>
        /// Gets the top state machine encompassing the state.
        /// </summary>
        /// <returns>The top state machine.</returns>
        public virtual NSFStateMachine TopStateMachine
        {
            get { return ParentState.TopStateMachine; }
        }

        #endregion Public Fields, Events, and Properties

        #region Public Methods

        /// <summary>
        /// Indicates if the state is currently active.
        /// </summary>
        /// <returns>True if the state is active, otherwise false.</returns>
        /// <remarks>
        /// When this method returns true, the state machine is said to be "in" this state.
        /// </remarks>
        public bool isActive()
        {
            return active;
        }

        /// <summary>
        /// Indicates if the specified state is active, i.e. is "in" the specified state.
        /// </summary>
        /// <param name="state">State in question.</param>
        /// <returns>True if the specified state is active, otherwise false.</returns>
        public virtual bool isInState(NSFState state)
        {
            if (!active)
            {
                return false;
            }

            return (this == state);
        }

        /// <summary>
        /// Indicates if the specified state is active, i.e. is "in" the specified state.
        /// </summary>
        /// <param name="stateName">Name of state in question.  Assumes unique names.</param>
        /// <returns>True if the specified state is active, otherwise false.</returns>
        public virtual bool isInState(NSFString stateName)
        {
            if (!active)
            {
                return false;
            }

            return (Name == stateName);
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Enters the state.
        /// </summary>
        /// <param name="context">The event arguments pertaining to the transition into the state.</param>
        /// <param name="useHistory">Used by history states to reconstitute state history.</param>
        /// <remarks>
        /// This method is for use only by the North State Framework's internal logic.
        /// </remarks>
        protected internal virtual void enter(NSFStateMachineContext context, bool useHistory)
        {
            active = true;

            if (parentRegion != null)
            {
                parentRegion.setActiveSubstate(this);

                if (!parentRegion.isActive())
                {
                    parentRegion.enter(context, false);
                }
            }

            if (TopStateMachine.LoggingEnabled && LogEntry)
            {
                NSFTraceLog.PrimaryTraceLog.addTrace(NSFTraceTags.StateEnteredTag,
                                     NSFTraceTags.StateMachineTag, TopStateMachine.Name,
                                     NSFTraceTags.StateTag, Name);
            }

            // Update context to indicate entering this state
            context.EnteringState = this;
            context.ExitingState = null;

            EntryActions.execute(context);

            NSFStateMachine parentStateMachine = ParentStateMachine;
            if (parentStateMachine != null)
            {
                parentStateMachine.executeStateChangeActions(context);
            }
        }

        /// <summary>
        /// Exits the state.
        /// </summary>
        /// <param name="context">The event arguments pertaining to the transition out of the state.</param>
        /// <remarks>
        /// This method is for use only by the North State Framework's internal logic.
        /// </remarks>
        protected internal virtual void exit(NSFStateMachineContext context)
        {
            active = false;

            // Update context to indicate exiting this state
            context.ExitingState = this;
            context.EnteringState = null;

            ExitActions.execute(context);

            if (parentRegion != null)
            {
                parentRegion.setActiveSubstate(NSFState.NullState);
            }
        }

        /// <summary>
        /// Processes an event.
        /// </summary>
        /// <param name="nsfEvent">The event to process.</param>
        /// <returns>NSFEventStatus.EventHandled if the event is handled, otherwise NSFEventStatus.EventUnhandled.</returns>
        /// <remarks>
        /// This method is for use only by the North State Framework's internal logic.
        /// </remarks>
        protected internal virtual NSFEventStatus processEvent(NSFEvent nsfEvent)
        {
            foreach (NSFTransition transition in outgoingTransitions)
            {
                if (transition.processEvent(nsfEvent) == NSFEventStatus.NSFEventHandled)
                {
                    return NSFEventStatus.NSFEventHandled;
                }
            }

            return NSFEventStatus.NSFEventUnhandled;
        }

        /// <summary>
        /// Resets the state to its initial configuration.
        /// </summary>
        /// <remarks>
        /// This method is for use only by the North State Framework's internal logic.
        /// </remarks>
        protected internal virtual void reset()
        {
            active = false;
        }

        #endregion Protected Methods

        #region Internal Fields, Events, and Properties

        internal static readonly NSFState nullState = new NSFState("NullState", (NSFRegion)null, null, null);

        internal bool active = false;
        internal List<NSFTransition> incomingTransitions = new List<NSFTransition>();
        internal bool logEntry = true;
        internal List<NSFTransition> outgoingTransitions = new List<NSFTransition>();
        internal NSFRegion parentRegion;

        /// <summary>
        /// Gets the null state.
        /// </summary>
        /// <returns>The null state.</returns>
        /// <remarks>
        /// This method is for use only by the North State Framework's internal logic.
        /// </remarks>
        protected internal static NSFState NullState
        {
            get { return nullState; }
        }

        /// <summary>
        /// Gets the parent region.
        /// </summary>
        /// <returns>The parent region.</returns>
        protected internal NSFRegion ParentRegion
        {
            get { return parentRegion; }
        }

        /// <summary>
        /// Gets the parent state.
        /// </summary>
        /// <returns>The parent state, or null if the state is not a substate.</returns>
        protected internal virtual NSFState ParentState
        {
            get
            {
                if (parentRegion != null)
                {
                    return parentRegion.ParentState;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the parent state machine.
        /// </summary>
        /// <returns>The parent state machine, or null if no parent state machine.</returns>
        protected internal virtual NSFStateMachine ParentStateMachine
        {
            get
            {
                NSFState parentState = ParentState;

                if (parentState == null)
                {
                    return null;
                }

                if (parentState.GetType().IsSubclassOf(typeof(NSFStateMachine)))
                {
                    return (NSFStateMachine)parentState;
                }
                else
                {
                    return parentState.ParentStateMachine;
                }
            }
        }

        #endregion Internal Fields, Events, and Properties

        #region Internal Methods

        /// <summary>
        /// Adds an incoming transition.
        /// </summary>
        /// <param name="transition">The incoming transition to add.</param>
        /// <remarks>
        /// This method is called from the base transition class constructor.
        /// </remarks>
        internal void addIncomingTransition(NSFTransition transition)
        {
            incomingTransitions.Add(transition);
        }

        /// <summary>
        /// Adds an outgoing transition.
        /// </summary>
        /// <param name="transition">The outgoing transition to add.</param>
        /// <remarks>
        /// This method is called from the internal transition class constructor.
        /// </remarks>
        internal void addOutgoingTransition(NSFInternalTransition transition)
        {
            // Insert before first transition that is not internal
            foreach (NSFTransition outgoingTransition in outgoingTransitions)
            {
                if (outgoingTransition is NSFInternalTransition)
                {
                    outgoingTransitions.Insert(outgoingTransitions.IndexOf(outgoingTransition), transition);
                    return;
                }
            }

            outgoingTransitions.Add(transition);
        }

        /// <summary>
        /// Adds an outgoing transition.
        /// </summary>
        /// <param name="transition">The outgoing transition to add.</param>
        /// <remarks>
        /// This method is called from the local transition class constructor.
        /// </remarks>
        internal void addOutgoingTransition(NSFLocalTransition transition)
        {
            // Insert before first external transition
            foreach (NSFTransition outgoingTransition in outgoingTransitions)
            {
                if (outgoingTransition is NSFExternalTransition)
                {
                    outgoingTransitions.Insert(outgoingTransitions.IndexOf(outgoingTransition), transition);
                    return;
                }
            }

            outgoingTransitions.Add(transition);
        }

        /// <summary>
        /// Adds an outgoing transition.
        /// </summary>
        /// <param name="transition">The outgoing transition to add.</param>
        /// <remarks>
        /// This method is called from the external transition class constructor.
        /// </remarks>
        internal void addOutgoingTransition(NSFExternalTransition transition)
        {
            outgoingTransitions.Add(transition);
        }

        /// <summary>
        /// Indicates if this state is a parent of the specified substate.
        /// </summary>
        /// <param name="substate">The substate in question.</param>
        /// <returns>True if this state is a parent, false otherwise.</returns>
        internal bool isParent(NSFState substate)
        {
            NSFState substateParent = substate.ParentState;

            while (substateParent != null)
            {
                if (substateParent == this)
                {
                    return true;
                }
                else
                {
                    substateParent = substateParent.ParentState;
                }
            }

            return false;
        }

        /// <summary>
        /// Removes an incoming transition.
        /// </summary>
        /// <param name="transition">The incoming transition to remove.</param>
        /// <remarks>
        /// It is called when re-routing a transition.
        /// </remarks>
        internal void removeIncomingTransition(NSFTransition transition)
        {
            incomingTransitions.Remove(transition);
        }

        /// <summary>
        /// Removes an outgoing transition.
        /// </summary>
        /// <param name="transition">The outgoing transition to remove.</param>
        /// <remarks>
        /// It is called when re-routing a transition.
        /// </remarks>
        internal void removeOutgoingTransition(NSFTransition transition)
        {
            outgoingTransitions.Remove(transition);
        }

        #endregion Internal Methods

        #region Private Methods

        /// <summary>
        /// Handles exceptions caught while executing entry actions.
        /// </summary>
        /// <param name="context">Additional contextual information.</param>
        private void handleEntryActionException(NSFExceptionContext context)
        {
            TopStateMachine.handleException(new Exception(Name + " state entry action exception", context.Exception));
        }

        /// <summary>
        /// Handles exceptions caught while executing exit actions.
        /// </summary>
        /// <param name="context">Additional contextual information.</param>
        private void handleExitActionException(NSFExceptionContext context)
        {
            TopStateMachine.handleException(new Exception(Name + " state exit action exception", context.Exception));
        }

        #endregion Private Methods
    }
}