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
    /// Test Choise State
    /// </summary>
    public class ChoiceStateTest : NSFStateMachine, ITestInterface
    {
        #region Fields

        private int value = 0;
        TestHarness testHarness = new TestHarness();

        //Events
        private NSFEvent evaluateEvent;
        private NSFEvent waitEvent;

        // Regions and states, from outer to inner
        private NSFInitialState initialChoiceStateTestState;
        private NSFCompositeState waitToEvaluateState;
        private NSFChoiceState evaluateValueState;
        private NSFCompositeState evaluatedState;
        private NSFInitialState initialEvaluatedState;
        private NSFCompositeState valueLowState;
        private NSFCompositeState valueMiddleState;
        private NSFCompositeState valueHighState;

        // Transitions, ordered internal, local, external
        private NSFExternalTransition initialChoiceStateTestToWaitToEvaluateTransition;
        private NSFExternalTransition waitToEvaluateToEvaluateValueTransition;
        private NSFExternalTransition initialEvaluatedToValueLowTransition;
        private NSFExternalTransition evaluateValueToValueLowTransition;
        private NSFExternalTransition evaluateValueToValueMiddleTransition;
        private NSFExternalTransition evaluateValueToValueHighTransition;
        private NSFExternalTransition evaluatedToWaitToEvaluateTransition;
        #endregion Fields

        #region Constructors
        public ChoiceStateTest(String name)
            : base(name, new NSFEventThread(name))
        {
            // Events
            evaluateEvent = new NSFEvent("EvaluateEvent", this);
            waitEvent = new NSFEvent("WaitEvent", this);

            // Regions and states, from outer to inner
            initialChoiceStateTestState = new NSFInitialState("InitialChoiceStateTest", this);
            waitToEvaluateState = new NSFCompositeState("WaitToEvaluate", this, null, null);
            evaluateValueState = new NSFChoiceState("EvaluateValue", this);
            evaluatedState = new NSFCompositeState("Evaluated", this, null, null);
            initialEvaluatedState = new NSFInitialState("InitialEvaluatedState", evaluatedState);
            valueLowState = new NSFCompositeState("ValueLow", evaluatedState, null, null);
            valueMiddleState = new NSFCompositeState("ValueMiddle", evaluatedState, null, null);
            valueHighState = new NSFCompositeState("ValueHigh", evaluatedState, null, null);

            // Transitions, ordered internal, local, external
            initialChoiceStateTestToWaitToEvaluateTransition = new NSFExternalTransition("InitialChoiceStateTestToWaitToEvaluate", initialChoiceStateTestState, waitToEvaluateState, null, null, null);
            waitToEvaluateToEvaluateValueTransition = new NSFExternalTransition("State1ToState2", waitToEvaluateState, evaluateValueState, evaluateEvent, null, null);
            //waitToEvaluateToEvaluateValueTransition = new NSFExternalTransition("State1ToState2", waitToEvaluateState, evaluatedState, evaluateEvent, null, null);
            initialEvaluatedToValueLowTransition = new NSFExternalTransition("InitialEvaluatedToValueLowTransition", initialEvaluatedState, valueLowState, null, null, null);
            evaluateValueToValueLowTransition = new NSFExternalTransition("EvaluateValueToValueLowTransition", evaluateValueState, valueLowState, null, isValueLow, null);
            evaluateValueToValueMiddleTransition = new NSFExternalTransition("EvaluateValueToValueMiddleTransition", evaluateValueState, valueMiddleState, null, Else, null);
            evaluateValueToValueHighTransition = new NSFExternalTransition("EvaluateValueToValueHighTransition", evaluateValueState, valueHighState, null, isValueHigh, null);
            evaluatedToWaitToEvaluateTransition = new NSFExternalTransition("EvaluatedToWaitToEvaluateTransition", evaluatedState, waitToEvaluateState, waitEvent, null, addValue);
        }

        #endregion Constructors

        #region Methods

        public bool runTest(ref String errorMessage)
        {
            startStateMachine();

            if (!testHarness.doesEventResultInState(null, waitToEvaluateState))
            {
                errorMessage = "State Machine did not start properly";
                stopStateMachine();
                return false;
            }

            if (!testHarness.doesEventResultInState(evaluateEvent, valueLowState))
            {
                errorMessage = "Choice State did not choose low.";
                stopStateMachine();
                return false;
            }

            if (!testHarness.doesEventResultInState(waitEvent, waitToEvaluateState))
            {
                errorMessage = "Simple Transition failed from valueLowState to waitToEvaluateState.";
                stopStateMachine();
                return false;
            }

            if (!testHarness.doesEventResultInState(evaluateEvent, valueMiddleState))
            {
                errorMessage = "Choice State did not choose middle.";
                stopStateMachine();
                return false;
            }

            if (!testHarness.doesEventResultInState(waitEvent, waitToEvaluateState))
            {
                errorMessage = "Simple Transition failed from valueMiddleState to waitToEvaluateState.";
                stopStateMachine();
                return false;
            }

            if (!testHarness.doesEventResultInState(evaluateEvent, valueHighState))
            {
                errorMessage = "Choice State did not choose high.";
                stopStateMachine();
                return false;
            }

            stopStateMachine();
            return true;
        }

        private bool isValueLow(NSFStateMachineContext context)
        {
            return (value < 10);
        }

        private bool isValueHigh(NSFStateMachineContext context)
        {
            return (value > 10);
        }

        private void addValue(NSFStateMachineContext context)
        {
            value += 10;
        }

        #endregion Methods
    }
}
