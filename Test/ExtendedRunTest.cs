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
    /// <summary>
    /// Extended run test
    /// Test transition order
    /// </summary>
    public class ExtendedRunTest : NSFStateMachine, ITestInterface
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
        private NSFInternalTransition test1ReactionToEvent2;
        private NSFInternalTransition state2ReactionToEvent1;
        private NSFExternalTransition initialToState1Transition;
        private NSFExternalTransition state1ToState2Transition;
        private NSFExternalTransition state2ToState3Transition;
        private NSFExternalTransition state3ToState2Transition;
        private NSFExternalTransition state2ToState1Transition;
        #endregion Fields

        #region Constructors
        public ExtendedRunTest(String name)
            : base(name, new NSFEventThread(name))
        {
            event1 = new NSFEvent("Event1", this);
            event2 = new NSFEvent("Event2", this);

            // Regions and states, from outer to inner
            initialState = new NSFInitialState("InitialTest1", this);
            state1 = new NSFCompositeState("State1", this, null, null);
            state2 = new NSFCompositeState("State2", this, null, null);
            state3 = new NSFCompositeState("State3", this, null, null);

            // Transitions, ordered internal, local, external
            state2ReactionToEvent1 = new NSFInternalTransition("State2ReactionToEvent1", state2, event1, null, state2ReactionToEvent1Actions);
            test1ReactionToEvent2 = new NSFInternalTransition("Test1ReactionToEvent2", this, event2, null, test13ReactionToEvent2Actions);
            initialToState1Transition = new NSFExternalTransition("InitialToState1", initialState, state1, null, null, null);
            state1ToState2Transition = new NSFExternalTransition("State1ToState2", state1, state2, event1, null, null);
            state2ToState3Transition = new NSFExternalTransition("State2ToState3", state2, state3, event2, null, null);
            state3ToState2Transition = new NSFExternalTransition("State3ToState2", state3, state2, event1, null, null);
            state2ToState1Transition = new NSFExternalTransition("State2ToState1", state2, state1, event1, null, null);
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

            // Run long test
            for (int i = 0; i < 10; i++)
            {
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
                if (!testHarness.doesEventResultInState(event2, state3))
                {
                    errorMessage = "State Machine did not handle simple event triggered transition from state3 to state3.";
                    stopStateMachine();
                    return false;
                }

                // Test 
                //  state machine event handling
                //  triggered transitions
                //  entry actions
                if (!testHarness.doesEventResultInState(event1, state2))
                {
                    errorMessage = "State Machine did not handle simple event triggered transition from state3 to state2.";
                    stopStateMachine();
                    return false;
                }

                // Test 
                //  state machine event handling
                //  triggered transitions
                //  transition order (in to out)
                //  Test reactions
                //  entry actions
                if (!testHarness.doesEventResultInState(event1, state3))
                {
                    errorMessage = "State Machine did not handle event1 correctly on transition from state2 to state3.";
                    stopStateMachine();
                    return false;
                }

                Thread.Sleep(50);
            }

            stopStateMachine();
            return true;
        }

        private void state2ReactionToEvent1Actions(NSFStateMachineContext context)
        {
            queueEvent(event2);
        }

        private void test13ReactionToEvent2Actions(NSFStateMachineContext context)
        {
            queueEvent(event1);
        }


        #endregion Methods
    }
}
