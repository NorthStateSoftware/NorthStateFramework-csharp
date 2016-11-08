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

using NSFTime = System.Int64;

namespace NSFTest
{
    /// <summary>
    /// Test the accuracy of the timer.
    /// </summary>
    public class TimerAccuracyTest : ITestInterface
    {
        #region Public Constructors

        public TimerAccuracyTest(String name)
        {
            Name = name;
            eventHandler = new NSFEventHandler(name, new NSFEventThread(name));
            testEvent = new NSFEvent(name, eventHandler);

            eventHandler.startEventHandler();
        }

        #endregion Public Constructors

        #region Public Fields, Events, and Properties

        public String Name { get; set; }

        #endregion Public Fields, Events, and Properties

        #region Public Methods

        public bool runTest(ref String errorMessage)
        {
            eventHandler.addEventReaction(testEvent, testEventAction);

            // Schedule test event to fire every millisecond
            testEvent.schedule(0, 1);

            // Wait for test to end
            NSFOSThread.sleep(1000);

            // Unschedule test event
            testEvent.unschedule();

            // Add results to name for test visibility
            Name += "; Min / Max Delta Time = " + minDeltaTime.ToString() + " / " + maxDeltaTime.ToString() + " mS";

            return true;
        }

        #endregion Public Methods

        #region Private Fields, Events, and Properties

        private NSFEventHandler eventHandler;
        private NSFEvent testEvent;
        private NSFTime lastTime = 0;
        private NSFTime maxDeltaTime = 0;
        private NSFTime minDeltaTime = NSFTime.MaxValue;

        #endregion Public Fields, Events, and Properties

        #region Private Methods

        void testEventAction(NSFEventContext context)
        {
            NSFTime currentTime = NSFTimerThread.PrimaryTimerThread.CurrentTime;

            if (lastTime == 0)
            {
                lastTime = currentTime;
                return;
            }

            NSFTime deltaTime = currentTime - lastTime;

            // Record max and min observed delta times
            if (deltaTime > maxDeltaTime) { maxDeltaTime = deltaTime; }
            if (deltaTime < minDeltaTime) { minDeltaTime = deltaTime; }

            // Record current time as last time through loop
            lastTime = currentTime;
        }

        #endregion Private Methods
    }
}
