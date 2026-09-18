namespace App.Domain.Execution
{
    /// <summary>Состояние сессии прохождения сценария.</summary>
    public enum ScenarioState
    {
        /// <summary>Сессия создана, но ещё не запущена.</summary>
        NotStarted = 0,

        /// <summary>Сценарий выполняется, действия пользователя принимаются.</summary>
        Running = 1,

        /// <summary>Все группы отработаны, можно подводить итоги.</summary>
        Finished = 2
    }
}
