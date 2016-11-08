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

namespace NSFExample
{
    public class Combat : NSFStateMachine
    {
        #region Fields
        // Events
        protected NSFEvent scoutEvent;

        // Regions and States
        protected NSFInitialState combatInitialState;
        protected NSFCompositeState scoutingState;
        protected NSFInitialState scoutingInitialState;
        protected NSFCompositeState patrolState;
        protected NSFCompositeState moveToEnemyState;
        protected NSFChoiceState attackChoiceState;
        protected NSFCompositeState attackState;

        // Transitions  
        protected NSFExternalTransition combatInitialToScoutingTransition;
        protected NSFExternalTransition scoutingToAttackChoiceTransition;
        protected NSFExternalTransition scoutingInitialToPatrolTransition;
        protected NSFExternalTransition attackChoiceToPatrolTransition;
        protected NSFExternalTransition attackChoiceToMoveToEnemyTransition;
        protected NSFExternalTransition attackChoiceToAttackTransition;

        #endregion Fields

        #region Properties

        public Double DistanceToEnemy { get; set; }
        public Double InRangeDistance { get; set; }
        public Double NearDistance { get; set; }

        //States
        public NSFCompositeState AttackState { get { return attackState; } }
        public NSFCompositeState ScoutingState { get { return scoutingState; } }
        public NSFCompositeState PatrolState { get { return patrolState; } }
        public NSFCompositeState MoveToEnemyState { get { return moveToEnemyState; } }

        // Transitions
        public NSFTransition CombatInitialToScoutingTransition { get { return combatInitialToScoutingTransition; } }
        public NSFTransition ScoutingToAttackChoiceTransition { get { return scoutingToAttackChoiceTransition; } }
        public NSFTransition AttackChoiceToPatrolTransition { get { return attackChoiceToPatrolTransition; } }
        public NSFTransition AttackChoiceToMoveToEnemyTransition { get { return attackChoiceToMoveToEnemyTransition; } }
        public NSFTransition AttackChoiceToAttackTransition { get { return attackChoiceToAttackTransition; } }
        public NSFTransition ScoutingInitialToPatrolTransition { get { return scoutingInitialToPatrolTransition; } }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Creates a combat state machine
        /// </summary>
        public Combat(String name)
            : base(name, new NSFEventThread(name))
        {
            DistanceToEnemy = 100;
            InRangeDistance = 25;
            NearDistance = 50;
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
            scoutEvent = new NSFEvent("Scout", this, this);

            // Regions and States
            combatInitialState = new NSFInitialState("CombatInitial", this);
            scoutingState = new NSFCompositeState("Scouting", this, null, null);
            scoutingInitialState = new NSFInitialState("ScountingInitial", scoutingState);
            patrolState = new NSFCompositeState("Patrol", scoutingState, null, null);
            moveToEnemyState = new NSFCompositeState("MoveToEnemy", scoutingState, null, null);
            attackChoiceState = new NSFChoiceState("AttackChoice", this);
            attackState = new NSFCompositeState("Attack", this, null, null);

            // Transitions  
            combatInitialToScoutingTransition = new NSFExternalTransition("CombatInitialToScouting", combatInitialState, scoutingState, null, null, null);
            scoutingToAttackChoiceTransition = new NSFExternalTransition("ScoutingToAttackChoice", scoutingState, attackChoiceState, scoutEvent, null, null);
            scoutingInitialToPatrolTransition = new NSFExternalTransition("ScoutingInitialToPatrol", scoutingInitialState, patrolState, null, null, null);
            attackChoiceToPatrolTransition = new NSFExternalTransition("AttackChoiceToPatrol", attackChoiceState, patrolState, null, Else, null);
            attackChoiceToAttackTransition = new NSFExternalTransition("AttackChoiceToAttack", attackChoiceState, attackState, null, isEnemyInRange, null);
            attackChoiceToMoveToEnemyTransition = new NSFExternalTransition("AttackChoiceToMoveToEnemy", attackChoiceState, moveToEnemyState, null, isEnemyNear, null);
        }

        #endregion Constructors

        #region Methods

        public void sendScoutTeam()
        {
            // Log distance to enemy
            NSFTraceLog.PrimaryTraceLog.addTrace(NSFTraceTags.InformationalTag,
                                 NSFTraceTags.SourceTag, this.Name,
                                 NSFTraceTags.VariableTag, "DistanceToEnemy",
                                 NSFTraceTags.ValueTag, DistanceToEnemy.ToString());

            queueEvent(scoutEvent);
        }

        protected virtual bool isEnemyNear(NSFStateMachineContext context)
        {
            // If the enemy is close but not in range it is near.
            return ((DistanceToEnemy < NearDistance) && (!isEnemyInRange(context)));
        }

        protected virtual bool isEnemyInRange(NSFStateMachineContext context)
        {
            return (DistanceToEnemy < InRangeDistance);
        }

        #endregion Methods
    }
}
