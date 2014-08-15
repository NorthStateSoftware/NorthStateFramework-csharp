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
    /// Test Fork Join to Fork Join Transitions
    /// Extra regional transitions to ForkJoins
    /// </summary>
    public class ForkJoinToForkJoinTransitionTest : NSFStateMachine, ITestInterface
    {
        #region Fields

        TestHarness testHarness = new TestHarness();

        // Fork Joins
        private NSFForkJoin bSynch;
        private NSFForkJoin abForkJoin;
        private NSFForkJoin bcForkJoin;

        // ARegion
        // Events
        private NSFEvent evA1;

        // Regions and states, from outer to inner
        private NSFRegion aRegion;
        private NSFInitialState initialAState;
        private NSFCompositeState stateA1;
        private NSFCompositeState stateA2;

        // Transitions, ordered internal, local, external
        private NSFExternalTransition initialAToStateA1Transition;
        private NSFExternalTransition stateA1ToABForkJoinTransition;
        private NSFExternalTransition abForkJoinToA2Transition;

        // B Region
        // Events
        private NSFEvent evB1;

        // Regions and states, from outer to inner
        private NSFRegion bRegion;
        private NSFInitialState initialBState;
        private NSFCompositeState stateB1;
        private NSFCompositeState stateB2;

        // Transitions, ordered internal, local, external
        private NSFExternalTransition initialBToStateB1Transition;
        private NSFExternalTransition stateB1ToBSynchTransition;
        private NSFExternalTransition bSynchToStateB2Transition;

        // C Region
        // Events
        private NSFEvent evC1;

        // Regions and states, from outer to inner
        private NSFRegion cRegion;
        private NSFInitialState initialCState;
        private NSFCompositeState stateC1;
        private NSFCompositeState stateC2;

        // Transitions, ordered internal, local, external
        private NSFExternalTransition initialCToStateC1Transition;
        private NSFExternalTransition stateC1ToBCForkJoinTransition;
        private NSFExternalTransition bcForkJoinToC2Transition;

        // Extra Regional Transitions
        private NSFExternalTransition bSynchToABForkJoinTransition;
        private NSFExternalTransition bSynchToBCForkJoinTransition;
        #endregion Fields

        #region Constructors
        public ForkJoinToForkJoinTransitionTest(String name)
            : base(name, new NSFEventThread(name))
        {
            // Fork Joins
            bSynch = new NSFForkJoin("BSynch", this);
            abForkJoin = new NSFForkJoin("ABForkJoin", this);
            bcForkJoin = new NSFForkJoin("BCForkJoin", this);
            // ARegion
            // Events
            evA1 = new NSFEvent("EvA1", this);
            // States
            aRegion = new NSFRegion("ARegion", this);
            initialAState = new NSFInitialState("InitialA", aRegion);
            stateA1 = new NSFCompositeState("StateA1", aRegion, null, null);
            stateA2 = new NSFCompositeState("StateA2", aRegion, null, null);
            // Transitions, ordered internal, local, external
            initialAToStateA1Transition = new NSFExternalTransition("InitialAToStateA1", initialAState, stateA1, null, null, null);
            stateA1ToABForkJoinTransition = new NSFExternalTransition("StateA1ToABForkJoin", stateA1, abForkJoin, evA1, null, null);
            abForkJoinToA2Transition = new NSFExternalTransition("ABForkJoinToA2", abForkJoin, stateA2, null, null, null);
            // B Region
            // Events
            evB1 = new NSFEvent("EvB1", this);
            // States
            bRegion = new NSFRegion("BRegion", this);
            initialBState = new NSFInitialState("InitialB", bRegion);
            stateB1 = new NSFCompositeState("StateB1", bRegion, null, null);
            stateB2 = new NSFCompositeState("StateB2", bRegion, null, null);
            // Transitions, ordered internal, local, external
            initialBToStateB1Transition = new NSFExternalTransition("InitialBToStateB1", initialBState, stateB1, null, null, null);
            stateB1ToBSynchTransition = new NSFExternalTransition("StateB1ToBSynch", stateB1, bSynch, evB1, null, null);
            bSynchToStateB2Transition = new NSFExternalTransition("BSynchToStateB2", bSynch, stateB2, null, null, null);
            // C Region
            // Events
            evC1 = new NSFEvent("EvC1", this);
            // States
            cRegion = new NSFRegion("CRegion", this);
            initialCState = new NSFInitialState("InitialC", cRegion);
            stateC1 = new NSFCompositeState("StateC1", cRegion, null, null);
            stateC2 = new NSFCompositeState("StateC2", cRegion, null, null);
            // Transitions, ordered internal, local, external
            initialCToStateC1Transition = new NSFExternalTransition("InitialCToStateC1", initialCState, stateC1, null, null, null);
            stateC1ToBCForkJoinTransition = new NSFExternalTransition("StateC1ToBCForkJoin", stateC1, bcForkJoin, evC1, null, null);
            bcForkJoinToC2Transition = new NSFExternalTransition("BCForkJoinToC2", bcForkJoin, stateC2, null, null, null);

            // Extra Regional Transitions
            bSynchToABForkJoinTransition = new NSFExternalTransition("BSynchToABForkJoin", bSynch, abForkJoin, null, null, null);
            bSynchToBCForkJoinTransition = new NSFExternalTransition("BSynchToBCForkJoin", bSynch, bcForkJoin, null, null, null);
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
            //  entry actions
            if (!testHarness.doesEventResultInState(null, stateA1, stateC1))
            {
                errorMessage = "State Machine did not start properly into stateA1 and stateC1.";
                stopStateMachine();
                return false;
            }

            // Test 
            //  state machine event handling
            //  triggered transitions
            //  Fork join entry
            //  entry actions
            if (!testHarness.doesEventResultInState(evA1, abForkJoin))
            {
                errorMessage = "State Machine did enter the fork join abForkJoin.";
                stopStateMachine();
                return false;
            }

            // Test
            //  That the fork join does not spontaneously exit
            if (testHarness.doesEventResultInState(null, stateA2))
            {
                errorMessage = "State Machine entered stateA2 and should not have until after all entry transitions had been taken.";
                stopStateMachine();
                return false;
            }

            // Test 
            //  state machine event handling
            //  triggered transitions
            //  Fork join transitions
            //  Fork join entry evaluation
            //  Fork join exit
            //  entry actions
            if (!testHarness.doesEventResultInState(evB1, stateA2, stateB2))
            {
                errorMessage = "State Machine did not handle the fork join to fork join transition properly.";
                stopStateMachine();
                return false;
            }

            // Test
            //  That the fork join does not spontaneously exit
            if (testHarness.doesEventResultInState(null, stateC2))
            {
                errorMessage = "State Machine entered stateC2 and should not have until after all entry transitions had been taken.";
                stopStateMachine();
                return false;
            }

            // Test 
            //  state machine event handling
            //  triggered transitions
            //  Fork join transitions
            //  Fork join entry evaluation
            //  Fork join exit
            //  entry actions
            if (!testHarness.doesEventResultInState(evC1, stateC2))
            {
                errorMessage = "State Machine did not handle the fork join to fork join transition properly.";
                stopStateMachine();
                return false;
            }

            stopStateMachine();
            return true;
        }

        #endregion Methods
    }
}
