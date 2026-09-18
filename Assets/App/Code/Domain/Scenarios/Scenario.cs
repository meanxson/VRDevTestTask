using System;
using System.Collections.Generic;

namespace App.Domain.Scenarios
{
    /// <summary>
    /// Сценарий тренировки целиком: упорядоченный набор групп шагов.
    /// Это неизменяемое описание, его загружает инфраструктура из конфигурации.
    /// </summary>
    public sealed class Scenario
    {
        private readonly StepGroup[] _groups;

        /// <summary>Название сценария.</summary>
        public string Title { get; }

        /// <summary>Группы шагов в порядке прохождения.</summary>
        public IReadOnlyList<StepGroup> Groups => _groups;

        public Scenario(string title, IReadOnlyList<StepGroup> groups)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Название сценария не может быть пустым.", nameof(title));
            if (groups == null)
                throw new ArgumentNullException(nameof(groups));
            if (groups.Count == 0)
                throw new ArgumentException("Сценарий должен содержать хотя бы одну группу.", nameof(groups));

            Title = title;

            _groups = new StepGroup[groups.Count];
            for (var i = 0; i < groups.Count; i++)
            {
                var group = groups[i];
                if (group == null)
                    throw new ArgumentException("Сценарий содержит пустую группу.", nameof(groups));

                _groups[i] = group;
            }
        }

        public override string ToString()
        {
            return Title + " (" + _groups.Length + " групп)";
        }
    }
}
