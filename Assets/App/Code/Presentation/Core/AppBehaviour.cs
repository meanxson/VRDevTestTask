using System;
using System.Collections.Generic;
using App.Application.Composition;
using UnityEngine;

namespace App.Presentation.Core
{
    /// <summary>
    /// База для компонентов сцены, которым нужны юзкейсы и события приложения.
    ///
    /// Зависимости компонент не ищет — их приносит композиционный корень сразу
    /// после загрузки сцены, до первого Start. Поэтому здесь нет ни обращения
    /// к глобальной точке доступа, ни зависимости от порядка создания объектов.
    ///
    /// Подписки, сделанные через Subscribe, снимаются автоматически при уничтожении
    /// объекта — забыть отписаться нельзя.
    /// </summary>
    public abstract class AppBehaviour : MonoBehaviour, IAppDependent
    {
        private readonly List<IDisposable> _subscriptions = new List<IDisposable>();

        /// <summary>Собранное приложение. Доступно начиная с OnReady.</summary>
        protected AppComposition App { get; private set; }

        public void Construct(AppComposition app)
        {
            App = app;
        }

        protected virtual void Start()
        {
            if (App == null)
            {
                Debug.LogError(
                    "Зависимости не получены: положите объект с AppRoot в сцену.", this);
                enabled = false;
                return;
            }

            OnReady();
        }

        protected virtual void OnDestroy()
        {
            for (var i = 0; i < _subscriptions.Count; i++)
                _subscriptions[i].Dispose();

            _subscriptions.Clear();
        }

        /// <summary>Вызывается один раз, когда зависимости получены.</summary>
        protected abstract void OnReady();

        /// <summary>Подписаться на событие шины на время жизни компонента.</summary>
        protected void Subscribe<TMessage>(Action<TMessage> handler)
        {
            _subscriptions.Add(App.Events.Subscribe(handler));
        }
    }
}
