using App.Domain.Actions;
using App.Presentation.Core;
using UnityEngine;
using UnityEngine.UI;

namespace App.Presentation.Interaction
{
    /// <summary>
    /// Сообщает сценарию о нажатии кнопки интерфейса.
    ///
    /// Кнопка Unity одинаково откликается и на луч контроллера (через XRUIInputModule),
    /// и на мышь, поэтому отдельная обработка ввода здесь не нужна.
    /// </summary>
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(InteractionTarget))]
    public sealed class UiButtonActionReporter : AppBehaviour
    {
        private InteractionTarget _target;
        private Button _button;

        protected override void OnReady()
        {
            _target = GetComponent<InteractionTarget>();
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClick);
        }

        protected override void OnDestroy()
        {
            if (_button != null)
                _button.onClick.RemoveListener(OnClick);

            base.OnDestroy();
        }

        private void OnClick()
        {
            App.SubmitUserAction.Execute(new UserAction(ActionType.PressUiButton, _target.TargetId));
        }
    }
}
