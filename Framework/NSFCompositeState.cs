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

using NSFString = System.String;

namespace NorthStateSoftware.NorthStateFramework
{
    /// <summary>
    /// Represents a state that may contain one or more orthogonal regions.
    /// </summary>
    /// <remarks>
    /// NSFCompositeState is the most commonly used state type.
    /// It affords the ability to nest regions and substates,
    /// making it flexible for extension.  There is minimal performance
    /// penalty for this capability, so this type should be preferred
    /// over the basic NSFState.
    /// </remarks>
    public class NSFCompositeState : NSFState
    {
        #region Public Constructors

        /// <summary>
        /// Creates a composite state.
        /// </summary>
        /// <param name="name">The name of the composite state.</param>
        /// <param name="parentRegion">The parent region of the composite state.</param>
        /// <param name="entryAction">The actions to be performed upon entry to the composite state.</param>
        /// <param name="exitAction">The actions to be performed upon exit of the composite state.</param>
        public NSFCompositeState(NSFString name, NSFRegion parentRegion, NSFVoidAction<NSFStateMachineContext> entryAction, NSFVoidAction<NSFStateMachineContext> exitAction)
            : base(name, parentRegion, entryAction, exitAction)
        {
        }

        /// <summary>
        /// Creates a composite state.
        /// </summary>
        /// <param name="name">The name of the composite state.</param>
        /// <param name="parentState">The parent state of the composite state.</param>
        /// <param name="entryAction">The actions to be performed upon entry to the composite state.</param>
        /// <param name="exitAction">The actions to be performed upon exit of the composite state.</param>
        public NSFCompositeState(NSFString name, NSFCompositeState parentState, NSFVoidAction<NSFStateMachineContext> entryAction, NSFVoidAction<NSFStateMachineContext> exitAction)
            : base(name, parentState, entryAction, exitAction)
        {
        }

        #endregion Public Constructors

        #region Public Methods

        public override bool isInState(NSFState state)
        {
            if (!active)
            {
                return false;
            }

            // Base class behavior
            if (base.isInState(state))
            {
                return true;
            }

            // Check regions
            foreach (NSFRegion region in regions)
            {
                if (region.isInState(state))
                {
                    return true;
                }
            }

            return false;
        }

        public override bool isInState(NSFString stateName)
        {
            if (!active)
            {
                return false;
            }

            // Base class behavior
            if (base.isInState(stateName))
            {
                return true;
            }

            // Check regions
            foreach (NSFRegion region in regions)
            {
                if (region.isInState(stateName))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion Public Methods

        #region Protected Methods

        protected internal override void enter(NSFStateMachineContext context, bool useHistory)
        {
            // Base class behavior
            base.enter(context, useHistory);

            // Additional behavior
            enterRegions(context, useHistory);
        }

        protected internal override void exit(NSFStateMachineContext context)
        {
            // Additional behavior
            exitRegions(context);

            // Base class behavior
            base.exit(context);
        }

        protected internal override NSFEventStatus processEvent(NSFEvent nsfEvent)
        {
            // Additional behavior

            // Let regions process event first
            NSFEventStatus eventStatus = NSFEventStatus.NSFEventUnhandled;

            foreach (NSFRegion region in regions)
            {
                NSFEventStatus status = region.processEvent(nsfEvent);

                if (status == NSFEventStatus.NSFEventHandled)
                {
                    eventStatus = NSFEventStatus.NSFEventHandled;
                }
            }

            if (eventStatus == NSFEventStatus.NSFEventHandled)
            {
                return NSFEventStatus.NSFEventHandled;
            }

            // Base class behavior
            return base.processEvent(nsfEvent);
        }

        protected internal override void reset()
        {
            // Base class behavior
            base.reset();

            // Reset regions
            foreach (NSFRegion region in regions)
            {
                region.reset();
            }
        }

        #endregion Protected Methods

        #region Internal Methods

        /// <summary>
        /// Adds a region to the composite state.
        /// </summary>
        /// <param name="region">The region to add.</param>
        internal void addRegion(NSFRegion region)
        {
            if (!regions.Contains(region))
            {
                regions.Add(region);
            }
        }

        /// <summary>
        /// Enters all the regions of the composite state.
        /// </summary>
        /// <param name="context">Additional contextual information.</param>
        /// <param name="useHistory">Flag indicating if regions should be entered using the history substate.</param>
        internal void enterRegions(NSFStateMachineContext context, bool useHistory)
        {
            foreach (NSFRegion region in regions)
            {
                if (!region.isActive())
                {
                    region.enter(context, useHistory);
                }
            }
        }

        /// <summary>
        /// Exits all the regions of the composite state.
        /// </summary>
        internal void exitRegions(NSFStateMachineContext context)
        {
            foreach (NSFRegion region in regions)
            {
                if (region.isActive())
                {
                    region.exit(context);
                }
            }
        }

        /// <summary>
        /// Gets the composite state's default region.
        /// </summary>
        internal NSFRegion getDefaultRegion()
        {
            if (defaultRegion == null)
            {
                defaultRegion = new NSFRegion(Name + "DefaultRegion", this);
            }
            return defaultRegion;
        }

        #endregion Internal Methods

        #region Private Fields, Events, and Properties

        private NSFRegion defaultRegion;
        private List<NSFRegion> regions = new List<NSFRegion>();


        #endregion Private Fields, Events, and Properties
    }
}