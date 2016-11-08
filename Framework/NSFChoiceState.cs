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

using NSFString = System.String;

namespace NorthStateSoftware.NorthStateFramework
{
    /// <summary>
    /// Represents a choice pseudo-state.
    /// </summary>
    /// <remarks>
    /// NSFChoiceState is a transient state.  
    /// Once entered, it forces evaluation of all the outgoing transitions.
    /// All outgoing transition must have no triggers.
    /// There can be one and only one "else" transition, which has a null guard.
    /// Internal transitions state are not handled by transient states.  
    /// If no outgoing transition is taken, then the model is ill formed and an exception is thrown.
    /// </remarks>
    public class NSFChoiceState : NSFState
    {
        #region Public Constructors

        /// <summary>
        /// Creates a choice pseudo-state.
        /// </summary>
        /// <param name="name">The name of the choice pseudo-state.</param>
        /// <param name="parentRegion">The parent region of the choice pseudo-state.</param>
        public NSFChoiceState(NSFString name, NSFRegion parentRegion)
            : base(name, parentRegion, null, null)
        {
        }

        /// <summary>
        /// Creates a choice pseudo-state.
        /// </summary>
        /// <param name="name">The name of the choice pseudo-state.</param>
        /// <param name="parentState">The parent state of the choice pseudo-state.</param>
        public NSFChoiceState(NSFString name, NSFCompositeState parentState)
            : base(name, parentState, null, null)
        {
        }

        #endregion Public Constructors

        #region Protected Methods

        protected internal override NSFEventStatus processEvent(NSFEvent nsfEvent)
        {
            NSFTransition elseTransition = null;

            // Check transitions out of state
            foreach (NSFTransition transition in outgoingTransitions)
            {
                if (!transition.Guards.isEmpty())
                {
                    if (transition.processEvent(nsfEvent) == NSFEventStatus.NSFEventHandled)
                    {
                        return NSFEventStatus.NSFEventHandled;
                    }
                }
                else
                {
                    if (elseTransition != null)
                    {
                        // A choice is ill formed if it has more than one "else" transition
                        throw new Exception(Name + " invalid choice state with multiple else transitions");
                    }

                    elseTransition = transition;
                }
            }

            if ((elseTransition != null) && (elseTransition.processEvent(nsfEvent) == NSFEventStatus.NSFEventHandled))
            {
                return NSFEventStatus.NSFEventHandled;
            }

            // If no transition is taken, then the model is ill formed and an exception is thrown.
            throw new Exception(Name + " invalid model with no transition taken from a choice state");
        }

        #endregion Protected Methods
    }
}