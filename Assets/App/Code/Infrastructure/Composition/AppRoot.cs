using App.Application.Composition;
using App.Common.Events;
using App.Infrastructure.Configuration;
using App.Infrastructure.Navigation;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace App.Infrastructure.Composition
{
    /// <summary>
    /// Композиционный корень приложения — единственное место, где создаются
    /// конкретные реализации и связываются со слоями. DI-контейнер не используется:
    /// вся сборка видна здесь целиком, в явном виде.
    ///
    /// Корень не только собирает приложение, но и раздаёт его компонентам сцены:
    /// при загрузке каждой сцены он находит всех, кому зависимости нужны, и отдаёт
    /// их явным вызовом. Компоненты ничего ниоткуда не вытягивают — им приносят.
    ///
    /// Синглтона здесь нет: корень не лежит в сценах и создаётся один раз
    /// из AppBootstrapper до загрузки первой сцены, поэтому ни статическая ссылка
    /// на себя, ни защита от дублей ему не нужны. Единственная проверка на дубль —
    /// диагностическая: она сообщает об ошибке сборки сцены, а не прячет её.
    ///
    /// Порядок выполнения задан явно и намеренно ранний: сборка и раздача должны
    /// пройти раньше, чем компоненты сцены дойдут до своего Start.
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    [DisallowMultipleComponent]
    public sealed class AppRoot : MonoBehaviour
    {
        [Header("Сценарий")]
        [Tooltip("JSON-файл с описанием групп и шагов тренировки.")]
        [SerializeField] private TextAsset _scenarioConfiguration;

        [Header("Сцены")]
        [SerializeField] private string _lobbySceneName = "Lobby";
        [SerializeField] private string _trainingSceneName = "Training";

        private EventBus _events;
        private AppComposition _composition;

        private void Awake()
        {
            WarnIfDuplicated();

            Compose();

            // Сцена, с которой стартовали, может быть уже загружена — тогда
            // события по ней не придёт, и её нужно обойти вручную.
            InjectInto(gameObject.scene);
            InjectInto(SceneManager.GetActiveScene());

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;

            if (_events != null)
                _events.Clear();
        }

        private void Compose()
        {
            _events = new EventBus();

            var scenarioSource = new JsonScenarioSource(_scenarioConfiguration);
            var navigator = new SceneNavigator(_lobbySceneName, _trainingSceneName);

            _composition = new AppComposition(_events, scenarioSource, navigator);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            InjectInto(scene);
        }

        /// <summary>
        /// Раздаёт сборку всем компонентам сцены, которым она нужна.
        ///
        /// Событие загрузки сцены приходит после Awake её объектов, но до их Start,
        /// поэтому к моменту первого использования зависимости уже на месте.
        /// Выключенные объекты тоже обходим: панель итогов стартует скрытой,
        /// но подписаться на события должна сразу.
        ///
        /// Повторная раздача безопасна — Construct только присваивает ссылку.
        /// </summary>
        private void InjectInto(Scene scene)
        {
            if (!scene.IsValid() || !scene.isLoaded)
                return;

            var roots = scene.GetRootGameObjects();

            for (var i = 0; i < roots.Length; i++)
            {
                var dependents = roots[i].GetComponentsInChildren<IAppDependent>(true);

                for (var j = 0; j < dependents.Length; j++)
                    dependents[j].Construct(_composition);
            }
        }

        /// <summary>
        /// Второй корень означает, что его забыли убрать из сцены: приложение
        /// соберётся дважды, и половина подписок уйдёт в чужую шину. Молча
        /// уничтожать дубль нельзя — это прячет ошибку до первого странного бага.
        /// </summary>
        private void WarnIfDuplicated()
        {
            var roots = FindObjectsByType<AppRoot>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            if (roots.Length > 1)
                Debug.LogError(
                    "В приложении несколько AppRoot: корень создаётся автоматически, " +
                    "убирать его из сцен не нужно, но и класть туда тоже.", this);
        }
    }
}
