using System;
using App.Domain.Execution;

namespace App.Domain.Events
{
    /// <summary>
    /// Активирована группа шагов. По этому событию выводится информационное
    /// сообщение о порядке ожидаемых действий.
    /// </summary>
    public sealed class GroupActivated : IScenarioEvent
    {
        /// <summary>Активированная группа.</summary>
        public StepGroupProgress Group { get; }

        public GroupActivated(StepGroupProgress group)
        {
            Group = group ?? throw new ArgumentNullException(nameof(group));
        }
    }
}
