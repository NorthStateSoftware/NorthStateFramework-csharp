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

namespace NorthStateSoftware.Examples.CommandProcessorWithResetStateMachineExample
{
    public class ResetStrategy : NSFStateMachine
    {
        #region Fields

        protected bool hardwareReset = false;
        protected bool commReady = false;

        protected int hardwareResetDelayTime = 1000;
        protected int commReadyDelayTime = 1000;

        private NSFEvent commReadyEvent;
        private NSFEvent hardwareResetEvent;

        // Regions and states, from outer to inner
        private NSFInitialState initialResetStrategyState;
        private NSFCompositeState resetHardwareState;
        private NSFCompositeState reestablishCommunicationsState;
        private NSFCompositeState readyState;

        // Transitions, ordered internal, local, external
#pragma warning disable 0414
        private NSFExternalTransition initialResetStrategyToResetHardwareTransition;
        private NSFExternalTransition resetHardwareToReestablishCommunicationsTransition;
        private NSFExternalTransition reestablishCommunicationsStateToReadyTransition;
#pragma warning restore 0414

        // Actions
        private NSFScheduledAction resetHardwareAction;
        private NSFScheduledAction readyCommAction;

        #endregion Fields

        #region Properties

        public NSFCompositeState ResetHardwareState { get { return resetHardwareState; } }
        public NSFCompositeState ReestablishCommunicationsState { get { return reestablishCommunicationsState; } }
        public NSFCompositeState ReadyState { get { return readyState; } }

        #endregion Properties

        #region Constructors

        public ResetStrategy(String name, NSFCompositeState parentState)
            : base(name, parentState)
        {
            createStateMachine();
        }

        public ResetStrategy(String name, NSFRegion parentRegion)
            : base(name, parentRegion)
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

            // Events
            commReadyEvent = new NSFEvent("CommReady", this);
            hardwareResetEvent = new NSFEvent("HardwareReset", this);

            // Regions and states, from outer to inner 
            // Initial state construtors take the form (name, parent)
            initialResetStrategyState = new NSFInitialState("InitialResetStrategy", this);
            // Composite state construtors take the form (name, parent, entry action, exit action)
            resetHardwareState = new NSFCompositeState("ResetHardware", this, resetHardwareEntryActions, null);
            reestablishCommunicationsState = new NSFCompositeState("ReestablishCommunications", this, reestablishCommunicationsEntryActions, null);
            readyState = new NSFCompositeState("ThreadReady", this, null, null);

            // Transitions, ordered internal, local, external
            // External transition construtors take the form (name, source, target, trigger, guard, action)
            initialResetStrategyToResetHardwareTransition = new NSFExternalTransition("InitialToResetHardware", initialResetStrategyState, resetHardwareState, null, null, resetVariables);
            resetHardwareToReestablishCommunicationsTransition = new NSFExternalTransition("ResetHardwareToReestablishCommunications", resetHardwareState, reestablishCommunicationsState, null, isHardwareReset, null);
            reestablishCommunicationsStateToReadyTransition = new NSFExternalTransition("ReestablishCommunicationsStateToReady", reestablishCommunicationsState, readyState, null, isCommReady, null);

            // Actions
            resetHardwareAction = new NSFScheduledAction("ReadHardware", resetHardware, EventThread);
            readyCommAction = new NSFScheduledAction("ResetComm", resetComm, EventThread);
        }

        #endregion Constructors

        #region Methods

        private void resetVariables(NSFStateMachineContext context)
        {
            hardwareReset = false;
            commReady = false;
        }

        private void resetHardware(NSFContext context)
        {
            hardwareReset = true;
            queueEvent(hardwareResetEvent);
        }

        private void resetComm(NSFContext context)
        {
            commReady = true;
            queueEvent(commReadyEvent);
        }

        private void resetHardwareEntryActions(NSFStateMachineContext context)
        {
            hardwareReset = false;

            // Simulate behavior using an action
            resetHardwareAction.schedule(hardwareResetDelayTime);
        }

        private void reestablishCommunicationsEntryActions(NSFStateMachineContext context)
        {
            commReady = false;

            // Simulate behavior using an action
            readyCommAction.schedule(commReadyDelayTime);
        }

        private bool isHardwareReset(NSFStateMachineContext context)
        {
            return hardwareReset;
        }

        private bool isCommReady(NSFStateMachineContext context)
        {
            return commReady;
        }

        #endregion Methods
    }
}
