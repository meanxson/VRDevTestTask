using System;
using App.Application.Ports;

namespace App.Application.UseCases
{
    /// <summary>Перейти из лобби к тренировке.</summary>
    public sealed class StartTrainingUseCase
    {
        private readonly ISceneNavigator _navigator;

        public StartTrainingUseCase(ISceneNavigator navigator)
        {
            _navigator = navigator ?? throw new ArgumentNullException(nameof(navigator));
        }

        public void Execute()
        {
            _navigator.LoadTraining();
        }
    }
}
