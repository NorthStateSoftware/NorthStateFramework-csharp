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
    /// Represents a local state transition.
    /// </summary>
    /// <remarks>
    /// Transitions may specify event triggers, guards, and/or actions.
    /// Local transitions do not exit their source state.
    /// </remarks>
    public class NSFLocalTransition : NSFTransition
    {
        #region Public Constructors

        /// <summary>
        /// Creates a local transition.
        /// </summary>
        /// <param name="source">Transition source.</param>
        /// <param name="target">Transition target.</param>
        /// <param name="trigger">Transition trigger.</param>
        /// <param name="guard">Transition guard.</param>
        /// <param name="action">Transition action.</param>
        public NSFLocalTransition(NSFCompositeState source, NSFState target, NSFEvent trigger, NSFBoolGuard<NSFStateMachineContext> guard, NSFVoidAction<NSFStateMachineContext> action)
            : this(source.Name + "To" + target.Name, source, target, trigger, guard, action)
        {
        }

        /// <summary>
        /// Creates a local transition.
        /// </summary>
        /// <param name="name">User assigned name for transition.</param>
        /// <param name="source">Transition source.</param>
        /// <param name="target">Transition target.</param>
        /// <param name="trigger">Transition trigger.</param>
        /// <param name="guard">Transition guard.</param>
        /// <param name="action">Transition action.</param>
        public NSFLocalTransition(NSFString name, NSFCompositeState source, NSFState target, NSFEvent trigger, NSFBoolGuard<NSFStateMachineContext> guard, NSFVoidAction<NSFStateMachineContext> action)
            : base(name, source, target, trigger, guard, action)
        {
            compositeSource = source;

            // Target must be substate of source or the source itself
            if ((!Source.isParent(Target)) && (Source != Target))
            {
                throw new Exception(Name + " invalid local transition, source is neither parent of nor equal to target");
            }

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
            compositeSource.exitRegions(context);

            // Reset context possibly changed by exiting states
            context.EnteringState = null;
            context.ExitingState = null;

            Actions.execute(context);

            if (Source != Target)
            {
                Target.enter(context, false);
            }
            else
            {
                compositeSource.enterRegions(context, false);
            }
        }

        #endregion Protected Methods

        #region Private Fields, Events, and Properties

        private NSFCompositeState compositeSource;

        #endregion Private Fields, Events, and Properties
    }
}