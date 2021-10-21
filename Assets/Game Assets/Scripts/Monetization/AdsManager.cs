using GoogleMobileAds.Api;
using Kaizen;
using System;
using UnityEngine;
using UnityEngine.Purchasing;

namespace Slime
{
    public class AdsManager : SingletonComponent<AdsManager>
    {
        public enum AdInterstitialState
        {
            inprogress,
            loaded,
            shown,
            closed,
            destroyed,
        }

        private BannerView bannerView;
        private InterstitialAd interstitial;

        public string bannerGameplayID;
        public string interstitialNextLevelID;
#if UNITY_EDITOR
        [Header("Debug")]
        public bool loadBannerAdOnEditor;
        public bool loadInterstitialAdOnEditor;
#endif

        public AdInterstitialState InterstitialState { get; private set; }

        public void Initialize(Action initCompleteAction = null)
        {
            InterstitialState = AdInterstitialState.destroyed;

            if (EncryptedPrefs.GetBool(AppManager.RemoveAdsPurchaseID))
            {
                initCompleteAction?.Invoke();
            }
            else
            {
                MobileAds.Initialize((status) =>
                {
                    ShowBanner();
                    initCompleteAction?.Invoke();
                });
            }
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (!loadInterstitialAdOnEditor)
                return;
#endif

            RequestInterstitial();
        }

        public void ValidateRemoveAdsPurchased(Product product)
        {
            if (EncryptedPrefs.GetBool(AppManager.RemoveAdsPurchaseID))
                return;

            var removeads = string.Equals(product.definition.id, AppManager.Instance.RemoveAdsProductID, StringComparison.Ordinal);
            if (removeads)
            {
                var popup = AppManager.Instance.GetPopup() as UIPopupController;
                popup.Initialize("Restore Purchase Successful", "Ads has been removed.");
                popup.Show();
                RemoveAds();
            }
        }

        public void RequestBanner()
        {
#if UNITY_EDITOR
            if (!loadBannerAdOnEditor)
                return;
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
            string adUnitId = bannerGameplayID;
#elif UNITY_EDITOR
            string adUnitId = "ca-app-pub-3940256099942544/6300978111";
#endif
            if (bannerView != null)
                bannerView.Destroy();

            adUnitId = bannerGameplayID == string.Empty ? "ca-app-pub-3940256099942544/6300978111" : bannerGameplayID;

            // Create a 320x50 banner at the top of the screen.
            bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);

            // Create an empty ad request.
            AdRequest request = new AdRequest.Builder().Build();

            // Load the banner with the request.
            bannerView.LoadAd(request);
        }

        public void RequestInterstitial()
        {
            if (EncryptedPrefs.GetBool(AppManager.RemoveAdsPurchaseID))
                return;

#if UNITY_EDITOR
            if (!loadInterstitialAdOnEditor)
                return;
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
            string adUnitId = interstitialNextLevelID;
#elif UNITY_EDITOR
            string adUnitId = "ca-app-pub-3940256099942544/1033173712";
#endif
            switch (InterstitialState)
            {
                case AdInterstitialState.inprogress:
                    return;
                case AdInterstitialState.loaded:
                    return;
                case AdInterstitialState.shown:
                    return;
                case AdInterstitialState.destroyed:
                    break;
                case AdInterstitialState.closed:
                    break;
                default:
                    break;
            }

            interstitial?.Destroy();

            adUnitId = interstitialNextLevelID == string.Empty ? "ca-app-pub-3940256099942544/1033173712" : interstitialNextLevelID;

            // Initialize an InterstitialAd.
            interstitial = new InterstitialAd(adUnitId);

            // Create an empty ad request.
            AdRequest request = new AdRequest.Builder().Build();
            // Called when an ad request has successfully loaded.
            interstitial.OnAdLoaded += Interstitial_HandleOnAdLoaded;
            // Called when an ad is shown.
            interstitial.OnAdOpening += Interstitial_handleOnAdOpened;
            // Called when the ad is closed.
            interstitial.OnAdClosed += Interstitial_handleOnAdClosed;

            InterstitialState = AdInterstitialState.inprogress;
            // Load the interstitial with the request.
            interstitial.LoadAd(request);
        }

        public void ShowBanner()
        {
            if (EncryptedPrefs.GetBool(AppManager.RemoveAdsPurchaseID))
                return;

#if UNITY_EDITOR
            if (!loadBannerAdOnEditor)
                return;
#endif

            RequestBanner();
            bannerView.Show();
        }

        public void ShowIntersitial()
        {
            if (EncryptedPrefs.GetBool(AppManager.RemoveAdsPurchaseID))
                return;

#if UNITY_EDITOR
            if (!loadInterstitialAdOnEditor)
                return;
#endif

            if (interstitial.IsLoaded())
            {
                interstitial.Show();
            }
        }

        public void DestroyBanner()
        {
            bannerView?.Destroy();
        }

        public void DestroyInterstitial()
        {
            interstitial?.Destroy();
            InterstitialState = AdInterstitialState.destroyed;
        }

        public void RemoveAds()
        {
            DestroyInterstitial();
            DestroyBanner();
            EncryptedPrefs.SetBool(AppManager.RemoveAdsPurchaseID, true);
        }

        public void Interstitial_HandleOnAdLoaded(object sender, EventArgs args)
        {
#if UNITY_EDITOR
            Debug.Log("Interstitial Ad loaded.");
#endif
            InterstitialState = AdInterstitialState.loaded;
        }

        private void Interstitial_handleOnAdOpened(object sender, EventArgs args)
        {
#if UNITY_EDITOR
            Debug.Log("Interstitial Ad opened.");
#endif
            InterstitialState = AdInterstitialState.shown;
        }

        private void Interstitial_handleOnAdClosed(object sender, EventArgs args)
        {
#if UNITY_EDITOR
            Debug.Log("Interstitial Ad closed.");
#endif
            InterstitialState = AdInterstitialState.closed;
        }
    }
}