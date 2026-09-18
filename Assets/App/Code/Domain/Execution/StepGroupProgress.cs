using System;
using System.Collections.Generic;
using App.Domain.Actions;
using App.Domain.Scenarios;

namespace App.Domain.Execution
{
    /// <summary>
    /// Состояние прохождения группы шагов: статус группы, её шаги и указатель
    /// на текущий шаг. Инкапсулирует поиск по шагам, который нужен домену,
    /// чтобы отличить нарушение последовательности от действия не по цели.
    /// </summary>
    public sealed class StepGroupProgress
    {
        private readonly StepProgress[] _steps;

        /// <summary>Неизменяемое описание группы.</summary>
        public StepGroup Definition { get; }

        /// <summary>Текущий статус группы.</summary>
        public GroupStatus Status { get; internal set; }

        /// <summary>Индекс текущего шага внутри группы.</summary>
        public int CurrentStepIndex { get; internal set; }

        /// <summary>Состояния шагов группы в порядке выполнения.</summary>
        public IReadOnlyList<StepProgress> Steps => _steps;

        internal StepGroupProgress(StepGroup definition)
        {
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));

            var steps = definition.Steps;
            _steps = new StepProgress[steps.Count];
            for (var i = 0; i < steps.Count; i++)
                _steps[i] = new StepProgress(steps[i]);

            Status = GroupStatus.Pending;
            CurrentStepIndex = 0;
        }

        /// <summary>Текущий шаг группы или null, если группа уже отработана.</summary>
        public StepProgress CurrentStep
        {
            get
            {
                if (CurrentStepIndex < 0 || CurrentStepIndex >= _steps.Length)
                    return null;

                return _steps[CurrentStepIndex];
            }
        }

        /// <summary>Есть ли ещё шаги после текущего.</summary>
        internal bool HasNextStep => CurrentStepIndex + 1 < _steps.Length;

        /// <summary>
        /// Повторяет ли действие что-то, что уже было засчитано в этой группе.
        /// Проверяются все шаги вплоть до текущего включительно.
        /// </summary>
        internal bool MatchesAlreadyFulfilledExpectation(UserAction action)
        {
            var lastIndex = Math.Min(CurrentStepIndex, _steps.Length - 1);

            for (var i = 0; i <= lastIndex; i++)
            {
                if (_steps[i].MatchesFulfilledExpectation(action))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Ожидает ли это действие какой-нибудь из ещё не начатых шагов группы.
        /// Если да — пользователь полез вперёд, это нарушение последовательности.
        /// </summary>
        internal bool MatchesExpectationOfLaterStep(UserAction action)
        {
            for (var i = CurrentStepIndex + 1; i < _steps.Length; i++)
            {
                if (_steps[i].MatchesPendingExpectation(action))
                    return true;
            }

            return false;
        }
    }
}
