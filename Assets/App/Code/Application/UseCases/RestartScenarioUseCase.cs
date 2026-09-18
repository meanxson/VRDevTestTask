using System;
using App.Application.Ports;

namespace App.Application.UseCases
{
    /// <summary>
    /// Повторить попытку прохождения.
    ///
    /// Это перезагрузка сцены тренировки, а не сброс состояния в памяти: так
    /// вместе с прогрессом возвращаются на места перенесённые предметы и игрок,
    /// и не остаётся шансов на рассинхрон сцены и сценария.
    /// </summary>
    public sealed class RestartScenarioUseCase
    {
        private readonly ISceneNavigator _navigator;

        public RestartScenarioUseCase(ISceneNavigator navigator)
        {
            _navigator = navigator ?? throw new ArgumentNullException(nameof(navigator));
        }

        public void Execute()
        {
            _navigator.LoadTraining();
        }
    }
}
