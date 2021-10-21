using DG.Tweening;
using System;
using UnityEngine;

namespace Kaizen
{
    public class Panel : MonoBehaviour
    {
        public enum AnimationType
        {
            Fade,
            Move,
        }

        public enum AnimationDirection
        {
            Up,
            Down,
            Left,
            Right
        }

        public delegate void OnSwapPanelDelegate(Panel current, Panel next);
        /// <summary>
        /// Call this action to notify PanelManager that this panel has been hidden. Add this panel to PanelManager's stack.
        /// </summary>
        public OnSwapPanelDelegate onSwapPanel;

        protected RectTransform rectTransform;
        protected CanvasGroup canvasGroup;
        protected Sequence sequence;

        public AnimationType animationType = AnimationType.Move;
        [Header("Show Animation")]
        public AnimationDirection showDirection = AnimationDirection.Left;
        [SerializeField] protected float showAnimationDuration = 0.5f;
        [SerializeField] protected Ease showAnimationEase = Ease.OutExpo;
        [SerializeField] protected float showAnimationOffset = 500.0f;
        [Header("Hide Animation")]
        public AnimationDirection hideDirection = AnimationDirection.Right;
        [SerializeField] protected float hideAnimationDuration = 0.5f;
        [SerializeField] protected Ease hideAnimationEase = Ease.InExpo;
        [SerializeField] protected float hideAnimationOffset = 500.0f;

        protected virtual void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();

            gameObject.SetActive(false);
        }

        protected virtual void Start()
        {
#if UNITY_EDITOR
            if (onSwapPanel == null)
                Debug.LogWarning("OnPanelMoved Action has not been initalized!", gameObject);
#endif
        }

        private void OnDestroy()
        {
            sequence?.Kill();
        }

        public virtual void ButtonShowNextPanel_OnClicked(Panel panel)
        {
            onSwapPanel.Invoke(this, panel);
        }

        public virtual void Show(Action onComplete = null)
        {
            if (sequence != null)
                sequence.Complete();

            gameObject.SetActive(true);

            switch (animationType)
            {
                case AnimationType.Fade:
                    ShowFade(onComplete);
                    break;
                case AnimationType.Move:
                    ShowMove(showDirection, onComplete);
                    break;
                default:
                    break;
            }
        }

        public virtual void Show(AnimationDirection direction, Action onComplete = null)
        {
            if (sequence != null)
                sequence.Complete();

            gameObject.SetActive(true);

            switch (animationType)
            {
                case AnimationType.Fade:
                    ShowFade(onComplete);
                    break;
                case AnimationType.Move:
                    ShowMove(direction, onComplete);
                    break;
                default:
                    break;
            }
        }

        protected virtual void ShowFade(Action onComplete = null)
        {
            sequence = DOTween.Sequence();
            sequence.SetUpdate(true);

            sequence.Insert(0.0f, canvasGroup.DOFade(0.0f, showAnimationDuration).From(1.0f).SetEase(showAnimationEase));
            sequence.OnKill(() => onComplete?.Invoke());
        }

        protected virtual void ShowMove(AnimationDirection direction, Action onComplete = null)
        {
            sequence = DOTween.Sequence();
            sequence.SetUpdate(true);

            var rectSize = rectTransform.rect.size;
            var movePosition = Vector2.zero;

            switch (direction)
            {
                case AnimationDirection.Up:
                    movePosition.y = -(rectSize.y + showAnimationOffset);
                    break;
                case AnimationDirection.Down:
                    movePosition.y = rectSize.y + showAnimationOffset;
                    break;
                case AnimationDirection.Left:
                    movePosition.x = -(rectSize.x + showAnimationOffset);
                    break;
                case AnimationDirection.Right:
                    movePosition.x = rectSize.x + showAnimationOffset;
                    break;
            }

            sequence.Insert(0.0f, rectTransform.DOAnchorPos(Vector2.zero, showAnimationDuration).From(movePosition).SetEase(showAnimationEase));
            sequence.OnKill(() => onComplete?.Invoke());
        }

        public virtual void Hide(Action onComplete = null)
        {
            if (sequence != null)
                sequence.Complete();

            switch (animationType)
            {
                case AnimationType.Fade:
                    HideFade(onComplete);
                    break;
                case AnimationType.Move:
                    HideMove(hideDirection, onComplete);
                    break;
                default:
                    break;
            }
        }

        public virtual void Hide(AnimationDirection direction, Action onComplete = null)
        {
            if (sequence != null)
                sequence.Complete();

            switch (animationType)
            {
                case AnimationType.Fade:
                    HideFade(onComplete);
                    break;
                case AnimationType.Move:
                    HideMove(direction, onComplete);
                    break;
                default:
                    break;
            }
        }

        protected virtual void HideFade(Action onComplete = null)
        {
            sequence = DOTween.Sequence();
            sequence.SetUpdate(true);

            sequence.Insert(0.0f, canvasGroup.DOFade(0.0f, hideAnimationDuration).From(1.0f).SetEase(hideAnimationEase));
            sequence.OnKill(() => onComplete?.Invoke());
        }

        protected virtual void HideMove(AnimationDirection direction, Action onComplete = null)
        {
            sequence = DOTween.Sequence();
            sequence.SetUpdate(true);

            var rectSize = rectTransform.rect.size;
            var movePosition = Vector2.zero;

            switch (direction)
            {
                case AnimationDirection.Up:
                    movePosition.y = rectSize.y + hideAnimationOffset;
                    break;
                case AnimationDirection.Down:
                    movePosition.y = -(rectSize.y + hideAnimationOffset);

                    break;
                case AnimationDirection.Left:
                    movePosition.x = rectSize.x + hideAnimationOffset;
                    break;
                case AnimationDirection.Right:
                    movePosition.x = -(rectSize.x + hideAnimationOffset);
                    break;
            }

            sequence.Insert(0.0f, rectTransform.DOAnchorPos(movePosition, hideAnimationDuration).From(Vector2.zero).SetEase(hideAnimationEase));
            sequence.OnKill(() => onComplete?.Invoke());
        }
    }
}