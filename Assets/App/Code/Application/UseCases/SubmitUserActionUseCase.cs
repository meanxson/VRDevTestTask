using System;
using App.Application.Services;
using App.Domain.Actions;

namespace App.Application.UseCases
{
    /// <summary>
    /// Передать действие пользователя сценарию.
    ///
    /// Единственная точка входа для всех детекторов в сцене: зон, хватаемых
    /// объектов, кликабельных объектов и кнопок интерфейса. Что означает
    /// конкретное действие, решает домен.
    /// </summary>
    public sealed class SubmitUserActionUseCase
    {
        private readonly ScenarioContext _context;
        private readonly ScenarioEventDispatcher _dispatcher;

        public SubmitUserActionUseCase(ScenarioContext context, ScenarioEventDispatcher dispatcher)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        }

        public void Execute(UserAction action)
        {
            var session = _context.Session;

            // Действия до запуска и после завершения сценария домен игнорирует сам,
            // здесь остаётся защититься только от отсутствующей сессии.
            if (session == null)
                return;

            _dispatcher.Dispatch(session.Apply(action));
        }
    }
}
