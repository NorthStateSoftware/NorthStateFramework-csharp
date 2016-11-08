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
    /// Represents a state transition.
    /// </summary>
    /// <remarks>
    /// This class implements common behaviors for the concrete transition types:
    /// NSFInternalTransition, NSFLocalTransition, NSFExternalTransition.
    /// Transitions may specify event triggers, guards, and/or actions.
    /// </remarks>
    public abstract class NSFTransition : NSFTaggedObject
    {
        #region Public Fields, Events, and Properties

        /// <summary>
        /// Actions to be executed whenever the transition is taken.
        /// </summary>
        public NSFVoidActions<NSFStateMachineContext> Actions = new NSFVoidActions<NSFStateMachineContext>();

        /// <summary>
        /// Guards to be evaluated to determine if the transition can be taken.
        /// </summary>
        public NSFBoolGuards<NSFStateMachineContext> Guards = new NSFBoolGuards<NSFStateMachineContext>();

        /// <summary>
        /// Gets or sets the source of the transition.
        /// </summary>
        public virtual NSFState Source
        {
            get { return source; }
            internal protected set
            {
                source = value;
                // Outgoing transitions must be added by concrete classes
            }
        }

        /// <summary>
        /// Gets or sets the target of the transition.
        /// </summary>
        public NSFState Target
        {
            get { return target; }
            internal protected set
            {
                target = value;
                target.addIncomingTransition(this);
            }
        }

        /// <summary>
        /// Gets the list of trigger events for the transition.
        /// </summary>
        /// <remarks>
        /// Returns a copy of the internally used list.
        /// Use addTrigger to add a new trigger.
        /// </remarks>
        public List<NSFEvent> Triggers
        {
            get { return new List<NSFEvent>(triggers); }
        }

        #endregion Public Fields, Events, and Properties

        #region Public Methods

        /// <summary>
        /// Adds a trigger to the list of trigger events.
        /// </summary>
        public void addTrigger(NSFEvent trigger)
        {
            if (trigger == null)
            {
                return;
            }

            triggers.Add(trigger);
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Fires the transition.
        /// </summary>
        /// <param name="context">The state machine context associated with the transition firing.</param>
        /// <remarks>
        /// This method is for use only by the North State Framework's internal logic.
        /// </remarks>
        protected abstract void fireTransition(NSFStateMachineContext context);

        #endregion Protected Methods

        #region Internal Constructors

        /// <summary>
        /// Creates a transition.
        /// </summary>
        /// <param name="name">User assigned name for transition.</param>
        /// <param name="source">Transition source.</param>
        /// <param name="target">Transition target.</param>
        /// <param name="trigger">Transition trigger.</param>
        /// <param name="guard">Transition guard.</param>
        /// <param name="action">Transition action.</param>
        /// <remarks>Deprecated - Use NSFExternalTransition or NSFLocalTransition</remarks>
        protected NSFTransition(NSFString name, NSFState source, NSFState target, NSFEvent trigger, NSFBoolGuard<NSFStateMachineContext> guard, NSFVoidAction<NSFStateMachineContext> action)
            : base(name)
        {
            this.source = source;
            this.target = target;
            Guards += guard;
            Actions += action;

            addTrigger(trigger);

            // Validity check
            if ((source == target) && (triggers.Count == 0) && Guards.isEmpty())
            {
                throw new Exception(Name + " invalid self-transition with no trigger or guard");
            }

            target.addIncomingTransition(this);
            // Outgoing transitions must be added by concrete classes

            Guards.setExceptionAction(handleGuardException);
            Actions.setExceptionAction(handleActionException);
        }

        #endregion Internal Constructors

        #region Internal Methods

        /// <summary>
        /// Processes an event, evaluating if the event results in the transition firing.
        /// </summary>
        /// <remarks>
        /// This method is for use only by the North State Framework's internal logic.
        /// </remarks>
        protected internal NSFEventStatus processEvent(NSFEvent nsfEvent)
        {
            bool transitionTriggered = false;

            if (triggers.Count == 0)
            {
                transitionTriggered = true;
            }
            else
            {
                foreach (NSFEvent trigger in Triggers)
                {
                    if (trigger.Id == nsfEvent.Id)
                    {
                        transitionTriggered = true;
                        break;
                    }
                }
            }

            if (!transitionTriggered)
            {
                return NSFEventStatus.NSFEventUnhandled;
            }

            NSFStateMachineContext newContext = new NSFStateMachineContext(Source.TopStateMachine, null, null, this, nsfEvent);

            if (!Guards.execute(newContext))
            {
                return NSFEventStatus.NSFEventUnhandled;
            }

            fireTransition(newContext);

            return NSFEventStatus.NSFEventHandled;
        }

        #endregion Internal Methods

        #region Private Fields, Events, and Properties

        private NSFState source;
        private NSFState target;
        private List<NSFEvent> triggers = new List<NSFEvent>();

        #endregion Private Fields, Events, and Properties

        #region Private Methods

        /// <summary>
        /// Handles an exception that occured while performing a transition action.
        /// </summary>
        /// <param name="context">The exception context.</param>
        private void handleActionException(NSFExceptionContext context)
        {
            Source.TopStateMachine.handleException(new Exception(Name + " transition action exception", context.Exception));
        }

        /// <summary>
        /// Handles an exception that occured while evaluating a transition guard.
        /// </summary>
        /// <param name="context">The exception context.</param>
        private void handleGuardException(NSFExceptionContext context)
        {
            Source.TopStateMachine.handleException(new Exception(Name + " transition guard exception", context.Exception));
        }

        #endregion Private Methods
    }
}