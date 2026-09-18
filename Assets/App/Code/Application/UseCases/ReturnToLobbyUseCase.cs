using System;
using App.Application.Ports;

namespace App.Application.UseCases
{
    /// <summary>Вернуться из тренировки в лобби.</summary>
    public sealed class ReturnToLobbyUseCase
    {
        private readonly ISceneNavigator _navigator;

        public ReturnToLobbyUseCase(ISceneNavigator navigator)
        {
            _navigator = navigator ?? throw new ArgumentNullException(nameof(navigator));
        }

        public void Execute()
        {
            _navigator.LoadLobby();
        }
    }
}
