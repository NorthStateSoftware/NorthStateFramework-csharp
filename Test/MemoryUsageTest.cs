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
using System.Diagnostics;

namespace NSFTest
{
    /// <summary>
    /// Executes a test a number of times and measures the change in the working set memory.
    /// </summary>
    public class MemoryUsageTest : ITestInterface
    {
        #region Constructors
        public MemoryUsageTest(ITestInterface testToRepeat, int numberOfCycles)
        {
            TestToRepeat = testToRepeat;
            Name = "Memory Usage Test - " + TestToRepeat.Name;
            NumberOfCycles = numberOfCycles;
        }

        #endregion Constructors

        public String Name { get; set; }

        public ITestInterface TestToRepeat { get; set; }

        public int NumberOfCycles { get; set; }

        #region Methods

        public bool runTest(ref String errorMessage)
        {
            bool returnValue = true;

            // Record the if the trace log is enabled or not 
            bool traceLogInitialStatus = NSFTraceLog.PrimaryTraceLog.Enabled;

            // get the memory usage at the start
            long initialMemoryUsage = Process.GetCurrentProcess().WorkingSet64;

            // Run the test once ... this will preload any statics
            TestToRepeat.runTest(ref errorMessage);

            // Give some time for everything to clean up
            NSFOSThread.sleep(1000);

            long startingMemoryUsage = Process.GetCurrentProcess().WorkingSet64;

            // Run the test over and over again ...
            for (int i = 0; i < NumberOfCycles; i++)
            {
                TestToRepeat.runTest(ref errorMessage);
            }

            // Give some time for everything to clean up
            NSFOSThread.sleep(1000);

            // Read the memory info at the end of the test.
            long endingMemoryUsage = Process.GetCurrentProcess().WorkingSet64;

            // If the memory has increased during the test then there may have been a problem.
            long delta = endingMemoryUsage - startingMemoryUsage;
            int percentChange = (int)((delta * 100) / startingMemoryUsage);
            if (percentChange >= 2)
            {
                errorMessage = "Increase in memory detected in " + TestToRepeat.Name + " Starting memory: " + startingMemoryUsage + ", Ending memory: " + endingMemoryUsage + ", Delta: " + delta + " bytes, " + percentChange + " %";
                returnValue = false;
            }
            NSFDebugUtility.PrimaryDebugUtility.writeLineToConsole("Memory Information: " + " Starting memory: " + startingMemoryUsage + ", Ending memory: " + endingMemoryUsage + ", Delta: " + delta + " bytes, " + percentChange + " %");

            // restore trace log enabled
            NSFTraceLog.PrimaryTraceLog.Enabled = traceLogInitialStatus;
            return returnValue;
        }

        #endregion Methods
    }
}
