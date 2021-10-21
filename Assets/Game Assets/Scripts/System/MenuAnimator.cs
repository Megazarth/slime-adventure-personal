using DG.Tweening;
using UnityEngine;


namespace Slime
{
    public class MenuAnimator : MonoBehaviour
    {
        [SerializeField] private Canvas canvas = null;
        [SerializeField] private CanvasGroup canvasGroup = null;
        [SerializeField] private RectTransform mainRectTransform = null;
        [Header("Show Animation")]
        [SerializeField] private float showAnimationDuration = 0.5f;
        [SerializeField] private Ease showAnimationEase = Ease.OutExpo;
        [SerializeField] private float showAnimationOffset = 500.0f;
        [Header("Hide Animation")]
        [SerializeField] private float hideAnimationDuration = 0.5f;
        [SerializeField] private Ease hideAnimationEase = Ease.InExpo;
        [SerializeField] private float hideAnimationOffset = 500.0f;
        private Vector3 originalPosition = Vector3.zero;
        private bool isAnimationPlaying = false;

        public float ShowAnimationOffset { get => showAnimationOffset; set => showAnimationOffset = value; }
        public float HideAnimationOffset { get => hideAnimationOffset; set => hideAnimationOffset = value; }

        public enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }

        public delegate void OnPreShowAnimationDelegate();
        public event OnPreShowAnimationDelegate onPreShowAnimationEvent;

        public delegate void OnPreHideAnimationDelegate();
        public event OnPreHideAnimationDelegate onPreHideAnimationEvent;

        public delegate void OnPostShowAnimationDelegate();
        public event OnPostShowAnimationDelegate onPostShowAnimationEvent;

        public delegate void OnPostHideAnimationDelegate();
        public event OnPostHideAnimationDelegate onPostHideAnimationEvent;

        private void Awake()
        {
            if (!canvas)
            {
                canvas = GetComponent<Canvas>();
            }
        }

        private void Start()
        {
            originalPosition = mainRectTransform.position;
        }

        /// <param name="direction">Will be casted into enum Direction.</param>
        public void Show(int direction)
        {
            if (isAnimationPlaying)
            {
                return;
            }

            isAnimationPlaying = true;

            canvas.enabled = true;
            onPreShowAnimationEvent?.Invoke();
            Sequence sequence = DOTween.Sequence();
            sequence.SetUpdate(true);

            if (mainRectTransform)
            {
                Vector3 offset = Vector3.zero;
                Direction offsetDirection = (Direction)direction;
                switch (offsetDirection)
                {
                    case Direction.Up:
                        offset.y = -ShowAnimationOffset;
                        break;
                    case Direction.Down:
                        offset.y = ShowAnimationOffset;
                        break;
                    case Direction.Left:
                        offset.x = ShowAnimationOffset;
                        break;
                    case Direction.Right:
                        offset.x = -ShowAnimationOffset;
                        break;
                }
                sequence.Insert(0.0f, mainRectTransform.DOMove(originalPosition, showAnimationDuration)
                    .From(originalPosition + offset)
                    .SetEase(showAnimationEase)
                );
            }

            if (canvasGroup)
            {
                sequence.Insert(0.0f, canvasGroup.DOFade(1.0f, showAnimationDuration)
                    .From(0.0f)
                    .SetEase(showAnimationEase)
                );
            }

            sequence.OnKill(() =>
            {
                isAnimationPlaying = false;
                onPostShowAnimationEvent?.Invoke();
            });
        }

        /// <param name="direction">Will be casted into enum Direction.</param>
        public void Hide(int direction)
        {
            if (isAnimationPlaying)
            {
                return;
            }

            isAnimationPlaying = true;

            onPreHideAnimationEvent?.Invoke();
            Sequence sequence = DOTween.Sequence();
            sequence.SetUpdate(true);

            if (mainRectTransform)
            {
                Vector3 offset = Vector3.zero;
                Direction offsetDirection = (Direction)direction;
                switch (offsetDirection)
                {
                    case Direction.Up:
                        offset.y = HideAnimationOffset;
                        break;
                    case Direction.Down:
                        offset.y = -HideAnimationOffset;
                        break;
                    case Direction.Left:
                        offset.x = -HideAnimationOffset;
                        break;
                    case Direction.Right:
                        offset.x = HideAnimationOffset;
                        break;
                }
                sequence.Insert(0.0f, mainRectTransform.DOMove(originalPosition + offset, hideAnimationDuration)
                    .From(originalPosition)
                    .SetEase(hideAnimationEase)
                );
            }

            if (canvasGroup)
            {
                sequence.Insert(0.0f, canvasGroup.DOFade(0.0f, hideAnimationDuration)
                    .From(1.0f)
                    .SetEase(hideAnimationEase)
                );
            }

            sequence.OnKill(() =>
            {
                isAnimationPlaying = false;
                canvas.enabled = false;
                onPostHideAnimationEvent?.Invoke();
            });
        }

        /// <param name="direction">Will be casted into enum Direction.</param>
        public void HideNoAnimation(Direction direction)
        {
            onPreHideAnimationEvent?.Invoke();

            if (mainRectTransform)
            {
                Vector3 offset = Vector3.zero;
                switch (direction)
                {
                    case Direction.Up:
                        offset.y = HideAnimationOffset;
                        break;
                    case Direction.Down:
                        offset.y = -HideAnimationOffset;
                        break;
                    case Direction.Left:
                        offset.x = -HideAnimationOffset;
                        break;
                    case Direction.Right:
                        offset.x = HideAnimationOffset;
                        break;
                }
                mainRectTransform.position = originalPosition + offset;
            }

            if (canvasGroup)
            {
                canvasGroup.alpha = 0.0f;
            }

            canvas.enabled = false;

            onPostHideAnimationEvent?.Invoke();
        }
    }
}
