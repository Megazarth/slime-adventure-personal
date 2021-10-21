using DG.Tweening;
using Kaizen;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Slime
{
    public class UIPauseMenuController : SceneSingletonComponent<UIPauseMenuController>
    {
        private const float AnimationDuration = 0.25f;

        private Sequence sequence;

        public Canvas canvas;
        public CanvasGroup canvasGroup;
        public RectTransform rectPanelLayout;
        public TextMeshProUGUI textLevelNumber;
        public TextMeshProUGUI textLevelTitle;
        [Header("Button")]
        public Toggle toggleMusic;
        public Toggle toggleSFX;

        //Reference to currently opened level's LevelController object
        public LevelController LevelController { get; set; }

        protected override void Awake()
        {
            base.Awake();
            canvas.enabled = false;
        }

        private void OnDestroy()
        {
            sequence?.Kill();
        }

        private void Initialize()
        {
            toggleMusic.isOn = !AudioManager.Instance.IsMusicMuted;
            toggleSFX.isOn = !AudioManager.Instance.IsSFXMuted;

#if UNITY_EDITOR
            if (GameManager.Instance.IsEditMode)
                return;
#endif

            textLevelNumber.text = $"Level { GameManager.Instance.CurrentLevel.number }";
            textLevelTitle.text = $"{ GameManager.Instance.CurrentCollection.name}";
        }

        public void Show(Action onComplete = null)
        {
            GameManager.Instance.Pause();

            if (LevelController != null && LevelController.IsPlayerAlive)
                InputManager.Instance.HideControls();

            Initialize();
            canvas.enabled = true;

            if (sequence == null)
            {
                InputBlocker.Instance.DisableInput();
                sequence = DOTween.Sequence();
                sequence.SetUpdate(true);
                sequence.SetAutoKill(false);

                sequence.Insert(0f, rectPanelLayout.DOAnchorPosY(0f, AnimationDuration).From(Vector2.down * 100f).SetEase(Ease.OutSine));
                sequence.Insert(0f, canvasGroup.DOFade(1f, AnimationDuration).From(0f));
            }
            else
            {
                sequence.Complete(true);
                InputBlocker.Instance.DisableInput();
                sequence.Restart();
            }

            sequence.OnComplete(() =>
            {
                InputBlocker.Instance.EnableInput();
                onComplete?.Invoke();
            });
        }

        public void Hide(Action onComplete = null)
        {
            InputBlocker.Instance.DisableInput();

            if (LevelController != null && LevelController.IsPlayerAlive)
                InputManager.Instance.ShowControls();

            sequence.Complete(true);
            sequence.onComplete = null;
            sequence.SmoothRewind();
            sequence.OnSmoothRewindCompleted(this, false, () =>
            {
                canvas.enabled = false;
                GameManager.Instance.Play();
                InputBlocker.Instance.EnableInput();
                onComplete?.Invoke();
            });
        }

        public void ButtonMusic_OnClicked(bool state)
        {
            AudioManager.Instance.ToggleBGM(state);
        }

        public void ButtonSFX_OnClicked(bool state)
        {
            AudioManager.Instance.ToggleSFX(state);
        }

        public void Resume()
        {
            Hide(() =>
            {
                AudioManager.Instance.PlayBGM();
            });
        }

        public void Retry()
        {
            GameAnalytics.Instance.TrackLevelRetry(GameManager.Instance.CurrentLevel.number);
            GameManager.Instance.LevelExit();

            Hide(() =>
            {
                GameManager.Instance.LevelRetry(false);
            });
        }

        public void Quit()
        {
            GameManager.Instance.LevelExit();

            Hide(() =>
            {
                GameManager.Instance.GoToStartScreen();
            });
        }
    }

}

