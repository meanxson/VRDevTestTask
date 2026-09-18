using System;
using System.Collections.Generic;
using App.Domain.Actions;
using App.Domain.Scenarios;

namespace App.Domain.Execution
{
    /// <summary>
    /// Состояние прохождения одного шага: его статус и то, какие из ожидаемых
    /// действий уже засчитаны. Мутирует только домен (ScenarioSession),
    /// наружу отдаются read-only свойства.
    /// </summary>
    public sealed class StepProgress
    {
        private readonly bool[] _fulfilled;

        /// <summary>Неизменяемое описание шага.</summary>
        public Step Definition { get; }

        /// <summary>Текущий статус шага.</summary>
        public StepStatus Status { get; internal set; }

        internal StepProgress(Step definition)
        {
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
            _fulfilled = new bool[definition.ExpectedActions.Count];
            Status = StepStatus.Pending;
        }

        /// <summary>Остались ли ещё незакрытые ожидаемые действия.</summary>
        public bool HasPendingExpectations
        {
            get
            {
                for (var i = 0; i < _fulfilled.Length; i++)
                {
                    if (!_fulfilled[i])
                        return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Засчитано ли ожидаемое действие с указанным индексом.
        /// Нужно представлению, чтобы гасить подсветку уже отработанных целей.
        /// </summary>
        public bool IsExpectationFulfilled(int expectationIndex)
        {
            return _fulfilled[expectationIndex];
        }

        /// <summary>Незакрытые ожидания шага — цели, которые сейчас надо подсветить.</summary>
        public IReadOnlyList<ExpectedAction> GetPendingExpectations()
        {
            var pending = new List<ExpectedAction>(_fulfilled.Length);
            var expectations = Definition.ExpectedActions;

            for (var i = 0; i < expectations.Count; i++)
            {
                if (!_fulfilled[i])
                    pending.Add(expectations[i]);
            }

            return pending;
        }

        /// <summary>
        /// Индекс незакрытого ожидания, которому соответствует действие пользователя,
        /// либо -1, если такого нет.
        /// </summary>
        internal int FindPendingExpectation(UserAction action)
        {
            var expectations = Definition.ExpectedActions;

            for (var i = 0; i < expectations.Count; i++)
            {
                if (!_fulfilled[i] && expectations[i].Matches(action))
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Повторяет ли действие уже засчитанное ожидание этого шага.
        /// Такое действие не является нарушением — пользователь просто повторил то,
        /// что ему уже зачли (например, снова взял в руки тот же предмет).
        /// </summary>
        internal bool MatchesFulfilledExpectation(UserAction action)
        {
            var expectations = Definition.ExpectedActions;

            for (var i = 0; i < expectations.Count; i++)
            {
                if (_fulfilled[i] && expectations[i].Matches(action))
                    return true;
            }

            return false;
        }

        /// <summary>Есть ли у шага незакрытое ожидание под это действие.</summary>
        internal bool MatchesPendingExpectation(UserAction action)
        {
            return FindPendingExpectation(action) >= 0;
        }

        internal void MarkExpectationFulfilled(int expectationIndex)
        {
            _fulfilled[expectationIndex] = true;
        }
    }
}
