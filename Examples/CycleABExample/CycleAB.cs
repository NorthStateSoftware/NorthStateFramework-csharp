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

namespace NorthStateSoftware.Examples.CycleABExample
{
    public class CycleAB : NSFStateMachine
    {
        #region Fields

        protected const int InitializeADelayTime = 100;
        protected const int InitializeBDelayTime = 200;

        protected const int CompleteADelayTime = 100;
        protected const int CompleteBDelayTime = 200;

        // State Machine Components
        // Define and initialize in the order:
        //   1) Events
        //   2) Regions and states, from outer to inner
        //   3) Transitions, ordered internal, local, external
        //   4) Group states and transitions within a region together.

        // Events
        protected NSFEvent cycleEvent;
        protected NSFEvent aReadyEvent;
        protected NSFEvent bReadyEvent;
        protected NSFEvent aCompleteEvent;
        protected NSFEvent bCompleteEvent;

        // Regions and states, from outer to inner 
        protected NSFRegion systemRegion;
        protected NSFRegion subsystemARegion;
        protected NSFRegion subsystemBRegion;
        protected NSFForkJoin initializeForkJoin;
        protected NSFForkJoin cycleForkJoin;
        protected NSFForkJoin completeForkJoin;

        // System Region
        // Regions and states, from outer to inner 
        protected NSFInitialState systemInitialState;
        protected NSFCompositeState waitForCycleEventState;
        // Transitions, ordered internal, local, external
        protected NSFExternalTransition systemInitialToInitializeForkJoinTransition;
        protected NSFExternalTransition initializeForkJoinToWaitForCycleEventTransition;
        protected NSFExternalTransition waitForCycleEventToCycleForkJoinTransition;
        protected NSFForkJoinTransition cycleForkJoinToCompleteForkJoinTransiiton;
        protected NSFExternalTransition completeForkJoinToWaitForCycleEventTransition;

        // Subystem A Region
        // Regions and states, from outer to inner 
        protected NSFInitialState subsystemAInitialState;
        protected NSFCompositeState initializeAState;
        protected NSFCompositeState cycleAState;
        // Transitions, ordered internal, local, external
        protected NSFExternalTransition subsystemAInitialToInitializeATransition;
        protected NSFExternalTransition initializeAToInitializeForkJoinTransition;
        protected NSFForkJoinTransition initializeForkJoinToCycleForkJoinARegionTransition;
        protected NSFExternalTransition cycleForkJoinToCycleATransition;
        protected NSFExternalTransition cycleAToCompleteForkJoinTransition;

        // Subystem B Region
        // Regions and states, from outer to inner 
        protected NSFInitialState subsystemBInitialState;
        protected NSFCompositeState initializeBState;
        protected NSFCompositeState cycleBState;
        // Transitions, ordered internal, local, external
        protected NSFExternalTransition subsystemBInitialToInitializeBTransition;
        protected NSFExternalTransition initializeBToInitializeForkJoinTransition;
        protected NSFForkJoinTransition initializeForkJoinToCycleForkJoinBRegionTransition;
        protected NSFExternalTransition cycleForkJoinToCycleBTransition;
        protected NSFExternalTransition cycleBToCompleteForkJoinTransition;

        #endregion Fields

        #region Constructors

        public CycleAB(String name)
            : base(name, new NSFEventThread(name))
        {
            createStateMachine();
        }

        private void createStateMachine()
        {
            // State Machine Components
            // Define and initialize in the order:
            //   1) Events
            //   2) Regions and states, from outer to inner
            //   3) Transitions, ordered internal, local, external
            //   4) Group states and transitions within a region together.
            // Maintain the same order of declaration and initialization.

            // Events
            // Event constructors take the form (name, parent)
            cycleEvent = new NSFEvent("CycleEvent", this);
            aReadyEvent = new NSFEvent("AReadyEvent", this);
            bReadyEvent = new NSFEvent("BReadyEvent", this);
            aCompleteEvent = new NSFEvent("ACompleteEvent", this);
            bCompleteEvent = new NSFEvent("BCompleteEvent", this);

            // Regions and states, from outer to inner 
            systemRegion = new NSFRegion("SystemRegion", this);
            subsystemARegion = new NSFRegion("SubsystemARegion", this);
            subsystemBRegion = new NSFRegion("SubsystemBRegion", this);
            initializeForkJoin = new NSFForkJoin("InitializeForkJoin", this);
            cycleForkJoin = new NSFForkJoin("CycleForkJoin", this);
            completeForkJoin = new NSFForkJoin("CompleteForkJoin", this);

            // System Region
            // Regions and states, from outer to inner 
            // Initial state construtors take the form (name, parent)
            systemInitialState = new NSFInitialState("SystemInitial", systemRegion);
            // Composite state construtors take the form (name, parent, entry actions, exit actions)
            waitForCycleEventState = new NSFCompositeState("WaitForCycleEvent", systemRegion, null, null);
            // Transitions, ordered internal, local, external
            // External transition construtors take the form (name, source, target, trigger, guard, action)
            systemInitialToInitializeForkJoinTransition = new NSFExternalTransition("SystemInitialToInitializeForkJoin", systemInitialState, initializeForkJoin, null, null, null);
            initializeForkJoinToWaitForCycleEventTransition = new NSFExternalTransition("InitializeForkJoinToWaitForCycleEvent", initializeForkJoin, waitForCycleEventState, null, null, null);
            waitForCycleEventToCycleForkJoinTransition = new NSFExternalTransition("WaitForCycleEventToCycleForkJoin", waitForCycleEventState, cycleForkJoin, cycleEvent, null, null);
            cycleForkJoinToCompleteForkJoinTransiiton = new NSFForkJoinTransition("CycleForkJoinToCompleteForkJoin", cycleForkJoin, completeForkJoin, systemRegion, null);
            completeForkJoinToWaitForCycleEventTransition = new NSFExternalTransition("CompleteForkJoinToWaitForCycleEvent", completeForkJoin, waitForCycleEventState, null, null, null);

            // Subystem A Region
            // Regions and states, from outer to inner 
            // Initial state construtors take the form (name, parent)
            subsystemAInitialState = new NSFInitialState("SubsystemAInitial", subsystemARegion);
            // Composite state construtors take the form (name, parent, entry actions, exit actions)
            initializeAState = new NSFCompositeState("InitializeA", subsystemARegion, initializeAEntryActions, null);
            cycleAState = new NSFCompositeState("CycleA", subsystemARegion, cycleAEntryActions, null);
            // Transitions, ordered internal, local, external
            // External transition construtors take the form (name, source, target, trigger, guard, action)
            subsystemAInitialToInitializeATransition = new NSFExternalTransition("SubsystemAInitialToInitializeA", subsystemAInitialState, initializeAState, null, null, null);
            initializeAToInitializeForkJoinTransition = new NSFExternalTransition("InitializeAToInitializeForkJoin", initializeAState, initializeForkJoin, aReadyEvent, null, null);
            initializeForkJoinToCycleForkJoinARegionTransition = new NSFForkJoinTransition("InitializeForkJoinToCycleForkJoinARegion", initializeForkJoin, cycleForkJoin, subsystemARegion, null);
            cycleForkJoinToCycleATransition = new NSFExternalTransition("CycleForkJoinToCycleA", cycleForkJoin, cycleAState, null, null, null);
            cycleAToCompleteForkJoinTransition = new NSFExternalTransition("CycleAToCompleteForkJoin", cycleAState, completeForkJoin, aCompleteEvent, null, null);

            // Subystem B Region
            // Regions and states, from outer to inner 
            // Initial state construtors take the form (name, parent)
            subsystemBInitialState = new NSFInitialState("SubsystemBInitial", subsystemBRegion);
            // Composite state construtors take the form (name, parent, entry actions, exit actions)
            initializeBState = new NSFCompositeState("InitializeB", subsystemBRegion, initializeBEntryActions, null);
            cycleBState = new NSFCompositeState("CycleB", subsystemBRegion, cycleBEntryActions, null);
            // Transitions, ordered internal, local, external
            // External transition construtors take the form (name, source, target, trigger, guard, action)
            subsystemBInitialToInitializeBTransition = new NSFExternalTransition("SubsystemBInitialToInitializeB", subsystemBInitialState, initializeBState, null, null, null);
            initializeBToInitializeForkJoinTransition = new NSFExternalTransition("InitializeBToInitializeForkJoin", initializeBState, initializeForkJoin, bReadyEvent, null, null);
            initializeForkJoinToCycleForkJoinBRegionTransition = new NSFForkJoinTransition("InitializeForkJoinToCycleForkJoinBRegion", initializeForkJoin, cycleForkJoin, subsystemBRegion, null);
            cycleForkJoinToCycleBTransition = new NSFExternalTransition("CycleForkJoinToCycleB", cycleForkJoin, cycleBState, null, null, null);
            cycleBToCompleteForkJoinTransition = new NSFExternalTransition("CycleBToCompleteForkJoin", cycleBState, completeForkJoin, bCompleteEvent, null, null);
        }

        #endregion Constructors

        #region Methods

        public bool isReadyToCycle()
        {
            if (!waitForCycleEventState.isActive())
            {
                return false;
            }

            if (hasEvent(cycleEvent))
            {
                return false;
            }

            return true;
        }

        public void startCycle()
        {
            queueEvent(cycleEvent);
        }

        private void initializeAEntryActions(NSFStateMachineContext context)
        {
            aReadyEvent.schedule(InitializeADelayTime, 0);
        }

        private void cycleAEntryActions(NSFStateMachineContext context)
        {
            aCompleteEvent.schedule(CompleteADelayTime, 0);
        }

        private void initializeBEntryActions(NSFStateMachineContext context)
        {
            bReadyEvent.schedule(InitializeBDelayTime, 0);
        }

        private void cycleBEntryActions(NSFStateMachineContext context)
        {
            bCompleteEvent.schedule(CompleteBDelayTime, 0);
        }

        #endregion Methods
    }
}
