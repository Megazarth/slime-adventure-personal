using Kaizen;
using TMPro;
using UnityEngine;


namespace Slime
{
    public class UILevelSelectionController : Panel
    {
        [SerializeField] private TextMeshProUGUI collectionNumberText = null;
        [SerializeField] private TextMeshProUGUI collectionTitleText = null;
        [SerializeField] private RectTransform levelButtonsContainer = null;

        private UILevelButton[] levelButtons = new UILevelButton[10];

        private Collection collection;

        public UILevelButton levelButtonPrefab;

        protected override void Awake()
        {
            base.Awake();

            for (int i = 0; i < levelButtons.Length; i++)
            {
                var levelButton = Instantiate(levelButtonPrefab);
                levelButton.transform.SetParent(levelButtonsContainer);

                levelButtons[i] = levelButton;
            }
        }

        public void Initialize(Collection collection)
        {
            this.collection = collection;
            collectionNumberText.SetText($"Collection {collection.number}");
            collectionTitleText.SetText(collection.name);
            SetupButtons();
        }

        private void SetupButtons()
        {
            for (int i = 0; i < levelButtons.Length; i++)
            {
                var levelButton = levelButtons[i];
                levelButton.button.onClick.RemoveAllListeners();

                if (i >= collection.levels.Count)
                {
                    levelButton.gameObject.SetActive(false);
                }
                else
                {
                    var index = i;
                    levelButton.gameObject.SetActive(true);

                    int levelNumber = collection.levels[index].number;
                    levelButton.numberText.SetText(levelNumber.ToString());

                    levelButton.button.onClick.AddListener(() =>
                    {
                        GameManager.Instance.CurrentCollection = collection;
                        GameManager.Instance.CurrentLevel = collection.levels[index];
                        GameManager.Instance.CurrentLevelIndex = index;
                        GameManager.Instance.LevelRetry();
                    });

                    var key = AppManager.Instance.GetPersistentLevelFormating(collection.levels[index].number);
                    levelButton.ToggleCompletionBadge(EncryptedPrefs.HasKey(key));
                }
            }
        }
    }
}
