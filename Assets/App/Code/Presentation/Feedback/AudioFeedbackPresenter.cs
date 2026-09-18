using App.Domain.Events;
using App.Presentation.Core;
using UnityEngine;

namespace App.Presentation.Feedback
{
    /// <summary>
    /// Звуковые сигналы прохождения: один на правильное действие, другой на нарушение.
    ///
    /// Сигнал привязан к ExpectationFulfilled, а не к завершению шага, чтобы
    /// в шаге из нескольких действий пользователь слышал отклик на каждое.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public sealed class AudioFeedbackPresenter : AppBehaviour
    {
        [Tooltip("Сигнал правильного выполнения.")]
        [SerializeField] private AudioClip _successClip;

        [Tooltip("Сигнал нарушения.")]
        [SerializeField] private AudioClip _violationClip;

        private AudioSource _source;

        protected override void OnReady()
        {
            _source = GetComponent<AudioSource>();
            _source.playOnAwake = false;

            Subscribe<ExpectationFulfilled>(OnExpectationFulfilled);
            Subscribe<ViolationRaised>(OnViolationRaised);
        }

        private void OnExpectationFulfilled(ExpectationFulfilled message)
        {
            Play(_successClip);
        }

        private void OnViolationRaised(ViolationRaised message)
        {
            Play(_violationClip);
        }

        private void Play(AudioClip clip)
        {
            if (clip == null || _source == null)
                return;

            _source.PlayOneShot(clip);
        }
    }
}
