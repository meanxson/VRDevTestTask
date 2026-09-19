using UnityEngine;

namespace App.Infrastructure.Composition
{
    /// <summary>
    /// Точка входа приложения: создаёт композиционный корень до загрузки первой сцены.
    ///
    /// Благодаря этому корень не лежит ни в одной сцене — значит, его физически
    /// не может оказаться два, и ему не нужна ни статическая ссылка на себя,
    /// ни защита от дублей. Сцены при этом остаются самодостаточными: любую можно
    /// открыть в редакторе и запустить, корень появится сам.
    ///
    /// Статический метод здесь — не глобальное состояние, а единственный
    /// предоставленный Unity способ выполнить код до загрузки сцен.
    /// </summary>
    public static class AppBootstrapper
    {
        /// <summary>Префаб корня в папке Resources, чтобы он гарантированно попал в сборку.</summary>
        private const string AppRootResource = "AppRoot";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void CreateAppRoot()
        {
            var prefab = Resources.Load<AppRoot>(AppRootResource);

            if (prefab == null)
            {
                Debug.LogError("Не найден префаб корня приложения в Resources/" + AppRootResource + ".");
                return;
            }

            var root = Object.Instantiate(prefab);
            root.name = prefab.name;
            Object.DontDestroyOnLoad(root.gameObject);
        }
    }
}
