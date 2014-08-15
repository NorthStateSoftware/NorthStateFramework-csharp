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

using NSFId = System.Int64;

using NSFString = System.String;

using NSFTime = System.Int64;

namespace NorthStateSoftware.NorthStateFramework
{
    /// <summary>
    /// Represents an event which can trigger a transition or be handled by an event handler.
    /// </summary>
    public class NSFEvent : NSFTimerAction, INSFIdObject
    {
        #region Public Constructors

        /// <summary>
        /// Creates an event.
        /// </summary>
        /// <param name="name">The name of the event.</param>
        /// <param name="parent">The parent of the event.</param>
        /// <remarks>
        /// The event source and destination will be set to the parent.
        /// </remarks>
        public NSFEvent(NSFString name, INSFEventHandler parent)
            : this(name, parent, parent)
        {
        }

        /// <summary>
        /// Creates an event.
        /// </summary>
        /// <param name="name">The name of the event.</param>
        /// <param name="source">The source of the event.</param>
        /// <param name="destination">The destination of the event.</param>
        public NSFEvent(NSFString name, INSFNamedObject source, INSFEventHandler destination)
            : base(name)
        {
            construct(NSFUniquelyNumberedObject.getNextUniqueId(), source, destination);
        }

        /// <summary>
        /// Creates an event.
        /// </summary>
        /// <param name="nsfEvent">The event to copy.</param>
        public NSFEvent(NSFEvent nsfEvent)
            : base(nsfEvent.Name)
        {
            construct(nsfEvent.Id, nsfEvent.Source, nsfEvent.Destination);
        }

        #endregion Public Constructors

        #region Public Fields, Events, and Properties

        /// <summary>
        /// Gets or sets the event destination.
        /// </summary>
        public INSFEventHandler Destination { get; set; }

        /// <summary>
        /// Gets or sets the event id.
        /// </summary>
        /// <remarks>
        /// Transitions use the event id to match the trigger event.  In other words, mulitple event objects with the same id can trigger a transition.
        /// Use the copy() method to create a new event object with the same id as an existing object.
        /// </remarks>
        public NSFId Id { get; protected set; }

        /// <summary>
        /// Gets or sets the event source.
        /// </summary>
        public INSFNamedObject Source { get; set; }

        #endregion Public Fields, Events, and Properties

        #region Public Methods

        /// <summary>
        /// Creates a deep copy.
        /// </summary>
        /// <returns>A copy of the event.</returns>
        /// <remarks>
        /// A common design pattern is to queue data event copies, each with its own unique data payload, to state machines for handling.
        /// </remarks>
        public virtual NSFEvent copy()
        {
            return new NSFEvent(this);
        }

        /// <summary>
        /// Creates a deep copy.
        /// </summary>
        /// <param name="name">The new name for the copy.</param>
        /// <returns>A copy of the event.</returns>
        /// <remarks>
        /// A common design pattern is to queue data event copies, each with its own unique data payload, to state machines for handling.
        /// </remarks>
        public NSFEvent copy(NSFString name)
        {
            NSFEvent eventCopy = copy();
            eventCopy.Name = name;
            return eventCopy;
        }

        /// <summary>
        /// Creates a deep copy, replacing the specified parameters.
        /// </summary>
        /// <param name="source">The new source for the copy.</param>
        /// <param name="destination">The new destination for the copy.</param>
        /// <returns>A copy of the event.</returns>
        /// <remarks>
        /// A common design pattern is to queue data event copies, each with its own unique data payload, to state machines for handling.
        /// </remarks>
        public NSFEvent copy(INSFNamedObject source, INSFEventHandler destination)
        {
            NSFEvent eventCopy = copy();
            eventCopy.Source = source;
            eventCopy.Destination = destination;
            return eventCopy;
        }

        /// <summary>
        /// Creates a deep copy, replacing the specified parameters.
        /// </summary>
        /// <param name="name">The new name for the copy.</param>
        /// <param name="source">The new source for the copy.</param>
        /// <param name="destination">The new destination for the copy.</param>
        /// <returns>A copy of the event.</returns>
        /// <remarks>
        /// A common design pattern is to queue data event copies, each with its own unique data payload, to state machines for handling.
        /// </remarks>
        public NSFEvent copy(NSFString name, INSFNamedObject source, INSFEventHandler destination)
        {
            NSFEvent eventCopy = copy();
            eventCopy.Name = name;
            eventCopy.Source = source;
            eventCopy.Destination = destination;
            return eventCopy;
        }

        /// <summary>
        /// Queues the event to its destination.
        /// </summary>
        public void queueEvent()
        {
            Destination.queueEvent(this);
        }

        /// <summary>
        /// Queues the event to its destination.
        /// </summary>
        /// <remarks>
        /// This method is provided to allow event queuing to be registered as an entry, exit, or transition action.
        /// The event source will be changed to the context source for logging purposes during execution of this method.
        /// This method is not thread safe from a logging perspective, so that two threads calling this method on the same
        /// event can result in indeterminate logging of the source.
        /// </remarks>
        public void queueEvent(NSFStateMachineContext context)
        {
            Destination.queueEvent(this, context.Source);
        }

        /// <summary>
        /// Schedules the event to execute.
        /// </summary>
        /// <param name="source">Source of the event.</param>
        /// <param name="destination">Destination of the event.</param>
        /// <param name="delayTime">Delay time before executing the event.</param>
        /// <param name="repeatTime">Repeat time, if desired.  Zero if one-shot.</param>
        public void schedule(INSFNamedObject source, INSFEventHandler destination, NSFTime delayTime, NSFTime repeatTime)
        {
            Source = source;
            Destination = destination;
            NSFTimerThread.PrimaryTimerThread.scheduleAction(this, delayTime, repeatTime);
        }

        /// <summary>
        /// Sets the source and destination of the event.
        /// </summary>
        /// <param name="source">Source of the event.</param>
        /// <param name="destination">Destination of the event.</param>
        public void setRouting(INSFNamedObject source, INSFEventHandler destination)
        {
            Source = source;
            Destination = destination;
        }

        #endregion Public Methods

        #region Internal Methods

        /// <summary>
        /// Callback method supporting NSFTimerAction interface.
        /// </summary>
        /// <remarks>
        /// This method is called by the NSFTimerThread to queue the event at the scheduled time.
        /// </remarks>
        internal override void execute()
        {
            Destination.queueEvent(this);
        }

        #endregion Internal Methods

        #region Private Methods

        private void construct(NSFId id, INSFNamedObject source, INSFEventHandler destination)
        {
            Id = id;
            Source = source;
            Destination = destination;
        }

        #endregion Private Methods
    }
}