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

using NorthStateSoftware.NorthStateFramework;

namespace NSFTest
{
    /// <summary>
    /// Provide static methods for determining if the event injected causes the desired state.
    /// </summary>
    public class TestHarness
    {
        /// <summary>
        /// The time in ms to wait for a state to enter, if the time has not otherwise been specified.
        /// </summary>
        private const int defaultWaitTime = 1000;
        private NSFOSSignal firstSignal;
        private NSFOSSignal secondSignal;

        public TestHarness()
        {
            firstSignal = NSFOSSignal.create("FirstSignal");
            secondSignal = NSFOSSignal.create("SecondSignal");
        }

        /// <summary>
        /// Determines if the queuing the an event causes a state to enter before defaultWaitTime elapses.
        /// </summary>
        /// <param name="eventToInject">The event to queue.</param>
        /// <param name="stateToWaitFor">The state to wait for.</param>
        /// <returns>
        ///     True if the stateToWaitFor is entered.
        ///     False if the stateToWaitFor is not entered before time expires.
        /// </returns>
        /// <remarks>
        /// This is a blocking call, the caller may be blocked for as long as <see cref="defaultWaitTime"/>.
        /// </remarks>
        public bool doesEventResultInState(NSFEvent eventToInject, NSFState stateToWaitFor)
        {
            return doesEventResultInState(eventToInject, stateToWaitFor, defaultWaitTime);
        }

        /// <summary>
        /// Determines if the queuing the an event causes a state to enter before defaultWaitTime elapses.
        /// </summary>
        /// <param name="eventToInject">The event to queue.</param>
        /// <param name="stateToWaitFor">The state to wait for.</param>
        /// <param name="waitTime">The time in ms to wait for the stateToWaitFor to enter.</param>
        /// <returns>
        ///     True if the stateToWaitFor is entered.
        ///     False if the stateToWaitFor is not entered before time expires.
        /// </returns>
        /// <remarks>
        /// This is a blocking call, the caller may be blocked for as long as waitTime.
        /// </remarks>
        public bool doesEventResultInState(NSFEvent eventToInject, NSFState stateToWaitFor, int waitTime)
        {
            firstSignal.clear();

            stateToWaitFor.EntryActions += firstSignal.send;

            if (eventToInject != null)
            {
                eventToInject.queueEvent();
            }

            if (stateToWaitFor.isActive())
            {
                stateToWaitFor.EntryActions -= firstSignal.send;
                return true;
            }

            if (!firstSignal.wait(waitTime))
            {
                stateToWaitFor.EntryActions -= firstSignal.send;
                return false;
            }

            stateToWaitFor.EntryActions -= firstSignal.send;
            return true;
        }

        /// <summary>
        /// Determines if the queuing the an event causes two states to enter before defaultWaitTime elapses.
        /// </summary>
        /// <param name="eventToInject">The event to queue.</param>
        /// <param name="stateToWaitFor">The first of two states to wait for.</param>
        /// <param name="secondStateToWaitFor">The second of two states to wait for.</param>
        /// <returns>
        ///     True if both states are entered.
        ///     False if either of the states fails to enter before time expires.
        /// </returns>
        /// <remarks>
        /// This is a blocking call, the caller may be blocked for as long as twice <see cref="defaultWaitTime"/>.
        /// The states may be congruently entered.
        /// </remarks>
        public bool doesEventResultInState(NSFEvent eventToInject, NSFState stateToWaitFor, NSFState secondStateToWaitFor)
        {
            return doesEventResultInState(eventToInject, stateToWaitFor, secondStateToWaitFor, defaultWaitTime);
        }

        /// <summary>
        /// Determines if the queuing the an event causes two states to enter before waitTime elapses.
        /// </summary>
        /// <param name="eventToInject">The event to queue.</param>
        /// <param name="stateToWaitFor">The first of two states to wait for.</param>
        /// <param name="secondStateToWaitFor">The second of two states to wait for.</param>
        /// <param name="waitTime">The time in ms to wait for the each state to enter.</param>
        /// <returns>
        ///     True if both states are entered.
        ///     False if either of the states fails to enter before time expires.
        /// </returns>
        /// <remarks>
        /// This is a blocking call, the caller may be blocked for as long as twice waitTime.
        /// The states may be congruently entered.
        /// </remarks>
        public bool doesEventResultInState(NSFEvent eventToInject, NSFState stateToWaitFor, NSFState secondStateToWaitFor, int waitTime)
        {
            secondSignal.clear();

            stateToWaitFor.EntryActions += secondSignal.send;

            secondStateToWaitFor.EntryActions += secondSignal.send;

            if (!doesEventResultInState(eventToInject, stateToWaitFor, waitTime))
            {
                secondStateToWaitFor.EntryActions -= secondSignal.send;
                return false;
            }

            if (secondStateToWaitFor.isActive())
            {
                secondStateToWaitFor.EntryActions -= secondSignal.send;
                return true;
            }

            if (!secondSignal.wait(waitTime))
            {
                secondStateToWaitFor.EntryActions -= secondSignal.send;
                return false;
            }

            secondStateToWaitFor.EntryActions -= secondSignal.send;
            return true;
        }
    }
}
