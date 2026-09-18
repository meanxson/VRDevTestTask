using System;

namespace App.Application.Composition
{
    /// <summary>
    /// Точка доступа к собранному приложению для компонентов сцены.
    ///
    /// Это осознанный компромисс: DI-контейнеры запрещены заданием, а объекты
    /// в сцене создаёт Unity, и передать им зависимости через конструктор нельзя.
    /// Поэтому композиционный корень один раз публикует сборку здесь, а компоненты
    /// сцены её забирают. Это единственное глобальное состояние в проекте, и живёт
    /// оно в прикладном слое — чтобы представление не зависело от инфраструктуры.
    /// </summary>
    public static class AppServices
    {
        /// <summary>Собранное приложение или null, если корень ещё не отработал.</summary>
        public static AppComposition Current { get; private set; }

        /// <summary>Готово ли приложение к работе.</summary>
        public static bool IsInstalled => Current != null;

        /// <summary>Опубликовать сборку. Вызывает только композиционный корень.</summary>
        public static void Install(AppComposition composition)
        {
            Current = composition ?? throw new ArgumentNullException(nameof(composition));
        }

        /// <summary>Снять сборку при завершении работы приложения.</summary>
        public static void Uninstall()
        {
            Current = null;
        }
    }
}
