using System;
using UnityEngine.Purchasing;

namespace Kaizen
{
	public class IAPManager : IStoreListener
	{
		private IStoreController controller;
		private IExtensionProvider extensions;

		private IGooglePlayStoreExtensions googlePlayStoreExtensions;

		private bool isRestoringPurchases;

		private static IAPManager instance;
		public static IAPManager Instance
		{
			get
			{
				if (instance == null)
					instance = new IAPManager();

				return instance;
			}
		}

		public Action<bool> OnInAppPurchaseInitialized;
		public Action<Product> OnRestorePurchaseSuccess;
		public Action<Product> OnProductPurchaseSuccess;
		public Action<Product, PurchaseFailureReason> OnProductPurchaseFailed;

		public bool IsInitialized { get; private set; }

		public InitializationFailureReason FailureReason { get; private set; }

		public void Initialize()
		{
			var module = StandardPurchasingModule.Instance();
			ConfigurationBuilder builder = ConfigurationBuilder.Instance(module);
			builder.AddProduct(AppManager.Instance.RemoveAdsProductID, ProductType.NonConsumable);
			UnityPurchasing.Initialize(this, builder);
		}

		public void PurchaseProduct(string productId)
		{
			controller.InitiatePurchase(productId);
		}

		public Product GetProductByID(string productID)
		{
			if (controller == null)
				return null;

			return controller.products.WithID(productID);
		}

		/// <summary>
		/// Called when Unity IAP is ready to make purchases.
		/// </summary>
		public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
		{
			this.controller = controller;
			this.extensions = extensions;

			googlePlayStoreExtensions = extensions.GetExtension<IGooglePlayStoreExtensions>();

			RestorePurchase();

			OnInAppPurchaseInitialized?.Invoke(true);
			IsInitialized = true;
		}

		private void RestorePurchase()
		{
			isRestoringPurchases = true;

			googlePlayStoreExtensions.RestoreTransactions(result =>
			{
				if (result)
				{
					// This does not mean anything was restored,
					// merely that the restoration process succeeded.
				}
				else
				{
					// Restoration failed.
				}
			});
		}

		/// <summary>
		/// Called when Unity IAP encounters an unrecoverable initialization error.
		///
		/// Note that this will not be called if Internet is unavailable; Unity IAP
		/// will attempt initialization until it becomes available.
		/// </summary>
		public void OnInitializeFailed(InitializationFailureReason error)
		{
			OnInAppPurchaseInitialized?.Invoke(false);
			FailureReason = error;
			IsInitialized = false;
		}

		/// <summary>
		/// Called when a purchase fails.
		/// </summary>
		public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
		{
			OnProductPurchaseFailed?.Invoke(product, failureReason);
		}

		/// <summary>
		/// Called when a purchase completes.
		///
		/// May be called at any time after OnInitialized().
		/// </summary>
		public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
		{
			if (isRestoringPurchases)
			{
				OnRestorePurchaseSuccess.Invoke(purchaseEvent.purchasedProduct);
				isRestoringPurchases = false;
			}
			else
			{
				OnProductPurchaseSuccess?.Invoke(purchaseEvent.purchasedProduct);
			}
			return PurchaseProcessingResult.Complete;
		}

		public void OnInitializeFailed(InitializationFailureReason error, string message)
		{

		}
	}
}