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
using System.Threading;

using NorthStateSoftware.NorthStateFramework;

namespace NorthStateSoftware.Examples.CommandProcessorWithResetStrategyExample
{
    public class CommandProcessorWithResetStrategy : CommandProcessor
    {
        #region Fields

        private ResetStrategy resetStrategy;

        #endregion Fields

        #region Constructors

        public CommandProcessorWithResetStrategy(String name)
            : base(name)
        {
            resetStrategy = new ResetStrategy("ResetStrategy", resetState);
        }

        #endregion Constructors

        #region Methods

        private bool isReady(NSFStateMachineContext context)
        {
            return resetStrategy.ReadyState.isActive();
        }

        #endregion Methods
    }
}
