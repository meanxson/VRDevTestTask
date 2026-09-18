using System;
using App.Domain.Execution;

namespace App.Domain.Events
{
    /// <summary>
    /// Шаг получил финальный статус: выполнен, выполнен с ошибкой или пропущен.
    /// Единая точка, по которой представление проставляет отметки в чек-листе.
    /// </summary>
    public sealed class StepResolved : IScenarioEvent
    {
        /// <summary>Завершённый шаг (актуальный статус — в самом шаге).</summary>
        public StepProgress Step { get; }

        public StepResolved(StepProgress step)
        {
            Step = step ?? throw new ArgumentNullException(nameof(step));
        }
    }
}
