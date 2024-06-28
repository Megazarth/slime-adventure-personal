using Kaizen;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

namespace Slime
{
	public class DebugController : MonoBehaviour
	{
		private List<Button> buttons = new List<Button>();
		private bool isShown;

		public RectTransform rectTextContainer;
		public RectTransform buttonsContainer;
		public Button buttonHideShow;
		public TextMeshProUGUI textButtonHideShow;
		public TextMeshProUGUI textInterstitialState;
		public TextMeshProUGUI textIAPError;

		private void Awake()
		{
			for (int i = 0; i < buttonsContainer.childCount; i++)
			{
				var button = buttonsContainer.GetChild(i).GetComponent<Button>();
				if (button != buttonHideShow)
					buttons.Add(button);
			}

			ToggleButtons(isShown);
		}

		private void Update()
		{
			if (AdsManager.Instance != null)
			{
				textInterstitialState.text = "Interstitial State = " + AdsManager.Instance.InterstitialState;
			}
		}

		private void IAPManager_OnInitializeFailed(InitializationFailureReason error)
		{
			textIAPError.text = $"Message: {error.GetTypeCode()}";
		}

		private void ToggleButtons(bool state)
		{
			foreach (var button in buttons)
			{
				button.gameObject.SetActive(state);
				textButtonHideShow.text = state ? "Hide" : "Show";
			}

			rectTextContainer.gameObject.SetActive(state);
		}

		public void ButtonHideShow_OnPressed()
		{
			isShown = !isShown;
			ToggleButtons(isShown);

			rectTextContainer.gameObject.SetActive(isShown);
		}

		public void ButtonResetProgress_OnPressed()
		{
			AppManager.Instance.DeleteSaveData();
		}

		public void ButtonNextLevel_OnPressed()
		{
			if (GameManager.Instance.State != GameManager.GameState.Start)
				return;

			GameManager.Instance.LevelComplete();
			GameManager.Instance.LevelNext();
		}
	}
}