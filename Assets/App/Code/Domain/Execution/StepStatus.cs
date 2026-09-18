namespace App.Domain.Execution
{
    /// <summary>Состояние шага в ходе прохождения сценария.</summary>
    public enum StepStatus
    {
        /// <summary>Шаг ещё не активирован.</summary>
        Pending = 0,

        /// <summary>Текущий шаг, система ждёт действий пользователя.</summary>
        Active = 1,

        /// <summary>Шаг выполнен надлежащим образом.</summary>
        Succeeded = 2,

        /// <summary>Шаг выполнен с ошибкой (было нарушение).</summary>
        Failed = 3,

        /// <summary>Шаг пропущен: группу закрыли досрочно из-за нарушения последовательности.</summary>
        Skipped = 4
    }
}
