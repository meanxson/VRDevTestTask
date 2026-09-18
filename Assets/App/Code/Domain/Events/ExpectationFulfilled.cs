using System;
using App.Domain.Execution;

namespace App.Domain.Events
{
    /// <summary>
    /// Пользователь выполнил одно из ожидаемых действий шага.
    /// Событие приходит на каждое правильное действие, в том числе на последнее,
    /// поэтому на него удобно вешать звуковой сигнал правильного выполнения
    /// и снятие подсветки с отработанной цели.
    /// </summary>
    public sealed class ExpectationFulfilled : IScenarioEvent
    {
        /// <summary>Шаг, ожидание которого закрыто.</summary>
        public StepProgress Step { get; }

        /// <summary>Индекс закрытого ожидания в описании шага.</summary>
        public int ExpectationIndex { get; }

        public ExpectationFulfilled(StepProgress step, int expectationIndex)
        {
            Step = step ?? throw new ArgumentNullException(nameof(step));
            ExpectationIndex = expectationIndex;
        }
    }
}
