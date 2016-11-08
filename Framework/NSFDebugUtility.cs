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

using NSFString = System.String;

namespace NorthStateSoftware.NorthStateFramework
{
    /// <summary>
    /// Represents a debug utility class.
    /// </summary>
    public class NSFDebugUtility : NSFTaggedObject
    {
        #region Public Methods

        /// <summary>
        /// Gets the primary framework debug utility
        /// </summary>
        public static NSFDebugUtility PrimaryDebugUtility { get { return primaryDebugUtility; } }

        /// <summary>
        /// Gets and sets the maximum number of pending writes.
        /// </summary>
        /// <remarks>
        /// Requests to write to console will be dropped if the maximum number of pending writes is exceeded.
        /// </remarks>
        public int MaxPendingWrites
        {
            get
            {
                lock (debugUtilityMutex)
                {
                    return maxPendingWrites;
                }
            }
            set
            {
                lock (debugUtilityMutex)
                {
                    maxPendingWrites = value;
                }
            }
        }

        /// <summary>
        /// Writes text to the console from a low priority thread.
        /// </summary>
        /// <param name="text">Text string to write to console.</param>
        /// <returns>
        /// False if the maximum number of pending writes is exceeded, true otherwise.
        /// </returns>
        public bool writeToConsole(NSFString text)
        {
            lock (debugUtilityMutex)
            {
                if (pendingWriteCount >= maxPendingWrites)
                {
                    return false;
                }

                ++pendingWriteCount;

                eventHandler.queueEvent(writeToConsoleEvent.copy(text));
            }

            return true;
        }

        /// <summary>
        /// Writes a line of text to the console from a low priority thread.
        /// </summary>
        /// <param name="text">Text string to write to console.</param>
        /// <returns>
        /// False if the maximum number of pending writes is exceeded, true otherwise.
        /// </returns>
        public bool writeLineToConsole(NSFString text)
        {
            lock (debugUtilityMutex)
            {
                if (pendingWriteCount >= maxPendingWrites)
                {
                    return false;
                }

                ++pendingWriteCount;

                eventHandler.queueEvent(writeLineToConsoleEvent.copy(text));
            }

            return true;
        }

        /// <summary>
        /// Checks if all output has been written to the console.
        /// </summary>
        /// <returns>
        /// True if all output text has been written, false otherwise.
        /// </returns>
        public bool allOutputTextWritten()
        {
            return !eventHandler.hasEvent();
        }

        #endregion Public Methods

        #region Private Fields, Events, and Properties

        private NSFEventHandler eventHandler;
        private NSFDataEvent<NSFString> writeToConsoleEvent;
        private NSFDataEvent<NSFString> writeLineToConsoleEvent;

        private object debugUtilityMutex = new object();
        private int pendingWriteCount = 0;
        private int maxPendingWrites = 100;
        private bool lastWriteOnSeparateLine = true;

        private static NSFDebugUtility primaryDebugUtility = new NSFDebugUtility();

        #endregion Private Fields, Events, and Properties

        #region Private Constructors
        
        /// <summary>
        /// Creates a debug utility.
        /// </summary>
        public NSFDebugUtility()
            : base("DebugUtility")
        {
            eventHandler = new NSFEventHandler(Name, new NSFEventThread(Name, NSFOSThread.LowestPriority));
            writeToConsoleEvent = new NSFDataEvent<NSFString>("WriteToConsole", eventHandler);
            writeLineToConsoleEvent = new NSFDataEvent<NSFString>("WriteLineToConsole", eventHandler);

            eventHandler.LoggingEnabled = false;
            eventHandler.addEventReaction(writeToConsoleEvent, performWriteToConsole);
            eventHandler.addEventReaction(writeLineToConsoleEvent, performWriteLineToConsole);
            eventHandler.startEventHandler();
        }

        #endregion Private Constructors

        #region Private Methods

        /// <summary>
        /// Reaction that performs the work of writing to the console.
        /// </summary>
        /// <param name="context">Additional contextual information.</param>
        private void performWriteToConsole(NSFEventContext context)
        {
            Console.Write(((NSFDataEvent<NSFString>)context.Event).Data);

            lastWriteOnSeparateLine = false;

            lock (debugUtilityMutex)
            {
                --pendingWriteCount;
            }
        }

        /// <summary>
        /// Reaction that performs the work of writing a line to the console.
        /// </summary>
        /// <param name="context">Additional contextual information.</param>
        private void performWriteLineToConsole(NSFEventContext context)
        {
            if (!lastWriteOnSeparateLine)
            {
                Console.WriteLine();
            }

            Console.WriteLine(((NSFDataEvent<NSFString>)context.Event).Data);

            lastWriteOnSeparateLine = true;

            lock (debugUtilityMutex)
            {
                --pendingWriteCount;
            }
        }

        #endregion Private Methods
    }
}
