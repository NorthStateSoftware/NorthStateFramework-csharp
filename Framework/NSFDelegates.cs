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
using System.ComponentModel;

using NSFString = System.String;

namespace NorthStateSoftware.NorthStateFramework
{
    #region Delegates

    /// <summary>
    /// Represents a general purpose guard.
    /// </summary>
    /// <typeparam name="ContextType">The context type passed as an argument to the guard.</typeparam>
    /// <remarks>
    /// An guard is a delegate type used by the North State Framework.
    /// </remarks>
    public delegate bool NSFBoolGuard<ContextType>(ContextType context);

    /// <summary>
    /// Represents a general purpose action.
    /// </summary>
    /// <typeparam name="ContextType">The context type passed as an argument to the action.</typeparam>
    /// <remarks>
    /// An action is a delegate type used by the North State Framework.
    /// </remarks>
    public delegate void NSFVoidAction<ContextType>(ContextType context);

    #endregion Delegates

    /// <summary>
    /// Represents a common base class for NSF delegate lists.
    /// </summary>
    public class NSFDelegateListBase
    {
        #region Internal Constructors

        /// <summary>
        /// Creates a delegate list base class.
        /// </summary>
        internal NSFDelegateListBase()
        {
        }

        #endregion Internal Constructors

        #region Internal Fields, Events, and Properties

        internal static object delegateListMutex = new object();

        internal NSFVoidAction<NSFExceptionContext> exceptionAction;

        #endregion Internal Fields, Events, and Properties

        #region Internal Methods

        /// <summary>
        /// Sets the exception action to call if a delegate throws an exception during invocation.
        /// </summary>
        /// <param name="action">The exception action.</param>
        internal void setExceptionAction(NSFVoidAction<NSFExceptionContext> action)
        {
            lock (delegateListMutex)
            {
                exceptionAction = action;
            }
        }

        #endregion Internal Methods
    }

    /// <summary>
    /// Represents a list of guards.
    /// </summary>
    /// <remarks>
    /// The primary purpose of this class is to provide exception handling around guard invocations.
    /// An exception thrown by any guard will be caught, allowing the other guards to execute.
    /// Notification of guard exceptions is also available.
    /// The execute() method returns true if all guards return true.
    /// If an exception is thrown by any guard, the execute() method returns false.
    /// </remarks>
    public class NSFBoolGuards<ContextType> : NSFDelegateListBase
    {
        #region Public Methods

        /// <summary>
        /// Adds a guard to the list of guards.
        /// </summary>
        /// <param name="list">The guard list.</param>
        /// <param name="guard">The guard to add.</param>
        /// <returns>The list of guards.</returns>
        public static NSFBoolGuards<ContextType> operator +(NSFBoolGuards<ContextType> list, NSFBoolGuard<ContextType> guard)
        {
            list.add(guard);
            return list;
        }

        /// <summary>
        /// Removes a guard from the list of guards.
        /// </summary>
        /// <param name="list">The guard list.</param>
        /// <param name="guard">The guard to remove.</param>
        /// <returns>The list of guards.</returns>
        public static NSFBoolGuards<ContextType> operator -(NSFBoolGuards<ContextType> list, NSFBoolGuard<ContextType> guard)
        {
            list.remove(guard);
            return list;
        }

        /// <summary>
        /// Checks if the list is empty.
        /// </summary>
        /// <returns>True if the list is empty, false otherwise.</returns>
        public bool isEmpty()
        {
            lock (delegateListMutex)
            {
                return ((guards == null) || (guards.GetInvocationList().Length == 0));
            }
        }

        #endregion Public Methods

        #region Internal Constructors

        /// <summary>
        /// Creates a list of guards.
        /// </summary>
        internal NSFBoolGuards()
            : base()
        {
        }

        #endregion Internal Constructors

        #region Internal Methods

        /// <summary>
        /// Adds a guard to the list of guards.
        /// </summary>
        /// <param name="guard">The guard to add.</param>
        internal void add(NSFBoolGuard<ContextType> guard)
        {
            lock (delegateListMutex)
            {
                guards += guard;
            }
        }

        /// <summary>
        /// Executes the guards.
        /// </summary>
        /// <param name="context">Additional contextual information.</param>
        /// <returns>
        /// True if all guards return true, false otherwise.
        /// If an exception is thrown by any guard, returns false.
        /// </returns>
        internal bool execute(ContextType context)
        {
            bool returnValue = true;
            NSFBoolGuard<ContextType> guardsCopy;

            lock (delegateListMutex)
            {
                guardsCopy = guards;
            }

            if (guardsCopy == null) return returnValue;

            foreach (NSFBoolGuard<ContextType> guard in guardsCopy.GetInvocationList())
            {
                // Try each guard independently so that an exception in one does not inhibit other guards
                try
                {
                    returnValue &= guard(context);
                }
                catch (Exception exception)
                {
                    returnValue = false;
                    try
                    {
                        if (exceptionAction != null)
                        {
                            exceptionAction(new NSFExceptionContext(this, new Exception("Exception executing delegate in list", exception)));
                        }
                    }
                    catch (Exception)
                    {
                        // Exception handler had a problem, nothing to do
                    }
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Removes a guard from the list of guards.
        /// </summary>
        /// <param name="guard">The guard to remove.</param>
        internal void remove(NSFBoolGuard<ContextType> guard)
        {
            lock (delegateListMutex)
            {
                guards -= guard;
            }
        }

        #endregion Internal Methods

        #region Private Fields, Events, and Properties

        private NSFBoolGuard<ContextType> guards;

        #endregion Private Fields, Events, and Properties
    }

    /// <summary>
    /// Represents a list of actions.
    /// </summary>
    /// <remarks>
    /// The primary purpose of this class is to provide exception handling around action invocations.
    /// An exception thrown by any action will be caught, allowing the other actions to execute.
    /// Notification of action exceptions is also available.
    /// </remarks>
    public class NSFVoidActions<ContextType> : NSFDelegateListBase
    {
        #region Public Methods

        /// <summary>
        /// Adds an action to the list of actions.
        /// </summary>
        /// <param name="list">The action list.</param>
        /// <param name="action">The action to add.</param>
        /// <returns>The list of actions.</returns>
        public static NSFVoidActions<ContextType> operator +(NSFVoidActions<ContextType> list, NSFVoidAction<ContextType> action)
        {
            list.add(action);
            return list;
        }

        /// <summary>
        /// Removes an action from the list of actions.
        /// </summary>
        /// <param name="list">The action list.</param>
        /// <param name="action">The action to remove.</param>
        /// <returns>The list of actions.</returns>
        public static NSFVoidActions<ContextType> operator -(NSFVoidActions<ContextType> list, NSFVoidAction<ContextType> action)
        {
            list.remove(action);
            return list;
        }

        /// <summary>
        /// Indicates if the list is empty.
        /// </summary>
        /// <returns>True if the list is empty, false otherwise.</returns>
        public bool isEmpty()
        {
            lock (delegateListMutex)
            {
                return ((actions == null) || (actions.GetInvocationList().Length == 0));
            }
        }

        #endregion Public Methods

        #region Internal Constructors

        /// <summary>
        /// Creates a list of actions.
        /// </summary>
        internal NSFVoidActions()
            : base()
        {
        }

        #endregion Internal Constructors

        #region Internal Methods

        /// <summary>
        /// Adds an action to the list of actions.
        /// </summary>
        /// <param name="action">The action to add.</param>
        internal void add(NSFVoidAction<ContextType> action)
        {
            lock (delegateListMutex)
            {
                actions += action;
            }
        }

        /// <summary>
        /// Executes the actions.
        /// </summary>
        /// <param name="context">Additional contextual information.</param>
        internal void execute(ContextType context)
        {
            NSFVoidAction<ContextType> actionsCopy;

            lock (delegateListMutex)
            {
                actionsCopy = actions;
            }

            if (actionsCopy == null) return;

            foreach (NSFVoidAction<ContextType> action in actionsCopy.GetInvocationList())
            {
                // Try each action independently so that an exception in one does not inhibit other actions
                try
                {
                    action(context);
                }
                catch (Exception exception)
                {
                    try
                    {
                        if (exceptionAction != null)
                        {
                            exceptionAction(new NSFExceptionContext(this, new Exception("Exception executing delegate in list", exception)));
                        }
                    }
                    catch (Exception)
                    {
                        // Exception handler had a problem, nothing to do
                    }
                }
            }
        }

        /// <summary>
        /// Removes an action from the list of actions.
        /// </summary>
        /// <param name="action">The action to remove.</param>
        internal void remove(NSFVoidAction<ContextType> action)
        {
            lock (delegateListMutex)
            {
                actions -= action;
            }
        }

        #endregion Internal Methods

        #region Private Fields, Events, and Properties

        private NSFVoidAction<ContextType> actions;

        #endregion Private Fields, Events, and Properties
    }
}