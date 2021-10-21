using DG.Tweening;
using Kaizen;
using System;
using UnityEngine;

namespace Slime
{
    public class UIExitConfirmationController : MonoBehaviour
    {
        private const float AnimationDuration = 0.25f;

        private Sequence sequence;

        public Canvas canvas;
        public CanvasGroup canvasGroup;
        public RectTransform rectPanelLayout;

        private void Start()
        {
            canvas.enabled = false;
        }

        private void OnDestroy()
        {
            sequence?.Kill();
        }

        private void Initialize()
        {
            sequence?.Kill();
            sequence = DOTween.Sequence();
            sequence.SetUpdate(true);
        }

        public void Show(Action onComplete = null)
        {
            GameManager.Instance.Pause();
            Initialize();
            canvas.enabled = true;

            sequence.Insert(0f, rectPanelLayout.DOAnchorPosY(0f, AnimationDuration).From(Vector2.down * 100f).SetEase(Ease.OutSine));
            sequence.Insert(0f, canvasGroup.DOFade(1f, AnimationDuration).From(0f));
            sequence.OnKill(() =>
            {
                onComplete?.Invoke();
            });
        }

        public void Hide(Action onComplete = null)
        {
            GameManager.Instance.Play();
            Initialize();

            sequence.Insert(0f, rectPanelLayout.DOAnchorPosY(-100f, AnimationDuration).From(Vector2.zero).SetEase(Ease.InSine));
            sequence.Insert(0f, canvasGroup.DOFade(0f, AnimationDuration).From(1f));
            sequence.OnKill(() =>
            {
                canvas.enabled = false;
                onComplete?.Invoke();
            });
        }

        public void ButtonCancel_OnClicked()
        {
            Hide();
        }

        public void ButtonConfirm_OnClicked()
        {
            AppManager.Instance.Quit();
        }
    }
}