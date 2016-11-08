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

using NSFString = System.String;

namespace NorthStateSoftware.NorthStateFramework
{
    /// <summary>
    /// Represents a fork-join state.
    /// </summary>
    /// <remarks>
    /// Fork-join states are used to provide synchronization and branching across multiple regions.
    /// </remarks>
    public class NSFForkJoin : NSFState
    {
        #region Public Constructors

        /// <summary>
        /// Creates a fork-join state.
        /// </summary>
        /// <param name="name">The name of the fork-join state.</param>
        /// <param name="parentState">The parent state of the fork-join state.</param>
        public NSFForkJoin(NSFString name, NSFCompositeState parentState)
            : base(name, (NSFRegion)null, null, null)
        {
            this.parentState = parentState;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Indicates if the fork-join is the active substate within the specified region.
        /// </summary>
        /// <param name="region">The region in question.</param>
        /// <returns>True if the fork-join is the active substate, otherwise false.</returns>
        public bool isActive(NSFRegion region)
        {
            return region.isInState(this);
        }

        #endregion Public Methods

        #region Internal Fields, Events, and Properties

        protected internal override NSFState ParentState
        {
            get { return parentState; }
        }

        #endregion Internal Fields, Events, and Properties

        #region Protected Methods

        protected internal override void enter(NSFStateMachineContext context, bool useHistory)
        {
            // Additional behavior

            // Enter parent state, if necessary
            if (!parentState.isActive())
            {
                parentState.enter(context, false);
            }

            // Add transition to list of completed transitions
            completedTransitions.Add(context.Transition);

            // Set associated region's active substate to this

            // Simple case of transition source is a state
            if (context.Transition.Source.ParentRegion != null)
            {
                context.Transition.Source.ParentRegion.setActiveSubstate(this);
            }
            else // A region can also be specified as part of a fork-join to fork-join transition
            {
                NSFForkJoinTransition forkJoinTransition = context.Transition as NSFForkJoinTransition;
                if ((forkJoinTransition != null) && (forkJoinTransition.ForkJoinRegion != null))
                {
                    forkJoinTransition.ForkJoinRegion.setActiveSubstate(this);
                }
            }

            base.enter(context, useHistory);
        }

        protected internal override void exit(NSFStateMachineContext context)
        {
            // Additional behavior
            if (!active)
            {
                return;
            }

            // Set all associated regions' active substates to null state
            foreach (NSFTransition incomingTransition in incomingTransitions)
            {
                // Simple case of transition source is a state
                if (incomingTransition.Source.ParentRegion != null)
                {
                    incomingTransition.Source.ParentRegion.setActiveSubstate(NSFState.NullState);
                }
                else // A region can also be specified as part of a fork-join to fork-join transition
                {
                    NSFForkJoinTransition forkJoinTransition = incomingTransition as NSFForkJoinTransition;
                    if ((forkJoinTransition != null) && (forkJoinTransition.ForkJoinRegion != null))
                    {
                        forkJoinTransition.ForkJoinRegion.setActiveSubstate(NSFState.NullState);
                    }
                }
            }

            // Remove all completed transitions
            completedTransitions.Clear();

            // Base class behavior
            base.exit(context);
        }

        protected internal override NSFEventStatus processEvent(NSFEvent nsfEvent)
        {
            // Check if all incoming transitions are satisfied
            bool incomingTransitionsSatisfied = true;
            foreach (NSFTransition incomingTransition in incomingTransitions)
            {
                if (!completedTransitions.Contains(incomingTransition))
                {
                    incomingTransitionsSatisfied = false;
                    break;
                }
            }

            if (incomingTransitionsSatisfied)
            {
                // Take all outgoing transitions
                foreach (NSFTransition outgoingTransition in outgoingTransitions)
                {
                    // All outgoing transitions must be null transitions by UML2.x standard
                    outgoingTransition.processEvent(nsfEvent);
                }

                return NSFEventStatus.NSFEventHandled;
            }

            return NSFEventStatus.NSFEventUnhandled;
        }

        protected internal override void reset()
        {
            // Base class behavior
            base.reset();

            // Additional behavior

            // Remove all incoming transition sources
            completedTransitions.Clear();
        }

        #endregion Protected Methods

        #region Private Fields, Events, and Properties

        private List<NSFTransition> completedTransitions = new List<NSFTransition>();
        private NSFCompositeState parentState;

        #endregion Private Fields, Events, and Properties
    }
}