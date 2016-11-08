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

namespace NSFTest
{
    /// <summary>
    /// Test History Transition with direct entry (not from initial state)
    /// Test Deep History Re-Entry without transition
    /// </summary>
    public class DeepHistoryReEntryTest : NSFStateMachine, ITestInterface
    {
        #region Fields

        TestHarness testHarness = new TestHarness();

        //Events
        private NSFEvent event1;
        private NSFEvent event2;
        private NSFEvent event3;
        private NSFEvent event4;
        private NSFEvent event5;
        private NSFEvent event6;
        private NSFEvent event7;
        private NSFEvent event8;

        // Regions and states, from outer to inner

        //Test2 Region
        private NSFInitialState test2InitialState;
        private NSFCompositeState state1;
        private NSFCompositeState state2;

        // State1 Region
        private NSFInitialState state1InitialState;
        private NSFDeepHistory state1History;
        private NSFCompositeState state1_1;
        private NSFCompositeState state1_2;

        // State1_2 Region
        private NSFInitialState state1_2InitialState;
        private NSFCompositeState state1_2_1;
        private NSFCompositeState state1_2_2;


        // Transitions, ordered internal, local, external
        // Test1 Region
        private NSFExternalTransition test1InitialToState1Transition;
        private NSFExternalTransition state1ToState2Transition;
        private NSFExternalTransition state2ToState1Transition;
        private NSFExternalTransition state1_2_2ToState2Transition;
        private NSFExternalTransition state2Tostate1HistoryTransition;

        // State1 Region
        private NSFExternalTransition state1InitialToState1_1Transition;

        private NSFExternalTransition state1HistoryToState1_1Transition;

        private NSFExternalTransition state1_1ToState1_2Transition;
        private NSFExternalTransition state1_2ToState1_1Transition;

        // State1_2 Region
        private NSFExternalTransition state1_2InitialStateToState1_2_1Transition;
        private NSFExternalTransition state1_2_1ToState1_2_2Transition;
        private NSFExternalTransition state1_2_2ToState1_2_1Transition;
        #endregion Fields

        #region Constructors
        public DeepHistoryReEntryTest(String name)
            : base(name, new NSFEventThread(name))
        {
            // Events
            event1 = new NSFEvent("Event1", this);
            event2 = new NSFEvent("Event2", this);
            event3 = new NSFEvent("Event3", this);
            event4 = new NSFEvent("Event4", this);
            event5 = new NSFEvent("Event5", this);
            event6 = new NSFEvent("Event6", this);
            event7 = new NSFEvent("Event7", this);
            event8 = new NSFEvent("Event8", this);

            //States
            //Test2 Region
            test2InitialState = new NSFInitialState("InitialTest8", this);
            state1 = new NSFCompositeState("State1", this, null, null);
            state2 = new NSFCompositeState("State2", this, null, null);

            //State 1 Region
            state1InitialState = new NSFInitialState("InitialState1", state1);
            state1History = new NSFDeepHistory("State1History", state1);
            state1_1 = new NSFCompositeState("State1_1", state1, null, null);
            state1_2 = new NSFCompositeState("State1_2", state1, null, null);

            // State1_2 Region
            state1_2InitialState = new NSFInitialState("InitialState1_2", state1_2);
            state1_2_1 = new NSFCompositeState("State1_2_1", state1_2, null, null);
            state1_2_2 = new NSFCompositeState("State1_2_2", state1_2, null, null);

            //Transitions
            // Test1 Region
            test1InitialToState1Transition = new NSFExternalTransition("Test1InitialToState1", test2InitialState, state1, null, null, null);
            state1ToState2Transition = new NSFExternalTransition("State1ToState2", state1, state2, event5, null, null);
            state2ToState1Transition = new NSFExternalTransition("State2ToState1", state2, state1, event6, null, null);
            state1_2_2ToState2Transition = new NSFExternalTransition("State1_2_2ToState2", state1_2_2, state2, event7, null, null);
            state2Tostate1HistoryTransition = new NSFExternalTransition("State2Tostate1History", state2, state1History, event8, null, null);

            // state1 Region
            state1InitialToState1_1Transition = new NSFExternalTransition("State1InitialToState1_1", state1InitialState, state1_1, null, null, null);
            state1HistoryToState1_1Transition = new NSFExternalTransition("State1HistoryToState1_1", state1History, state1_1, null, null, null);
            state1_1ToState1_2Transition = new NSFExternalTransition("State1_1ToState1_2", state1_1, state1_2, event1, null, null);
            state1_2ToState1_1Transition = new NSFExternalTransition("State1_2ToState1_1Transition ", state1_2, state1_1, event2, null, null);

            // state1_2 Region
            state1_2InitialStateToState1_2_1Transition = new NSFExternalTransition("State1_2InitialStateToState1_2_1", state1_2InitialState, state1_2_1, null, null, null);
            state1_2_1ToState1_2_2Transition = new NSFExternalTransition("State1_2_1ToState1_2_2", state1_2_1, state1_2_2, event3, null, null);
            state1_2_2ToState1_2_1Transition = new NSFExternalTransition("State1_2_2ToState1_2_1", state1_2_2, state1_2_1, event4, null, null);
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
            //  composite states
            //  default regions
            //  nested states
            //  entry actions
            if (!testHarness.doesEventResultInState(null, state1_1))
            {
                errorMessage = "State Machine did not start properly.";
                stopStateMachine();
                return false;
            }

            // Test 
            //  state machine event handling
            //  triggered transitions
            //  initial state entering 
            //  null transition.
            //  entry actions
            if (!testHarness.doesEventResultInState(event1, state1_2_1))
            {
                errorMessage = "State Machine did not handle simple event triggered transition from state1_1 to state1_2_1";
                stopStateMachine();
                return false;
            }

            // Test 
            //  state machine event handling
            //  triggered transitions
            //  entry actions
            if (!testHarness.doesEventResultInState(event3, state1_2_2))
            {
                errorMessage = "State Machine did not handle simple event triggered transition from state1_2_1 to state1_2_2.";
                stopStateMachine();
                return false;
            }

            // Test 
            //  state machine event handling
            //  triggered transitions
            //  entry actions
            if (!testHarness.doesEventResultInState(event5, state2))
            {
                errorMessage = "State Machine did not handle simple event triggered transition from state1_2_2 to state2.";
                stopStateMachine();
                return false;
            }

            // Test 
            //  state machine event handling
            //  triggered transitions
            //  deep history direct entry
            //  entry actions
            if (!testHarness.doesEventResultInState(event8, state1_2_2))
            {
                errorMessage = "State Machine did not handle transitioning into the deep history properly.";
                stopStateMachine();
                return false;
            }

            // Test 
            //  state machine event handling
            //  triggered transitions
            //  entry actions
            if (!testHarness.doesEventResultInState(event7, state2))
            {
                errorMessage = "State Machine did not handle simple event triggered transition from state1_2_2 to state2 after history.";
                stopStateMachine();
                return false;
            }

            // Test 
            //  state machine event handling
            //  triggered transitions
            //  deep history direct entry
            //  deep history re-entry
            //  entry actions
            if (!testHarness.doesEventResultInState(event8, state1_2_2))
            {
                errorMessage = "State Machine did not handle re-entry into the deep history properly.";
                stopStateMachine();
                return false;
            }

            stopStateMachine();
            return true;
        }

        #endregion Methods
    }
}
