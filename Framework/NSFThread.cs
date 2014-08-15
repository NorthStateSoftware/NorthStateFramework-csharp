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
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

using NSFString = System.String;

namespace NorthStateSoftware.NorthStateFramework
{
    /// <summary>
    /// Represents the termination status of a thread.
    /// </summary>
    public enum NSFThreadTerminationStatus { ThreadReady = 1, ThreadTerminating, ThreadTerminated };

    /// <summary>
    /// Represents the common framework thread functionality.
    /// </summary>
    public abstract class NSFThread : NSFTaggedObject
    {
        #region Public Fields, Events, and Properties

        /// <summary>
        /// Actions to execute if an exception is encountered while the thread is executing.
        /// </summary>
        public NSFVoidActions<NSFExceptionContext> ExceptionActions = new NSFVoidActions<NSFExceptionContext>();

        /// <summary>
        /// Gets the underlying OS thread.
        /// </summary>
        public NSFOSThread OSThread { get; private set; }

        /// <summary>
        /// Gets the thread termination status.
        /// </summary>
        public NSFThreadTerminationStatus TerminationStatus { get; private set; }

        /// <summary>
        /// Gets the amount of time (mS) the <see cref="terminate"/> method sleeps between checks
        /// for thread termination.
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

        #endregion Public Fields, Events, and Properties

        #region Public Methods

        /// <summary>
        /// Terminates the thread by causing the execution method to return.
        /// </summary>
        /// <param name="waitForTerminated">Flag indicating if the method should wait until the thread is terminated (true), or if it should return immediately (false).</param>
        /// <remarks>
        /// This method is useful to guarantee that a thread is no longer active, so that it can be garbage collected.
        /// If the waitForTerminated flag is set true, this method must not be called from its thread of execution.
        /// </remarks>
        public virtual void terminate(bool waitForTerminated)
        {
            lock (threadMutex)
            {
                if (TerminationStatus != NSFThreadTerminationStatus.ThreadTerminated)
                {
                    TerminationStatus = NSFThreadTerminationStatus.ThreadTerminating;
                }
            }

            if (waitForTerminated)
            {
                for (uint i = 0; i < TerminationTimeout; i += TerminationSleepTime)
                {
                    if (TerminationStatus == NSFThreadTerminationStatus.ThreadTerminated)
                    {
                        return;
                    }

                    NSFOSThread.sleep(TerminationSleepTime);
                }

                handleException(new Exception("Thread was unable to terminate"));
            }
        }

        #endregion Public Methods

        #region Protected Constructors and Destructors

        /// <summary>
        /// Creates a thread.
        /// </summary>
        /// <param name="name">The name of the thread.</param>
        /// <remarks>
        /// The thread is created with medium priority.
        /// To change the priority, use the getOSThread() method to access the underlying thread object.
        /// </remarks>
        protected NSFThread(NSFString name)
            : this(name, NSFOSThread.MediumPriority)
        {
        }

        /// <summary>
        /// Creates an event thread.
        /// </summary>
        /// <param name="name">The name of the thread.</param>
        /// <param name="priority">The priority of the thread.</param>
        protected NSFThread(NSFString name, int priority)
            : base(name)
        {
            TerminationStatus = NSFThreadTerminationStatus.ThreadReady;
            OSThread = NSFOSThread.create(Name, new NSFVoidAction<NSFContext>(threadEntry), priority);
            NSFEnvironment.addThread(this);
        }

        #endregion Protected Constructors and Destructors

        #region Protected Fields, Events, and Properties

        protected object threadMutex = new object();
        private static uint terminationSleepTime = 10;
        private static int terminationTimeout = 60000;

        #endregion Protected Fields, Events, and Properties

        #region Protected Methods

        /// <summary>
        /// Handles exceptions caught by main event processing loop.
        /// </summary>
        /// <param name="exception">The exception caught.</param>
        protected void handleException(Exception exception)
        {
            NSFExceptionContext newContext = new NSFExceptionContext(this, new Exception(Name + " thread exception", exception));
            ExceptionActions.execute(newContext);
            NSFExceptionHandler.handleException(newContext);
        }

        /// <summary>
        /// Starts the thread by calling its execution action.
        /// </summary>
        protected void startThread()
        {
            OSThread.startThread();
        }

        /// <summary>
        /// Implements the main thread processing loop.
        /// </summary>
        protected abstract void threadLoop();

        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        /// Implements the entry point for the thread.
        /// </summary>
        /// <param name="context">Additional contextual information.</param>
        private void threadEntry(NSFContext context)
        {
            try
            {
                threadLoop();
            }
            catch (Exception exception)
            {
                handleException(new Exception(Name + " thread loop exception", exception));
            }

            lock (threadMutex)
            {
                TerminationStatus = NSFThreadTerminationStatus.ThreadTerminated;
                NSFEnvironment.removeThread(this);
            }
        }

        #endregion Private Methods
    }
}
