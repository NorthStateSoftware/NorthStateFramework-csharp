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
    /// Synchronous test used to exercise many simultaneous state machines.
    /// </summary>
    public class MultipleStateMachineStressTest : ITestInterface
    {
        #region Fields
        private bool readyToTerminate = false;
        #endregion Fields

        #region Constructors
        public MultipleStateMachineStressTest(String name, int numberOfInstances, int numberOfCycles)
        {
            Name = name;
            NumberOfCycles = numberOfCycles;
            NumberOfInstances = numberOfInstances;
        }

        #endregion Constructors

        public String Name { get; set; }

        public int NumberOfInstances { get; set; }

        public int NumberOfCycles { get; set; }

        #region Methods

        public bool runTest(ref String errorMessage)
        {
            bool returnValue = true;
            readyToTerminate = false;

            List<ContinuouslyRunningTest> tests = new List<ContinuouslyRunningTest>();
            ContinuouslyRunningTest firstTest = null;

            for (int i = 0; i < NumberOfInstances; ++i)
            {
                ContinuouslyRunningTest test = new ContinuouslyRunningTest(Name + ".SubTest" + i.ToString(), NumberOfCycles);
                if (i == 0)
                {
                    firstTest = test;
                    test.State1_2_2.EntryActions += terminateTest;
                }

                tests.Add(test);

                test.runTest(ref errorMessage);
            }

            while (!readyToTerminate)
            {
                NSFOSThread.sleep(1);
            }

            // terminate all the test
            foreach (ContinuouslyRunningTest test in tests)
            {
                test.terminate(false);
            }

            // Wait for all test to terminate
            foreach (ContinuouslyRunningTest test in tests)
            {
                test.terminate(true);
            }

            firstTest.State1_2_2.EntryActions -= terminateTest;

            NSFDebugUtility.PrimaryDebugUtility.writeLineToConsole("");

            return returnValue;
        }

        void terminateTest(NSFStateMachineContext context)
        {
            readyToTerminate = true;
        }

        #endregion Methods
    }
}
