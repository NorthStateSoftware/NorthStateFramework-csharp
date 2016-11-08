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
    /// Represents a general purpose action or guard context.
    /// </summary>
    public class NSFContext : EventArgs
    {
        #region Public Constructors

        /// <summary>
        /// Creates a general purpose action or guard context.
        /// </summary>
        /// <param name="source">The source of invocation.</param>
        public NSFContext(object source)
        {
            Source = source;
        }

        #endregion Public Constructors

        #region Public Fields, Events, and Properties

        /// <summary>
        /// Gets or sets the source of invocation.
        /// </summary>
        public object Source { get; private set; }

        #endregion Public Fields, Events, and Properties
    }

    /// <summary>
    /// Represents contextual information for NSF event actions.
    /// </summary>
    public class NSFEventContext : NSFContext
    {
        #region Public Constructors

        /// <summary>
        /// Creates an NSF event context.
        /// </summary>
        /// <param name="source">The source of invocation.</param>
        /// <param name="nsfEvent">The event.</param>
        public NSFEventContext(object source, NSFEvent nsfEvent)
            : base(source)
        {
            Event = nsfEvent;
        }

        #endregion Public Constructors

        #region Public Fields, Events, and Properties

        /// <summary>
        /// Gets or sets the NSF event.
        /// </summary>
        public NSFEvent Event { get; private set; }

        #endregion Public Fields, Events, and Properties
    }

    /// <summary>
    /// Represents contextual information for exception actions.
    /// </summary>
    public class NSFExceptionContext : NSFContext
    {
        #region Public Constructors

        /// <summary>
        /// Creates an exception context.
        /// </summary>
        /// <param name="source">The source of invocation.</param>
        /// <param name="exception">The exception that occurred.</param>
        public NSFExceptionContext(object source, Exception exception)
            : base(source)
        {
            Exception = exception;
        }

        #endregion Public Constructors

        #region Public Fields, Events, and Properties

        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        public Exception Exception { get; private set; }

        #endregion Public Fields, Events, and Properties
    }

    /// <summary>
    /// Represents contextual information for state machine actions and guards.
    /// </summary>
    public class NSFStateMachineContext : NSFContext
    {
        #region Public Constructors

        /// <summary>
        /// Creates a state machine context.
        /// </summary>
        /// <param name="source">The state machine.</param>
        /// <param name="enteringState">The state being entered.</param>
        /// <param name="exitingState">The state being exited.</param>
        /// <param name="transition">The associated transition.</param>
        /// <param name="trigger">The triggering event.</param>
        public NSFStateMachineContext(NSFStateMachine source, NSFState enteringState, NSFState exitingState, NSFTransition transition, NSFEvent trigger)
            : base(source)
        {
            EnteringState = enteringState;
            ExitingState = exitingState;
            Transition = transition;
            Trigger = trigger;
        }

        #endregion Public Constructors

        #region Public Fields, Events, and Properties

        /// <summary>
        /// Gets or sets the state being entered.
        /// </summary>
        /// <remarks>
        /// Entering state is null unless the context corresponds to an entry action.
        /// </remarks>
        public NSFState EnteringState { get; internal set; }

        /// <summary>
        /// Gets or sets the state being exited.
        /// </summary>
        /// <remarks>
        /// Exiting state is null unless the context corresponds to an exit action.
        /// </remarks>
        public NSFState ExitingState { get; internal set; }

        /// <summary>
        /// Gets the source of invocation.
        /// </summary>
        new public NSFStateMachine Source
        {
            get
            {
                return (base.Source as NSFStateMachine);
            }
        }

        /// <summary>
        /// Gets or sets the associated transition.
        /// </summary>
        public NSFTransition Transition { get; private set; }

        /// <summary>
        /// Gets or sets the triggering event.
        /// </summary>
        public NSFEvent Trigger { get; private set; }

        #endregion Public Fields, Events, and Properties
    }
}