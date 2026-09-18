using System;
using System.Collections.Generic;
using App.Common.Events;
using App.Domain.Events;

namespace App.Application.Services
{
    /// <summary>
    /// Рассылает доменные события по шине.
    ///
    /// Домен возвращает события списком и сам никуда их не отправляет — рассылка
    /// вынесена сюда, чтобы юзкейсы не повторяли один и тот же цикл, а домен
    /// оставался проверяемым без шины.
    /// </summary>
    public sealed class ScenarioEventDispatcher
    {
        private readonly IEventPublisher _publisher;

        public ScenarioEventDispatcher(IEventPublisher publisher)
        {
            _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
        }

        /// <summary>
        /// Разослать события. Тип каждого определяется по факту, поэтому подписчики
        /// получают их конкретными типами, а не общим интерфейсом.
        /// </summary>
        public void Dispatch(IReadOnlyList<IScenarioEvent> events)
        {
            if (events == null)
                throw new ArgumentNullException(nameof(events));

            for (var i = 0; i < events.Count; i++)
                _publisher.Publish(events[i]);
        }
    }
}
