using UnityEngine;
using UnityEngine.UI;


namespace Slime
{
    public class VolumeToggleController : MonoBehaviour
    {
        public Toggle toggleButton;
        public AudioManager.AudioType audioType;
        public MenuAnimator parentMenu;

        private void OnEnable()
        {
            parentMenu.onPreShowAnimationEvent += GetAudioInfo;
        }

        private void Start()
        {
            GetAudioInfo();
        }

        private void OnDisable()
        {
            parentMenu.onPreShowAnimationEvent -= GetAudioInfo;
        }

        private void GetAudioInfo()
        {
            toggleButton.isOn = (audioType == AudioManager.AudioType.Music ? AudioManager.Instance.IsMusicMuted : AudioManager.Instance.IsSFXMuted);
        }
    }
}
