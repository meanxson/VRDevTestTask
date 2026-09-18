namespace App.Domain.Actions
{
    /// <summary>
    /// Тип действия, которое пользователь может совершить в сцене тренировки.
    /// Перечень закрыт: он повторяет список из технического задания.
    /// </summary>
    public enum ActionType
    {
        /// <summary>Дойти до точки интереса (подсвеченной зоны).</summary>
        ReachPoint = 0,

        /// <summary>Взять объект рукой (граб).</summary>
        GrabObject = 1,

        /// <summary>Кликнуть по объекту лучом-поинтером.</summary>
        ClickObject = 2,

        /// <summary>Нажать кнопку интерфейса.</summary>
        PressUiButton = 3
    }
}
