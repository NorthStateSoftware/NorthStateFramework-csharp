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

namespace NorthStateSoftware.Examples.CommandProcessorExample
{
    class CommandProcessorExample
    {
        static void Main(String[] args)
        {
            NSFExceptionHandler.ExceptionActions += globalExceptionAction;

            NSFTraceLog.PrimaryTraceLog.Enabled = true;

            CommandProcessor commandProcessor = new CommandProcessor("CommandProcessor");
#pragma warning disable 0219
            CommandProcessorObserver commandProcessorObserver = new CommandProcessorObserver(commandProcessor);
#pragma warning restore 0219

            commandProcessor.startStateMachine();

            // Simple example of polling for state
            int i = 0;
            while (true)
            {
                Thread.Sleep(500);

                if (commandProcessor.isInState(commandProcessor.WaitForCommandState))
                {
                    NSFDebugUtility.PrimaryDebugUtility.writeLineToConsole("Press Enter key to inject a command");
                    Console.ReadKey();
                    commandProcessor.addCommand("TestCommand");
                }
                else if (commandProcessor.isInState(commandProcessor.WaitForResponseState))
                {
                    // Only send two responses
                    if (++i <= 2)
                    {
                        commandProcessor.addResponse("TestResponse");
                    }
                }
                else if (commandProcessor.isInState(commandProcessor.ErrorState))
                {
                    break;
                }
            }

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
