using App.Domain.Actions;
using App.Presentation.Core;
using UnityEngine;

namespace App.Presentation.Interaction
{
    /// <summary>
    /// Зона интереса: сообщает сценарию, что пользователь до неё дошёл.
    ///
    /// Требует триггерный коллайдер на том же объекте.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(InteractionTarget))]
    public sealed class PointOfInterestZone : AppBehaviour
    {
        [Tooltip("Тег игрока. Пусто — засчитывать вход любого коллайдера.")]
        [SerializeField] private string _playerTag = "Player";

        private InteractionTarget _target;

        protected override void OnReady()
        {
            _target = GetComponent<InteractionTarget>();
        }

        private void OnTriggerEnter(Collider other)
        {
            // Пока приложение не готово, вход в зону просто игнорируем.
            if (App == null || _target == null)
                return;

            if (!IsPlayer(other))
                return;

            App.SubmitUserAction.Execute(new UserAction(ActionType.ReachPoint, _target.TargetId));
        }

        private bool IsPlayer(Component other)
        {
            return string.IsNullOrWhiteSpace(_playerTag) || other.CompareTag(_playerTag);
        }

        private void Reset()
        {
            var zoneCollider = GetComponent<Collider>();
            if (zoneCollider != null)
                zoneCollider.isTrigger = true;
        }
    }
}
