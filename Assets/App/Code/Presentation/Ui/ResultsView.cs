using System.Text;
using App.Application.Messages;
using App.Domain.Events;
using App.Domain.Execution;
using App.Presentation.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace App.Presentation.Ui
{
    /// <summary>
    /// Экран итогов: перечень всех шагов с их статусами и кнопки
    /// "Попытаться ещё" и "Возврат в лобби".
    ///
    /// Здесь же показывается ошибка чтения конфигурации — пользователь не должен
    /// остаться в пустой сцене без объяснений.
    /// </summary>
    public sealed class ResultsView : AppBehaviour
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private TMP_Text _titleLabel;
        [SerializeField] private TMP_Text _reportLabel;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _returnToLobbyButton;

        protected override void OnReady()
        {
            SetVisible(false);

            if (_restartButton != null)
                _restartButton.onClick.AddListener(OnRestart);

            if (_returnToLobbyButton != null)
                _returnToLobbyButton.onClick.AddListener(OnReturnToLobby);

            Subscribe<ScenarioFinished>(OnScenarioFinished);
            Subscribe<ScenarioLoadFailed>(OnScenarioLoadFailed);
        }

        protected override void OnDestroy()
        {
            if (_restartButton != null)
                _restartButton.onClick.RemoveListener(OnRestart);

            if (_returnToLobbyButton != null)
                _returnToLobbyButton.onClick.RemoveListener(OnReturnToLobby);

            base.OnDestroy();
        }

        private void OnScenarioFinished(ScenarioFinished message)
        {
            if (_titleLabel != null)
                _titleLabel.text = "Итоги: " + message.Session.Scenario.Title;

            if (_reportLabel != null)
                _reportLabel.text = BuildReport(message.Session);

            SetVisible(true);
        }

        private void OnScenarioLoadFailed(ScenarioLoadFailed message)
        {
            if (_titleLabel != null)
                _titleLabel.text = "Не удалось загрузить сценарий";

            if (_reportLabel != null)
                _reportLabel.text = message.Reason;

            SetVisible(true);
        }

        private static string BuildReport(ScenarioSession session)
        {
            var builder = new StringBuilder();
            var groups = session.Groups;

            for (var i = 0; i < groups.Count; i++)
            {
                builder.Append(StepStatusFormatter.FormatGroup(groups[i]));

                if (i < groups.Count - 1)
                    builder.AppendLine();
            }

            return builder.ToString();
        }

        private void OnRestart()
        {
            App.RestartScenario.Execute();
        }

        private void OnReturnToLobby()
        {
            App.ReturnToLobby.Execute();
        }

        private void SetVisible(bool isVisible)
        {
            if (_panel != null)
                _panel.SetActive(isVisible);
        }
    }
}
