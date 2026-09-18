using App.Presentation.Feedback;
using UnityEngine;

namespace App.Presentation.Interaction
{
    /// <summary>
    /// Помечает объект сцены целью сценария. Идентификатор совпадает с полем
    /// "target" в конфигурации: по нему домен сверяет действия пользователя,
    /// а подсветка находит, что выделить.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class InteractionTarget : MonoBehaviour
    {
        [Tooltip("Идентификатор цели из файла сценария, например counter_zone.")]
        [SerializeField] private string _targetId;

        /// <summary>Идентификатор цели.</summary>
        public string TargetId => _targetId;

        /// <summary>Подсветка этой цели, если она есть на объекте.</summary>
        public TargetHighlighter Highlighter { get; private set; }

        private void Awake()
        {
            Highlighter = GetComponent<TargetHighlighter>();

            if (string.IsNullOrWhiteSpace(_targetId))
                Debug.LogError("У цели не задан идентификатор.", this);
        }

        private void OnEnable()
        {
            InteractionTargetRegistry.Register(this);
        }

        private void OnDisable()
        {
            InteractionTargetRegistry.Unregister(this);
        }
    }
}
