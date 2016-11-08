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
using System.Text;

using NSFString = System.String;

namespace NorthStateSoftware.NorthStateFramework
{
    /// <summary>
    /// Represents information and methods applicable to the entire framework environment.
    /// </summary>
    /// <remarks>
    /// This class can be used to terminate the framework environment.
    /// </remarks>
    public class NSFEnvironment
    {
        #region Public Fields, Events, and Properties

        /// <summary>
        /// Gets a list of threads in the enviroment.
        /// </summary>
        public static List<NSFThread> Threads
        {
            get
            {
                lock (environmentMutex)
                {
                    return new List<NSFThread>(threads);
                }
            }
        }

        #endregion Public Fields, Events, and Properties

        #region Public Methods

        /// <summary>
        /// Terminates the framework environment.
        /// </summary>
        /// <remarks>
        /// This method should be called near the end of an application to coordinate terminating the application.
        /// </remarks>
        public static void terminate()
        {
            // Get all thread terminations started
            List<NSFThread> threadsCopy = Threads;
            foreach (NSFThread thread in threadsCopy)
            {
                thread.terminate(false);
            }

            // Wait for all threads to be terminated
            foreach (NSFThread thread in threadsCopy)
            {
                thread.terminate(true);
            }
        }

        #endregion Public Methods

        #region Internal Methods

        /// <summary>
        /// Adds a thread to the environment's list of threads.
        /// </summary>
        internal static void addThread(NSFThread thread)
        {
            lock (environmentMutex)
            {
                threads.Add(thread);
            }
        }

        /// <summary>
        /// Removes a thread from the environment's list of threads.
        /// </summary>
        internal static void removeThread(NSFThread thread)
        {
            lock (environmentMutex)
            {
                threads.Remove(thread);
            }
        }

        #endregion Internal Methods

        #region Private Constructors

        private NSFEnvironment()
        {
#pragma warning disable 0219
            // Forced instantiations
            NSFTimerThread timerThread = NSFTimerThread.PrimaryTimerThread;
            NSFTraceLog traceLog = NSFTraceLog.PrimaryTraceLog;
#pragma warning restore 0219
        }

        #endregion Private Constructors

        #region Private Fields, Events, and Properties

        private static object environmentMutex = new object();
        private static List<NSFThread> threads = new List<NSFThread>();

#pragma warning disable 0414
        // Must be last for proper construction
        private static NSFEnvironment theEnvironment = new NSFEnvironment();
#pragma warning restore 0414

        #endregion Private Fields, Events, and Properties
    }
}