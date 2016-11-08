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
    /// Represents an operating system thread.
    /// </summary>
    public class NSFOSThread : NSFTaggedObject
    {
        #region Public Constructors

        /// <summary>
        /// Creates an operating system thread.
        /// </summary>
        /// <param name="name">The name for the thread.</param>
        /// <param name="executionAction">The action executed by the thread.</param>
        /// <returns>The new thread.</returns>
        /// <remarks>
        /// The thread execution action is typically an execution loop.
        /// When the action returns, the thread is terminated.
        /// </remarks>
        public NSFOSThread(NSFString name, NSFVoidAction<NSFContext> executionAction)
            : this(name, executionAction, MediumPriority)
        {
        }

        /// <summary>
        /// Creates an operating system thread in the .Net environment.
        /// </summary>
        /// <param name="name">The name for the thread.</param>
        /// <param name="executionAction">The action executed by the thread.</param>
        /// <param name="priority">The priority of the thread.</param>
        /// <returns>The new thread.</returns>
        /// <remarks>
        /// The thread execution action is typically an execution loop.
        /// When the action returns, the thread is terminated.
        /// </remarks>
        public NSFOSThread(NSFString name, NSFVoidAction<NSFContext> executionAction, int priority)
            : base(name)
        {
            action = executionAction;
            thread = new Thread(new ThreadStart(threadEntry));
            thread.Name = name;
            Priority = priority;
            OSThreadId = 0;  // set later on entry
        }

        #endregion Public Constructors

        #region Public Fields, Events, and Properties

        /// <summary>
        /// Gets the value for the highest priority thread.
        /// </summary>
        /// <returns>The highest thread priority.</returns>
        /// <remarks>
        /// This methods returns the highest priority that an NSFOSThread may have.
        /// The system may support higher thread priorities which are reserved for system use.
        /// Only the NSFOSTimer should use this priority level.
        /// </remarks>
        public static int HighestPriority
        {
            get { return (int)ThreadPriority.Highest; }
        }

        /// <summary>
        /// Gets the value for a high priority thread.
        /// </summary>
        /// <returns>The high thread priority.</returns>
        /// <remarks>
        /// Use the Priority property to set a threads priority.
        /// </remarks>
        public static int HighPriority
        {
            get { return (int)ThreadPriority.AboveNormal; }
        }

        /// <summary>
        /// Gets the value for a medium priority thread.
        /// </summary>
        /// <returns>The medium thread priority.</returns>
        /// <remarks>
        /// Use the Priority property to set a threads priority.
        /// </remarks>
        public static int MediumPriority
        {
            get { return (int)ThreadPriority.Normal; }
        }

        /// <summary>
        /// Gets the value for a low priority thread.
        /// </summary>
        /// <returns>The low thread priority.</returns>
        /// <remarks>
        /// Use the Priority property to set a threads priority.
        /// </remarks>
        public static int LowPriority
        {
            get { return (int)ThreadPriority.BelowNormal; }
        }

        /// <summary>
        /// Gets the value for the lowest priority thread.
        /// </summary>
        /// <returns>The lowest thread priority.</returns>
        /// <remarks>
        /// This methods returns the lowest priority that an NSFOSThread may have.
        /// The system may support lower thread priorities which are reserved for system use.
        /// </remarks>
        public static int LowestPriority
        {
            get { return (int)ThreadPriority.Lowest; }
        }

        /// <summary>
        /// Gets or sets the priority of the thread.
        /// </summary>
        /// <remarks>
        /// This property specifies the priority of the thread.
        /// </remarks>
        public int Priority
        {
            get { return (int)thread.Priority; }
            set { thread.Priority = (ThreadPriority)value; }
        }

        /// <summary>
        /// Gets the operating system specific thread id.
        /// </summary>
        /// <returns>The OS specific thread id.</returns>
        public int OSThreadId { get; protected set; }

        #endregion Public Fields, Events, and Properties

        #region Public Methods

        /// <summary>
        /// Creates an operating system thread.
        /// </summary>
        /// <param name="name">The name for the thread.</param>
        /// <param name="executionAction">The action executed by the thread.</param>
        /// <returns>The new thread.</returns>
        /// <remarks>
        /// The thread execution action is typically an execution loop.
        /// When the action returns, the thread is terminated.
        /// This method is included for interface compatibility with other language implementations.
        /// </remarks>
        public static NSFOSThread create(NSFString name, NSFVoidAction<NSFContext> executionAction)
        {
            return new NSFOSThread(name, executionAction);
        }

        /// <summary>
        /// Creates an operating system thread.
        /// </summary>
        /// <param name="name">The name for the thread.</param>
        /// <param name="executionAction">The action executed by the thread.</param>
        /// <param name="priority">The priority of the thread.</param>
        /// <returns>The new thread.</returns>
        /// <remarks>
        /// The thread execution action is typically an execution loop.
        /// When the action returns, the thread is terminated.
        /// This method is included for interface compatibility with other language implementations.
        /// </remarks>
        public static NSFOSThread create(NSFString name, NSFVoidAction<NSFContext> executionAction, int priority)
        {
            return new NSFOSThread(name, executionAction, priority);
        }

        /// <summary>
        /// Executes the thread execution action.
        /// </summary>
        /// <remarks>
        /// The thread execution action is typically an execution loop.
        /// When the action returns, the thread is terminated.
        /// Derived classes should call this method from their thread entry method.
        /// This method should not be called in any other circumnstances.
        /// </remarks>
        public void executeAction()
        {
            try
            {
                action(new NSFContext(this));
            }
            catch (Exception exception)
            {
                Exception newException = new Exception(Name + " thread execution action exception", exception);
                NSFExceptionHandler.handleException(new NSFExceptionContext(this, newException));
            }
        }

        /// <summary>
        /// Sleeps the calling thread for the specified number of milliseconds.
        /// </summary>
        /// <param name="sleepTime">Time to sleep in milliseconds.</param>
        /// <remarks>
        /// This method sleeps the calling thread, not the thread object on which it is called.
        /// A sleep time of zero has OS defined behavior.
        /// </remarks>
        public static void sleep(UInt32 sleepTime)
        {
            Thread.Sleep((int)sleepTime);
        }

        /// <summary>
        /// Starts the thread by calling its execution action.
        /// </summary>
        public void startThread()
        {
            thread.Start();
        }

        #endregion Public Methods

        #region Private Fields, Events, and Properties

        private NSFVoidAction<NSFContext> action;
        private Thread thread;

        #endregion Private Fields, Events, and Properties

        #region Private Methods

        /// <summary>
        /// Thread entry point.
        /// </summary>
        private void threadEntry()
        {
#pragma warning disable 0618
            // Although deprecated, this method returns the id visible in the VS debugger,
            // whereas the ManagedThreadId is not the same as viewed in VS.
            OSThreadId = AppDomain.GetCurrentThreadId();
#pragma warning restore 0618

            executeAction();
        }

        #endregion Private Methods
    }
}
