using DG.Tweening;
using Kaizen;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Slime
{
    public class RemoveAdsController : MonoBehaviour
    {
        private const float AnimationDuration = 0.25f;

        private Sequence sequence;
        private Coroutine coroutineButtonBuyCooldown;

        public Canvas canvas;
        public CanvasGroup canvasGroup;
        public RectTransform rectPanelLayout;
        public TextMeshProUGUI textPrice;
        public TextMeshProUGUI textButtonBuy;
        public TextMeshProUGUI textError;
        [Space]
        public RectTransform rectInformationContainer;
        public RectTransform rectProcessingContainer;
        [Header("Button")]
        public Button buttonBuy;

        private void Awake()
        {
            canvas.enabled = false;
            textError.gameObject.SetActive(false);
            rectInformationContainer.gameObject.SetActive(true);
            rectProcessingContainer.gameObject.SetActive(false);
        }

        private void Initialize()
        {
            if (IAPManager.Instance.IsInitialized)
            {
                var product = IAPManager.Instance.GetProductByID(AppManager.Instance.RemoveAdsProductID);

                if (product == null)
                {
                    textPrice.text = "Price unavailable.";
                    textButtonBuy.text = "Try again later.";
                }

                else
                {
                    textPrice.text = $"{product.metadata.localizedPriceString}";
                    textButtonBuy.text = "Buy Remove Ads";
                }
            }

            buttonBuy.interactable = IAPManager.Instance.IsInitialized;

            if (!IAPManager.Instance.IsInitialized)
            {
                textError.gameObject.SetActive(true);
                textError.text = $"{IAPManager.Instance.FailureReason.ToString()} : {IAPManager.Instance.FailureReason.GetTypeCode()}";
            }
        }

        private IEnumerator ButtonBuyClickCooldownCoroutine()
        {
            buttonBuy.interactable = false;
            yield return new WaitForSecondsRealtime(2f);
            buttonBuy.interactable = true;
        }

        private void ButtonBuyClickCooldown()
        {
            if (coroutineButtonBuyCooldown != null)
            {
                StopCoroutine(coroutineButtonBuyCooldown);
                coroutineButtonBuyCooldown = null;
            }

            coroutineButtonBuyCooldown = StartCoroutine(ButtonBuyClickCooldownCoroutine());
        }

        public void Show(Action onComplete = null)
        {
            Initialize();
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

        public void ButtonBuy_OnClicked()
        {
            ButtonBuyClickCooldown();
            IAPManager.Instance.PurchaseProduct(AppManager.Instance.RemoveAdsProductID);
        }

        public void ButtonShow_OnClicked()
        {
            Show();
        }

        public void ButtonHide_OnClicked()
        {
            AudioManager.Instance.PlaySFX(GameManager.Instance.audioWindowClosed);
            Hide();
        }
    }
}