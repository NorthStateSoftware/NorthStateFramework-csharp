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

namespace NSFTest
{
    public class State2Strategy : NSFStateMachine
    {
        #region Fields
        // Events
        private NSFEvent event3;

        // Regions and states, from outer to inner
        private NSFInitialState intialState2;
        private NSFCompositeState state4;
        private NSFCompositeState state5;

        // Transitions, ordered internal, local, external
        private NSFExternalTransition intialState2ToState4Transition;
        private NSFExternalTransition state4ToState5Transition;
        #endregion Fields

        #region Properties
        public NSFEvent Event3 { get { return event3; } }
        public NSFCompositeState State4 { get { return state4; } }
        public NSFCompositeState State5 { get { return state5; } }
        #endregion Properties

        #region Constructors
        public State2Strategy(String name, NSFCompositeState parentState)
            : base(name, parentState)
        {
            // Events
            event3 = new NSFEvent("Event3", this);
            // Regions and states, from outer to inner
            intialState2 = new NSFInitialState("IntialState2", this);
            state4 = new NSFCompositeState("state4", this, null, null);
            state5 = new NSFCompositeState("State5", this, null, null);
            // Transitions, ordered internal, local, external
            intialState2ToState4Transition = new NSFExternalTransition("InitialToState4", intialState2, state4, null, null, null);
            state4ToState5Transition = new NSFExternalTransition("State4ToState5", state4, state5, event3, null, null);
        }

        #endregion Constructors
    }

    /// <summary>
    /// Test using state machine as substates.
    /// </summary>
    public class StrategyTest : NSFStateMachine, ITestInterface
    {
        #region Fields

        TestHarness testHarness = new TestHarness();

        //Events
        private NSFEvent event1;
        private NSFEvent event2;

        // Regions and states, from outer to inner
        private NSFInitialState initialState;
        private NSFCompositeState state1;
        private NSFCompositeState state2;
        private NSFCompositeState state3;

        // Transitions, ordered internal, local, external
        private NSFExternalTransition initialToState1Transition;
        private NSFExternalTransition state1ToState2Transition;
        private NSFExternalTransition state2ToState3Transition;
        private NSFExternalTransition state3ToState2Transition;
        private NSFExternalTransition state2ToState1Transition;

        private State2Strategy state2Strategy;
        #endregion Fields

        #region Constructors
        public StrategyTest(String name)
            : base(name, new NSFEventThread(name))
        {
            // Events
            event1 = new NSFEvent("Event1", this);
            event2 = new NSFEvent("Event2", this);

            // Regions and states, from outer to inner
            initialState = new NSFInitialState("InitialTest15", this);
            state1 = new NSFCompositeState("State1", this, null, null);
            state2 = new NSFCompositeState("State2", this, state5EntryActions, null);
            state3 = new NSFCompositeState("State3", this, null, null);
            //Transitions
            initialToState1Transition = new NSFExternalTransition("InitialToState1", initialState, state1, null, null, null);
            state1ToState2Transition = new NSFExternalTransition("State1ToState2", state1, state2, event1, null, null);
            state2ToState3Transition = new NSFExternalTransition("State2ToState3", state2, state3, event2, null, null);
            state3ToState2Transition = new NSFExternalTransition("State3ToState2", state3, state2, event1, null, null);
            state2ToState1Transition = new NSFExternalTransition("State2ToState1", state2, state1, event1, null, null);
            state2Strategy = new State2Strategy("State2Strategy", state2);
        }

        #endregion Constructors

        #region Methods

        public bool runTest(ref String errorMessage)
        {
            startStateMachine();

            // Test 
            //  state machine start up 
            //  initial state entering 
            //  null transition.
            //  entry actions
            if (!testHarness.doesEventResultInState(null, state1))
            {
                errorMessage = "State Machine did not start properly.";
                stopStateMachine();
                return false;
            }

            // Test 
            //  state machine event handling
            //  triggered transitions
            //  entry actions
            if (!testHarness.doesEventResultInState(event1, state2))
            {
                errorMessage = "State Machine did not handle simple event triggered transition from state1 to state2.";
                stopStateMachine();
                return false;
            }

            // Test 
            //  state machine event handling
            //  triggered transitions
            //  entry actions
            //  Test strategy event reception 
            if (!testHarness.doesEventResultInState(state2Strategy.Event3, state3))
            {
                errorMessage = "State2Strategy did not handle event3 properly, stateMachine did not transition from state2 to state3.";
                stopStateMachine();
                return false;
            }

            // Test 
            //  state machine event handling
            //  triggered transitions
            //  entry actions
            //  Test strategy re-entry
            if (!testHarness.doesEventResultInState(event1, state2Strategy.State4))
            {
                errorMessage = "State2Strategy did re-enter into the state state2Strategy.State4 correctly.";
                stopStateMachine();
                return false;
            }

            stopStateMachine();
            return true;
        }

        private void state5EntryActions(NSFStateMachineContext context)
        {
            event2.queueEvent();
        }

        #endregion Methods
    }
}
