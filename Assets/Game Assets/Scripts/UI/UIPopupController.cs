using DG.Tweening;
using Kaizen;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Slime
{
    public class UIPopupController : Popup
    {
        private const float AnimationDuration = 0.25f;

        private Sequence sequence;

        public Canvas canvas;
        public CanvasGroup canvasGroup;
        public RectTransform rectPanelLayout;
        public TextMeshProUGUI textTitle;
        public TextMeshProUGUI textMessage;
        [Header("Button")]
        public Button buttonClose;

        private void Awake()
        {
            canvas.enabled = false;
        }

        private void OnDestroy()
        {
            sequence?.Kill();
        }

        public void Initialize(string title, string message, bool destroyWhenClosed = true)
        {
            textTitle.text = title;
            textMessage.text = message;
            this.destroyWhenClosed = destroyWhenClosed;
        }

        public override void Show(bool pauseGame = true)
        {
            if (pauseGame)
                GameManager.Instance.Pause();

            if (InputBlocker.Instance != null)
                canvas.sortingOrder = InputBlocker.Instance.SortingOrder + 1;

            canvas.enabled = true;

            if (sequence == null)
            {
                sequence = DOTween.Sequence();
                sequence.SetUpdate(true);
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
                OnShow?.Invoke();
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="destroy">Should this popup be destroyed when closed?</param>
        public override void Hide()
        {
            sequence.Complete(true);
            sequence.onComplete = null;
            sequence.SmoothRewind();
            sequence.OnSmoothRewindCompleted(this, true, () =>
            {
                canvas.enabled = false;
                GameManager.Instance.Play();
                OnHide?.Invoke();
                if (destroyWhenClosed)
                    Destroy(gameObject);
            });
        }

        public void ButtonClose_OnClicked()
        {
            Hide();
        }
    }
}