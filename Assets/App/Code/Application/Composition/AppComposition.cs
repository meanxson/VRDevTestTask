using System;
using App.Application.Ports;
using App.Application.Services;
using App.Application.UseCases;
using App.Common.Events;

namespace App.Application.Composition
{
    /// <summary>
    /// Собранное приложение: шина, общее состояние прохождения и все юзкейсы.
    ///
    /// Сборка сделана руками, без DI-контейнера — так требует техническое задание.
    /// Наружу класс принимает только порты, поэтому его можно собрать и в тесте,
    /// подставив заглушки вместо Unity-реализаций.
    /// </summary>
    public sealed class AppComposition
    {
        public AppComposition(IEventBus events, IScenarioSource scenarioSource, ISceneNavigator navigator)
        {
            if (scenarioSource == null)
                throw new ArgumentNullException(nameof(scenarioSource));
            if (navigator == null)
                throw new ArgumentNullException(nameof(navigator));

            Events = events ?? throw new ArgumentNullException(nameof(events));
            Scenario = new ScenarioContext();

            var dispatcher = new ScenarioEventDispatcher(events);

            StartScenario = new StartScenarioUseCase(scenarioSource, Scenario, dispatcher, events);
            SubmitUserAction = new SubmitUserActionUseCase(Scenario, dispatcher);
            StartTraining = new StartTrainingUseCase(navigator);
            RestartScenario = new RestartScenarioUseCase(navigator);
            ReturnToLobby = new ReturnToLobbyUseCase(navigator);
        }

        /// <summary>Шина событий приложения.</summary>
        public IEventBus Events { get; }

        /// <summary>Текущее описание сценария и текущая попытка прохождения.</summary>
        public ScenarioContext Scenario { get; }

        public StartScenarioUseCase StartScenario { get; }

        public SubmitUserActionUseCase SubmitUserAction { get; }

        public StartTrainingUseCase StartTraining { get; }

        public RestartScenarioUseCase RestartScenario { get; }

        public ReturnToLobbyUseCase ReturnToLobby { get; }
    }
}
