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
    /// Represents a state machine.
    /// </summary>
    public class NSFStateMachine : NSFCompositeState, INSFEventHandler
    {
        #region Public Constructors

        /// <summary>
        /// Creates a state machine
        /// </summary>
        /// <param name="name">The user defined name for the state machine.</param>
        /// <param name="thread">The thread on which events for the state machine are queued.</param>
        public NSFStateMachine(NSFString name, NSFEventThread thread)
            : base(name, (NSFRegion)null, null, null)
        {
            construct(thread);
        }

        /// <summary>
        /// Creates a state machine.
        /// </summary>
        /// <param name="name">The user defined name for the state machine.</param>
        /// <param name="parentRegion">The parent region of the state machine.</param>
        public NSFStateMachine(NSFString name, NSFRegion parentRegion)
            : base(name, parentRegion, null, null)
        {
            construct(TopStateMachine.EventThread);
        }

        /// <summary>
        /// Creates a state machine.
        /// </summary>
        /// <param name="name">The user defined name for the state machine.</param>
        /// <param name="parentState">The parent composite state for the state machine.</param>
        public NSFStateMachine(NSFString name, NSFCompositeState parentState)
            : base(name, parentState, null, null)
        {
            construct(TopStateMachine.EventThread);
        }

        #endregion Public Constructors

        #region Public Fields, Events, and Properties

        /// <summary>
        /// Actions to execute if an exception is encountered by the state machine.
        /// </summary>
        public NSFVoidActions<NSFExceptionContext> ExceptionActions = new NSFVoidActions<NSFExceptionContext>();

        /// <summary>
        /// Actions to execute whenever a new state is entered.
        /// </summary>
        public NSFVoidActions<NSFStateMachineContext> StateChangeActions = new NSFVoidActions<NSFStateMachineContext>();

        /// <summary>
        /// Gets or sets the flag indicating if consecutive loop detection is enabled.
        /// </summary>
        /// <remarks>
        /// State machines have the ability to detect if they are ill-formed by way of consecutive loop detection.
        /// This mechanism looks for repeated transitions without a pause (as defined by the condition when there
        /// are no more events queued to the state machine, and the final run-to-completion step has occurred).
        /// If more than the specified number of transitions occur without a pause, the state machine will remove
        /// its queued events, call the consecutive loop limit actions, and stop after executing any events queued
        /// by the actions.
        /// </remarks>
        public bool ConsecutiveLoopDetectionEnabled { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of transitions without a pause allowed by consecutive loop detection.
        /// </summary>
        /// <remarks>
        /// State machines have the ability to detect if they are ill-formed by way of consecutive loop detection.
        /// This mechanism looks for repeated transitions without a pause (as defined by the condition when there
        /// are no more events queued to the state machine, and the final run-to-completion step has occurred).
        /// If more than the specified number of transitions occur without a pause, the state machine will remove
        /// its queued events, call the consecutive loop limit actions, and stop after executing any events queued
        /// by the actions.
        /// </remarks>
        public int ConsecutiveLoopLimit { get; set; }

        /// <summary>
        /// Actions to execute if consecutive loop limit is reached.
        /// </summary>
        /// <remarks>
        /// State machines have the ability to detect if they are ill-formed by way of consecutive loop detection.
        /// This mechanism looks for repeated transitions without a pause (as defined by the condition when there
        /// are no more events queued to the state machine, and the final run-to-completion step has occurred).
        /// If more than the specified number of transitions occur without a pause, the state machine will remove
        /// its queued events, call the consecutive loop limit actions, and stop after executing any events queued
        /// by the limit actions.
        /// </remarks>
        public NSFVoidActions<NSFStateMachineContext> ConsecutiveLoopLimitActions = new NSFVoidActions<NSFStateMachineContext>();

        /// <summary>
        /// Gets or sets the flag indicating if event limit detection is enabled.
        /// </summary>
        /// <remarks>
        /// State machines have the ability to detect if they are not processing messages in a timely fashion by
        /// way of event limit detection.  This mechanism looks at the number of events in the state machine's
        /// event queue.  If the more than the specified number of events are queued, the state machine will
        /// remove its queued events, call the event limit actions, and stop after executing any events queued
        /// by the limit actions.
        /// </remarks>
        public bool EventLimitDetectionEnabled { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of transitions without a pause allowed by consecutive loop detection.
        /// </summary>
        /// <remarks>
        /// State machines have the ability to detect if they are not processing messages in a timely fashion by
        /// way of event limit detection.  This mechanism looks at the number of events in the state machine's
        /// event queue.  If the more than the specified number of events are queued, the state machine will
        /// remove its queued events, call the event limit actions, and stop after executing any events queued
        /// by the limit actions.
        /// </remarks>
        public int EventLimit { get; set; }

        /// <summary>
        /// Actions to execute if consecutive loop limit is reached.
        /// </summary>
        /// <remarks>
        /// State machines have the ability to detect if they are not processing messages in a timely fashion by
        /// way of event limit detection.  This mechanism looks at the number of events in the state machine's
        /// event queue.  If the more than the specified number of events are queued, the state machine will
        /// remove its queued events, call the event limit actions, and stop after executing any events queued
        /// by the limit actions.
        /// </remarks>
        public NSFVoidActions<NSFStateMachineContext> EventLimitActions = new NSFVoidActions<NSFStateMachineContext>();

        /// <summary>
        ///  Provides a syntactical method for specifying "Else" as a transition guard.
        /// </summary>
        public NSFBoolGuard<NSFStateMachineContext> Else { get { return null; } }

        /// <summary>
        ///  Provides a syntactical method for specifying "Never" as a transition guard, effectively blocking the transition from ever occuring.
        /// </summary>
        public bool Never(NSFStateMachineContext context) { return false; }

        /// <summary>
        /// Gets or sets the state machine's event thread.
        /// </summary>
        public NSFEventThread EventThread { get; private set; }

        /// <summary>
        /// Gets or sets the number of events currently queued in state machine's event thread.
        /// </summary>
        /// <remarks>
        /// This mechanism only works if events are queued throught the state machine's queueEvent methods.
        /// </remarks>
        public Int32 QueuedEvents { get; private set; }

        /// <summary>
        /// Gets or sets the flag indicating if state machine event logging is enabled.
        /// </summary>
        public bool LoggingEnabled { get; set; }

        public NSFEventHandlerRunStatus RunStatus { get; private set; }
        public NSFEventHandlerTerminationStatus TerminationStatus { get; private set; }

        /// <summary>
        /// Gets the amount of time (mS) the <see cref="terminate"/> method sleeps between checks
        /// for state machine termination.
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

        public override NSFStateMachine TopStateMachine
        {
            get
            {
                if (isTopStateMachine())
                {
                    return this;
                }
                else
                {
                    return base.TopStateMachine;
                }
            }
        }

        #endregion Public Fields, Events, and Properties

        #region Public Methods

        /// <summary>
        /// Forces the state machine to evaluate transitions.
        /// </summary>
        /// <remarks>
        /// This method is rarely needed, but can be used, for example, to force evaluation if a guard condition has changed.
        /// </remarks>
        public void forceStateMachineEvaluation()
        {
            runToCompletion();
        }

        /// <summary>
        /// Handles an event.
        /// </summary>
        /// <param name="nsfEvent">The event to handle.</param>
        /// <returns>Status indicating if the event was handled or not.</returns>
        /// <remarks>
        /// This method is for use only by the North State Framework's internal logic.
        /// It processes the event using UML defined behavior, including run to completion.
        /// </remarks>
        public NSFEventStatus handleEvent(NSFEvent nsfEvent)
        {
            lock (stateMachineMutex)
            {
                --QueuedEvents;

                // This should only happen if events are queued without using the queueEvent method
                if (QueuedEvents < 0)
                {
                    QueuedEvents = 0;
                }
            }

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
            else if (nsfEvent == resetEvent)
            {
                reset();
            }

            // Don't process events if stopped
            if (RunStatus == NSFEventHandlerRunStatus.EventHandlerStopped)
            {
                return NSFEventStatus.NSFEventUnhandled;
            }

            // If not already active, enter state machine at the root
            if (!active)
            {
                enter(new NSFStateMachineContext(this, this, null, null, startEvent), false);
            }

            // Process the event
            NSFEventStatus eventStatus = NSFEventStatus.NSFEventUnhandled;
            try
            {
                eventStatus = processEvent(nsfEvent);

                if (eventStatus == NSFEventStatus.NSFEventHandled)
                {
                    runToCompletion();
                }

                // Consecutive loop detection looks for too many events without the state machine pausing.
                // If more than the specified number of transitions occur without a pause, the state machine will remove
                // its queued events, call the consecutive loop limit actions, and stop after executing any events queued
                // by the actions.
                if (ConsecutiveLoopDetectionEnabled)
                {
                    ++consecutiveLoopCount;

                    if (consecutiveLoopCount == ConsecutiveLoopLimit)
                    {
                        lock (stateMachineMutex)
                        {
                            EventThread.removeEventsFor(this);
                            QueuedEvents = 0;
                        }

                        ConsecutiveLoopLimitActions.execute(new NSFStateMachineContext(this, null, null, null, nsfEvent));

                        // Stop the state machine so that no more event processing occurs until started again
                        stopStateMachine();

                        // Reset consecutive loop count in case state machine is started again
                        consecutiveLoopCount = 0;

                        NSFTraceLog.PrimaryTraceLog.addTrace(NSFTraceTags.ErrorTag, NSFTraceTags.SourceTag, Name, NSFTraceTags.MessageTag, "ConsecutiveLoopLimit");
                    }
                    else if (QueuedEvents == 0)
                    {
                        // If no events are queued for this state machine, then it has paused, indicating it's not in an infinite loop.
                        consecutiveLoopCount = 0;                
                    }
                }
            }
            catch (Exception exception)
            {
                handleException(new Exception(nsfEvent.Name + " event handling exception", exception));
            }

            return eventStatus;
        }

        public void queueEvent(NSFEvent nsfEvent)
        {
            queueEvent(nsfEvent, false, LoggingEnabled);
        }

        public void queueEvent(NSFEvent nsfEvent, INSFNamedObject source)
        {
            nsfEvent.Source = source;
            queueEvent(nsfEvent);
        }

        /// <summary>
        /// Resets the state machine back to its initial default state.
        /// </summary>
        /// <remarks>
        /// If the state machine is currently running (started),
        /// it will process all events currently in its queue before resetting and
        /// it will continue running after the reset.
        /// If the state machine is currently stopped, it will continue to be stopped after the reset.
        /// While a state machine is stopped, all events are dropped from its queue.
        /// </remarks>
        public void resetStateMachine()
        {
            queueEvent(resetEvent);
        }

        public void startEventHandler()
        {
            startStateMachine();
        }

        public void stopEventHandler()
        {
            stopStateMachine();
        }

        public void startStateMachine()
        {
            queueEvent(startEvent);
        }

        public void stopStateMachine()
        {
            queueEvent(stopEvent);
        }

        public void terminate(bool waitForTerminated)
        {
            if (!isTopStateMachine())
            {
                TopStateMachine.terminate(waitForTerminated);
                return;
            }

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

                handleException(new Exception("State machine was unable to terminate"));
            }

        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Handles exceptions raised during state machine processing.
        /// </summary>
        /// <param name="exception">The exception thrown.</param>
        /// <remarks>
        /// By default, any exception actions are executed first,
        /// then the exception is forwarded to the global exception handler, NSFExceptionHandler.handleException().
        /// </remarks>
        protected internal void handleException(Exception exception)
        {
            NSFExceptionContext newContext = new NSFExceptionContext(this, new Exception(Name + " state machine exception", exception));
            ExceptionActions.execute(newContext);
            NSFExceptionHandler.handleException(newContext);
        }

        /// <summary>
        /// Checks if an event is queued for processing.
        /// </summary>
        /// <param name="nsfEvent">The event in queustion.</param>
        /// <returns>True if the event is the in queue, otherwise false.</returns>
        protected bool hasEvent(NSFEvent nsfEvent)
        {
            return EventThread.hasEvent(nsfEvent);
        }

        /// <summary>
        /// Checks if any events are queued for processing.
        /// </summary>
        /// <returns>True if there are events queued for processing, otherwise false.</returns>
        bool hasEvent()
        {
            return EventThread.hasEventFor(this);
        }

        /// <summary>
        /// Checks if the state machine is the top state machine in a nested heirarchy.
        /// </summary>
        /// <returns>True if the state machine is the top state machine, otherwise false.</returns>
        protected bool isTopStateMachine()
        {
            return (parentRegion == null);
        }

        /// <summary>
        /// Re-routes a transition between a specified source and destination
        /// </summary>
        /// <param name="transition">The transition to re-route.</param>
        /// <param name="source">The new source for the transition.</param>
        /// <param name="target">The new target for the transition.</param>
        /// <remarks>
        /// This method can be used when extending state machines.
        /// Careful design consideration should be made before using this method.
        /// Never change a state machines structure while it is running.
        /// </remarks>
        protected void rerouteTransition(NSFTransition transition, NSFState source, NSFState target)
        {
            transition.Source.removeOutgoingTransition(transition);
            transition.Target.removeIncomingTransition(transition);
            transition.Source = source;
            transition.Target = target;
        }

        protected internal override void reset()
        {
            // Base class behavior
            base.reset();

            // Additional behavior
            consecutiveLoopCount = 0;
        }

        #endregion Protected Methods

        #region Internal Methods

        /// <summary>
        /// Executes the actions in the state change actions list.
        /// </summary>
        internal void executeStateChangeActions(NSFStateMachineContext context)
        {
            StateChangeActions.execute(context);

            NSFStateMachine parentStateMachine = ParentStateMachine;
            if (parentStateMachine != null)
            {
                parentStateMachine.executeStateChangeActions(context);
            }
        }

        #endregion Internal Methods

        #region Private Fields, Events, and Properties

        private int consecutiveLoopCount = 0;
        private object stateMachineMutex = new object();

        private NSFEvent resetEvent;
        private NSFEvent runToCompletionEvent;
        private NSFEvent startEvent;
        private NSFEvent stopEvent;
        private NSFEvent terminateEvent;
        private static uint terminationSleepTime = 10;
        private static int terminationTimeout = 60000;

        #endregion Private Fields, Events, and Properties

        #region Private Methods

        /// <summary>
        /// Performs common construction behaviors.
        /// </summary>
        /// <param name="thread">The state machine's thread.</param>
        private void construct(NSFEventThread thread)
        {
            ConsecutiveLoopDetectionEnabled = true;
            ConsecutiveLoopLimit = 1000;
            EventLimitDetectionEnabled = true;
            EventLimit = 100;
            EventThread = thread;
            QueuedEvents = 0;
            LoggingEnabled = true;
            RunStatus = NSFEventHandlerRunStatus.EventHandlerStopped;
            TerminationStatus = NSFEventHandlerTerminationStatus.EventHandlerReady;

            resetEvent = new NSFEvent("Reset", this);
            runToCompletionEvent = new NSFEvent("RunToCompletion", this);
            startEvent = new NSFEvent("Start", this);
            stopEvent = new NSFEvent("Stop", this);
            terminateEvent = new NSFEvent("Terminate", this);

            StateChangeActions.setExceptionAction(handleStateChangeActionException);

            if (isTopStateMachine())
            {
                EventThread.addEventHandler(this);
            }
        }

        /// <summary>
        /// Handles exceptions caught while executing state change actions.
        /// </summary>
        /// <param name="context">Additional contextual information.</param>
        private void handleStateChangeActionException(NSFExceptionContext context)
        {
            handleException(new Exception("State change action exception", context.Exception));
        }

        /// <summary>
        /// Adds the specified event to the state machine's event queue.
        /// </summary>
        /// <param name="nsfEvent">The event to queue.</param>
        /// <param name="isPriorityEvent">Flag indicating if the event should be queued to the back of the queue (false) or the front of the queue (true).</param>
        /// <param name="logEventQueued">Flag indicating if an event queued trace should be added to the trace log.</param>
        private void queueEvent(NSFEvent nsfEvent, bool isPriorityEvent, bool logEventQueued)
        {
            if (!isTopStateMachine())
            {
                TopStateMachine.queueEvent(nsfEvent, isPriorityEvent, logEventQueued);
                return;
            }

            lock (stateMachineMutex)
            {
                // Do not allow events to be queued if terminating or terminated,
                // except for run to completion event, which may be queued if terminating to allow proper semantics to continue until terminated.
                if ((TerminationStatus == NSFEventHandlerTerminationStatus.EventHandlerTerminated) ||
                    ((TerminationStatus == NSFEventHandlerTerminationStatus.EventHandlerTerminating) && (nsfEvent != runToCompletionEvent)))
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

                // Event limit detection looks for too many events queued for the state machine.
                // If more than the specified number of events are queued, the state machine will remove
                // its queued events, call the event limit actions, and stop after executing any events queued
                // by the limit actions.
                if ((EventLimitDetectionEnabled) && (QueuedEvents == EventLimit))
                {
                    EventThread.removeEventsFor(this);
                    QueuedEvents = 0;

                    EventLimitActions.execute(new NSFStateMachineContext(this, null, null, null, nsfEvent));

                    // Stop the state machine so that no more event processing occurs until started again
                    stopStateMachine();

                    NSFTraceLog.PrimaryTraceLog.addTrace(NSFTraceTags.ErrorTag, NSFTraceTags.SourceTag, Name, NSFTraceTags.MessageTag, "EventLimit");

                    return;
                }

                nsfEvent.Destination = this;

                EventThread.queueEvent(nsfEvent, isPriorityEvent, logEventQueued);

                ++QueuedEvents;
            }
        }

        /// <summary>
        /// Forces the state machine to run to completion.
        /// </summary>
        private void runToCompletion()
        {
            queueEvent(runToCompletionEvent, true, false);
        }

        #endregion Private Methods
    }
}