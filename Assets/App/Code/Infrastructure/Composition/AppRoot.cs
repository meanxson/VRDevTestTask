using App.Application.Composition;
using App.Common.Events;
using App.Infrastructure.Configuration;
using App.Infrastructure.Navigation;
using UnityEngine;

namespace App.Infrastructure.Composition
{
    /// <summary>
    /// Композиционный корень приложения — единственное место, где создаются
    /// конкретные реализации и связываются со слоями. DI-контейнер не используется:
    /// вся сборка видна здесь целиком, в явном виде.
    ///
    /// Объект живёт между сценами, поэтому переход "лобби → тренировка → лобби"
    /// не пересобирает приложение и не теряет уже загруженный сценарий.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class AppRoot : MonoBehaviour
    {
        [Header("Сценарий")]
        [Tooltip("JSON-файл с описанием групп и шагов тренировки.")]
        [SerializeField] private TextAsset _scenarioConfiguration;

        [Header("Сцены")]
        [SerializeField] private string _lobbySceneName = "Lobby";
        [SerializeField] private string _trainingSceneName = "Training";

        private static AppRoot _instance;

        private EventBus _events;

        private void Awake()
        {
            // Корень попадает в обе сцены, но жить должен только первый:
            // так его можно положить и в лобби, и в тренировку и запускать
            // проект с любой сцены.
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            Compose();
        }

        private void OnDestroy()
        {
            if (_instance != this)
                return;

            AppServices.Uninstall();
            _events.Clear();
            _instance = null;
        }

        private void Compose()
        {
            _events = new EventBus();

            var scenarioSource = new JsonScenarioSource(_scenarioConfiguration);
            var navigator = new SceneNavigator(_lobbySceneName, _trainingSceneName);

            AppServices.Install(new AppComposition(_events, scenarioSource, navigator));
        }
    }
}
