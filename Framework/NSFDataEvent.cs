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

using NSFId = System.Int64;

using NSFString = System.String;

using NSFTime = System.Int64;

namespace NorthStateSoftware.NorthStateFramework
{
    /// <summary>
    /// Represents an event that contains a data payload.
    /// </summary>
    public class NSFDataEvent<DataType> : NSFEvent
    {
        #region Public Constructors

        /// <summary>
        /// Creates event that can have a data payload.
        /// </summary>
        /// <param name="name">The name of the event.</param>
        /// <param name="parent">The parent of the event.</param>
        /// <remarks>
        /// The event source and destination will be set to the parent.
        /// </remarks>
        public NSFDataEvent(NSFString name, INSFEventHandler parent)
            : base(name, parent)
        {
        }

        /// <summary>
        /// Creates event that can have a data payload.
        /// </summary>
        /// <param name="name">The name of the event.</param>
        /// <param name="parent">The parent of the event.</param>
        /// <param name="data">The data of the event.</param>
        /// <remarks>
        /// The event source and destination will be set to the parent.
        /// </remarks>
        public NSFDataEvent(NSFString name, INSFEventHandler parent, DataType data)
            : base(name, parent)
        {
            Data = data;
        }

        /// <summary>
        /// Creates event that can have a data payload.
        /// </summary>
        /// <param name="name">The name of the event.</param>
        /// <param name="source">The source of the event.</param>
        /// <param name="destination">The destination of event.</param>
        public NSFDataEvent(NSFString name, INSFNamedObject source, INSFEventHandler destination)
            : base(name, source, destination)
        {
        }

        /// <summary>
        /// Creates event that can have a data payload.
        /// </summary>
        /// <param name="name">The name of the event.</param>
        /// <param name="source">The source of the event.</param>
        /// <param name="destination">The destination of the event.</param>
        /// <param name="data">The data of the event.</param>
        public NSFDataEvent(NSFString name, INSFNamedObject source, INSFEventHandler destination, DataType data)
            : base(name, source, destination)
        {
            Data = data;
        }

        /// <summary>
        /// Creates event that can have a data payload.
        /// </summary>
        /// <param name="nsfEvent">The event to copy.</param>
        public NSFDataEvent(NSFDataEvent<DataType> nsfEvent)
            : base(nsfEvent)
        {
            Data = nsfEvent.Data;
        }

        /// <summary>
        /// Creates event that can have a data payload.
        /// </summary>
        /// <param name="nsfEvent">The event to copy.</param>
        /// <param name="data">The data of the event.</param>
        public NSFDataEvent(NSFDataEvent<DataType> nsfEvent, DataType data)
            : base(nsfEvent)
        {
            Data = data;
        }

        #endregion Public Constructors

        #region Public Fields, Events, and Properties

        /// <summary>
        /// Gets or sets the data payload.
        /// </summary>
        public DataType Data { get; set; }

        #endregion Public Fields, Events, and Properties

        #region Public Methods

        /// <summary>
        /// Creates a deep copy.
        /// </summary>
        /// <returns>The copy of the event.</returns>
        /// <remarks>
        /// A common design pattern is to queue data event copies, each with its own unique data payload, to state machines for handling.
        /// </remarks>
        public override NSFEvent copy()
        {
            return new NSFDataEvent<DataType>(this);
        }

        /// <summary>
        /// Creates a deep copy, replacing the specified parameters.
        /// </summary>
        /// <param name="data">The new data for the copy.</param>
        /// <returns>A copy of the event.</returns>
        /// <remarks>
        /// A common design pattern is to queue data event copies, each with its own unique data payload, to state machines for handling.
        /// </remarks>
        public virtual NSFDataEvent<DataType> copy(DataType data)
        {
            return new NSFDataEvent<DataType>(this, data);
        }

        /// <summary>
        /// Creates a deep copy, replacing the specified parameters.
        /// </summary>
        /// <param name="name">The new name for the copy.</param>
        /// <param name="data">The new data for the copy.</param>
        /// <returns>A copy of the event.</returns>
        /// <remarks>
        /// A common design pattern is to queue data event copies, each with its own unique data payload, to state machines for handling.
        /// </remarks>
        public NSFDataEvent<DataType> copy(NSFString name, DataType data)
        {
            NSFDataEvent<DataType> eventCopy = copy(data);
            eventCopy.Name = name;
            return eventCopy;
        }

        /// <summary>
        /// Creates a deep copy, replacing the specified parameters.
        /// </summary>
        /// <param name="source">The new source for the copy.</param>
        /// <param name="destination">The new destination for the copy.</param>
        /// <param name="data">The new data for the copy.</param>
        /// <returns>A copy of the event.</returns>
        /// <remarks>
        /// A common design pattern is to queue data event copies, each with its own unique data payload, to state machines for handling.
        /// </remarks>
        public NSFDataEvent<DataType> copy(INSFNamedObject source, INSFEventHandler destination, DataType data)
        {
            NSFDataEvent<DataType> eventCopy = copy(data);
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
        /// <param name="data">The new data for the copy.</param>
        /// <returns>A copy of the event.</returns>
        /// <remarks>
        /// A common design pattern is to queue data event copies, each with its own unique data payload, to state machines for handling.
        /// </remarks>
        public NSFDataEvent<DataType> copy(NSFString name, INSFNamedObject source, INSFEventHandler destination, DataType data)
        {
            NSFDataEvent<DataType> eventCopy = copy(data);
            eventCopy.Name = name;
            eventCopy.Source = source;
            eventCopy.Destination = destination;
            return eventCopy;
        }

        #endregion Public Methods
    }
}