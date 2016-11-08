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
using System.ComponentModel;

using NSFString = System.String;

namespace NorthStateSoftware.NorthStateFramework
{
    /// <summary>
    /// Represents a composite state region for nested substates.
    /// </summary>
    /// <remarks>
    /// Multiple regions within a composite state indicate concurrent behaviors.
    /// </remarks>
    public sealed class NSFRegion : NSFTaggedObject
    {
        #region Public Constructors

        /// <summary>
        /// Creates a region.
        /// </summary>
        /// <param name="name">The name of the region.</param>
        /// <param name="parentState">The parent state of the region.</param>
        public NSFRegion(NSFString name, NSFCompositeState parentState)
            : base(name)
        {
            if (parentState == null)
            {
                throw new Exception(Name + " invalid region with null parent state");
            }

            this.parentState = parentState;
            parentState.addRegion(this);
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Indicates if the region's active substate is the specified state.
        /// </summary>
        /// <param name="state">The state in question.</param>
        /// <returns>True if the region's active substate is the specified state, false otherwise.</returns>
        public bool isInState(NSFState state)
        {
            if (!active)
            {
                return false;
            }

            return activeSubstate.isInState(state);
        }

        /// <summary>
        /// Indicates if the region's active substate is the specified state.
        /// </summary>
        /// <param name="stateName">The name of the state in question.</param>
        /// <returns>True if the region's active substate is the specified state, false otherwise.</returns>
        public bool isInState(NSFString stateName)
        {
            if (!active)
            {
                return false;
            }

            return activeSubstate.isInState(stateName);
        }

        #endregion Public Methods

        #region Internal Fields, Events, and Properties

        /// <summary>
        /// Gets the history substate.
        /// </summary>
        internal NSFState HistorySubstate { get { return historySubstate; } }

        /// <summary>
        /// Gets the parent state.
        /// </summary>
        internal NSFState ParentState { get { return parentState; } }

        #endregion Internal Fields, Events, and Properties

        #region Internal Methods

        /// <summary>
        /// Adds a substate to the region's list of substates.
        /// </summary>
        /// <param name="substate">The substate to add.</param>
        internal void addSubstate(NSFState substate)
        {
            substates.Add(substate);

            // Any single state in a region can be the initial state
            // If more than one state exists in a region, there must be one and only one NSFInitialState

            // First substate is automatically registered as initial state
            if (substates.Count == 1)
            {
                initialState = substate;
                return;
            }

            // Not first state

            // If initial state is not already an NSFInitialState
            if (!(initialState is NSFInitialState))
            {
                if (substate is NSFInitialState)
                {
                    initialState = substate;
                }
                else // Not adding an NSFInitialState
                {
                    // Reset initial state to null, enforcing requirement for an NSFInitialState
                    initialState = NSFState.NullState;
                }
            }
            else // Initial state already established
            {
                if (substate is NSFInitialState)
                {
                    throw new Exception(Name + " only one initial state allowed in a region");
                }
            }
        }

        /// <summary>
        /// Enters the region.
        /// </summary>
        /// <param name="context">Additional contextual information.</param>
        /// <param name="useHistory">Flag indicating whether or not to use the history state as entry point.</param>
        internal void enter(NSFStateMachineContext context, bool useHistory)
        {
            active = true;

            if (!parentState.isActive())
            {
                parentState.enter(context, false);
            }

            if (useHistory)
            {
                if (historySubstate != NSFState.NullState)
                {
                    historySubstate.enter(context, useHistory);
                }
            }

            if (activeSubstate == NSFState.NullState)
            {
                initialState.enter(context, false);
            }
        }

        /// <summary>
        /// Exits the region.
        /// </summary>
        /// <param name="context">Additional contextual information.</param>
        internal void exit(NSFStateMachineContext context)
        {
            active = false;

            if (activeSubstate != NSFState.NullState)
            {
                activeSubstate.exit(context);
            }
        }

        /// <summary>
        /// Indicates if the region is currently active.
        /// </summary>
        /// <returns>True if the region is active, false otherwise.</returns>
        internal bool isActive()
        {
            return active;
        }

        /// <summary>
        /// Processes an event.
        /// </summary>
        /// <param name="nsfEvent">The event to process</param>
        /// <returns>NSFEventHandled if the event is acted upon, otherwise NSFEventUnhandled.</returns>
        internal NSFEventStatus processEvent(NSFEvent nsfEvent)
        {
            return activeSubstate.processEvent(nsfEvent);
        }

        /// <summary>
        /// Resets the region to its initial condition.
        /// </summary>
        internal void reset()
        {
            active = false;

            activeSubstate = NSFState.NullState;
            historySubstate = NSFState.NullState;

            foreach (NSFState state in substates)
            {
                state.reset();
            }
        }

        /// <summary>
        /// Sets the region's active substate.
        /// </summary>
        /// <param name="substate">The new active substate.</param>
        internal void setActiveSubstate(NSFState substate)
        {
            // Set history substate to the current, active substate whenever the active substate is about to becomes the null state,
            // except that the history substate should not be set to the null state or the initial state.
            if ((substate == NSFState.NullState) && (activeSubstate != NSFState.NullState) && (activeSubstate != initialState))
            {
                historySubstate = activeSubstate;
            }

            activeSubstate = substate;
        }

        #endregion Internal Methods

        #region Private Fields, Events, and Properties

        private bool active = false;
        private NSFState activeSubstate = NSFState.NullState;
        private NSFState historySubstate = NSFState.NullState;
        private NSFState initialState = NSFState.NullState;
        private NSFCompositeState parentState;
        private List<NSFState> substates = new List<NSFState>();

        #endregion Private Fields, Events, and Properties
    }
}