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
using System.ComponentModel;

using NSFString = System.String;

namespace NorthStateSoftware.NorthStateFramework
{
    /// <summary>
    /// Represents an external state transition.
    /// </summary>
    /// <remarks>
    /// Transitions may specify an event trigger, guards, and/or a transition actions.
    /// External transitions force exiting of their source state.
    /// </remarks>
    public class NSFExternalTransition : NSFTransition
    {
        #region Public Constructors

        /// <summary>
        /// Creates an external transition.
        /// </summary>
        /// <param name="source">Transition source.</param>
        /// <param name="target">Transition target.</param>
        /// <param name="trigger">Transition trigger.</param>
        /// <param name="guard">Transition guard.</param>
        /// <param name="action">Transition action.</param>
        /// <remarks>The default name for the transition is [soure.Name]To[target.Name]</remarks>
        public NSFExternalTransition(NSFState source, NSFState target, NSFEvent trigger, NSFBoolGuard<NSFStateMachineContext> guard, NSFVoidAction<NSFStateMachineContext> action)
            : this(source.Name + "To" + target.Name, source, target, trigger, guard, action)
        {
        }

        /// <summary>
        /// Creates an external transition.
        /// </summary>
        /// <param name="name">User assigned name for transition.</param>
        /// <param name="source">Transition source.</param>
        /// <param name="target">Transition target.</param>
        /// <param name="trigger">Transition trigger.</param>
        /// <param name="guard">Transition guard.</param>
        /// <param name="action">Transition action.</param>
        public NSFExternalTransition(NSFString name, NSFState source, NSFState target, NSFEvent trigger, NSFBoolGuard<NSFStateMachineContext> guard, NSFVoidAction<NSFStateMachineContext> action)
            : base(name, source, target, trigger, guard, action)
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
            Source.exit(context);

            // Exit parent states until common parent is found
            NSFState parentState = Source.ParentState;
            while ((parentState != null) && (!parentState.isParent(Target)))
            {
                parentState.exit(context);
                parentState = parentState.ParentState;
            }

            // Reset context possibly changed by exiting states
            context.EnteringState = null;
            context.ExitingState = null;

            Actions.execute(context);

            Target.enter(context, false);
        }

        #endregion Protected Methods
    }
}