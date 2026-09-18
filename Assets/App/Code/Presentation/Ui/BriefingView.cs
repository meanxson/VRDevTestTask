using System.Collections;
using System.Text;
using App.Domain.Events;
using App.Domain.Scenarios;
using App.Presentation.Core;
using TMPro;
using UnityEngine;

namespace App.Presentation.Ui
{
    /// <summary>
    /// Информационное сообщение о порядке ожидаемых действий: показывается при
    /// активации группы шагов и через несколько секунд убирается, чтобы не мешать
    /// обзору.
    ///
    /// Если у группы не задан текст, он собирается из описаний её шагов —
    /// пользователь в любом случае узнает, что от него ждут и в каком порядке.
    /// </summary>
    public sealed class BriefingView : AppBehaviour
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private TMP_Text _titleLabel;
        [SerializeField] private TMP_Text _bodyLabel;

        [Tooltip("Сколько секунд держать сообщение на экране.")]
        [SerializeField] private float _displaySeconds = 6f;

        private Coroutine _hideRoutine;

        protected override void OnReady()
        {
            Hide();
            Subscribe<GroupActivated>(OnGroupActivated);
            Subscribe<ScenarioFinished>(OnScenarioFinished);
        }

        private void OnGroupActivated(GroupActivated message)
        {
            var group = message.Group.Definition;

            if (_titleLabel != null)
                _titleLabel.text = group.Title;

            if (_bodyLabel != null)
                _bodyLabel.text = string.IsNullOrWhiteSpace(group.Briefing)
                    ? ComposeBriefing(group)
                    : group.Briefing;

            Show();
        }

        private void OnScenarioFinished(ScenarioFinished message)
        {
            Hide();
        }

        /// <summary>Собирает подсказку из описаний шагов в порядке выполнения.</summary>
        private static string ComposeBriefing(StepGroup group)
        {
            var builder = new StringBuilder();
            var steps = group.Steps;

            for (var i = 0; i < steps.Count; i++)
                builder.Append(i + 1).Append(". ").AppendLine(steps[i].Description);

            return builder.ToString();
        }

        private void Show()
        {
            if (_panel != null)
                _panel.SetActive(true);

            if (_hideRoutine != null)
                StopCoroutine(_hideRoutine);

            if (_displaySeconds > 0f)
                _hideRoutine = StartCoroutine(HideAfterDelay());
        }

        private IEnumerator HideAfterDelay()
        {
            yield return new WaitForSeconds(_displaySeconds);

            _hideRoutine = null;
            Hide();
        }

        private void Hide()
        {
            if (_panel != null)
                _panel.SetActive(false);
        }
    }
}
