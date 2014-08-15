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
    /// Test Fork Join basic functionality.
    /// </summary>
    public class BasicForkJoinTest : NSFStateMachine, ITestInterface
    {
        #region Fields

        TestHarness testHarness = new TestHarness();

        //Events
        private NSFEvent event1;
        private NSFEvent event2;

        // Regions and states, from outer to inner
        // Region A
        private NSFRegion regionA;
        private NSFInitialState initialAState;
        private NSFCompositeState stateA1;
        private NSFCompositeState stateA2;

        // Region B
        private NSFRegion regionB;
        private NSFInitialState initialBState;
        private NSFCompositeState stateB1;
        private NSFCompositeState stateB2;

        // ForkJoins
        private NSFForkJoin abForkJoin;

        // Transitions, ordered internal, local, external
        // Region A
        private NSFExternalTransition initialAStateToStateA1Transition;
        private NSFExternalTransition stateA1ToABForkJoinTransition;
        private NSFExternalTransition abForkJoinToStateA2Transition;

        // Region B
        private NSFExternalTransition initialBStateToStateB1Transition;
        private NSFExternalTransition stateB1ToABForkJoinTransition;
        private NSFExternalTransition abForkJoinToStateB2Transition;
        #endregion Fields

        #region Constructors
        public BasicForkJoinTest(String name)
            : base(name, new NSFEventThread(name))
        {
            // Events
            event1 = new NSFEvent("Event1", this);
            event2 = new NSFEvent("Event2", this);

            //States
            // Region A
            regionA = new NSFRegion("RegionA", this);
            initialAState = new NSFInitialState("InitialA", regionA);
            stateA1 = new NSFCompositeState("StateA1", regionA, null, null);
            stateA2 = new NSFCompositeState("StateA2", regionA, null, null);

            // Region B
            regionB = new NSFRegion("RegionB", this);
            initialBState = new NSFInitialState("InitialB", regionB);
            stateB1 = new NSFCompositeState("StateB1", regionB, null, null);
            stateB2 = new NSFCompositeState("StateB2", regionB, null, null);

            // ForkJoins
            abForkJoin = new NSFForkJoin("ABForkJoin", this);

            // Transitions, ordered internal, local, external
            // Region A
            initialAStateToStateA1Transition = new NSFExternalTransition("InitialAStateToStateA1", initialAState, stateA1, null, null, null);
            stateA1ToABForkJoinTransition = new NSFExternalTransition("StateA1ToABForkJoin", stateA1, abForkJoin, event1, null, null);
            abForkJoinToStateA2Transition = new NSFExternalTransition("AbForkJoinToStateA2", abForkJoin, stateA2, null, null, null);

            // Region B
            initialBStateToStateB1Transition = new NSFExternalTransition("InitialBStateToStateB1", initialBState, stateB1, null, null, null);
            stateB1ToABForkJoinTransition = new NSFExternalTransition("StateB1ToABForkJoin", stateB1, abForkJoin, event2, null, null);
            abForkJoinToStateB2Transition = new NSFExternalTransition("AbForkJoinToStateB2", abForkJoin, stateB2, null, null, null);
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
            //  user regions
            //  congruent regions
            //  nested states
            //  entry actions
            if (!testHarness.doesEventResultInState(null, stateA1))
            {
                errorMessage = "State Machine did not start properly with congruent states.";
                stopStateMachine();
                return false;
            }

            // Test 
            //  state machine event handling
            //  triggered transitions
            //  Fork join entry
            //  entry actions
            if (!testHarness.doesEventResultInState(event1, abForkJoin, stateB1))
            {
                errorMessage = "State Machine did enter the fork join or did not stay in stateB1.";
                stopStateMachine();
                return false;
            }

            // Test 
            //  state machine event handling
            //  triggered transitions
            //  Fork join entry evaluation
            //  Fork join exit
            //  entry actions
            if (!testHarness.doesEventResultInState(event2, stateA2, stateB2))
            {
                errorMessage = "State Machine did exit the fork join properly.";
                stopStateMachine();
                return false;
            }

            stopStateMachine();
            return true;
        }

        #endregion Methods
    }
}
