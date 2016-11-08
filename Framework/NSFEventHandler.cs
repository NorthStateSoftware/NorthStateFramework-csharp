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
using System.Threading;

using NSFId = System.Int64;

using NSFString = System.String;

namespace NorthStateSoftware.NorthStateFramework
{
    #region Enumerations

    /// <summary>
    /// Represents the status of the event after attempting to handle it.
    /// </summary>
    /// <remarks>
    /// The status NSFEventHandled indicates that an action was taken as the result of the event.
    /// The status NSFEventUnhandled indicates that no action was taken as the result of the event.
    /// </remarks>
    public enum NSFEventStatus { NSFEventUnhandled = 1, NSFEventHandled }

    /// <summary>
    /// Represents the run status of an event handler.
    /// </summary>
    /// <remarks>
    /// The run status indicates if the event handler is processing events in its queue.
    /// When the method getRunStatus() returns EventHandlerStarted, the event handler is processing events.
    /// Use the methods startEventHandler() and stopEventHandler() control event processing.
    /// </remarks>
    public enum NSFEventHandlerRunStatus { EventHandlerStopped = 1, EventHandlerStarted };

    /// <summary>
    /// Represents the termination status of an event handler.
    /// </summary>
    /// <remarks>
    /// The termination status of an event handler can be used to determine when it is safe to delete.
    /// When the method getTerminationStatus() returns EventHandlerTerminated, it is safe to delete the event handler.
    /// Use the method terminate(...) to initiate the termination process.
    /// </remarks>
    public enum NSFEventHandlerTerminationStatus { EventHandlerReady = 1, EventHandlerTerminating, EventHandlerTerminated };

    #endregion Enumerations

    /// <summary>
    /// Represents the interface for classes that can queue and handle events.
    /// </summary>
    public interface INSFEventHandler : INSFNamedObject
    {
        #region Fields, Events, and Properties

        /// <summary>
        /// Gets the run status of the event handler.
        /// </summary>
        /// <returns>The run status.</returns>
        /// <remarks>
        /// The run status indicates if the event handler is processing events in its queue.
        /// When the method getRunStatus() returns EventHandlerStarted, the event handler is processing events.
        /// Use the methods startEventHandler() and stopEventHandler() control event processing.
        /// </remarks>
        NSFEventHandlerRunStatus RunStatus { get; }

        /// <summary>
        /// Gets the termination status of the event handler.
        /// </summary>
        /// <returns>The termination status.</returns>
        /// <remarks>
        /// The termination status of an event handler can be used to determine when it is safe to delete.
        /// When the method getTerminationStatus() returns EventHandlerTerminated, it is safe to delete the event handler.
        /// Use the method terminate(...) to initiate the termination process.
        /// </remarks>
        NSFEventHandlerTerminationStatus TerminationStatus { get; }

        #endregion Fields, Events, and Properties

        #region Methods

        /// <summary>
        /// Handles an event.
        /// </summary>
        /// <param name="nsfEvent">The event to handle.</param>
        /// <returns>Status indicating if the event was handled or not.</returns>
        /// <remarks>
        /// This method is for use only by the North State Framework's internal logic.
        /// </remarks>
        NSFEventStatus handleEvent(NSFEvent nsfEvent);

        /// <summary>
        /// Queues an event for the handler.
        /// </summary>
        /// <param name="nsfEvent">The event to queue.</param>
        void queueEvent(NSFEvent nsfEvent);

        /// <summary>
        /// Queues an event for the handler.
        /// </summary>
        /// <param name="nsfEvent">The event to queue.</param>
        /// <param name="source">The source of the event.</param>
        void queueEvent(NSFEvent nsfEvent, INSFNamedObject source);

        /// <summary>
        /// Starts event processing.
        /// </summary>
        /// <remarks>
        /// When stopped, events queued prior to calling the method startEventHandler() will be dropped.
        /// Events queued after calling the method startEventHandler() will be processed, until the next call to stopEventHandler().
        /// When started, events queued prior to calling the method stopEventHandler() will be processed.
        /// Events queued after calling the method stopEventHandler() will be dropped, until the next call to startEventHandler().
        /// </remarks>
        void startEventHandler();

        /// <summary>
        /// Stops event processing.
        /// </summary>
        /// <remarks>
        /// When stopped, events queued prior to calling the method startEventHandler() will be dropped.
        /// Events queued after calling the method startEventHandler() will be processed, until the next call to stopEventHandler().
        /// When started, events queued prior to calling the method stopEventHandler() will be processed.
        /// Events queued after calling the method stopEventHandler() will be dropped, until the next call to startEventHandler().
        /// </remarks>
        void stopEventHandler();

        /// <summary>
        /// Terminates the event handler, so that it can be garbage collected.
        /// </summary>
        /// <param name="waitForTerminated">Flag indicating if the method should wait until the event handler is terminated (true), or if it should return immediately (false).</param>
        /// <remarks>
        /// This method terminates the event handler in a coordinated fashion,
        /// making sure all remaining events are cleared, so that it can be garbage collected.
        /// After calling this method, the event handler cannot be restarted.
        /// This method should only be called if the event handler should be garbage collected.
        /// If the waitForTerminated flag is set true, this method must not be called from the event handler's thread of execution.
        /// </remarks>
        void terminate(bool waitForTerminated);

        #endregion Methods
    }

    /// <summary>
    /// Represents a light-weight event handler.
    /// </summary>
    /// <remarks>
    /// This class associates actions with events, executing the actions when the event is handled.
    /// Events are queued to the specified thread and handled in the order they are received.
    /// Actions execute on the specified thread.
    /// This class is a light-weight event handler which can be used instead of a state machine.
    /// </remarks>
    public class NSFEventHandler : NSFTaggedObject, INSFEventHandler
    {
        #region Public Constructors

        /// <summary>
        /// Creates an event handler
        /// </summary>
        /// <param name="name">The user defined name for the state machine.</param>
        /// <param name="thread">The thread on which events for the state machine are queued.</param>
        public NSFEventHandler(NSFString name, NSFEventThread thread)
            : base(name)
        {
            EventThread = thread;
            LoggingEnabled = true;
            RunStatus = NSFEventHandlerRunStatus.EventHandlerStopped;
            TerminationStatus = NSFEventHandlerTerminationStatus.EventHandlerReady;
            startEvent = new NSFEvent("Start", this);
            stopEvent = new NSFEvent("Stop", this);
            terminateEvent = new NSFEvent("Terminate", this);

            EventThread.addEventHandler(this);
        }

        #endregion Public Constructors

        #region Public Fields, Events, and Properties

        public NSFEventHandlerRunStatus RunStatus { get; private set; }
        public NSFEventHandlerTerminationStatus TerminationStatus { get; private set; }

        /// <summary>
        /// Gets the amount of time (mS) the <see cref="terminate"/> method sleeps between checks
        /// for event handler termination.
        /// </summary>
        public static uint TerminationSleepTime
        {
            get { return terminationSleepTime; }
            set { terminationSleepTime = value; }
        }

        /// <summary>
        /// The amount of time the <see cref="terminate"/> method will wait for event processing to complete.
        /// </summary>
        public static int TerminationTimeout
        {
            get { return terminationTimeout; }
            set { terminationTimeout = value; }
        }

        /// <summary>
        /// Gets or sets the event handler's thread.
        /// </summary>
        public NSFEventThread EventThread { get; private set; }

        /// <summary>
        /// Gets or sets the flag indicating if event logging is enabled.
        /// </summary>
        public bool LoggingEnabled { get; set; }

        #endregion Public Fields, Events, and Properties

        #region Public Methods

        /// <summary>
        /// Adds a reaction to a specified event.
        /// </summary>
        /// <param name="nsfEvent">The event causing the action.</param>
        /// <param name="action">The action taken as a result of the event.</param>
        public void addEventReaction(NSFEvent nsfEvent, NSFVoidAction<NSFEventContext> action)
        {
            lock (eventHandlerMutex)
            {
                if (!eventReactions.ContainsKey(nsfEvent.Id))
                {
                    eventReactions.Add(nsfEvent.Id, new NSFVoidActions<NSFEventContext>());
                    eventReactions[nsfEvent.Id].setExceptionAction(handleEventReactionException);
                }

                eventReactions[nsfEvent.Id].add(action);
            }
        }

        /// <summary>
        /// Handles an event.
        /// </summary>
        /// <param name="nsfEvent">The event to handle.</param>
        /// <returns>Status indicating if the event was handled or not.</returns>
        /// <remarks>
        /// This method is for use only by the North State Framework's internal logic.
        /// It calls the actions associated with the event, if any.
        /// </remarks>
        public NSFEventStatus handleEvent(NSFEvent nsfEvent)
        {
            // Handle status changing events
            if ((nsfEvent == startEvent))
            {
                RunStatus = NSFEventHandlerRunStatus.EventHandlerStarted;
            }
            else if (nsfEvent == stopEvent)
            {
                RunStatus = NSFEventHandlerRunStatus.EventHandlerStopped;
            }
            else if (nsfEvent == terminateEvent)
            {
                TerminationStatus = NSFEventHandlerTerminationStatus.EventHandlerTerminated;
                EventThread.removeEventHandler(this);
                return NSFEventStatus.NSFEventHandled;
            }

            // Don't process events if stopped
            if (RunStatus == NSFEventHandlerRunStatus.EventHandlerStopped)
            {
                return NSFEventStatus.NSFEventUnhandled;
            }

            // Process the event

            bool actionsToExecute = false;

            lock (eventHandlerMutex)
            {
                if (eventReactions.ContainsKey(nsfEvent.Id))
                {
                    actionsToExecute = true;
                }
            }

            if (actionsToExecute)
            {
                eventReactions[nsfEvent.Id].execute(new NSFEventContext(this, nsfEvent));
                return NSFEventStatus.NSFEventHandled;
            }
            else
            {
                return NSFEventStatus.NSFEventUnhandled;
            }
        }

        /// <summary>
        /// Checks if an event is queued for handling.
        /// </summary>
        /// <param name="nsfEvent">The event in queustion.</param>
        /// <returns>True if the event is the in queue, otherwise false.</returns>
        public bool hasEvent(NSFEvent nsfEvent)
        {
            return EventThread.hasEvent(nsfEvent);
        }

        /// <summary>
        /// Checks if any events are queued for handling.
        /// </summary>
        /// <returns>True if there are events queued for handling, otherwise false.</returns>
        public bool hasEvent()
        {
            return EventThread.hasEventFor(this);
        }

        public void queueEvent(NSFEvent nsfEvent)
        {
            lock (eventHandlerMutex)
            {
                // Do not allow events to be queued if terminating or terminated (i.e. not ready)
                if (TerminationStatus != NSFEventHandlerTerminationStatus.EventHandlerReady)
                {
                    return;
                }

                // Handle special case of terminate event by setting status and queuing a single terminate event.
                // Terminate event must be the last event queued to guarantee safe deletion when it is handled.
                if (nsfEvent == terminateEvent)
                {
                    if (TerminationStatus == NSFEventHandlerTerminationStatus.EventHandlerReady)
                    {
                        TerminationStatus = NSFEventHandlerTerminationStatus.EventHandlerTerminating;
                    }
                }

                nsfEvent.Destination = this;
                EventThread.queueEvent(nsfEvent, false, LoggingEnabled);
            }
        }

        public void queueEvent(NSFEvent nsfEvent, INSFNamedObject source)
        {
            nsfEvent.Source = source;
            queueEvent(nsfEvent);
        }

        /// <summary>
        /// Removes a reaction to a specified event.
        /// </summary>
        /// <param name="nsfEvent">The event causing the action.</param>
        /// <param name="action">The action taken as a result of the event.</param>
        public void removeEventReaction(NSFEvent nsfEvent, NSFVoidAction<NSFEventContext> action)
        {
            lock (eventHandlerMutex)
            {
                if (eventReactions.ContainsKey(nsfEvent.Id))
                {
                    eventReactions[nsfEvent.Id].remove(action);
                }
            }
        }

        public void startEventHandler()
        {
            queueEvent(startEvent);
        }

        public void stopEventHandler()
        {
            queueEvent(stopEvent);
        }

        public void terminate(bool waitForTerminated)
        {
            queueEvent(terminateEvent);

            if (waitForTerminated)
            {
                for (uint i = 0; i < TerminationTimeout; i += TerminationSleepTime)
                {
                    if (TerminationStatus == NSFEventHandlerTerminationStatus.EventHandlerTerminated)
                    {
                        return;
                    }

                    NSFOSThread.sleep(TerminationSleepTime);
                }

                handleException(new Exception("Event handler was unable to terminate"));
            }
        }

        #endregion Public Methods

        #region Private Fields, Events, and Properties

        private Dictionary<NSFId, NSFVoidActions<NSFEventContext>> eventReactions = new Dictionary<NSFId, NSFVoidActions<NSFEventContext>>();
        private object eventHandlerMutex = new object();
        private NSFEvent startEvent;
        private NSFEvent stopEvent;
        private NSFEvent terminateEvent;
        private static uint terminationSleepTime = 10;
        private static int terminationTimeout = 60000;

        #endregion Private Fields, Events, and Properties

        #region Private Methods

        /// <summary>
        /// Handles exceptions caught while executing event actions.
        /// </summary>
        /// <param name="context">Additional contextual information.</param>
        private void handleEventReactionException(NSFExceptionContext context)
        {
            handleException(new Exception("Event reaction exception", context.Exception));
        }

        /// <summary>
        /// Handles exceptions raised during processing.
        /// </summary>
        /// <param name="exception">The exception thrown.</param>
        /// <remarks>
        /// The exception is forwarded to the global exception handler, NSFExceptionHandler.handleException().
        /// </remarks>
        private void handleException(Exception exception)
        {
            NSFExceptionContext newContext = new NSFExceptionContext(this, new Exception(Name + " event handler exception", exception));
            NSFExceptionHandler.handleException(newContext);
        }

        #endregion Private Methods
    }
}
