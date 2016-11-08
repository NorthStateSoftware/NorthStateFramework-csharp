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
using System.Threading;

using NorthStateSoftware.NorthStateFramework;

namespace WorkHardPlayHardExample
{
    class WorkHardPlayHardExample
    {


        static void Main(String[] args)
        {
            NSFExceptionHandler.ExceptionActions += globalExceptionAction;

            NSFTraceLog.PrimaryTraceLog.Enabled = true;

            WorkHardPlayHard workHardPlayHardExample = new WorkHardPlayHard("WorkHardPlayHardExample");
            workHardPlayHardExample.startStateMachine();

            NSFDebugUtility.PrimaryDebugUtility.writeLineToConsole("WorkHardPlayHardExample - Review trace file to see results.");

            // Illustrate transitions and history
            // Work for a while, then play for a while, then take a break, then back to playing via history, then back to work
            Thread.Sleep(100);
            workHardPlayHardExample.milestoneMet();
            Thread.Sleep(100);
            workHardPlayHardExample.takeBreak();
            Thread.Sleep(100);
            workHardPlayHardExample.breakOver();
            Thread.Sleep(100);
            workHardPlayHardExample.backToWork();
            Thread.Sleep(100);

            // Save trace log
            NSFTraceLog.PrimaryTraceLog.saveLog("CommandProcessorExampleTrace.xml");

            NSFDebugUtility.PrimaryDebugUtility.writeLineToConsole("Press Enter key to continue");
            Console.ReadKey();

            NSFEnvironment.terminate();
        }

        static void globalExceptionAction(NSFExceptionContext context)
        {
            NSFDebugUtility.PrimaryDebugUtility.writeLineToConsole("Exception caught: " + context.Exception.ToString());
        }
    }
}