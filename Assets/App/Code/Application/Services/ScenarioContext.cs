using App.Domain.Execution;
using App.Domain.Scenarios;

namespace App.Application.Services
{
    /// <summary>
    /// Общее состояние прохождения, с которым работают юзкейсы: описание сценария
    /// и текущая попытка.
    ///
    /// Описание загружается один раз и переиспользуется — оно неизменяемо,
    /// а каждая новая попытка получает собственную сессию.
    /// Менять состояние может только прикладной слой, наружу оно доступно на чтение.
    /// </summary>
    public sealed class ScenarioContext
    {
        /// <summary>Загруженное описание сценария или null, пока его не прочитали.</summary>
        public Scenario Scenario { get; internal set; }

        /// <summary>Текущая попытка прохождения или null, пока тренировка не запущена.</summary>
        public ScenarioSession Session { get; internal set; }
    }
}
