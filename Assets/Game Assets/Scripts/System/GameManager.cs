using Kaizen;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.SceneManagement;

namespace Slime
{
    public class GameManager : SingletonComponent<GameManager>
    {
        public enum GameState
        {
            Initialization,
            MainMenu,
            Preparation,
            Start,
            End,
        }

        private Scene currentLoadedScene;
        private float levelTimestamp;

        private bool isRetrying;

        [SerializeField]
        private Collection mainCollection = null;

        private Action onTransitionShowCompleted;

        public AudioClip audioButtonClicked;
        public AudioClip audioWindowClosed;
        public Collection MainCollection { get => mainCollection; }
        public GameState State { get; private set; }

#if UNITY_EDITOR
        public bool IsEditMode { get; private set; }
#endif

        public Collection CurrentCollection { get; set; }
        public Level CurrentLevel { get; set; }
        public int CurrentLevelIndex { get; set; }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        public void Initialize()
        {
            AdsManager.Instance.Initialize();

            Physics2D.queriesStartInColliders = false;
#if UNITY_EDITOR
            if (SceneManager.sceneCount != 1)
            {
                IsEditMode = true;

                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    var scene = SceneManager.GetSceneAt(i);

                    if (scene.name == Global.SceneMain)
                        continue;

                    currentLoadedScene = scene;
                    TransitionAnimator.Instance.Hide();
                }

                if (currentLoadedScene.name.Contains(Global.LevelPrefix))
                {
                    UIPauseMenuController.Instance.textLevelTitle.text = "EDIT MODE";
                    UIPauseMenuController.Instance.textLevelNumber.text = $"{currentLoadedScene.name}";
                    State = GameState.Start;

                    InputManager.Instance.ShowControls();
                }
                else if (!currentLoadedScene.name.Contains(Global.SceneMainMenu))
                {
                    currentLoadedScene = default;
                    StartCoroutine(LoadSceneEnumerator(Global.SceneMainMenu, () =>
                    {
                        TransitionAnimator.Instance.Hide(() => State = GameState.MainMenu, Color.black);
                    }));
                }

                SceneManager.sceneLoaded += OnSceneLoaded;
                SceneManager.sceneUnloaded += OnSceneUnloaded;

                return;
            }
#endif

#if DEVELOPMENT_BUILD
            SceneManager.LoadScene("Debug", LoadSceneMode.Additive);
#endif
            State = GameState.Initialization;

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;

            StartCoroutine(LoadSceneEnumerator(Global.SceneMainMenu, () =>
            {
                TransitionAnimator.Instance.Hide(() => State = GameState.MainMenu, Color.black);
            }));
        }

        public void SetCollectionLevelEditorMode(string name)
        {
            var collection = mainCollection;
            while (collection != null)
            {
                Level selectedLevel = null;
                foreach (var levelSample in collection.levels)
                {
                    var filename = Path.GetFileNameWithoutExtension(levelSample.scene.ScenePath);
                    if (filename.Contains(name))
                    {
                        selectedLevel = levelSample;
                        break;
                    }
                }

                if (selectedLevel != null && selectedLevel.scene != null)
                {
                    selectedLevel.description = collection.name;
                    CurrentCollection = collection;
                    CurrentLevel = selectedLevel;
                    break;
                }

                collection = collection.next;
            }
        }

        public void Pause()
        {
            Time.timeScale = 0.0f;
        }

        public void Play()
        {
            Time.timeScale = 1.0f;
        }

        public void LevelComplete()
        {
            State = GameState.End;

            var key = AppManager.Instance.GetPersistentLevelFormating(CurrentLevel.number);
            var elapsedTime = Time.time - levelTimestamp;

            if (!EncryptedPrefs.HasKey(key))
                GameAnalytics.Instance.TrackLevelComplete(CurrentLevel.number);

            EncryptedPrefs.SetFloat(key, elapsedTime);

            GameAnalytics.Instance.TrackLevelEnd(CurrentLevel.number);
            GameAnalytics.Instance.TrackLevelDuration(CurrentLevel.number, elapsedTime);
#if UNITY_EDITOR
            Debug.Log($"Level {CurrentLevel.number} with completion time: {elapsedTime} recorded!");
            EncryptedPrefs.SaveProgress();
#endif
        }

        /// <summary>
        /// Notify the Game Manager that a level is about to be abandoned.
        /// </summary>
        public void LevelExit()
        {
            State = GameState.End;
        }

        public void LevelNext()
        {
            CurrentLevelIndex += 1;
            if (CurrentLevelIndex == CurrentCollection.levels.Count)
            {
                if (CurrentCollection.next == null)
                {
                    LoadScene(Global.SceneMainMenu);
                }
                else
                {
                    CurrentCollection = CurrentCollection.next;
                    CurrentLevel = CurrentCollection.levels[0];
                    CurrentLevelIndex = 0;
                    LevelRetry();
                }
            }
            else
            {
                CurrentLevel = CurrentCollection.levels[CurrentLevelIndex];
                LevelRetry();
            }
        }

        public void LevelRetry(bool showInterstitial = true)
        {
            if (isRetrying)
                return;

            if (showInterstitial)
            {
                onTransitionShowCompleted = AdsManager.Instance.ShowIntersitial;
            }

            isRetrying = true;

            LoadLevel(CurrentLevel, () => isRetrying = false);
        }

        public void GoToStartScreen()
        {
            InputManager.Instance.HideControls();
            LoadScene(Global.SceneMainMenu);
        }

        private void LoadLevel(Level level, Action onComplete = null)
        {
            var scenename = Path.GetFileNameWithoutExtension(level.scene);

            InputManager.Instance.HideControls();
            State = GameState.Preparation;

            TransitionAnimator.Instance.Show(level.number, () =>
            {
                onTransitionShowCompleted?.Invoke();
                onTransitionShowCompleted = null;
                StartCoroutine(LoadSceneEnumerator(scenename, () =>
                    {
                        OnSceneLoaded();
                    }));
            });

            void OnSceneLoaded()
            {
                TransitionAnimator.Instance.Hide(() =>
                {
                    onComplete?.Invoke();
                    InputManager.Instance.ShowControls();
                    State = GameState.Start;
                    levelTimestamp = Time.time;
                });
            }
        }

        private void LoadScene(string sceneName, Color transitionColor, Action onComplete = null)
        {
            TransitionAnimator.Instance.Show(() =>
            {
                StartCoroutine(LoadSceneEnumerator(sceneName, () =>
                {
                    TransitionAnimator.Instance.Hide(onComplete, transitionColor);
                }));
            }, transitionColor);
        }

        private void LoadScene(string sceneName, Action onComplete = null)
        {
            TransitionAnimator.Instance.Show(() =>
            {
                UIPauseMenuController.Instance.canvas.enabled = false;
                StartCoroutine(LoadSceneEnumerator(sceneName, () =>
                {
                    TransitionAnimator.Instance.Hide(onComplete);
                }));
            });
        }

        private IEnumerator LoadSceneEnumerator(string sceneName, Action onComplete = null)
        {
            if (currentLoadedScene != null && currentLoadedScene.isLoaded)
            {
                var unloadSceneOperation = SceneManager.UnloadSceneAsync(currentLoadedScene);
                while (!unloadSceneOperation.isDone)
                {
                    yield return null;
                }
            }

            var loadSceneOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            while (!loadSceneOperation.isDone)
            {
                yield return null;
            }

            onComplete?.Invoke();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            currentLoadedScene = scene;
#if DEVELOPMENT_BUILD
             if (scene.name.Equals("Debug"))
                currentLoadedScene = default;
#endif
        }

        private void OnSceneUnloaded(Scene scene)
        {
            CameraManager.Instance.ClearTrackedObjects();
            GC.Collect();
        }

        public void ButtonCustom_OnClicked(ButtonCustom button)
        {
            AudioManager.Instance.PlaySFX(audioButtonClicked);
        }

        public void IAPManager_onPurchaseSuccess(Product product)
        {
            var removeads = string.Equals(product.definition.id, AppManager.Instance.RemoveAdsProductID, StringComparison.Ordinal);
            if (removeads)
            {
                var popup = AppManager.Instance.GetPopup() as UIPopupController;
                popup.Initialize("Purchase Successful", "Ads has been removed.");
                popup.Show();

                AdsManager.Instance.RemoveAds();

                GameAnalytics.Instance.TrackRemoveAdsBought((float)product.metadata.localizedPrice);
            }
        }

        public void IAPManager_OnProductPurchaseFailed(Product product, PurchaseFailureReason reason)
        {
            var removeads = string.Equals(product.definition.id, AppManager.Instance.RemoveAdsProductID, StringComparison.Ordinal);
            if (removeads)
            {
                var popup = AppManager.Instance.GetPopup() as UIPopupController;
                popup.Initialize("Purchase Failed", $"{reason.ToString()}");
                popup.Show();
            }
        }
    }

}