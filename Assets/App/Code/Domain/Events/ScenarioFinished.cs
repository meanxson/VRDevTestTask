using System;
using App.Domain.Execution;

namespace App.Domain.Events
{
    /// <summary>
    /// Сценарий пройден. По этому событию показывается экран итогов:
    /// перечень шагов и их статусы доступны через сессию.
    /// </summary>
    public sealed class ScenarioFinished : IScenarioEvent
    {
        /// <summary>Сессия прохождения с итоговыми статусами всех шагов.</summary>
        public ScenarioSession Session { get; }

        public ScenarioFinished(ScenarioSession session)
        {
            Session = session ?? throw new ArgumentNullException(nameof(session));
        }
    }
}
