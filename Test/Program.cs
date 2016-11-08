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
using System.Threading;
using System.Diagnostics;

namespace NSFTest
{
    class Program
    {
        static void Main(String[] args)
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;

            // Create Test
            List<ITestInterface> tests = new List<ITestInterface>();

            if ((args.Length >= 1) && (args[0] == "/c"))
            {
                tests.Add(new ContinuouslyRunningTest("Continuously Running Test", 100));
            }
            else
            {
                tests.Add(new TimerResolutionTest("Timer Resolution Test"));
                tests.Add(new TimerAccuracyTest("Timer Accuracy Test"));
                tests.Add(new TimerGetTimeTest("Timer Get Time Test"));
                tests.Add(new ContextSwitchTest("Context Switch Test"));
                tests.Add(new BasicStateMachineTest("Basic State Machine Test"));
                tests.Add(new ShallowHistoryTest("Shallow History Test"));
                tests.Add(new DeepHistoryTest("Deep History Test"));
                tests.Add(new BasicForkJoinTest("Basic Fork Join Test"));
                tests.Add(new DeepHistoryReEntryTest("Deep History Re-entry Test"));
                tests.Add(new StateMachineRestartTest("State Machine Restart Test"));
                tests.Add(new ForkJoinToForkJoinTransitionTest("Fork Join to Fork Join Transition Test"));
                tests.Add(new MultipleTriggersOnTransitionTest("Multiple Triggers on Transition Test"));
                tests.Add(new ExtendedRunTest("Extended Run Test"));
                tests.Add(new ExceptionHandlingTest("Exception Handling Test"));
                tests.Add(new StrategyTest("Strategy Test"));
                tests.Add(new TransitionOrderTest("Transition Order Test"));
                tests.Add(new ChoiceStateTest("Choice State Test"));
                tests.Add(new DocumentNavigationTest("Document Test Load", "ValidTest.xml"));
                tests.Add(new DocumentLoadTest("Invalid Document Test Load", "InvalidTest.xml"));
                tests.Add(new StateMachineDeleteTest("State Machine Delete Test"));
                tests.Add(new TraceAddTest("Trace Add Test", 10000));
                tests.Add(new TimerObservedTimeGapTest("Timer Observed Time Gap Test"));
                tests.Add(new MultipleStateMachineStressTest("Multiple State Machine Stress Test", 100, 100));
                tests.Add(new TimerObservedTimeGapTest("Timer Observed Time Gap Test"));
            }
            // Set up global exception handler
            NSFExceptionHandler.ExceptionActions += globalExceptionAction;

            // Run Test
            bool allTestPassed = true;
            NSFTraceLog.PrimaryTraceLog.addTrace(NSFTraceTags.InformationalTag, "Test", "Start");

            foreach (ITestInterface test in tests)
            {
                string errorMessage = string.Empty;
                if (test.runTest(ref errorMessage))
                {
                    NSFDebugUtility.PrimaryDebugUtility.writeLineToConsole("Passed - " + test.Name);
                }
                else
                {
                    allTestPassed = false;
                    NSFDebugUtility.PrimaryDebugUtility.writeLineToConsole("Failed - " + test.Name);
                    NSFDebugUtility.PrimaryDebugUtility.writeLineToConsole(errorMessage);
                    NSFTraceLog.PrimaryTraceLog.addTrace(NSFTraceTags.InformationalTag, "Test", errorMessage);
                    NSFTraceLog.PrimaryTraceLog.addTrace(NSFTraceTags.InformationalTag, "Test", "End");
                    NSFTraceLog.PrimaryTraceLog.saveLog("TraceFile_" + test.Name + "_Failed.xml");
                }

            }

            if (allTestPassed)
            {
                NSFDebugUtility.PrimaryDebugUtility.writeLineToConsole("All tests passed");
                NSFTraceLog.PrimaryTraceLog.addTrace(NSFTraceTags.InformationalTag, "Test", "All tests passed");
                NSFTraceLog.PrimaryTraceLog.addTrace(NSFTraceTags.InformationalTag, "Test", "End");
                NSFTraceLog.PrimaryTraceLog.saveLog("TraceFile_AllPassed.xml");
            }
            else
            {
                NSFDebugUtility.PrimaryDebugUtility.writeLineToConsole("Some test failed");
            }

            NSFDebugUtility.PrimaryDebugUtility.writeLineToConsole("Press Enter key to continue");
            Console.ReadKey();

            NSFEnvironment.terminate();
        }

        static void globalExceptionAction(NSFExceptionContext context)
        {
            if (!context.Exception.ToString().Contains(ExceptionHandlingTest.IntentionalExceptionString))
            {
                NSFDebugUtility.PrimaryDebugUtility.writeLineToConsole("Global exception caught: " + context.Exception.ToString());
            }
        }
    }

}
