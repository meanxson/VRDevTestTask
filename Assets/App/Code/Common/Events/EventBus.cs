using System;
using System.Collections.Generic;

namespace App.Common.Events
{
    /// <summary>
    /// Простая типизированная шина событий — собственная реализация вместо готовых
    /// решений, как того требует техническое задание.
    ///
    /// Рассылка идёт по фактическому типу сообщения, его базовым классам и
    /// интерфейсам, поэтому подписчик может слушать как конкретное событие
    /// (StepResolved), так и всё семейство сразу (IScenarioEvent) — это удобно
    /// для отладочного логгера.
    ///
    /// Шина не потокобезопасна: в Unity все события приходят из главного потока.
    /// </summary>
    public sealed class EventBus : IEventBus
    {
        /// <summary>Обработчики по типу сообщения, на который они подписаны.</summary>
        private readonly Dictionary<Type, List<Handler>> _handlers = new Dictionary<Type, List<Handler>>();

        /// <summary>
        /// Кэш цепочки типов для рассылки: у каждого типа сообщения она постоянна,
        /// поэтому рефлексия выполняется один раз на тип.
        /// </summary>
        private readonly Dictionary<Type, Type[]> _dispatchTypes = new Dictionary<Type, Type[]>();

        public IDisposable Subscribe<TMessage>(Action<TMessage> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var messageType = typeof(TMessage);

            if (!_handlers.TryGetValue(messageType, out var handlers))
            {
                handlers = new List<Handler>();
                _handlers.Add(messageType, handlers);
            }

            // Приведение типа прячем в замыкание: на рассылке остаётся обычный
            // вызов делегата, без DynamicInvoke и его накладных расходов.
            handlers.Add(new Handler(handler, message => handler((TMessage)message)));

            return new Subscription(this, messageType, handler);
        }

        public void Publish(object message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var dispatchTypes = GetDispatchTypes(message.GetType());

            for (var i = 0; i < dispatchTypes.Length; i++)
            {
                if (!_handlers.TryGetValue(dispatchTypes[i], out var handlers) || handlers.Count == 0)
                    continue;

                // Копия на время рассылки: обработчик имеет право подписаться
                // или отписаться прямо во время обработки события.
                var snapshot = handlers.ToArray();
                for (var j = 0; j < snapshot.Length; j++)
                    snapshot[j].Invoke(message);
            }
        }

        /// <summary>Снимает все подписки. Вызывается при завершении работы приложения.</summary>
        public void Clear()
        {
            _handlers.Clear();
        }

        private void Unsubscribe(Type messageType, Delegate handler)
        {
            if (!_handlers.TryGetValue(messageType, out var handlers))
                return;

            for (var i = 0; i < handlers.Count; i++)
            {
                if (!handlers[i].Matches(handler))
                    continue;

                handlers.RemoveAt(i);
                return;
            }
        }

        /// <summary>
        /// Типы, по которым рассылается сообщение: сам тип, все его базовые классы
        /// (включая object — подписка на object получает вообще все сообщения)
        /// и все реализованные интерфейсы.
        /// </summary>
        private Type[] GetDispatchTypes(Type messageType)
        {
            if (_dispatchTypes.TryGetValue(messageType, out var cached))
                return cached;

            var types = new List<Type>();

            for (var type = messageType; type != null; type = type.BaseType)
                types.Add(type);

            var interfaces = messageType.GetInterfaces();
            for (var i = 0; i < interfaces.Length; i++)
                types.Add(interfaces[i]);

            var result = types.ToArray();
            _dispatchTypes.Add(messageType, result);

            return result;
        }

        /// <summary>Подписанный обработчик: исходный делегат плюс типизированная обёртка.</summary>
        private readonly struct Handler
        {
            private readonly Delegate _original;
            private readonly Action<object> _invoke;

            public Handler(Delegate original, Action<object> invoke)
            {
                _original = original;
                _invoke = invoke;
            }

            public bool Matches(Delegate handler)
            {
                return _original.Equals(handler);
            }

            public void Invoke(object message)
            {
                _invoke(message);
            }
        }

        /// <summary>
        /// Дескриптор подписки. Отписка через Dispose позволяет подписчику
        /// не помнить ни тип сообщения, ни сам метод-обработчик.
        /// </summary>
        private sealed class Subscription : IDisposable
        {
            private readonly EventBus _bus;
            private readonly Type _messageType;
            private readonly Delegate _handler;

            private bool _disposed;

            public Subscription(EventBus bus, Type messageType, Delegate handler)
            {
                _bus = bus;
                _messageType = messageType;
                _handler = handler;
            }

            public void Dispose()
            {
                if (_disposed)
                    return;

                _disposed = true;
                _bus.Unsubscribe(_messageType, _handler);
            }
        }
    }
}
