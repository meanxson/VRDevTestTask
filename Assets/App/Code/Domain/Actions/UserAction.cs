using System;

namespace App.Domain.Actions
{
    /// <summary>
    /// Факт действия пользователя: что он сделал и над каким объектом.
    /// Слой представления превращает любое VR-взаимодействие в такую структуру,
    /// поэтому домен ничего не знает ни о коллайдерах, ни о лучах, ни о UI.
    /// </summary>
    public readonly struct UserAction
    {
        /// <summary>Тип совершённого действия.</summary>
        public ActionType Type { get; }

        /// <summary>
        /// Идентификатор цели: зоны, объекта или кнопки.
        /// Совпадает с полем "target" в описании шага сценария.
        /// </summary>
        public string TargetId { get; }

        public UserAction(ActionType type, string targetId)
        {
            if (string.IsNullOrWhiteSpace(targetId))
                throw new ArgumentException("Идентификатор цели не может быть пустым.", nameof(targetId));

            Type = type;
            TargetId = targetId;
        }

        public override string ToString()
        {
            return Type + " -> " + TargetId;
        }
    }
}
