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
using System.Threading;

using NSFString = System.String;

namespace NorthStateSoftware.NorthStateFramework
{
    /// <summary>
    /// Represents an operating system signal that may be used to block thread execution until the signal is sent.
    /// </summary>
    public class NSFOSSignal : NSFTimerAction
    {
        #region Public Constructors

        /// <summary>
        /// Creates an operating system signal.
        /// </summary>
        /// <param name="name">The name of the signal.</param>
        public NSFOSSignal(NSFString name)
            : base(name)
        {
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Creates an operating system signal.
        /// </summary>
        /// <param name="name">The name of the signal.</param>
        /// <returns>The new signal.</returns>
        /// <remarks>
        /// This method is included for interface compatibility with other language implementations.
        /// </remarks>
        public static NSFOSSignal create(NSFString name)
        {
            return new NSFOSSignal(name);
        }

        /// <summary>
        /// Clears the signal.
        /// </summary>
        /// <remarks>
        /// This method sets the signal to a non-signaled state, so that the next wait will block until a send is called.
        /// </remarks>
        public void clear()
        {
            wait(0);
        }

        /// <summary>
        /// Sends the signal.
        /// </summary>
        /// <param name="context">The context in which the method is called.</param>
        public void send(NSFContext context)
        {
            send();
        }

        /// <summary>
        /// Sends the signal.
        /// </summary>
        public void send()
        {
            autoResetEvent.Set();
        }

        /// <summary>
        /// Waits for signal to be sent.
        /// </summary>
        /// <returns>True if the signal was sent, false otherwise.</returns>
        /// <remarks>
        /// This method will block the calling thread until the signal is sent.
        /// Multiple threads must not wait on the same signal.
        /// </remarks>
        public bool wait()
        {
            return autoResetEvent.WaitOne(-1);
        }

        /// <summary>
        /// Waits for up to <paramref name="timeout"/> milliseconds for a signal to be sent.
        /// </summary>
        /// <param name="timeout">The maximum number of milliseconds to wait.</param>
        /// <returns>True if the signal was sent, false otherwise.</returns>
        /// <remarks>
        /// This method will block the calling thread until the signal is sent or the timeout occurs.
        /// Multiple threads must not wait on the same signal.
        /// </remarks>
        public bool wait(Int32 timeout)
        {
            return autoResetEvent.WaitOne(timeout);
        }

        #endregion Public Methods

        #region Internal Methods

        /// <summary>
        /// Callback method supporting NSFTimerAction interface.
        /// </summary>
        /// <remarks>
        /// This method is called by the NSFTimerThread to send the signal at the scheduled time.
        /// </remarks>
        internal override void execute()
        {
            send();
        }

        #endregion Internal Methods

        #region Private Fields, Events, and Properties

        private AutoResetEvent autoResetEvent = new AutoResetEvent(false);

        #endregion Private Fields, Events, and Properties
    }
}
