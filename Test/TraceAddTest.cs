// Copyright 2004-2014, North State Software, LLC.  All rights reserved.

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
    /// Test adding traces to log.
    /// </summary>
    public class TraceAddTest : ITestInterface
    {
        #region Public Constructors

        public TraceAddTest(String name, int numberOfTraces)
        {
            Name = name;
            NumberOfTraces = numberOfTraces;
        }

        #endregion Public Constructors

        #region Public Fields, Events, and Properties

        public String Name { get; set; }

        public int NumberOfTraces { get; set; }

        #endregion Public Fields, Events, and Properties

        #region Public Methods

        public bool runTest(ref String errorMessage)
        {
            NSFTime startTime = NSFTimerThread.PrimaryTimerThread.CurrentTime;
            
            for (int i = 0; i < NumberOfTraces; ++i)
            {
                NSFTraceLog.PrimaryTraceLog.addTrace(NSFTraceTags.InformationalTag, NSFTraceTags.SourceTag, Name);
            }

            NSFTime addEndTime = NSFTimerThread.PrimaryTimerThread.CurrentTime;

            while (!NSFTraceLog.PrimaryTraceLog.allTracesLogged())
            {
                NSFOSThread.sleep(1);
            }

            NSFTime logEndTime = NSFTimerThread.PrimaryTimerThread.CurrentTime;

            NSFTime addTime = ((addEndTime - startTime) * 1000) / NumberOfTraces;
            NSFTime logTime = ((logEndTime - startTime) * 1000) / NumberOfTraces;

            // Add results to name for test visibility
            Name += "; Add / Log Time = " + addTime.ToString() + " / " + logTime.ToString() + " uS";

            return true;
        }

        #endregion Public Methods
    }
}
