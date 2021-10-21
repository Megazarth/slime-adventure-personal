using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

namespace Slime
{
    public class Window : MonoBehaviour
    {
        protected const float AnimationDuration = 0.15f;

        [SerializeField]
        protected Vector2 offset = new Vector2(0.0f, 55.0f);
        protected Vector2 initialPosition;

        protected Sequence sequence;

        protected Canvas canvas;
        protected CanvasGroup canvasGroup;

        public RectTransform rectPanelMenu;

        protected virtual void Awake()
        {
            canvas = GetComponentInChildren<Canvas>();
            canvasGroup = GetComponentInChildren<CanvasGroup>();
            initialPosition = rectPanelMenu.anchoredPosition;
        }

        protected virtual void OnDestroy()
        {
            if (sequence != null)
                sequence.Kill();
        }

        private void SmoothRewind(Sequence sequence, Action onFinished = null)
        {
            if (sequence != null)
                StartCoroutine(SmoothRewindCoroutine(sequence, onFinished));
            else
                onFinished?.Invoke();
        }

        private IEnumerator SmoothRewindCoroutine(Sequence sequence, Action onFinished = null)
        {
            sequence.Complete();
            sequence.SmoothRewind();
            while (sequence.IsPlaying())
                yield return null;

            onFinished?.Invoke();
        }

        public virtual void Show(Action onFinished = null)
        {
            canvas.enabled = true;

            if (sequence == null)
            {
                sequence = DOTween.Sequence();

                sequence.SetAutoKill(false);
                sequence.SetUpdate(true);

                sequence.Insert(0.0f, rectPanelMenu.DOAnchorPos(initialPosition, AnimationDuration).From(initialPosition - offset).SetEase(Ease.Linear));
                sequence.Insert(0.0f, canvasGroup.DOFade(0.0f, AnimationDuration).From().SetEase(Ease.Linear));

                sequence.OnComplete(() => onFinished?.Invoke());
            }
            else
            {
                sequence.Complete();
                sequence.OnComplete(() => onFinished?.Invoke());
                sequence.Restart();
            }
        }

        public virtual void Hide(Action onFinished = null)
        {
            SmoothRewind(sequence, () =>
            {
                canvas.enabled = false;
                onFinished?.Invoke();
            });
        }
    }

}
