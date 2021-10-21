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

        [SerializeField] private AudioClip audioFinish = null;
        [SerializeField] private AudioClip levelBGM = null;

        private List<Finish> finishLines = new List<Finish>();
        private List<Player> players = new List<Player>();
        private List<Enemy> enemies = new List<Enemy>();

        [Header("Custom Settings")]
        public float cameraOrthographicMinSize;
        public Vector2 cameraOffset;
        [Space]
        public RectTransform rectGameOverUIContainer;
        public Image imageGameOverBanner;
        public TextMeshProUGUI textGameOver;
        public Button buttonPause;

        public bool IsPlayerAlive { get; private set; }

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
            GameAnalytics.Instance.TrackLevelStart(GameManager.Instance.CurrentLevel.number);

            IsPlayerAlive = true;
            UIPauseMenuController.Instance.LevelController = this;
        }

        private void Initialize()
        {
            AudioManager.Instance.SetBGM(levelBGM);

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
                    player.onDead = OnPlayerDead;
                    players.Add(player);
                    CameraManager.Instance.AddObjectToTrack(item);
                }
                if (item.TryGetComponent(out Finish finish))
                {
                    finishLines.Add(finish);
                    finish.onEnter = OnPlayerEnter;
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
                enemy.AllPlayers = players;
            }
        }

        private void OnPlayerEnter()
        {
            if (GameManager.Instance.State == GameManager.GameState.End)
                return;

            Vibration.TriggerTaptic(1);

            foreach (var finish in finishLines)
            {
                if (!finish.IsFinish)
                    return;
            }

            AudioManager.Instance.PlaySFX(audioFinish);

            GameManager.Instance.LevelComplete();
            GameManager.Instance.LevelNext();
        }

        private void OnPlayerDead(Player player)
        {
            if (IsPlayerAlive)
                GameAnalytics.Instance.TrackPlayerDied(GameManager.Instance.CurrentLevel.number);

            IsPlayerAlive = false;
            InputManager.Instance.HideControls();

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

            foreach (var enemy in enemies)
            {
                enemy.enabled = false;
            }

            Vibration.TriggerTaptic(3);
        }

        public void ButtonPause_OnClicked()
        {
            AudioManager.Instance.PauseBGM();
            UIPauseMenuController.Instance.Show();
        }

        public void ButtonRetry_OnClicked()
        {
            GameManager.Instance.LevelExit();
            GameManager.Instance.LevelRetry(false);
        }
    }
}
