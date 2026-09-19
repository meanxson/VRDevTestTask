using UnityEngine;

namespace App.Presentation.Feedback
{
    /// <summary>
    /// Подсветка напольной зоны маркером "встаньте сюда".
    ///
    /// Маркер виден всегда, но приглушённо: игрок понимает, где вообще есть зоны,
    /// и при этом активная зовёт его заметно ярче. Переход плавный — резкое
    /// переключение яркости в VR читается как мигание и раздражает.
    ///
    /// Яркость задаётся через MaterialPropertyBlock, поэтому все зоны делят один
    /// материал: копий материала не создаётся и чистить за собой нечего.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Renderer))]
    public sealed class ZoneMarkerHighlighter : TargetHighlighter
    {
        private static readonly int IntensityId = Shader.PropertyToID("_Intensity");

        [Tooltip("Яркость, когда шаг не активен.")]
        [Range(0f, 1f)]
        [SerializeField] private float _idleIntensity = 0.18f;

        [Tooltip("Яркость активной зоны.")]
        [Range(0f, 1f)]
        [SerializeField] private float _activeIntensity = 1f;

        [Tooltip("Скорость перехода между яркостями.")]
        [SerializeField] private float _transitionSpeed = 5f;

        private Renderer _targetRenderer;
        private MaterialPropertyBlock _propertyBlock;

        private float _currentIntensity;
        private float _desiredIntensity;

        private void Awake()
        {
            _targetRenderer = GetComponent<Renderer>();
            _propertyBlock = new MaterialPropertyBlock();

            _currentIntensity = _idleIntensity;
            _desiredIntensity = _idleIntensity;
            ApplyIntensity();
        }

        public override void SetHighlighted(bool isHighlighted)
        {
            _desiredIntensity = isHighlighted ? _activeIntensity : _idleIntensity;
        }

        private void Update()
        {
            if (Mathf.Approximately(_currentIntensity, _desiredIntensity))
                return;

            _currentIntensity = Mathf.MoveTowards(
                _currentIntensity, _desiredIntensity, _transitionSpeed * Time.deltaTime);

            ApplyIntensity();
        }

        private void ApplyIntensity()
        {
            _targetRenderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetFloat(IntensityId, _currentIntensity);
            _targetRenderer.SetPropertyBlock(_propertyBlock);
        }
    }
}
