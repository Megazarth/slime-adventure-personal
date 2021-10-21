using Kaizen;
using UnityEngine;
using UnityEngine.UI;

namespace Slime
{
    public class UIMainMenuController : PanelManager
    {
        public AudioClip mainMenuBGM;
        [Header("Buttons")]
        public Toggle toggleMusic;
        public Toggle toggleSFX;
        [Space]
        public UIExitConfirmationController confirmationController;

        protected override void Start()
        {
            base.Start();
            AudioManager.Instance.SetBGM(mainMenuBGM);

            toggleMusic.isOn = !AudioManager.Instance.IsMusicMuted;
            toggleSFX.isOn = !AudioManager.Instance.IsSFXMuted;
        }

        private void Update()
        {
#if !UNITY_EDITOR
            // Make sure user is on Android platform
            if (Application.platform == RuntimePlatform.Android)
            {
#endif
            // Check if Back was pressed this frame
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (buttonBack.gameObject.activeSelf)
                    ButtonBack_OnClicked();
                else
                    ShowConfirmationExit();
            }
#if !UNITY_EDITOR
            }
#endif
        }

        private void ShowConfirmationExit()
        {
            confirmationController.Show();
        }

        public void ButtonMusicToggle_OnClicked(bool state)
        {
            AudioManager.Instance.ToggleBGM(state);
        }

        public void ButtonSFXToggle_OnClicked(bool state)
        {
            AudioManager.Instance.ToggleSFX(state);
        }
    }
}

