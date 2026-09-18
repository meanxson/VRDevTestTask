using System;
using System.Collections.Generic;
using App.Domain.Actions;
using App.Domain.Events;
using App.Domain.Scenarios;

namespace App.Domain.Execution
{
    /// <summary>
    /// Одно прохождение сценария: хранит состояние всех групп и шагов и содержит
    /// все правила из технического задания. Это единственное место, где состояние
    /// прохождения меняется.
    ///
    /// Класс намеренно ничего не знает ни о Unity, ни о шине событий: он принимает
    /// действие пользователя и возвращает список произошедших доменных событий.
    /// Рассылкой занимается прикладной слой, а рестарт — это просто новая сессия
    /// на том же неизменяемом описании сценария.
    /// </summary>
    public sealed class ScenarioSession
    {
        private readonly StepGroupProgress[] _groups;

        /// <summary>
        /// Буфер событий текущей операции. Переиспользуется между вызовами,
        /// наружу всегда отдаётся копия, чтобы подписчики не зависели от него.
        /// </summary>
        private readonly List<IScenarioEvent> _events = new List<IScenarioEvent>();

        private int _groupIndex = -1;

        /// <summary>Неизменяемое описание сценария.</summary>
        public Scenario Scenario { get; }

        /// <summary>Состояния групп в порядке прохождения.</summary>
        public IReadOnlyList<StepGroupProgress> Groups => _groups;

        /// <summary>Состояние сессии.</summary>
        public ScenarioState State { get; private set; }

        public ScenarioSession(Scenario scenario)
        {
            Scenario = scenario ?? throw new ArgumentNullException(nameof(scenario));

            var groups = scenario.Groups;
            _groups = new StepGroupProgress[groups.Count];
            for (var i = 0; i < groups.Count; i++)
                _groups[i] = new StepGroupProgress(groups[i]);

            State = ScenarioState.NotStarted;
        }

        /// <summary>Текущая группа или null, если сценарий не запущен либо уже завершён.</summary>
        public StepGroupProgress CurrentGroup
        {
            get
            {
                if (_groupIndex < 0 || _groupIndex >= _groups.Length)
                    return null;

                return _groups[_groupIndex];
            }
        }

        /// <summary>Текущий шаг или null, если ждать от пользователя сейчас нечего.</summary>
        public StepProgress CurrentStep
        {
            get
            {
                var group = CurrentGroup;
                return group?.CurrentStep;
            }
        }

        /// <summary>
        /// Запускает сценарий: активирует первую группу и её первый шаг.
        /// Повторный вызов ничего не делает.
        /// </summary>
        /// <returns>События, произошедшие при запуске.</returns>
        public IReadOnlyList<IScenarioEvent> Start()
        {
            _events.Clear();

            if (State != ScenarioState.NotStarted)
                return Flush();

            State = ScenarioState.Running;
            _events.Add(new ScenarioStarted(this));
            ActivateNextGroup();

            return Flush();
        }

        /// <summary>
        /// Обрабатывает действие пользователя по правилам сценария.
        ///
        /// Порядок разбора:
        /// 1. Действие закрывает ожидание текущего шага — засчитываем;
        /// 2. действие повторяет уже зачтённое в этой группе — игнорируем;
        /// 3. действие ожидается более поздним шагом группы — нарушение порядка,
        ///    группа закрывается целиком;
        /// 4. всё остальное — действие не по цели, текущий шаг закрывается с ошибкой.
        /// </summary>
        /// <returns>События, произошедшие в результате действия.</returns>
        public IReadOnlyList<IScenarioEvent> Apply(UserAction action)
        {
            _events.Clear();

            if (State != ScenarioState.Running)
                return Flush();

            var group = CurrentGroup;
            var step = group.CurrentStep;

            // 1. Ожидаемое действие текущего шага.
            var expectationIndex = step.FindPendingExpectation(action);
            if (expectationIndex >= 0)
            {
                step.MarkExpectationFulfilled(expectationIndex);
                _events.Add(new ExpectationFulfilled(step, expectationIndex));

                if (!step.HasPendingExpectations)
                    ResolveCurrentStep(StepStatus.Succeeded);

                return Flush();
            }

            // 2. Повтор уже зачтённого действия: пользователь не отклонился от сценария,
            //    поэтому нарушение не фиксируем и состояние не трогаем.
            if (group.MatchesAlreadyFulfilledExpectation(action))
                return Flush();

            // 3. Действие из более позднего шага — нарушение последовательности.
            if (group.MatchesExpectationOfLaterStep(action))
            {
                _events.Add(new ViolationRaised(ViolationKind.WrongOrder, action, step));
                InterruptCurrentGroup();
                return Flush();
            }

            // 4. Действие не по ожидаемой цели.
            _events.Add(new ViolationRaised(ViolationKind.WrongTarget, action, step));
            ResolveCurrentStep(StepStatus.Failed);

            return Flush();
        }

        /// <summary>Активирует следующую непустую группу либо завершает сценарий.</summary>
        private void ActivateNextGroup()
        {
            while (true)
            {
                _groupIndex++;

                if (_groupIndex >= _groups.Length)
                {
                    State = ScenarioState.Finished;
                    _events.Add(new ScenarioFinished(this));
                    return;
                }

                var group = _groups[_groupIndex];

                // Группа без шагов ничего не требует от пользователя: закрываем её молча.
                if (group.Steps.Count == 0)
                {
                    group.Status = GroupStatus.Completed;
                    continue;
                }

                group.Status = GroupStatus.Active;
                group.CurrentStepIndex = 0;
                _events.Add(new GroupActivated(group));
                ActivateCurrentStep();
                return;
            }
        }

        private void ActivateCurrentStep()
        {
            var step = CurrentStep;
            step.Status = StepStatus.Active;
            _events.Add(new StepActivated(step));
        }

        /// <summary>Закрывает текущий шаг с указанным статусом и переходит дальше.</summary>
        private void ResolveCurrentStep(StepStatus status)
        {
            var group = CurrentGroup;
            var step = group.CurrentStep;

            step.Status = status;
            _events.Add(new StepResolved(step));

            if (group.HasNextStep)
            {
                group.CurrentStepIndex++;
                ActivateCurrentStep();
                return;
            }

            group.CurrentStepIndex = group.Steps.Count;
            group.Status = GroupStatus.Completed;
            _events.Add(new GroupClosed(group));
            ActivateNextGroup();
        }

        /// <summary>
        /// Нарушена последовательность: закрываем всю группу, всем незавершённым
        /// шагам ставим "пропущен" и переходим к следующей группе.
        /// </summary>
        private void InterruptCurrentGroup()
        {
            var group = CurrentGroup;

            for (var i = group.CurrentStepIndex; i < group.Steps.Count; i++)
            {
                var step = group.Steps[i];
                step.Status = StepStatus.Skipped;
                _events.Add(new StepResolved(step));
            }

            group.CurrentStepIndex = group.Steps.Count;
            group.Status = GroupStatus.Interrupted;
            _events.Add(new GroupClosed(group));
            ActivateNextGroup();
        }

        /// <summary>Отдаёт накопленные события отдельным массивом.</summary>
        private IReadOnlyList<IScenarioEvent> Flush()
        {
            return _events.ToArray();
        }
    }
}
