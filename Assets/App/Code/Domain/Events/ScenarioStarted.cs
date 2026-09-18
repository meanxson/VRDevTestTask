using System;
using App.Domain.Execution;

namespace App.Domain.Events
{
    /// <summary>Сценарий запущен. Представление может показать стартовую информацию.</summary>
    public sealed class ScenarioStarted : IScenarioEvent
    {
        /// <summary>Сессия прохождения.</summary>
        public ScenarioSession Session { get; }

        public ScenarioStarted(ScenarioSession session)
        {
            Session = session ?? throw new ArgumentNullException(nameof(session));
        }
    }
}
