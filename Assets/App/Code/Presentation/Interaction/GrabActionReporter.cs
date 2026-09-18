using App.Domain.Actions;

namespace App.Presentation.Interaction
{
    /// <summary>
    /// Сообщает сценарию, что объект взяли в руку.
    /// Вешается на объект с XRGrabInteractable.
    /// </summary>
    public sealed class GrabActionReporter : InteractableActionReporter
    {
        protected override ActionType ActionType => ActionType.GrabObject;
    }
}
