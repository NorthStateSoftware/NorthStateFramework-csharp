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

using NSFString = System.String;

namespace NorthStateSoftware.NorthStateFramework
{
    /// <summary>
    /// Represents a thread that has an event queue and dispatches events to their destinations.
    /// </summary>
    public class NSFEventThread : NSFThread
    {
        #region Public Constructors

        /// <summary>
        /// Creates an event thread.
        /// </summary>
        /// <param name="name">The name of the thread.</param>
        /// <remarks>
        /// The thread is created with medium priority.
        /// To change the priority, use the Thread property to access the underlying thread object.
        /// </remarks>
        public NSFEventThread(NSFString name)
            : this(name, NSFOSThread.MediumPriority)
        {
        }

        /// <summary>
        /// Creates an event thread.
        /// </summary>
        /// <param name="name">The name of the thread.</param>
        /// <param name="priority">The priority of the thread.</param>
        public NSFEventThread(NSFString name, int priority)
            : base(name, priority)
        {
            signal = NSFOSSignal.create(Name);
            startThread();
        }

        #endregion Public Constructors

        #region Public Fields, Events, and Properties

        /// <summary>
        /// Gets a list of event handlers using the thread.
        /// </summary>
        public List<INSFEventHandler> EventHandlers
        {
            get
            {
                lock (threadMutex)
                {
                    return new List<INSFEventHandler>(eventHandlers);
                }
            }
        }

        #endregion Public Fields, Events, and Properties

        #region Public Methods

        /// <summary>
        /// Indicates if the event queue contains an event that matches the specified event.
        /// </summary>
        /// <param name="nsfEvent">The event to match.</param>
        /// <returns>True if the event queue contains a matching event, false otherwise.</returns>
        /// <remarks>
        /// Two events match if they have the same id.  Events may be copied to create new events with the same id.
        /// </remarks>
        public bool hasEvent(NSFEvent nsfEvent)
        {
            lock (threadMutex)
            {
                foreach (NSFEvent queuedEvent in nsfEvents)
                {
                    if (queuedEvent.Id == nsfEvent.Id)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Indicates if the event queue contains an event for the specified destination.
        /// </summary>
        /// <param name="eventHandler">The event handler destination.</param>
        /// <returns>True if the event queue contains an event with the specified destination, false otherwise.</returns>
        public bool hasEventFor(INSFEventHandler eventHandler)
        {
            lock (threadMutex)
            {
                foreach (NSFEvent nsfEvent in nsfEvents)
                {
                    if (nsfEvent.Destination == eventHandler)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Removes all events for the specified event handler.
        /// </summary>
        /// <param name="eventHandler">The event handler destination.</param>
        public void removeEventsFor(INSFEventHandler eventHandler)
        {
            lock (threadMutex)
            {
                LinkedListNode<NSFEvent> node = nsfEvents.First;
                while (node != null)
                {
                    LinkedListNode<NSFEvent> nextNode = node.Next;
                    if (node.Value.Destination == eventHandler)
                    {
                        nsfEvents.Remove(node);
                    }
                    node = nextNode;
                }
            }
        }

        /// <summary>
        /// Queues the specified event.
        /// </summary>
        /// <param name="nsfEvent">The event to queue.</param>
        /// <param name="isPriorityEvent">Flag indicating if the event should be queued to the back of the queue (false) or the front of the queue (true).</param>
        /// <param name="logEventQueued">Flag indicating if an event queued trace should be added to the trace log.</param>
        public void queueEvent(NSFEvent nsfEvent, bool isPriorityEvent, bool logEventQueued)
        {
            // Do not allow events to be queued if terminated
            if (TerminationStatus == NSFThreadTerminationStatus.ThreadTerminated)
            {
                return;
            }

            lock (threadMutex)
            {
                if (logEventQueued)
                {
                    if (nsfEvent.Source != null)
                    {
                        NSFTraceLog.PrimaryTraceLog.addTrace(NSFTraceTags.EventQueuedTag,
                            NSFTraceTags.NameTag, nsfEvent.Name,
                            NSFTraceTags.SourceTag, nsfEvent.Source.Name,
                            NSFTraceTags.DestinationTag, nsfEvent.Destination.Name);
                    }
                    else
                    {
                        NSFTraceLog.PrimaryTraceLog.addTrace(NSFTraceTags.EventQueuedTag,
                            NSFTraceTags.NameTag, nsfEvent.Name,
                            NSFTraceTags.SourceTag, NSFTraceTags.UnknownTag,
                            NSFTraceTags.DestinationTag, nsfEvent.Destination.Name);
                    }
                }

                if (isPriorityEvent)
                {
                    nsfEvents.AddFirst(nsfEvent);
                }
                else
                {
                    nsfEvents.AddLast(nsfEvent);
                }
            }

            signal.send();
        }

        /// <summary>
        /// Terminates the thread by causing the thread loop to return.
        /// </summary>
        /// <param name="waitForTerminated">Flag indicating if the method should wait until the event thread is terminated (true), or if it should return immediately (false).</param>
        /// <remarks>
        /// This method is useful to guarantee that a thread is no longer active, so that it can be garbage collected.
        /// If the waitForTerminated flag is set true, this method must not be called from its thread of execution.
        /// Before terminating the event thread, this method terminates all event handlers using the thread.
        /// </remarks>
        public override void terminate(bool waitForTerminated)
        {
            // Get all event handler terminations started
            List<INSFEventHandler> eventHandlersCopy = EventHandlers;
            foreach (INSFEventHandler eventHandler in eventHandlersCopy)
            {
                eventHandler.terminate(false);
            }

            // Base class behavior, but return immediately so signal can be sent to wake up event thread
            base.terminate(false);

            signal.send();

            // Wait for thread to terminate after signal has been sent
            base.terminate(waitForTerminated);
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Implements the main event processing loop.
        /// </summary>
        protected override void threadLoop()
        {
            while (true)
            {
                // Wait for signal to indicate there's work to do
                signal.wait();

                while (true)
                {
                    NSFEvent nsfEvent = null;

                    lock (threadMutex)
                    {
                        if (nsfEvents.Count != 0)
                        {
                            nsfEvent = nsfEvents.First.Value;
                            nsfEvents.RemoveFirst();
                        }
                    }

                    if (nsfEvent == null)
                    {
                        break;
                    }

                    // Guard a bad event from taking down event thread
                    try
                    {
                        nsfEvent.Destination.handleEvent(nsfEvent);
                    }
                    catch (Exception exception)
                    {
                        handleException(new Exception(Name + " event handling exception", exception));
                    }
                }

                // Thread loop will exit when terminating and all event handlers are terminated
                if ((TerminationStatus == NSFThreadTerminationStatus.ThreadTerminating) && allEventHandlersTerminated())
                {
                    nsfEvents.Clear();
                    return;
                }
            }
        }

        #endregion Protected Methods

        #region Internal Methods

        /// <summary>
        /// Checks if all event handlers are terminated.
        /// </summary>
        internal bool allEventHandlersTerminated()
        {
            List<INSFEventHandler> eventHandlersCopy = EventHandlers;
            foreach (INSFEventHandler eventHandler in eventHandlersCopy)
            {
                if (eventHandler.TerminationStatus != NSFEventHandlerTerminationStatus.EventHandlerTerminated)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Adds an event handler to the list of event handlers.
        /// </summary>
        internal void addEventHandler(INSFEventHandler eventHandler)
        {
            lock (threadMutex)
            {
                eventHandlers.Add(eventHandler);
            }
        }

        /// <summary>
        /// Removes an event handler from the list of event handlers.
        /// </summary>
        internal void removeEventHandler(INSFEventHandler eventHandler)
        {
            lock (threadMutex)
            {
                eventHandlers.Remove(eventHandler);
            }
        }

        #endregion Internal Methods

        #region Private Fields, Events, and Properties

        private List<INSFEventHandler> eventHandlers = new List<INSFEventHandler>();
        private LinkedList<NSFEvent> nsfEvents = new LinkedList<NSFEvent>();
        private NSFOSSignal signal;

        #endregion Private Fields, Events, and Properties
    }
}