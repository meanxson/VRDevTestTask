using System;
using App.Application.Messages;
using App.Application.Ports;
using App.Application.Services;
using App.Common.Events;
using App.Domain.Execution;

namespace App.Application.UseCases
{
    /// <summary>
    /// Начать прохождение сценария.
    ///
    /// Вызывается при загрузке сцены тренировки — в том числе после перезапуска
    /// сцены по кнопке "Попытаться ещё": каждый вызов создаёт новую попытку
    /// на том же описании сценария.
    /// </summary>
    public sealed class StartScenarioUseCase
    {
        private readonly IScenarioSource _scenarioSource;
        private readonly ScenarioContext _context;
        private readonly ScenarioEventDispatcher _dispatcher;
        private readonly IEventPublisher _publisher;

        public StartScenarioUseCase(
            IScenarioSource scenarioSource,
            ScenarioContext context,
            ScenarioEventDispatcher dispatcher,
            IEventPublisher publisher)
        {
            _scenarioSource = scenarioSource ?? throw new ArgumentNullException(nameof(scenarioSource));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
        }

        public void Execute()
        {
            if (!TryLoadScenario())
                return;

            _context.Session = new ScenarioSession(_context.Scenario);
            _dispatcher.Dispatch(_context.Session.Start());
        }

        /// <summary>
        /// Читает описание сценария при первом обращении.
        /// Конфигурация — внешние данные, поэтому ошибку чтения превращаем
        /// в сообщение для пользователя, а не роняем сцену молча.
        /// </summary>
        private bool TryLoadScenario()
        {
            if (_context.Scenario != null)
                return true;

            try
            {
                _context.Scenario = _scenarioSource.Load();
            }
            catch (Exception exception)
            {
                _publisher.Publish(new ScenarioLoadFailed(exception.Message));
                return false;
            }

            return true;
        }
    }
}
