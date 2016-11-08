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

using NSFString = System.String;

namespace NorthStateSoftware.NorthStateFramework
{
    /// <summary>
    /// Represents a shallow history pseudo-state.
    /// </summary>
    /// <remarks>
    /// NSFShallowHistory is a transient state.  
    /// Once entered, it forces entry into the last active substate in the region.
    /// </remarks>
    public class NSFShallowHistory : NSFState
    {
        #region Public Constructors

        /// <summary>
        /// Creates a shallow history pseudo-state.
        /// </summary>
        /// <param name="name">The name of the shallow history pseudo-state.</param>
        /// <param name="parentRegion">The parent region of the shallow history pseudo-state.</param>
        public NSFShallowHistory(NSFString name, NSFRegion parentRegion)
            : base(name, parentRegion, null, null)
        {
        }

        /// <summary>
        /// Creates a shallow history pseudo-state.
        /// </summary>
        /// <param name="name">The name of the shallow history pseudo-state.</param>
        /// <param name="parentState">The parent state of the shallow history pseudo-state.</param>
        public NSFShallowHistory(NSFString name, NSFCompositeState parentState)
            : base(name, parentState, null, null)
        {
        }

        #endregion Public Constructors

        #region Protected Methods

        protected internal override void enter(NSFStateMachineContext context, bool useHistory)
        {
            // Base class behavior
            base.enter(context, false);

            // Additional behavior

            // Enter history substate, not using recursive history
            NSFState historySubstate = parentRegion.HistorySubstate;
            if (historySubstate != NSFState.NullState)
            {
                historySubstate.enter(context, false);
            }
        }

        #endregion Protected Methods
    }
}
