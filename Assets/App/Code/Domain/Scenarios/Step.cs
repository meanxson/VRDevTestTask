using System;
using System.Collections.Generic;
using App.Domain.Actions;

namespace App.Domain.Scenarios
{
    /// <summary>
    /// Описание одного шага сценария — неизменяемые данные, загруженные из конфигурации.
    /// Состояние прохождения хранится отдельно (см. StepProgress), поэтому одно и то же
    /// описание можно переиспользовать при рестарте тренировки.
    /// </summary>
    public sealed class Step
    {
        private readonly ExpectedAction[] _expectedActions;

        /// <summary>Номер шага внутри группы, используется для отображения и отладки.</summary>
        public int Id { get; }

        /// <summary>Текст задания, который видит пользователь.</summary>
        public string Description { get; }

        /// <summary>
        /// Ожидаемые от пользователя действия. Шаг может требовать нескольких
        /// действий сразу (например, подойти в зону и нажать кнопку).
        /// Порядок внутри шага не важен — важно закрыть все ожидания.
        /// </summary>
        public IReadOnlyList<ExpectedAction> ExpectedActions => _expectedActions;

        public Step(int id, string description, IReadOnlyList<ExpectedAction> expectedActions)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Описание шага не может быть пустым.", nameof(description));
            if (expectedActions == null)
                throw new ArgumentNullException(nameof(expectedActions));
            if (expectedActions.Count == 0)
                throw new ArgumentException("Шаг должен ожидать хотя бы одно действие.", nameof(expectedActions));

            Id = id;
            Description = description;

            // Копируем в собственный массив, чтобы вызывающий код не смог
            // изменить состав ожиданий уже после создания шага.
            _expectedActions = new ExpectedAction[expectedActions.Count];
            for (var i = 0; i < expectedActions.Count; i++)
                _expectedActions[i] = expectedActions[i];
        }

        public override string ToString()
        {
            return "Шаг " + Id + ": " + Description;
        }
    }
}
