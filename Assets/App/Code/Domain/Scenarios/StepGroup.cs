using System;
using System.Collections.Generic;

namespace App.Domain.Scenarios
{
    /// <summary>
    /// Группа шагов — блок связанных по смыслу шагов ("проверка документов" и т.п.).
    /// Шаги внутри группы выполняются строго по порядку: выполнение более позднего
    /// шага раньше текущего считается нарушением последовательности.
    /// </summary>
    public sealed class StepGroup
    {
        private readonly Step[] _steps;

        /// <summary>Название группы, показывается пользователю и в итогах.</summary>
        public string Title { get; }

        /// <summary>
        /// Информационное сообщение о порядке ожидаемых действий, которое выводится
        /// при активации группы. Если не задано — слой представления соберёт текст
        /// из описаний шагов.
        /// </summary>
        public string Briefing { get; }

        /// <summary>Шаги группы в порядке выполнения.</summary>
        public IReadOnlyList<Step> Steps => _steps;

        public StepGroup(string title, IReadOnlyList<Step> steps, string briefing = null)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Название группы не может быть пустым.", nameof(title));
            if (steps == null)
                throw new ArgumentNullException(nameof(steps));

            Title = title;
            Briefing = briefing;

            _steps = new Step[steps.Count];
            for (var i = 0; i < steps.Count; i++)
            {
                var step = steps[i];
                if (step == null)
                    throw new ArgumentException("Группа содержит пустой шаг.", nameof(steps));

                _steps[i] = step;
            }
        }

        public override string ToString()
        {
            return Title + " (" + _steps.Length + " шагов)";
        }
    }
}
