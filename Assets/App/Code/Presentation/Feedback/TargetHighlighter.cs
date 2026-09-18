using System.Collections.Generic;
using UnityEngine;

namespace App.Presentation.Feedback
{
    /// <summary>
    /// Подсветка цели контуром. Материал обводки добавляется к рендерерам объекта
    /// на время подсветки и убирается после — исходные материалы не трогаются.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class TargetHighlighter : MonoBehaviour
    {
        /// <summary>Имя шейдера в папке Resources, чтобы он гарантированно попал в сборку.</summary>
        private const string OutlineShaderResource = "AppOutline";

        [SerializeField] private Color _color = new Color(1f, 0.75f, 0.1f, 1f);

        [Range(0.001f, 0.1f)]
        [SerializeField] private float _width = 0.02f;

        [Tooltip("Рендереры для обводки. Пусто — будут взяты все рендереры объекта и его детей.")]
        [SerializeField] private Renderer[] _renderers;

        private Material _outlineMaterial;
        private bool _isHighlighted;

        private void Awake()
        {
            if (_renderers == null || _renderers.Length == 0)
                _renderers = GetComponentsInChildren<Renderer>();
        }

        private void OnDestroy()
        {
            if (_outlineMaterial != null)
                Destroy(_outlineMaterial);
        }

        /// <summary>Включить или выключить подсветку.</summary>
        public void SetHighlighted(bool isHighlighted)
        {
            if (_isHighlighted == isHighlighted)
                return;

            _isHighlighted = isHighlighted;

            if (isHighlighted)
                AddOutline();
            else
                RemoveOutline();
        }

        private void AddOutline()
        {
            var material = GetOrCreateMaterial();
            if (material == null)
                return;

            foreach (var target in _renderers)
            {
                if (target == null)
                    continue;

                var materials = new List<Material>(target.sharedMaterials);
                if (materials.Contains(material))
                    continue;

                materials.Add(material);
                target.sharedMaterials = materials.ToArray();
            }
        }

        private void RemoveOutline()
        {
            if (_outlineMaterial == null)
                return;

            foreach (var target in _renderers)
            {
                if (target == null)
                    continue;

                var materials = new List<Material>(target.sharedMaterials);
                if (!materials.Remove(_outlineMaterial))
                    continue;

                target.sharedMaterials = materials.ToArray();
            }
        }

        private Material GetOrCreateMaterial()
        {
            if (_outlineMaterial != null)
                return _outlineMaterial;

            var shader = Resources.Load<Shader>(OutlineShaderResource);

            if (shader == null)
            {
                Debug.LogError("Не найден шейдер обводки в Resources/" + OutlineShaderResource + ".", this);
                return null;
            }

            _outlineMaterial = new Material(shader);
            _outlineMaterial.SetColor("_OutlineColor", _color);
            _outlineMaterial.SetFloat("_OutlineWidth", _width);

            return _outlineMaterial;
        }
    }
}
