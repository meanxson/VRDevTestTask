using System;
using App.Domain.Actions;
using App.Domain.Execution;

namespace App.Domain.Events
{
    /// <summary>
    /// Зафиксировано нарушение. По этому событию проигрывается звуковой сигнал
    /// нарушения; отметки в шагах приходят отдельными событиями StepResolved.
    /// </summary>
    public sealed class ViolationRaised : IScenarioEvent
    {
        /// <summary>Вид нарушения.</summary>
        public ViolationKind Kind { get; }

        /// <summary>Действие пользователя, которое привело к нарушению.</summary>
        public UserAction Action { get; }

        /// <summary>Шаг, на котором произошло нарушение.</summary>
        public StepProgress Step { get; }

        public ViolationRaised(ViolationKind kind, UserAction action, StepProgress step)
        {
            Kind = kind;
            Action = action;
            Step = step ?? throw new ArgumentNullException(nameof(step));
        }
    }
}
