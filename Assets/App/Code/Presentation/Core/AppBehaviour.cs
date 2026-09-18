using System;
using System.Collections.Generic;
using App.Application.Composition;
using UnityEngine;

namespace App.Presentation.Core
{
    /// <summary>
    /// База для компонентов сцены, которым нужны юзкейсы и события приложения.
    ///
    /// Берёт сборку в Start, а не в Awake: к этому моменту композиционный корень
    /// точно отработал, в каком бы порядке Unity ни создавала объекты.
    /// Подписки, сделанные через Subscribe, снимаются автоматически при уничтожении
    /// объекта — забыть отписаться нельзя.
    /// </summary>
    public abstract class AppBehaviour : MonoBehaviour
    {
        private readonly List<IDisposable> _subscriptions = new List<IDisposable>();

        /// <summary>Собранное приложение. Доступно начиная с OnReady.</summary>
        protected AppComposition App { get; private set; }

        protected virtual void Start()
        {
            App = AppServices.Current;

            if (App == null)
            {
                Debug.LogError(
                    "Не найден композиционный корень: положите объект с AppRoot в сцену.", this);
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

        /// <summary>Вызывается один раз, когда приложение доступно.</summary>
        protected abstract void OnReady();

        /// <summary>Подписаться на событие шины на время жизни компонента.</summary>
        protected void Subscribe<TMessage>(Action<TMessage> handler)
        {
            _subscriptions.Add(App.Events.Subscribe(handler));
        }
    }
}
