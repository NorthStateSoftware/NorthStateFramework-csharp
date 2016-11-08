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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

using NSFString = System.String;

using NSFTime = System.Int64;

namespace NorthStateSoftware.NorthStateFramework
{
    /// <summary>
    /// Represents a timer thread.
    /// </summary>
    /// <summary>
    /// A timer thread contains a list of timer actions to execute at specified times.
    /// </summary>
    public class NSFTimerThread : NSFThread
    {
        #region Public Constructors

        /// <summary>
        /// Creates a timer thread.
        /// </summary>
        /// <param name="name">User specified name for timer.</param>
        public NSFTimerThread(NSFString name)
            : base(name, NSFOSThread.HighestPriority)
        {
            MaxAllowableTimeGap = 5000;
            TimeGapActions.setExceptionAction(handleTimeGapActionException);

            timer = NSFOSTimer.create(name);

            startThread();
        }

        #endregion Public Constructors

        #region Public Fields, Events, and Properties

        /// <summary>
        /// Actions to be executed whenever the timer encounters delay greater than MaxAllowableTimeGap in processing actions.
        /// </summary>
        /// <remarks>
        /// In a well behaved system this event should not fire.  It is provided for diagnostic purposes.
        /// </remarks>
        public NSFVoidActions<NSFContext> TimeGapActions = new NSFVoidActions<NSFContext>();

        /// <summary>
        /// Gets the current time.
        /// </summary>
        public NSFTime CurrentTime { get { return Timer.CurrentTime; } }

        /// <summary>
        /// Gets or sets the maximum allowable delay the timer can see before firing the TimeGapActions.
        /// </summary>
        /// <remarks>
        /// This property specifies the time in milli-seconds.  Set to zero to disable.
        /// </remarks>
        public int MaxAllowableTimeGap { get; set; }

        /// <summary>
        /// Gets or sets the maximum observed delay in executing timer actions.
        /// </summary>
        public NSFTime MaxObservedTimeGap { get; protected set; }

        /// <summary>
        /// Gets the underlying timer.
        /// </summary>
        public NSFOSTimer Timer { get { return timer; } }

        /// <summary>
        /// Gets the primary timer thread of the North State Framework.
        /// </summary>
        /// <remarks>
        /// Although it is possible to create additional timers, this is very rare and should be carefully considered if it is necessary.
        /// </remarks>
        public static NSFTimerThread PrimaryTimerThread { get { return primaryTimerThread; } }

        #endregion Public Fields, Events, and Properties

        #region Public Methods

        /// <summary>
        /// Checks if an action is scheduled.
        /// </summary>
        /// <param name="action">The action in question.</param>
        /// <returns>
        /// True if the action is scheduled, otherwise false.
        /// </returns>
        public bool isScheduled(NSFTimerAction action)
        {
            lock (threadMutex)
            {
                return actions.Contains(action);
            }
        }

        /// <summary>
        /// Schedules an action to execute at its previously designated execution time.
        /// </summary>
        /// <param name="action">The action to schedule.</param>
        public void scheduleAction(NSFTimerAction action)
        {
            lock (threadMutex)
            {
                // Do not schedule any actions if terminating or terminated (i.e. not ready)
                if (TerminationStatus != NSFThreadTerminationStatus.ThreadReady)
                {
                    return;
                }

                insertAction(action);

                // Set next timeout if action was inserted in the front of the list
                if (action == actions.First.Value)
                {
                    timer.setNextTimeout(action.ExecutionTime);
                }
            }
        }

        /// <summary>
        /// Schedules an action with the timer.
        /// </summary>
        /// <param name="action">The action to schedule.</param>
        /// <param name="delayTime">The delay time before the action should execute.</param>
        /// <param name="repeatTime">The repeat time if the action is periodic, or 0 if the action is non-periodic.</param>
        public void scheduleAction(NSFTimerAction action, NSFTime delayTime, NSFTime repeatTime)
        {
            action.ExecutionTime = CurrentTime + delayTime;
            action.RepeatTime = repeatTime;
            action.DelayTime = delayTime;

            scheduleAction(action);
        }

        public override void terminate(bool waitForTerminated)
        {
            // Base class behavior, but return immediately so timer can be set to wake up timer thread
            base.terminate(false);

            lock (threadMutex)
            {
                timer.setNextTimeout(CurrentTime);
            }

            // Wait as specified for thread to terminate after waking up
            base.terminate(waitForTerminated);
        }

        /// <summary>
        /// Unschedules a previously scheduled action.
        /// </summary>
        /// <param name="action">The action to unschedule.</param>
        /// <remarks>
        /// Unscheduling an action that is not currently scheduled has no effect.
        /// </remarks>
        public void unscheduleAction(NSFTimerAction action)
        {
            lock (threadMutex)
            {
                if (actions.Count != 0)
                {
                    if (action == actions.First.Value)
                    {
                        actions.Remove(action);

                        if (actions.Count != 0)
                        {
                            timer.setNextTimeout(actions.First.Value.ExecutionTime);
                        }
                    }
                    else
                    {
                        actions.Remove(action);
                    }
                }
            }
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Implements the main timer processing loop.
        /// </summary>
        protected override void threadLoop()
        {
            NSFTime currentTime = CurrentTime;
            List<NSFTimerAction> readyActions = new List<NSFTimerAction>();

            while (true)
            {
                // Set up next timeout
                lock (threadMutex)
                {
                    if (actions.Count != 0)
                    {
                        timer.setNextTimeout(actions.First.Value.ExecutionTime);
                    }
                    else
                    {
                        timer.setNextTimeout(Int32.MaxValue);
                    }
                }

                timer.waitForNextTimeout();

                // Clean up and return if terminating
                if (TerminationStatus != NSFThreadTerminationStatus.ThreadReady)
                {
                    actions.Clear();
                    return;
                }

                currentTime = CurrentTime;

                lock (threadMutex)
                {
                    // Create list of actions ready to execute
                    foreach (NSFTimerAction action in actions)
                    {
                        if (action.ExecutionTime <= currentTime)
                        {
                            readyActions.Add(action);
                        }
                        else
                        {
                            break;
                        }
                    }

                    // Reschedule repetitive actions
                    foreach (NSFTimerAction action in readyActions)
                    {
                        if (action.RepeatTime != 0)
                        {
                            action.ExecutionTime = action.ExecutionTime + action.RepeatTime;
                            insertAction(action);
                        }
                        else
                        {
                            actions.Remove(action);
                        }
                    }
                }

                // Check for excessive gap between current time and execution time
                if (readyActions.Count != 0)
                {
                    NSFTime timeGap = currentTime - readyActions[0].ExecutionTime;

                    if ((MaxAllowableTimeGap > 0) && (timeGap > MaxAllowableTimeGap) && (currentTime > nextTimeGapInterval))
                    {
                        NSFTraceLog.PrimaryTraceLog.addTrace(NSFTraceTags.ErrorTag, NSFTraceTags.SourceTag, Name, NSFTraceTags.MessageTag, "TimeGap", NSFTraceTags.ValueTag, timeGap.ToString());
                        TimeGapActions.execute(new NSFContext(this));

                        // Set time when next time gap can be recorded
                        // This prevents all gapped timer events from recording time gap trace
                        nextTimeGapInterval = currentTime + MaxAllowableTimeGap;
                    }

                    if (timeGap > MaxObservedTimeGap)
                    {
                        MaxObservedTimeGap = timeGap;
                    }
                }

                // Execute all ready actions
                while (readyActions.Count != 0)
                {
                    executeAction(readyActions[0]);
                    readyActions.RemoveAt(0);
                }
            }
        }

        #endregion Protected Methods

        #region Private Fields, Events, and Properties

        private static NSFTimerThread primaryTimerThread = new NSFTimerThread("PrimaryTimerThread");

        private LinkedList<NSFTimerAction> actions = new LinkedList<NSFTimerAction>();
        private NSFOSTimer timer;

        private NSFTime nextTimeGapInterval = 0;

        #endregion Private Fields, Events, and Properties

        #region Private Methods

        /// <summary>
        /// Executes the specified action, and reschedules it if a repeat time is specified.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        private void executeAction(NSFTimerAction action)
        {
            // Guard a bad action from taking down timer thread
            try
            {
                action.execute();
            }
            catch (Exception exception)
            {
                handleException(new Exception(action.Name + " action execution exception", exception));
            }
        }

        /// <summary>
        /// Handles an exception caught while executing time gap actions.
        /// </summary>
        /// <param name="context">The exception context.</param>
        private void handleTimeGapActionException(NSFExceptionContext context)
        {
            handleException(new Exception(Name + " time gap action exception", context.Exception));
        }

        private void insertAction(NSFTimerAction action)
        {
            // Make sure action is not already in list
            actions.Remove(action);

            // Insert into list based on execution time order
            // Actions with equal execution times are executed in FIFO order
            LinkedListNode<NSFTimerAction> nextNode = actions.First;
            while (nextNode != null)
            {
                if (action.ExecutionTime < nextNode.Value.ExecutionTime)
                {
                    actions.AddBefore(nextNode, action);
                    return;
                }
                nextNode = nextNode.Next;
            }

            // Insert action at end of list if not alread inserted
            actions.AddLast(action);
        }

        #endregion Private Methods
    }
}
