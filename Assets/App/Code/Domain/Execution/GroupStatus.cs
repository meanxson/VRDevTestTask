namespace App.Domain.Execution
{
    /// <summary>Состояние группы шагов в ходе прохождения сценария.</summary>
    public enum GroupStatus
    {
        /// <summary>Группа ещё не активирована.</summary>
        Pending = 0,

        /// <summary>Текущая группа.</summary>
        Active = 1,

        /// <summary>Все шаги группы отработаны (успешно или с ошибками).</summary>
        Completed = 2,

        /// <summary>Группа закрыта досрочно из-за нарушения последовательности шагов.</summary>
        Interrupted = 3
    }
}
