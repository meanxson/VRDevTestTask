using System;
using App.Application.Ports;
using UnityEngine.SceneManagement;

namespace App.Infrastructure.Navigation
{
    /// <summary>
    /// Переходы между сценами средствами Unity. Имена сцен приходят снаружи,
    /// из композиционного корня, — прикладной слой о них не знает.
    /// </summary>
    public sealed class SceneNavigator : ISceneNavigator
    {
        private readonly string _lobbySceneName;
        private readonly string _trainingSceneName;

        public SceneNavigator(string lobbySceneName, string trainingSceneName)
        {
            if (string.IsNullOrWhiteSpace(lobbySceneName))
                throw new ArgumentException("Не задано имя сцены лобби.", nameof(lobbySceneName));
            if (string.IsNullOrWhiteSpace(trainingSceneName))
                throw new ArgumentException("Не задано имя сцены тренировки.", nameof(trainingSceneName));

            _lobbySceneName = lobbySceneName;
            _trainingSceneName = trainingSceneName;
        }

        public void LoadLobby()
        {
            SceneManager.LoadScene(_lobbySceneName);
        }

        public void LoadTraining()
        {
            SceneManager.LoadScene(_trainingSceneName);
        }
    }
}
