using UnityEngine;

namespace App.Presentation.Ui
{
    /// <summary>
    /// Мягкое сопровождение панели за взглядом игрока.
    ///
    /// Панель, жёстко приколоченная к камере, в VR читается как грязь на стекле:
    /// она не даёт осмотреться и быстро утомляет. Панель, намертво стоящая в мире,
    /// теряется из виду, как только игрок повернулся. Здесь компромисс, который
    /// обычно и применяют в VR: пока панель в пределах комфортного угла обзора,
    /// она не двигается вовсе; стоит игроку отвернуться дальше порога — она
    /// плавно догоняет и снова замирает.
    ///
    /// Заодно компонент всегда доворачивает панель лицом к игроку. Мировой канвас
    /// читается со стороны, противоположной его forward, поэтому forward смотрит
    /// от игрока — руками такую ориентацию легко перепутать.
    ///
    /// Объект с этим компонентом должен лежать в корне сцены, а не внутри рига:
    /// иначе перемещение игрока будет сдвигать панель мгновенно, без сглаживания.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ComfortFollowPanel : MonoBehaviour
    {
        [Tooltip("Камера игрока. Пусто — будет взята основная камера сцены.")]
        [SerializeField] private Transform _viewer;

        [Tooltip("Расстояние от игрока до панели, метры.")]
        [SerializeField] private float _distance = 1.2f;

        [Tooltip("Смещение по высоте относительно уровня глаз, метры.")]
        [SerializeField] private float _heightOffset = -0.25f;

        [Tooltip("Угол, за которым панель начинает догонять взгляд, градусы.")]
        [Range(5f, 80f)]
        [SerializeField] private float _followThreshold = 28f;

        [Tooltip("Насколько панель может отстать по расстоянию, прежде чем догонять, метры.")]
        [SerializeField] private float _maxDrift = 0.6f;

        [Tooltip("Насколько близко к цели панель считает себя догнавшей, метры.")]
        [SerializeField] private float _settleDistance = 0.05f;

        [Tooltip("Скорость сглаживания перемещения.")]
        [SerializeField] private float _positionDamping = 3.5f;

        [Tooltip("Скорость сглаживания поворота.")]
        [SerializeField] private float _rotationDamping = 6f;

        /// <summary>Догоняет ли панель взгляд прямо сейчас.</summary>
        private bool _isCatchingUp;

        private void Start()
        {
            if (_viewer == null && Camera.main != null)
                _viewer = Camera.main.transform;

            if (_viewer == null)
            {
                Debug.LogError("Не найдена камера игрока для панели сопровождения.", this);
                enabled = false;
                return;
            }

            // На старте ставим панель сразу в нужное место, без наплыва из точки сцены.
            transform.SetPositionAndRotation(GetDesiredPosition(), GetDesiredRotation(GetDesiredPosition()));
        }

        private void LateUpdate()
        {
            var desiredPosition = GetDesiredPosition();
            var offsetAngle = GetOffsetAngle();
            var distanceToTarget = Vector3.Distance(transform.position, desiredPosition);

            // Начинаем догонять, если игрок отвернулся или отошёл — второе покрывает
            // и телепорт, и обычный шаг в сторону.
            if (!_isCatchingUp && (offsetAngle > _followThreshold || distanceToTarget > _maxDrift))
                _isCatchingUp = true;
            // Останавливаемся по расстоянию до цели, а не по углу: угол сходится раньше,
            // и панель замирала бы на полпути, ближе к лицу, чем задано.
            else if (_isCatchingUp && distanceToTarget < _settleDistance)
                _isCatchingUp = false;

            if (_isCatchingUp)
            {
                transform.position = Vector3.Lerp(
                    transform.position, desiredPosition, 1f - Mathf.Exp(-_positionDamping * Time.deltaTime));
            }

            // Лицом к игроку панель доворачивается всегда: даже неподвижную панель
            // нужно держать читаемой, когда игрок смещается вбок.
            transform.rotation = Quaternion.Slerp(
                transform.rotation, GetDesiredRotation(transform.position), 1f - Mathf.Exp(-_rotationDamping * Time.deltaTime));
        }

        /// <summary>Угол между направлением взгляда и направлением на панель, по горизонтали.</summary>
        private float GetOffsetAngle()
        {
            var toPanel = Flatten(transform.position - _viewer.position);
            if (toPanel.sqrMagnitude < 0.0001f)
                return 0f;

            return Vector3.Angle(Flatten(_viewer.forward), toPanel);
        }

        private Vector3 GetDesiredPosition()
        {
            var forward = Flatten(_viewer.forward);
            if (forward.sqrMagnitude < 0.0001f)
                forward = Vector3.forward;

            return _viewer.position + forward.normalized * _distance + Vector3.up * _heightOffset;
        }

        /// <summary>
        /// Поворот, при котором панель читается: её forward направлен от игрока.
        /// </summary>
        private Quaternion GetDesiredRotation(Vector3 panelPosition)
        {
            var awayFromViewer = Flatten(panelPosition - _viewer.position);
            if (awayFromViewer.sqrMagnitude < 0.0001f)
                return transform.rotation;

            return Quaternion.LookRotation(awayFromViewer.normalized, Vector3.up);
        }

        /// <summary>Убирает наклон: панель не должна заваливаться вслед за головой.</summary>
        private static Vector3 Flatten(Vector3 direction)
        {
            direction.y = 0f;
            return direction;
        }
    }
}
