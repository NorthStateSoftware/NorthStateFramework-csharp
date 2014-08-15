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
using NorthStateSoftware.NorthStateFramework;

namespace NSFTest
{
    /// <summary>
    /// Test multiple triggers for a single transition.
    /// </summary>
    public class MultipleTriggersOnTransitionTest : NSFStateMachine, ITestInterface
    {
        #region Fields

        TestHarness testHarness = new TestHarness();

        // Events
        private NSFEvent event1;
        private NSFEvent event2;
        private NSFEvent event3;
        private NSFEvent event4;

        // Regions and states, from outer to inner
        private NSFInitialState initialState;
        private NSFCompositeState state1;
        private NSFCompositeState state2;

        // Transitions, ordered internal, local, external
        private NSFExternalTransition initialAToState1Transition;
        private NSFExternalTransition state1ToState2Transition;
        private NSFExternalTransition state2ToState1Transition;
        #endregion Fields

        #region Constructors
        public MultipleTriggersOnTransitionTest(String name)
            : base(name, new NSFEventThread(name))
        {
            event1 = new NSFEvent("Event1", this);
            event2 = new NSFEvent("Event2", this);
            event3 = new NSFEvent("Evnet3", this);
            event4 = new NSFEvent("Event4", this);
            // Regions and states, from outer to inner
            initialState = new NSFInitialState("InitialTest2", this);
            state1 = new NSFCompositeState("State1", this, null, null);
            state2 = new NSFCompositeState("State2", this, null, null);
            // Transitions, ordered internal, local, external
            initialAToState1Transition = new NSFExternalTransition("InitialAToState1", initialState, state1, null, null, null);
            state1ToState2Transition = new NSFExternalTransition("State1ToState2", state1, state2, event1, null, null);
            state2ToState1Transition = new NSFExternalTransition("State2ToState1", state2, state1, event2, null, null);
            state1ToState2Transition.addTrigger(event3);
            state2ToState1Transition.addTrigger(event4);
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
                errorMessage = "State Machine multiple trigger transition failed when event1 did not result in transition from state1 to state2.";
                stopStateMachine();
                return false;
            }

            // Test 
            //  state machine event handling
            //  triggered transitions
            //  entry actions
            if (!testHarness.doesEventResultInState(event2, state1))
            {
                errorMessage = "State Machine multiple trigger transition failed when event2 did not result in transition from state2 to state1.";
                stopStateMachine();
                return false;
            }

            // Test 
            //  state machine event handling
            //  triggered transitions
            //  entry actions
            if (!testHarness.doesEventResultInState(event3, state2))
            {
                errorMessage = "State Machine multiple trigger transition failed when event3 did not result in transition from state1 to state2.";
                stopStateMachine();
                return false;
            }

            // Test 
            //  state machine event handling
            //  triggered transitions
            //  entry actions
            if (!testHarness.doesEventResultInState(event4, state1))
            {
                errorMessage = "State Machine multiple trigger transition failed when event4 did not result in transition from state2 to state1.";
                stopStateMachine();
                return false;
            }

            stopStateMachine();
            return true;
        }

        #endregion Methods
    }
}
