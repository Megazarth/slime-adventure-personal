using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Slime
{
    public class ButtonLevelCollection : MonoBehaviour
    {
        private Collection collection;

        public TextMeshProUGUI textNumber;
        public TextMeshProUGUI textTitle;
        public Button button;

        public Action<Collection> passCollection;

        public void Initialize(Collection collection)
        {
            this.collection = collection;
            textNumber.text = $"{collection.number}";
            textTitle.text = $"{collection.name}";

            button.onClick.AddListener(OnClicked);
        }

        private void OnClicked()
        {
            passCollection.Invoke(collection);
        }
    }
}
