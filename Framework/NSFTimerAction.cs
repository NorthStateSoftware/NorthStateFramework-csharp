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

using NSFTime = System.Int64;

namespace NorthStateSoftware.NorthStateFramework
{
    /// <summary>
    /// Represents the base class functionality for implementing a timer action.
    /// </summary>
    /// <remarks>
    /// Timer actions must be short in duration and must not block, as they are called directly from the timer thread.
    /// Concrete timer actions in the framework are NSFEvent, NSFScheduledAction, and NSFOSSignal.
    /// </remarks>
    public abstract class NSFTimerAction : INSFNamedObject
    {
        #region Public Fields, Events, and Properties

        /// <summary>
        /// Gets or sets the delay time of the action.
        /// </summary>
        public NSFTime DelayTime { get; set; }

        /// <summary>
        /// Gets or sets the execution time of the action.
        /// </summary>
        public NSFTime ExecutionTime { get; set; }

        /// <summary>
        /// Gets or sets the name of the action.
        /// </summary>
        public NSFString Name { get; set; }

        /// <summary>
        /// Gets or sets the repeat time for periodic actions, 0 if non-periodic.
        /// </summary>
        public NSFTime RepeatTime { get; set; }

        #endregion Public Fields, Events, and Properties

        #region Public Methods

        /// <summary>
        /// Checks if the action is already scheduled.
        /// </summary>
        /// <returns>True if scheduled, otherwise false</returns>
        public bool isScheduled()
        {
            return NSFTimerThread.PrimaryTimerThread.isScheduled(this);
        }

        /// <summary>
        /// Schedules the action to execute at the previously designated delay and repeat times.
        /// </summary>
        public void schedule()
        {
            NSFTimerThread.PrimaryTimerThread.scheduleAction(this, DelayTime, RepeatTime);
        }

        /// <summary>
        /// Schedules the action to execute after the specified delay time with zero repeat time.
        /// </summary>
        /// <param name="delayTime">The delay time until the action executes.</param>
        public void schedule(NSFTime delayTime)
        {
            NSFTimerThread.PrimaryTimerThread.scheduleAction(this, delayTime, 0);
        }

        /// <summary>
        /// Schedules the action to execute at the specified times.
        /// </summary>
        /// <param name="delayTime">The delay time until the action executes.</param>
        /// <param name="repeatTime">The repeat time for periodic actions, 0 if non-periodic.</param>
        public void schedule(NSFTime delayTime, NSFTime repeatTime)
        {
            NSFTimerThread.PrimaryTimerThread.scheduleAction(this, delayTime, repeatTime);
        }

        /// <summary>
        /// Schedules the action to execute at its designated execution time.
        /// </summary>
        public void scheduleAbsoluteExecution()
        {
            NSFTimerThread.PrimaryTimerThread.scheduleAction(this);
        }

        /// <summary>
        /// Schedules the action to execute at the specified execution time.
        /// </summary>
        /// <param name="executionTime">The time the action executes.</param>
        public void scheduleAbsoluteExecution(NSFTime executionTime)
        {
            ExecutionTime = executionTime;
            NSFTimerThread.PrimaryTimerThread.scheduleAction(this);
        }

        /// <summary>
        /// Schedules the action to execute at the specified execution time.
        /// </summary>
        /// <param name="executionTime">The time the action executes.</param>
        /// <param name="repeatTime">The repeat time for periodic actions, 0 if non-periodic.</param>
        public void scheduleAbsoluteExecution(NSFTime executionTime, NSFTime repeatTime)
        {
            ExecutionTime = executionTime;
            RepeatTime = repeatTime;
            NSFTimerThread.PrimaryTimerThread.scheduleAction(this);
        }

        /// <summary>
        /// Unschedules the action.
        /// </summary>
        public void unschedule()
        {
            NSFTimerThread.PrimaryTimerThread.unscheduleAction(this);
        }

        #endregion Public Methods

        #region Protected Constructors

        /// <summary>
        /// Creates a timer action.
        /// </summary>
        protected NSFTimerAction(NSFString name)
        {
            Name = name;
            DelayTime = 0;
            ExecutionTime = 0;
            RepeatTime = 0;
        }

        #endregion Protected Constructors

        #region Internal Methods

        /// <summary>
        /// Callback method called by timer at expiration time.
        /// </summary>
        internal abstract void execute();

        #endregion Internal Methods
    }
}