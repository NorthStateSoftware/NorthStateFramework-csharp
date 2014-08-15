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
