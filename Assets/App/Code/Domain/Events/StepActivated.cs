using System;
using App.Domain.Execution;

namespace App.Domain.Events
{
    /// <summary>
    /// Активирован шаг. По этому событию подсвечиваются цели, которых ждёт шаг.
    /// </summary>
    public sealed class StepActivated : IScenarioEvent
    {
        /// <summary>Активированный шаг.</summary>
        public StepProgress Step { get; }

        public StepActivated(StepProgress step)
        {
            Step = step ?? throw new ArgumentNullException(nameof(step));
        }
    }
}
