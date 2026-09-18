using System;

namespace App.Application.Messages
{
    /// <summary>
    /// Сценарий не удалось загрузить. Публикуется вместо падения сцены,
    /// чтобы интерфейс мог показать причину, а не пустую тренировку.
    /// </summary>
    public sealed class ScenarioLoadFailed
    {
        /// <summary>Причина, пригодная для показа пользователю.</summary>
        public string Reason { get; }

        public ScenarioLoadFailed(string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Причина не может быть пустой.", nameof(reason));

            Reason = reason;
        }
    }
}
