using System;
using App.Domain.Execution;

namespace App.Domain.Events
{
    /// <summary>
    /// Группа закрыта — либо все шаги отработаны, либо она прервана из-за
    /// нарушения последовательности. Итоговый статус лежит в самой группе.
    /// </summary>
    public sealed class GroupClosed : IScenarioEvent
    {
        /// <summary>Закрытая группа.</summary>
        public StepGroupProgress Group { get; }

        public GroupClosed(StepGroupProgress group)
        {
            Group = group ?? throw new ArgumentNullException(nameof(group));
        }
    }
}
