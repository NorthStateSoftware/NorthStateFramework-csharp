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
using System.ComponentModel;

using NSFString = System.String;

namespace NorthStateSoftware.NorthStateFramework
{
    /// <summary>
    /// Represents an internal state transition.
    /// </summary>
    /// <remarks>
    /// Transitions may specify event triggers, guards, and/or actions.
    /// Internal transitions do not exit or enter any states.  They are simply reactions to events.
    /// </remarks>
    public class NSFInternalTransition : NSFTransition
    {
        #region Public Constructors

        /// <summary>
        /// Creates an internal transition.
        /// </summary>
        /// <param name="state">Transition state.</param>
        /// <param name="trigger">Transition trigger.</param>
        /// <param name="guard">Transition guard.</param>
        /// <param name="action">Transition action.</param>
        /// <remarks>The default name for the transition is [state.Name]ReactionsTo[trigger.Name]</remarks>
        public NSFInternalTransition(NSFState state, NSFEvent trigger, NSFBoolGuard<NSFStateMachineContext> guard, NSFVoidAction<NSFStateMachineContext> action)
            : this(state.Name + "ReactionsTo" + trigger.Name, state, trigger, guard, action)
        {
        }
        
        /// <summary>
        /// Creates an internal transition.
        /// </summary>
        /// <param name="name">User assigned name for transition.</param>
        /// <param name="state">Transition state.</param>
        /// <param name="trigger">Transition trigger.</param>
        /// <param name="guard">Transition guard.</param>
        /// <param name="action">Transition action.</param>
        public NSFInternalTransition(NSFString name, NSFState state, NSFEvent trigger, NSFBoolGuard<NSFStateMachineContext> guard, NSFVoidAction<NSFStateMachineContext> action)
            : base(name, state, state, trigger, guard, action)
        {
            Source.addOutgoingTransition(this);
        }

        #endregion Public Constructors

        #region Public Fields, Events, and Properties

        public override NSFState Source
        {
            internal protected set
            {
                base.Source = value;
                Source.addOutgoingTransition(this);
            }
        }

        #endregion Public Fields, Events, and Properties

        #region Protected Methods

        protected override void fireTransition(NSFStateMachineContext context)
        {
            Actions.execute(context);
        }

        #endregion Protected Methods
    }
}