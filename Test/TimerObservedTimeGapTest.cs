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
    /// Report the observed time gap of the timer
    /// </summary>
    public class TimerObservedTimeGapTest : ITestInterface
    {
        #region Public Constructors

        public TimerObservedTimeGapTest(String name)
        {
            Name = name;
        }

        #endregion Public Constructors

        #region Public Fields, Events, and Properties

        public String Name { get; set; }

        #endregion Public Fields, Events, and Properties

        #region Public Methods

        public bool runTest(ref String errorMessage)
        {
            // Add results to name for test visibility
            Name += "; Max Observed Time Gap = " + NSFTimerThread.PrimaryTimerThread.MaxObservedTimeGap.ToString() + " mS";

            return true;
        }

        #endregion Public Methods
    }
}
