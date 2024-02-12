using System;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using UnityEngine.SceneManagement;

namespace Kaizen
{
	public class AppManager : SingletonComponent<AppManager>
	{
		private const string SaveFileName = "savedata";
		private const string SaveFileType = ".sav";

		public static string RemoveAdsPurchaseID = "remove_ads_purchased";

		[Serializable]
		public class ButtonCustomOnClickedEvent : UnityEvent<ButtonCustom> { }

		[Serializable]
		public class OnInAppPurchaseInitializedEvent : UnityEvent<bool> { }

		[Serializable]
		public class OnRestorePurchaseSuccessEvent : UnityEvent<Product> { }

		[Serializable]
		public class OnProductPurchaseSuccessEvent : UnityEvent<Product> { }

		[Serializable]
		public class OnProductPurchaseFailedEvent : UnityEvent<Product, PurchaseFailureReason> { }

		[SerializeField]
		private string encryptionKey = string.Empty;

		public Popup popupPrefab;

		[Header("Events")]
		public UnityEvent onInitialized = new UnityEvent();
		public UnityEvent onEncryptedPrefsInitialized = new UnityEvent();
		public OnInAppPurchaseInitializedEvent onInAppPurchaseInitialized = new OnInAppPurchaseInitializedEvent();
		public OnRestorePurchaseSuccessEvent onRestorePurchaseSuccess = new OnRestorePurchaseSuccessEvent();
		public OnProductPurchaseSuccessEvent onProductPurchaseSuccess = new OnProductPurchaseSuccessEvent();
		public OnProductPurchaseFailedEvent onProductPurchaseFailed = new OnProductPurchaseFailedEvent();
		public ButtonCustomOnClickedEvent onButtonCustomClicked = new ButtonCustomOnClickedEvent();

		public delegate void onSceneLoaded(Scene scene, LoadSceneMode mode);
		public delegate void onSceneUnloaded(Scene scene);

		public onSceneLoaded onSceneLoadedEvent;
		public onSceneUnloaded onSceneUnloadedEvent;

		public string RemoveAdsProductID
		{
			get
			{
				return Application.identifier.ToLower() + ".removeads";
			}
		}

		protected override void Awake()
		{
			base.Awake();

			Application.targetFrameRate = 60;

			EncryptedPrefs.Initialize(encryptionKey);
			EncryptedPrefs.LoadProgress();
			onEncryptedPrefsInitialized?.Invoke();

			IAPManager.Instance.OnInAppPurchaseInitialized += IAPManager_OnInAppPurchaseInitialized;
			IAPManager.Instance.OnRestorePurchaseSuccess += IAPManager_OnRestorePurchaseSuccess;
			IAPManager.Instance.OnProductPurchaseSuccess += IAPManager_OnProductPurchaseSuccess;
			IAPManager.Instance.OnProductPurchaseFailed += IAPManager_OnProductPurchaseFailed;
			IAPManager.Instance.Initialize();

#if UNITY_ANDROID
			Vibration.Initialize();
#endif

			SceneManager.sceneLoaded += SceneManager_loadScene;
			SceneManager.sceneUnloaded += SceneManager_unloadScene;

#if UNITY_STANDALONE_WIN
			Screen.fullScreen = false;
			Screen.SetResolution(1280, 720, false);
#endif
		}

		private void Start()
		{
			onInitialized?.Invoke();
		}

		private void OnApplicationPause(bool pause)
		{
			EncryptedPrefs.SaveProgress();
		}

		private void IAPManager_OnInAppPurchaseInitialized(bool success)
		{
			onInAppPurchaseInitialized.Invoke(success);
		}

		private void IAPManager_OnRestorePurchaseSuccess(Product product)
		{
			onRestorePurchaseSuccess.Invoke(product);
		}

		private void IAPManager_OnProductPurchaseSuccess(Product product)
		{
			onProductPurchaseSuccess.Invoke(product);
		}

		private void IAPManager_OnProductPurchaseFailed(Product product, PurchaseFailureReason reason)
		{
			onProductPurchaseFailed.Invoke(product, reason);
		}

		private void SceneManager_loadScene(Scene scene, LoadSceneMode mode)
		{
			onSceneLoadedEvent?.Invoke(scene, mode);
		}

		private void SceneManager_unloadScene(Scene scene)
		{
			onSceneUnloadedEvent?.Invoke(scene);
		}
		public AsyncOperation UnloadSceneAsync(string sceneName)
		{
			return UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);
		}

		public AsyncOperation LoadSceneAsync(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode)
		{
			return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
		}

		public Popup GetPopup()
		{
			return Instantiate(popupPrefab);
		}

		public string GetPersistentLevelFormating(int levelNumber)
		{
			var sb = new StringBuilder();
			sb.Append("level_" + levelNumber);
			return sb.ToString();
		}

		public void DeleteSaveData()
		{
			FileManager.Delete(SaveFileName, SaveFileType);
		}

		public void Quit(int exitCode = 0)
		{
			Application.Quit(exitCode);
		}
	}
}

