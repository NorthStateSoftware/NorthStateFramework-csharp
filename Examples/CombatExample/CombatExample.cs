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

namespace NSFExample
{
    class CombatExample
    {
        static void Main(String[] args)
        {
            NSFTraceLog.PrimaryTraceLog.Enabled = true;

            Combat combatExample = new Combat("CombatExample");
            combatExample.startStateMachine();

            NSFDebugUtility.PrimaryDebugUtility.writeLineToConsole("CombatExample - Review trace file to see results.");

            // Illustrate transitions taken as distance to enemy changes
            combatExample.sendScoutTeam();
            Thread.Sleep(100);
            combatExample.DistanceToEnemy = combatExample.NearDistance - 1;
            combatExample.sendScoutTeam();
            Thread.Sleep(100);
            combatExample.DistanceToEnemy = combatExample.InRangeDistance - 1;
            combatExample.sendScoutTeam();
            Thread.Sleep(100);

            NSFTraceLog.PrimaryTraceLog.saveLog("CombatExampleTrace.xml");

            NSFDebugUtility.PrimaryDebugUtility.writeLineToConsole("Press Enter key to continue");
            Console.ReadKey();

            NSFEnvironment.terminate();
        }
    }
}
