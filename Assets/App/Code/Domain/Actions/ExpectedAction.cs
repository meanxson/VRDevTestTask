using System;

namespace App.Domain.Actions
{
    /// <summary>
    /// Действие, которого шаг ожидает от пользователя.
    /// Шаг считается выполненным, когда закрыты все его ожидаемые действия.
    /// </summary>
    public readonly struct ExpectedAction
    {
        /// <summary>Тип ожидаемого действия.</summary>
        public ActionType Type { get; }

        /// <summary>Идентификатор ожидаемой цели.</summary>
        public string TargetId { get; }

        public ExpectedAction(ActionType type, string targetId)
        {
            if (string.IsNullOrWhiteSpace(targetId))
                throw new ArgumentException("Идентификатор цели не может быть пустым.", nameof(targetId));

            Type = type;
            TargetId = targetId;
        }

        /// <summary>
        /// Совпадает ли действие пользователя с ожидаемым.
        /// Идентификаторы сравниваются без учёта регистра: их задаёт дизайнер
        /// сценария в JSON, и расхождение в регистре не должно ломать прохождение.
        /// </summary>
        public bool Matches(UserAction action)
        {
            return action.Type == Type
                   && string.Equals(action.TargetId, TargetId, StringComparison.OrdinalIgnoreCase);
        }

        public override string ToString()
        {
            return Type + " -> " + TargetId;
        }
    }
}
