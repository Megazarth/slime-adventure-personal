using Kaizen;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;


namespace Slime
{
    public class MenuController : MonoBehaviour
    {
        [Tooltip("These buttons will be disabled when animation is played, and enabled after.")]
        [SerializeField] private List<Button> buttons = new List<Button>();
        private MenuAnimator menuAnimator = null;

        public List<Button> Buttons { get => buttons; set => buttons = value; }


        private void Awake()
        {
            menuAnimator = GetComponent<MenuAnimator>();
        }

        private void OnEnable()
        {
            menuAnimator.onPreShowAnimationEvent += DisableButtons;
            menuAnimator.onPostShowAnimationEvent += EnableButtons;
        }

        private void Start()
        {
            int screenWidth = Screen.currentResolution.width;
            menuAnimator.ShowAnimationOffset = screenWidth;
            menuAnimator.HideAnimationOffset = screenWidth;
        }

        private void OnDisable()
        {
            menuAnimator.onPreShowAnimationEvent -= DisableButtons;
            menuAnimator.onPostShowAnimationEvent -= EnableButtons;
        }

        public void Quit()
        {
            AppManager.Instance.Quit();
        }

        private void DisableButtons()
        {
            foreach (Button button in buttons)
            {
                if (button)
                {
                    button.interactable = false;
                }
            }
        }

        private void EnableButtons()
        {
            foreach (Button button in buttons)
            {
                if (button)
                {
                    button.interactable = true;
                }
            }
        }
    }
}
