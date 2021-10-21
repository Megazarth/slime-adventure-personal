using Kaizen;
using System;
using UnityEngine.UI;

namespace Slime
{
    public class SettingsController : Window
    {
        [Serializable]
        public struct Setting
        {
            public float audioVolumeMaster;
            public float audioVolumeMusic;
            public float audioVolumeSFX;
        }

        private Setting settings;

        public Slider sliderSoundMaster;
        public Slider sliderSoundMusic;
        public Slider sliderSoundSFX;

        protected override void Awake()
        {
            base.Awake();
            canvas.enabled = false;
        }

        public void Initialize()
        {
            if (EncryptedPrefs.HasKey(Global.KeySettings))
            {
                settings = (Setting)EncryptedPrefs.GetJson<Setting>(Global.KeySettings);
            }
            else
            {
                settings = new Setting()
                {
                    audioVolumeMaster = 1.0f,
                    audioVolumeMusic = 1.0f,
                    audioVolumeSFX = 1.0f,
                };
            }

            sliderSoundMaster.value = settings.audioVolumeMaster;
            sliderSoundMusic.value = settings.audioVolumeMusic;
            sliderSoundSFX.value = settings.audioVolumeSFX;
        }


        public void ShowButton()
        {
            base.Show(null);
        }

        public void HideButton()
        {
            SaveSettings();
            base.Hide(null);
        }

        public void SaveSettings()
        {
            EncryptedPrefs.SetJson(Global.KeySettings, settings);
        }
    }
}
