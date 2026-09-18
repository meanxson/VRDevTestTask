using System;

namespace App.Common.Events
{
    /// <summary>Отправляет сообщение подписчикам.</summary>
    public interface IEventPublisher
    {
        /// <summary>
        /// Разослать сообщение. Получатели определяются по фактическому типу
        /// сообщения, его базовым классам и интерфейсам.
        /// </summary>
        void Publish(object message);
    }

    /// <summary>Подписывает обработчики на сообщения нужного типа.</summary>
    public interface IEventSubscriber
    {
        /// <summary>
        /// Подписаться на сообщения типа <typeparamref name="TMessage"/>.
        /// Возвращённый объект отписывает обработчик при Dispose.
        /// </summary>
        IDisposable Subscribe<TMessage>(Action<TMessage> handler);
    }

    /// <summary>
    /// Шина событий. Интерфейсы разделены намеренно: издателю не нужна подписка,
    /// подписчику не нужна рассылка, и каждый класс зависит только от того,
    /// чем реально пользуется.
    /// </summary>
    public interface IEventBus : IEventPublisher, IEventSubscriber
    {
    }
}
