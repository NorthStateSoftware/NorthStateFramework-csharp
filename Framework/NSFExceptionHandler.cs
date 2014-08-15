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

using NSFString = System.String;

namespace NorthStateSoftware.NorthStateFramework
{
    /// <summary>
    /// Represents a global exception handler through which clients can receive notification of runtime exceptions.
    /// </summary>
    /// <remarks>
    /// Register exception actions with this class to receive notification of any runtime exception caught by the North State Framework.
    /// </remarks>
    public class NSFExceptionHandler
    {
        #region Public Fields, Events, and Properties

        /// <summary>
        /// Actions to execute when an exception is thrown.
        /// </summary>
        public static NSFVoidActions<NSFExceptionContext> ExceptionActions = new NSFVoidActions<NSFExceptionContext>();

        #endregion Public Fields, Events, and Properties

        #region Public Methods

        /// <summary>
        /// Global exception handling method.
        /// </summary>
        /// <param name="context">Additional contextual information.</param>
        /// <remarks>
        /// All exceptions are ultimately routed to this method. It logs the exception to the trace log,
        /// saves the trace log if a file name has been set, and executes any exception actions.
        /// </remarks>
        public static void handleException(NSFExceptionContext context)
        {
            try
            {
                NSFTraceLog.PrimaryTraceLog.addTrace(NSFTraceTags.ExceptionTag, NSFTraceTags.MessageTag, context.Exception.ToString());
            }
            catch (Exception)
            {
                // Nothing to do
            }

            ExceptionActions.execute(context);
        }

        #endregion Public Methods

        #region Private Constructors

        private NSFExceptionHandler()
        {
        }

        #endregion Private Constructors

        #region Private Fields, Events, and Properties

        #endregion Private Fields, Events, and Properties
    }
}