using DG.Tweening;
using Kaizen;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Slime
{
	public class LevelController : MonoBehaviour
	{
		private const string TextBlueSlimeDead = "<color=#66CAE4>Blue</color> <color=#FF0000>Died</color>";
		private const string TextPinkSlimeDead = "<color=#CB6889>Pink</color> <color=#FF0000>Died</color>";

		private List<Finish> finishLines = new List<Finish>();
		private List<Player> players = new List<Player>();
		private List<Enemy> enemies = new List<Enemy>();

		[Header("Audio Clips")]
		[SerializeField] private AudioClip clipAudioFinish = null;
		[SerializeField] private AudioClip clipLevelBGM = null;
		[SerializeField] private AudioClip clipPlayerEnterFinish = null;
		[SerializeField] private AudioClip clipPlayerDead = null;

		[Header("Custom Settings")]
		public float cameraOrthographicMinSize;
		public Vector2 cameraOffset;
		[Space]
		public RectTransform rectGameOverUIContainer;
		public Image imageGameOverBanner;
		public TextMeshProUGUI textGameOver;
		public Button buttonPause;

		public bool IsPlayerDead { get; private set; }

		private void Awake()
		{
			rectGameOverUIContainer.gameObject.SetActive(false);
		}

		private void Start()
		{
			Initialize();
#if UNITY_EDITOR
			if (GameManager.Instance.IsEditMode)
				return;
#endif
			GameManager.Instance.LevelStart(this);
		}

		private void Initialize()
		{
			AudioManager.Instance.SetBGM(clipLevelBGM);

			var allGOs = gameObject.scene.GetRootGameObjects();

			foreach (var item in allGOs)
			{
				if (item.TryGetComponent(out GridController gridController))
				{
					var tilemap = gridController.tilemapVoid;
					tilemap.CompressBounds();

					var minTile = tilemap.CellToWorld(tilemap.cellBounds.min);
					var maxTile = tilemap.CellToWorld(tilemap.cellBounds.max);

					CameraManager.Instance.SetCustomOffset(cameraOffset);
					CameraManager.Instance.SetCameraBoundary(minTile, maxTile);
					CameraManager.Instance.SetStartPositionAndSize();
				}
				if (item.TryGetComponent(out Player player))
				{
					player.onOtherEntityContactEvent = Player_onOtherEntityContact;
					players.Add(player);
					GameManager.Instance.LevelController_onPlayerGetEvent(player);
					CameraManager.Instance.AddObjectToTrack(item);
				}
				if (item.TryGetComponent(out Finish finish))
				{
					finishLines.Add(finish);
					finish.onPlayerEnterEvent = OnPlayerEnterFinish;
				}

				if (item.TryGetComponent(out Enemy enemy))
				{
					enemies.Add(enemy);
				}
			}

			CameraManager.Instance.SetOrthographicMinSize(cameraOrthographicMinSize);
			CameraManager.Instance.Move(true);

			foreach (var enemy in enemies)
			{
				enemy.SetPlayerReferences(players);
			}
		}

		private void OnPlayerEnterFinish(Player player, Finish finish)
		{
			if (GameManager.Instance.State == GameManager.GameState.End)
				return;

			AudioManager.Instance.PlaySFX(clipPlayerEnterFinish);
			Vibration.TriggerTaptic(1);

			foreach (var finishCached in finishLines)
			{
				if (!finishCached.IsFinish)
					return;
			}

			AudioManager.Instance.PlaySFX(clipAudioFinish);

			GameManager.Instance.LevelComplete();
			GameManager.Instance.LevelNext();
		}

		private void Player_onOtherEntityContact(Player player, Entity entity)
		{
			if (entity is Enemy && !player.IsDead)
			{
				player.Dead();
				foreach (var enemy in enemies)
				{
					enemy.enabled = false;
				}

				InputManager.Instance.HideControls();

				AudioManager.Instance.PlaySFX(clipPlayerDead);

				if (player is PlayerBlue)
					textGameOver.text = TextBlueSlimeDead;
				else
					textGameOver.text = TextPinkSlimeDead;

				var bannerSize = imageGameOverBanner.rectTransform.sizeDelta;
				bannerSize.y = 0f;

				if (!rectGameOverUIContainer.gameObject.activeSelf)
				{
					rectGameOverUIContainer.DOKill(true);
					imageGameOverBanner.rectTransform.DOSizeDelta(imageGameOverBanner.rectTransform.sizeDelta, 0.25f).From(bannerSize).SetEase(Ease.Linear);
				}

				rectGameOverUIContainer.gameObject.SetActive(true);

				Vibration.TriggerTaptic(3);

				GameManager.Instance.LevelFailed();
				IsPlayerDead = true;
			}
		}

		public void ButtonPause_OnClicked()
		{
			AudioManager.Instance.PauseBGM();
			GameManager.Instance.ShowPauseMenu();
		}

		public void ButtonRetry_OnClicked()
		{
			GameManager.Instance.LevelExit();
			GameManager.Instance.LevelRetry(false);
		}
	}
}
