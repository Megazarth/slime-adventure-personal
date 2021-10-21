using DG.Tweening;
using Kaizen;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Slime
{
    public class UICreditsController : MonoBehaviour
    {
        private const float AnimationDuration = 0.25f;
        private Sequence sequence;

        [Header("Credits Variables")]
        public Canvas canvas;
        public CanvasGroup canvasGroup;
        public Image background;
        public RectTransform rectPanelLayout;

        private void Start()
        {
            canvas.enabled = false;
        }

        private void OnDestroy()
        {
            sequence?.Kill();
        }

        public void Show(Action onComplete = null)
        {
            canvas.enabled = true;

            if (sequence == null)
            {
                sequence = DOTween.Sequence();
                sequence.SetAutoKill(false);

                sequence.Insert(0f, rectPanelLayout.DOAnchorPosY(0f, AnimationDuration).From(Vector2.down * 100f).SetEase(Ease.OutSine));
                sequence.Insert(0f, canvasGroup.DOFade(1f, AnimationDuration).From(0f));
            }
            else
            {
                sequence.Complete(true);
                sequence.Restart();
            }

            sequence.OnComplete(() =>
            {
                onComplete?.Invoke();
            });
        }

        public void Hide(Action onComplete = null)
        {
            sequence.Complete(true);
            sequence.onComplete = null;
            sequence.SmoothRewind();
            sequence.OnSmoothRewindCompleted(this, false, () =>
            {
                canvas.enabled = false;
                onComplete?.Invoke();
            });
        }

        public void ButtonShow_OnClicked()
        {
            Show();
        }

        public void ButtonClose_OnClicked()
        {
            AudioManager.Instance.PlaySFX(GameManager.Instance.audioWindowClosed);
            Hide();
        }
    }
}
