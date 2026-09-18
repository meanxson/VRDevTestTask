using App.Domain.Actions;
using App.Presentation.Core;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace App.Presentation.Interaction
{
    /// <summary>
    /// База для компонентов, которые превращают взаимодействие XR Interaction Toolkit
    /// в действие пользователя для сценария.
    ///
    /// Наследник отвечает только за то, каким типом действия считать выбор объекта.
    /// </summary>
    [RequireComponent(typeof(InteractionTarget))]
    public abstract class InteractableActionReporter : AppBehaviour
    {
        private InteractionTarget _target;
        private XRBaseInteractable _interactable;

        /// <summary>Тип действия, о котором сообщает наследник.</summary>
        protected abstract ActionType ActionType { get; }

        protected override void OnReady()
        {
            _target = GetComponent<InteractionTarget>();
            _interactable = GetComponent<XRBaseInteractable>();

            if (_interactable == null)
            {
                Debug.LogError("Не найден XRBaseInteractable на объекте цели.", this);
                enabled = false;
                return;
            }

            _interactable.selectEntered.AddListener(OnSelectEntered);
        }

        protected override void OnDestroy()
        {
            if (_interactable != null)
                _interactable.selectEntered.RemoveListener(OnSelectEntered);

            base.OnDestroy();
        }

        /// <summary>Сообщить сценарию о действии над этой целью.</summary>
        protected void Report()
        {
            if (App == null || _target == null)
                return;

            App.SubmitUserAction.Execute(new UserAction(ActionType, _target.TargetId));
        }

        private void OnSelectEntered(SelectEnterEventArgs args)
        {
            Report();
        }
    }
}
