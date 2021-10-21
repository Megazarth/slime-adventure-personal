using Kaizen;
using System;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

namespace Slime
{
    public class UITitleMenuController : Panel
    {
        [Space]
        public UICollectionSelectionController collectionSelectionController;
        public RemoveAdsController removeAdsController;
        public Button buttonPlay;
        public Button buttonRemoveAds;

        protected override void Awake()
        {
            base.Awake();

            if (!EncryptedPrefs.GetBool(Global.KeyTutorialLevel1Completed))
                buttonPlay.onClick.AddListener(OpenLevelTutorial);
            else
                buttonPlay.onClick.AddListener(ShowCollectionUI);
        }

        protected override void Start()
        {
            base.Start();
            var removeAdsPurchased = EncryptedPrefs.GetBool(AppManager.RemoveAdsPurchaseID);
            if (removeAdsPurchased)
            {
                buttonRemoveAds.gameObject.SetActive(false);
            }
            else
            {
                AppManager.Instance.onProductPurchaseSuccess.AddListener(IAPManager_onPurchaseSuccess);
                AppManager.Instance.onRestorePurchaseSuccess.AddListener(IAPManager_onPurchaseSuccess);
            }
        }

        public void OnRemoveAdsSuccessful()
        {
            if (removeAdsController.canvas.enabled)
            {
                removeAdsController.Hide();
            }

            buttonRemoveAds.gameObject.SetActive(false);
            AppManager.Instance.onProductPurchaseSuccess.RemoveListener(IAPManager_onPurchaseSuccess);
            AppManager.Instance.onRestorePurchaseSuccess.RemoveListener(IAPManager_onPurchaseSuccess);
        }

        private void OpenLevelTutorial()
        {
            GameManager.Instance.CurrentCollection = GameManager.Instance.MainCollection;
            GameManager.Instance.CurrentLevel = GameManager.Instance.MainCollection.levels[0];
            GameManager.Instance.CurrentLevelIndex = 0;
            GameManager.Instance.LevelRetry(false);
        }

        private void ShowCollectionUI()
        {
            ButtonShowNextPanel_OnClicked(collectionSelectionController);
        }

        private void IAPManager_onPurchaseSuccess(Product product)
        {
            var removeads = string.Equals(product.definition.id, AppManager.Instance.RemoveAdsProductID, StringComparison.Ordinal);
            if (removeads)
            {
                OnRemoveAdsSuccessful();
            }
        }
    }
}