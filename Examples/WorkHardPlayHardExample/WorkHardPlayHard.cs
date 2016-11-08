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

namespace WorkHardPlayHardExample
{
    public class WorkHardPlayHard : NSFStateMachine
    {
        #region Fields

        // State Machine Components
        // Define and initialize in the order:
        //   1) Events
        //   2) Regions and states, from outer to inner
        //   3) Transitions, ordered internal, local, external
        //   4) Group states and transitions within a region together.

        // Events
        protected NSFEvent breakEvent;
        protected NSFEvent breakOverEvent;
        protected NSFEvent milestoneMetEvent;
        protected NSFEvent backToWorkEvent;

        // Regions and states, from outer to inner
        protected NSFInitialState workHardPlayHardInitialState;
        protected NSFCompositeState takeABreakState;
        protected NSFCompositeState breakOverState;
        protected NSFInitialState breakOverInitialState;
        protected NSFDeepHistory breakOverHistoryState;
        protected NSFCompositeState workHardState;
        protected NSFCompositeState playHardState;

        // Transitions, ordered internal, local, external
        protected NSFExternalTransition workHardPlayHardInitialToBreakOverTransition;
        protected NSFExternalTransition takeABreakToBreakOverTransition;
        protected NSFExternalTransition breakOverToTakeABreakTransition;
        protected NSFExternalTransition breakOverInitialToBreakOverHistoryTransition;
        protected NSFExternalTransition breakOverHistoryToWorkHardTransition;
        protected NSFExternalTransition workHardToPlayHardTransition;
        protected NSFExternalTransition playHardToWorkHardTransition;

        #endregion Fields

        #region Constructors

        public WorkHardPlayHard(String name)
            : base(name, new NSFEventThread(name))
        {
            createStateMachine();
        }

        private void createStateMachine()
        {
            // Event ructors take the form (name, parent)
            breakEvent = new NSFEvent("Break", this);
            breakOverEvent = new NSFEvent("BreakOver", this);
            milestoneMetEvent = new NSFEvent("MilestoneMet", this);
            backToWorkEvent = new NSFEvent("BackToWork", this);

            // Regions and states, from outer to inner 
            // Initial state rutors take the form (name, parent)
            workHardPlayHardInitialState = new NSFInitialState("WorkHardPlayHardInitial", this);
            // Composite state rutors take the form (name, parent, entry actions, exit actions)
            takeABreakState = new NSFCompositeState("TakeABreak", this, null, null);
            breakOverState = new NSFCompositeState("BreakOver", this, null, null);
            breakOverInitialState = new NSFInitialState("BreakOverInitial", breakOverState);
            breakOverHistoryState = new NSFDeepHistory("BreakOverHistory", breakOverState);
            workHardState = new NSFCompositeState("WorkHard", breakOverState, null, null);
            playHardState = new NSFCompositeState("PlayHard", breakOverState, null, null);

            // Transitions, ordered internal, local, external
            // External transition rutors take the form (name, source, target, trigger, guard, action)
            workHardPlayHardInitialToBreakOverTransition = new NSFExternalTransition("WorkHardPlayHardInitialToBreakOver", workHardPlayHardInitialState, breakOverState, null, null, null);
            takeABreakToBreakOverTransition = new NSFExternalTransition("TakeABreakToBreakOver", takeABreakState, breakOverState, breakOverEvent, null, null);
            breakOverToTakeABreakTransition = new NSFExternalTransition("BreakOverToTakeABreak", breakOverState, takeABreakState, breakEvent, null, null);
            breakOverInitialToBreakOverHistoryTransition = new NSFExternalTransition("BreakOverInitialToBreakOverHistory", breakOverInitialState, breakOverHistoryState, null, null, null);
            breakOverHistoryToWorkHardTransition = new NSFExternalTransition("BreakOverHistoryToWorkHard", breakOverHistoryState, workHardState, null, null, null);
            workHardToPlayHardTransition = new NSFExternalTransition("WorkHardToPlayHard", workHardState, playHardState, milestoneMetEvent, null, null);
            playHardToWorkHardTransition = new NSFExternalTransition("PlayHardToWorkHard", playHardState, workHardState, backToWorkEvent, null, null);
        }

        #endregion Constructors

        #region Methods

        internal void milestoneMet()
        {
            queueEvent(milestoneMetEvent);
        }

        internal void takeBreak()
        {
            queueEvent(breakEvent);
        }

        internal void breakOver()
        {
            queueEvent(breakOverEvent);
        }

        internal void backToWork()
        {
            queueEvent(backToWorkEvent);
        }

        #endregion Methods
    }
}
