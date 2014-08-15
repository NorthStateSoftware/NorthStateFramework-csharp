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
using System.Threading;

namespace NSFTest
{
    /// <summary>
    /// Test Exception handling
    /// </summary>
    public class ExceptionHandlingTest : NSFStateMachine, ITestInterface
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
        #endregion Fields

        #region Constructors
        public ExceptionHandlingTest(String name)
            : base(name, new NSFEventThread(name))
        {
            event1 = new NSFEvent("Event1", this);
            event2 = new NSFEvent("Event2", this);

            // Regions and states, from outer to inner
            initialState = new NSFInitialState("InitialTest14", this);
            state1 = new NSFCompositeState("State1", this, null, null);
            state2 = new NSFCompositeState("State2", this, state2EntryActions, null);
            state3 = new NSFCompositeState("State3", this, null, null);
            // Transitions, ordered internal, local, external
            initialToState1Transition = new NSFExternalTransition("InitialToState1", initialState, state1, null, null, null);
            state1ToState2Transition = new NSFExternalTransition("State1ToState2", state1, state2, event1, null, null);
            state2ToState3Transition = new NSFExternalTransition("State2ToState3", state2, state3, event2, null, null);
            state3ToState2Transition = new NSFExternalTransition("State3ToState2", state3, state2, event1, null, null);
            state2ToState1Transition = new NSFExternalTransition("State2ToState1", state2, state1, event1, null, null);
            ExceptionActions += localHandleException;
        }

        #endregion Constructors

        #region Fields

        public static String IntentionalExceptionString = "Intentional exception";

        #endregion Fields

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
            //  exception handling
            //  entry actions
            if (!testHarness.doesEventResultInState(event1, state3))
            {
                errorMessage = "State Machine did not handle exception properly.";
                stopStateMachine();
                return false;
            }

            stopStateMachine();
            return true;
        }

        private void state2EntryActions(NSFStateMachineContext context)
        {
            throw new Exception("Intentional exception");
        }

        private void localHandleException(NSFExceptionContext context)
        {
            event2.queueEvent();
        }

        #endregion Methods
    }
}
