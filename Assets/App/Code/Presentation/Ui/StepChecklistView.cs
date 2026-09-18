using System.Text;
using App.Domain.Events;
using App.Domain.Execution;
using App.Presentation.Core;
using TMPro;
using UnityEngine;

namespace App.Presentation.Ui
{
    /// <summary>
    /// Чек-лист текущей группы: список шагов с отметками о выполнении.
    /// Перерисовывается на каждое событие, меняющее состояние прохождения.
    /// </summary>
    public sealed class StepChecklistView : AppBehaviour
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private TMP_Text _titleLabel;
        [SerializeField] private TMP_Text _stepsLabel;

        protected override void OnReady()
        {
            Subscribe<GroupActivated>(message => Render(message.Group));
            Subscribe<StepActivated>(message => Render(CurrentGroup()));
            Subscribe<StepResolved>(message => Render(CurrentGroup()));
            Subscribe<ExpectationFulfilled>(message => Render(CurrentGroup()));
            Subscribe<ScenarioFinished>(message => SetVisible(false));
        }

        private StepGroupProgress CurrentGroup()
        {
            var session = App.Scenario.Session;
            return session?.CurrentGroup;
        }

        private void Render(StepGroupProgress group)
        {
            if (group == null)
            {
                SetVisible(false);
                return;
            }

            SetVisible(true);

            if (_titleLabel != null)
                _titleLabel.text = group.Definition.Title;

            if (_stepsLabel == null)
                return;

            var builder = new StringBuilder();
            var steps = group.Steps;

            for (var i = 0; i < steps.Count; i++)
                builder.AppendLine(StepStatusFormatter.FormatStep(steps[i]));

            _stepsLabel.text = builder.ToString();
        }

        private void SetVisible(bool isVisible)
        {
            if (_panel != null)
                _panel.SetActive(isVisible);
        }
    }
}
