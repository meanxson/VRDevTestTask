using App.Domain.Actions;

namespace App.Presentation.Interaction
{
    /// <summary>
    /// Сообщает сценарию, что по объекту кликнули лучом-поинтером.
    /// Вешается на объект с XRSimpleInteractable.
    ///
    /// Дополнительно принимает обычный клик мышью: по заданию интерфейсом можно
    /// управлять и контроллерами, и мышью, и на объекты сцены это распространяется
    /// тоже — иначе проект нельзя проверить без гарнитуры.
    /// </summary>
    public sealed class ClickActionReporter : InteractableActionReporter
    {
        protected override ActionType ActionType => ActionType.ClickObject;

        private void OnMouseDown()
        {
            Report();
        }
    }
}
