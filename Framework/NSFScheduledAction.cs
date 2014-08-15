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
    /// Represents an action or actions that can be scheduled for execution on a specified thread at a specified time.
    /// </summary>
    /// <remarks>
    /// Use this class to encapsulate a delegate or delegates so that they can be
    /// scheduled with the NSFOSTimer.  Actions execute on the specified event thread.
    /// </remarks>
    public class NSFScheduledAction : NSFTimerAction
    {
        #region Public Constructors

        /// <summary>
        /// Creates a scheduled action.
        /// </summary>
        /// <param name="name">The name of the scheduled action.</param>
        /// <param name="action">The action to execute.</param>
        /// <param name="eventThread">The thread on which the action will execute.</param>
        /// <remarks>
        /// Use null or String.Empty for the name if no name is desired.
        /// The action will be logged in the trace if the name is anything other than null or String.Empty.
        /// </remarks>
        public NSFScheduledAction(NSFString name, NSFVoidAction<NSFContext> action, NSFEventThread eventThread)
            : base(name)
        {
            Actions += action;
            Actions.setExceptionAction(handleActionException);

            eventHandler = new NSFEventHandler(Name, eventThread);
            executeActionsEvent = new NSFEvent(Name, this, eventHandler);

            eventHandler.LoggingEnabled = false;
            eventHandler.addEventReaction(executeActionsEvent, executeActions);

            eventHandler.startEventHandler();
        }

        #endregion Public Constructors

        #region Public Fields, Events, and Properties

        /// <summary>
        /// Actions to be executed at the scheduled time.
        /// </summary>
        public NSFVoidActions<NSFContext> Actions = new NSFVoidActions<NSFContext>();

        /// <summary>
        /// Gets the event thread that the actions will execute on.
        /// </summary>
        public NSFEventThread EventThread
        {
            get { return eventHandler.EventThread; }
        }

        #endregion Public Fields, Events, and Properties

        #region Internal Methods

        /// <summary>
        /// Callback method supporting NSFTimerAction interface.
        /// </summary>
        /// <remarks>
        /// This method is called by the NSFTimerThread to execute the actions at the scheduled time.
        /// </remarks>
        internal override void execute()
        {
            // Queue up event to be handled by the event handler on its thread.
            eventHandler.queueEvent(executeActionsEvent);
        }

        #endregion Internal Methods

        #region Private Fields, Events, and Properties

        private NSFEventHandler eventHandler;
        private NSFEvent executeActionsEvent;

        #endregion Private Fields, Events, and Properties

        #region Private Methods

        /// <summary>
        /// Event handler action that executes the scheduled action.
        /// </summary>
        /// <param name="context">Additional contextual information.</param>
        private void executeActions(NSFEventContext context)
        {
            if ((Name != null) && (Name != String.Empty))
            {
                NSFTraceLog.PrimaryTraceLog.addTrace(NSFTraceTags.ActionExecutedTag, NSFTraceTags.ActionTag, Name);
            }

            Actions.execute(context);
        }

        /// <summary>
        /// Handles exceptions caught during action execution.
        /// </summary>
        /// <param name="context">Additional contextual information.</param>
        private void handleActionException(NSFExceptionContext context)
        {
            Exception newException = new Exception(Name + " action exception", context.Exception);
            NSFExceptionHandler.handleException(new NSFExceptionContext(this, newException));
        }

        #endregion Private Methods
    }
}
