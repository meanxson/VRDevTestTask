using App.Presentation.Core;

namespace App.Presentation.Bootstrap
{
    /// <summary>
    /// Запускает прохождение при загрузке сцены тренировки.
    ///
    /// Отдельный компонент нужен потому, что сцена может открываться и из лобби,
    /// и напрямую из редактора, и после перезапуска по кнопке "Попытаться ещё" —
    /// во всех случаях попытка должна начинаться одинаково.
    /// </summary>
    public sealed class TrainingSceneBootstrap : AppBehaviour
    {
        protected override void OnReady()
        {
            App.StartScenario.Execute();
        }
    }
}
