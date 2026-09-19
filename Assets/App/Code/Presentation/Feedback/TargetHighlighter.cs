using UnityEngine;

namespace App.Presentation.Feedback
{
    /// <summary>
    /// Подсветка цели сценария.
    ///
    /// Способов подсветить цель больше одного: объёмному предмету идёт контур,
    /// а плоской напольной зоне — анимированный маркер, потому что контур
    /// плоского диска сверху не виден. Презентеру подсветки эта разница не важна:
    /// он знает только "включить" и "выключить", а как именно — решает конкретная
    /// реализация на самом объекте.
    /// </summary>
    public abstract class TargetHighlighter : MonoBehaviour
    {
        /// <summary>Включить или выключить подсветку.</summary>
        public abstract void SetHighlighted(bool isHighlighted);
    }
}
