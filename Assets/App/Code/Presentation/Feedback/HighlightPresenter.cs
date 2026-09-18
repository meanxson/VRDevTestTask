using System.Collections.Generic;
using App.Domain.Events;
using App.Presentation.Core;
using App.Presentation.Interaction;

namespace App.Presentation.Feedback
{
    /// <summary>
    /// Подсвечивает цели, которых ждёт текущий шаг.
    ///
    /// Работает целиком на доменных событиях: шаг активировался — подсветили его
    /// цели, ожидание закрылось — сняли подсветку именно с этой цели, шаг завершился
    /// или сценарий закончился — погасили всё.
    /// </summary>
    public sealed class HighlightPresenter : AppBehaviour
    {
        /// <summary>Идентификаторы целей, подсвеченных прямо сейчас.</summary>
        private readonly List<string> _highlighted = new List<string>();

        protected override void OnReady()
        {
            Subscribe<StepActivated>(OnStepActivated);
            Subscribe<ExpectationFulfilled>(OnExpectationFulfilled);
            Subscribe<StepResolved>(OnStepResolved);
            Subscribe<ScenarioFinished>(OnScenarioFinished);
        }

        protected override void OnDestroy()
        {
            ClearAll();
            base.OnDestroy();
        }

        private void OnStepActivated(StepActivated message)
        {
            ClearAll();

            var expectations = message.Step.GetPendingExpectations();
            for (var i = 0; i < expectations.Count; i++)
                SetHighlighted(expectations[i].TargetId, true);
        }

        private void OnExpectationFulfilled(ExpectationFulfilled message)
        {
            var expectation = message.Step.Definition.ExpectedActions[message.ExpectationIndex];
            SetHighlighted(expectation.TargetId, false);
        }

        private void OnStepResolved(StepResolved message)
        {
            ClearAll();
        }

        private void OnScenarioFinished(ScenarioFinished message)
        {
            ClearAll();
        }

        private void ClearAll()
        {
            for (var i = 0; i < _highlighted.Count; i++)
                Apply(_highlighted[i], false);

            _highlighted.Clear();
        }

        private void SetHighlighted(string targetId, bool isHighlighted)
        {
            if (isHighlighted)
            {
                if (_highlighted.Contains(targetId))
                    return;

                _highlighted.Add(targetId);
            }
            else if (!_highlighted.Remove(targetId))
            {
                return;
            }

            Apply(targetId, isHighlighted);
        }

        private static void Apply(string targetId, bool isHighlighted)
        {
            if (!InteractionTargetRegistry.TryGet(targetId, out var targets))
                return;

            for (var i = 0; i < targets.Count; i++)
            {
                var highlighter = targets[i].Highlighter;

                if (highlighter != null)
                    highlighter.SetHighlighted(isHighlighted);
            }
        }
    }
}
