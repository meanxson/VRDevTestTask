using App.Presentation.Core;
using UnityEngine;
using UnityEngine.UI;

namespace App.Presentation.Ui
{
    /// <summary>
    /// Меню лобби: переход к сцене тренировки.
    /// Кнопка одинаково работает и лучом контроллера, и мышью.
    /// </summary>
    public sealed class LobbyMenuView : AppBehaviour
    {
        [SerializeField] private Button _startTrainingButton;

        protected override void OnReady()
        {
            if (_startTrainingButton == null)
            {
                Debug.LogError("Не назначена кнопка перехода к тренировке.", this);
                return;
            }

            _startTrainingButton.onClick.AddListener(OnStartTraining);
        }

        protected override void OnDestroy()
        {
            if (_startTrainingButton != null)
                _startTrainingButton.onClick.RemoveListener(OnStartTraining);

            base.OnDestroy();
        }

        private void OnStartTraining()
        {
            App.StartTraining.Execute();
        }
    }
}
