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
    /// Test transition order (
    /// </summary>
    public class TransitionOrderTest : NSFStateMachine, ITestInterface
    {
        #region Fields

        TestHarness testHarness = new TestHarness();

        //Events
        private NSFEvent event1;
        private NSFEvent event2;
        private NSFEvent event3;

        // Regions and states, from outer to inner
        private NSFInitialState initialTest15;
        private NSFCompositeState state1;
        private NSFCompositeState state2;
        private NSFInitialState intialState2;
        private NSFCompositeState state2_1;
        private NSFCompositeState state3;
        private NSFInitialState initialState3;
        private NSFCompositeState state3_1;
        private NSFCompositeState state4;
        private NSFCompositeState errorState;

        // Transitions, ordered internal, local, external
        // Intentionally mis-ordered
        private NSFExternalTransition initialTest15ToState1Transition;
        private NSFExternalTransition state1ToState2Transition;
        private NSFInternalTransition state1ReactionToEvent1;
        private NSFExternalTransition state1ToErrorTransition;

        private NSFExternalTransition state2ToState3Transition;
        private NSFExternalTransition state2ToErrorTransition;
        private NSFLocalTransition state2ToState2Transition;
        private NSFExternalTransition intialState2ToState2_1Transition;

        private NSFExternalTransition state3ToState4Transition;
        private NSFLocalTransition state3ToState3Transition;
        private NSFInternalTransition state3ReactionToEvent1;
        private NSFExternalTransition initialState3ToState3_1Transition;
        private NSFExternalTransition state3ToErrorTransition;
        #endregion Fields

        #region Constructors
        public TransitionOrderTest(String name)
            : base(name, new NSFEventThread(name))
        {
            // Events
            event1 = new NSFEvent("Event1", this);
            event2 = new NSFEvent("Event2", this);
            event3 = new NSFEvent("Event3", this);

            // Regions and states, from outer to inner
            initialTest15 = new NSFInitialState("InitialTest16", this);
            state1 = new NSFCompositeState("State1", this, null, null);
            state2 = new NSFCompositeState("State2", this, null, null);
            intialState2 = new NSFInitialState("IntialState2", state2);
            state2_1 = new NSFCompositeState("State2_1", state2, null, state2_1ExitActions);
            state3 = new NSFCompositeState("State3", this, null, null);
            initialState3 = new NSFInitialState("InitialState3", state3);
            state3_1 = new NSFCompositeState("State3_1", state3, null, state3_1ExitActions);
            state4 = new NSFCompositeState("State4", this, null, null);
            errorState = new NSFCompositeState("Error", this, null, null);

            // Transitions, ordered internal, local, external
            initialTest15ToState1Transition = new NSFExternalTransition("InitialTest15ToState1", initialTest15, state1, null, null, null);
            state1ToState2Transition = new NSFExternalTransition("State1ToState2", state1, state2, event2, null, null);
            state1ReactionToEvent1 = new NSFInternalTransition("State1ReactionToEvent1", state1, event1, null, state1ReactionToEvent1Actions);
            state1ToErrorTransition = new NSFExternalTransition("State1ToError", state1, errorState, event1, null, null);

            state2ToState3Transition = new NSFExternalTransition("State2ToState3Transition", state2, state3, event2, null, null);
            state2ToErrorTransition = new NSFExternalTransition("State2ToErrorTransition", state2, errorState, event1, null, null);
            state2ToState2Transition = new NSFLocalTransition("State2ToState2Transition", state2, state2, event1, null, null);
            intialState2ToState2_1Transition = new NSFExternalTransition("intialState2ToState2_1Transition", intialState2, state2_1, null, null, null);

            state3ToState4Transition = new NSFExternalTransition("State3ToState4Transition", state3, state4, event2, null, null);
            state3ToState3Transition = new NSFLocalTransition("State3ToState3Transition", state3, state3, event1, null, null);
            state3ReactionToEvent1 = new NSFInternalTransition("state3ReactionToEvent1", state3, event1, null, state3ReactionToEvent1Actions);
            initialState3ToState3_1Transition = new NSFExternalTransition("InitialState3ToState3_1Transition", initialState3, state3_1, null, null, null);
            state3ToErrorTransition = new NSFExternalTransition("State3ToErrorTransition", state3, errorState, event3, null, null);
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
            //  that internal transitions are taken before external transitions
            //  internal tranision actions
            //  entry actions
            if (!testHarness.doesEventResultInState(event1, state2))
            {
                errorMessage = "Internal transition was not taken before external transition.";
                stopStateMachine();
                return false;
            }

            // Test 
            //  state machine event handling
            //  triggered transitions
            //  that local transitions are taken before external transitions
            //  that local tranisition properly exit sub states
            //  entry actions
            if (!testHarness.doesEventResultInState(event1, state3))
            {
                errorMessage = "Local transition was not taken before external transition.";
                stopStateMachine();
                return false;
            }

            // Test 
            //  state machine event handling
            //  triggered transitions
            //  that internal transitions are taken before local transitions
            //  internal tranision actions
            //  entry actions
            if (!testHarness.doesEventResultInState(event1, state4))
            {
                errorMessage = "Internal transition was not taken before local transition.";
                stopStateMachine();
                return false;
            }

            stopStateMachine();
            return true;
        }

        private void state1ReactionToEvent1Actions(NSFStateMachineContext context)
        {
            queueEvent(event2);
        }

        private void state3ReactionToEvent1Actions(NSFStateMachineContext context)
        {
            queueEvent(event2);
        }

        private void state2_1ExitActions(NSFStateMachineContext context)
        {
            queueEvent(event2);
        }

        private void state3_1ExitActions(NSFStateMachineContext context)
        {
            queueEvent(event3);
        }
        #endregion Methods
    }
}
