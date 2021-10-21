using Kaizen;
using UnityEngine;

namespace Slime
{
    public class UICollectionSelectionController : Panel
    {
        [SerializeField] private ButtonLevelCollection collectionButton = null;
        [SerializeField] private UILevelSelectionController levelSelectionController = null;
        public RectTransform rectLayout;

        protected override void Start()
        {
            base.Start();
            SetupButtons();
        }

        private void SetupButtons()
        {
            var collection = GameManager.Instance.MainCollection;
            while (collection != null)
            {
                var button = Instantiate(collectionButton, Vector3.zero, Quaternion.identity, rectLayout);
                button.Initialize(collection);
                button.passCollection = (value) => levelSelectionController.Initialize(value);
                button.button.onClick.AddListener(() => ButtonShowNextPanel_OnClicked(levelSelectionController));
                collection = collection.next;
            }
        }
    }
}
