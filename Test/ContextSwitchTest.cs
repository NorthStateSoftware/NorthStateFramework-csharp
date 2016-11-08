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
    /// Test context switch time
    /// </summary>
    public class ContextSwitchTest : ITestInterface
    {
        #region Constructors

        public ContextSwitchTest(String name)
        {
            Name = name;

            signal1 = NSFOSSignal.create("Signal1");
            signal2 = NSFOSSignal.create("Signal2");

            thread1 = NSFOSThread.create("Thread1", thread1Loop, NSFOSThread.HighestPriority);
            thread2 = NSFOSThread.create("Thread2", thread2Loop, NSFOSThread.HighestPriority);
        }

        #endregion Constructors

        public String Name { get; set; }

        public int NumberOfTraces { get; set; }

        #region Public Methods

        public bool runTest(ref String errorMessage)
        {
            NSFTime startTime = NSFTimerThread.PrimaryTimerThread.CurrentTime;

            thread1.startThread();
            thread2.startThread();

            while (contextSwitchCount < TotalContextSwitches)
            {
                NSFOSThread.sleep(1);
            }

            NSFTime endTime = NSFTimerThread.PrimaryTimerThread.CurrentTime;

            NSFTime contextSwitchTime = ((endTime - startTime) * 1000) / TotalContextSwitches;

            // Add time to name for test visibility
            Name += "; Switch time = " + contextSwitchTime.ToString() + " uS";

            return true;
        }

        #endregion Public Methods

        #region Private Members

        NSFOSThread thread1;
        NSFOSThread thread2;

        NSFOSSignal signal1;
        NSFOSSignal signal2;

        int contextSwitchCount = 0;
        const int TotalContextSwitches = 100000;

        #endregion Private Members

        #region Private Methods

        private void thread1Loop(NSFContext context)
        {
            while (contextSwitchCount < TotalContextSwitches)
            {
                signal1.send();
                signal2.wait(1000);
                contextSwitchCount += 2;
            }
        }

        private void thread2Loop(NSFContext context)
        {
            while (contextSwitchCount < TotalContextSwitches)
            {
                signal1.wait(1000);
                signal2.send();
            }
        }

        #endregion Private Methods
    }
}
