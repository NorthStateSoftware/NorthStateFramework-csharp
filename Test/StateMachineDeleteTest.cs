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
    /// Test terminating a state machine and then letting it fall out of scope.
    /// </summary>
    public class StateMachineDeleteTest : ITestInterface
    {
        #region Constructors
        public StateMachineDeleteTest(String name)
        {
            Name = name;
        }

        #endregion Constructors

        public String Name { get; set; }

        #region Methods

        public bool runTest(ref String errorMessage)
        {
            ContinuouslyRunningTest test = new ContinuouslyRunningTest("StateMachineExercise", 100);

            test.runTest(ref errorMessage);

            NSFOSThread.sleep(20);

            test.terminate(true);
            return true;
        }

        #endregion Methods
    }
}
