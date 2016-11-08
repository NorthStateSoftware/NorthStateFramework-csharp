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
    /// Represents an operating system timer that can be used to retrieve accurate time and block waiting for the next timeout.
    /// </summary>
    public class NSFOSTimer : NSFTaggedObject
    {
        #region Public Constructors and Destructors

        /// <summary>
        /// Creates an operating system timer.
        /// </summary>
        /// <param name="name">The name of the timer.</param>
        public NSFOSTimer(NSFString name)
            : base(name)
        {
            timerSignal = NSFOSSignal.create(name);
        }

        #endregion Public Constructors and Destructors

        #region Public Fields, Events, and Properties

        /// <summary>
        /// Gets the current time in milliseconds since the timer was created.
        /// </summary>
        /// <returns>The current time in milliseconds.</returns>
        public NSFTime CurrentTime
        {
            get { return stopwatch.ElapsedMilliseconds; }
        }

        #endregion Public Fields, Events, and Properties

        #region Public Methods

        /// <summary>
        /// Creates an operating system timer.
        /// </summary>
        /// <param name="name">The name of the timer.</param>
        /// <returns>The new timer.</returns>
        /// <remarks>
        /// This method is included for interface compatibility with other language implementations.
        /// </remarks>
        public static NSFOSTimer create(NSFString name)
        {
            return new NSFOSTimer(name);
        }

        /// <summary>
        /// Sets the next timeout.
        /// </summary>
        /// <param name="timeout">The absolute time of the next timeout in milliseconds.</param>
        /// <remarks>
        /// A thread blocked by the method waitForNextTimeTimeout() must unblock after the specified timeout.
        /// </remarks>
        public void setNextTimeout(NSFTime timeout)
        {
            bool sendSignal = false;
            lock(timerMutex)
            {
                // If new timeout is before next timeout, wake up timer blocked in waitForNextTimeout, so that it can set up the new shorter timeout
                if (timeout < nextTimeout)
                {
                    sendSignal = true;
                }
                nextTimeout = timeout;
            }

            if (sendSignal)
            {
                timerSignal.send();
            }
        }

        /// <summary>
        /// Waits for the next tick period.
        /// </summary>
        /// <remarks>
        /// This method shall block the calling thread until the timeout specified by setNextTimeout(...).
        /// </remarks>
        public void waitForNextTimeout()
        {
            NSFTime relativeTimeout;
            lock (timerMutex)
            {
                relativeTimeout = nextTimeout - CurrentTime;
            }

            // Only wait if relative time indicates next timeout has not already passed
            if (relativeTimeout > 0)
            {
                Int32 waitTime;

                if (relativeTimeout > Int32.MaxValue)
                {
                    waitTime = Int32.MaxValue;
                }
                else
                {
                    waitTime = (Int32)relativeTimeout;
                }

                timerSignal.wait(waitTime);
            }
        }

        #endregion Public Methods

        #region Private Fields, Events, and Properties

        private Stopwatch stopwatch = Stopwatch.StartNew();
        private NSFTime nextTimeout = Int32.MaxValue;
        private object timerMutex = new object();
        NSFOSSignal timerSignal;

        #endregion Private Fields, Events, and Properties
    }
}
