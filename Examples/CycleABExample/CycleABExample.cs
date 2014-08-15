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
using System.Threading;

using NorthStateSoftware.NorthStateFramework;

namespace NorthStateSoftware.Examples.CycleABExample
{
    class CycleABExample
    {
        static void Main(String[] args)
        {

            NSFTraceLog.PrimaryTraceLog.Enabled = true;

            CycleAB cycleABExample = new CycleAB("CycleABExample");
            cycleABExample.startStateMachine();

            NSFDebugUtility.PrimaryDebugUtility.writeLineToConsole("CycleABExample - Review trace file to see results.");

            // Illustrate a cycle
            // Wait until ready to cycle, cycle, wait until ready again
            while (!cycleABExample.isReadyToCycle())
            {
                Thread.Sleep(100);
            }
            cycleABExample.startCycle();
            while (!cycleABExample.isReadyToCycle())
            {
                Thread.Sleep(100);
            }

            // Save trace log
            NSFTraceLog.PrimaryTraceLog.saveLog("CycleABExampleTrace.xml");

            NSFDebugUtility.PrimaryDebugUtility.writeLineToConsole("Press Enter key to continue");
            Console.ReadKey();

            NSFEnvironment.terminate();
        }
    }
}
