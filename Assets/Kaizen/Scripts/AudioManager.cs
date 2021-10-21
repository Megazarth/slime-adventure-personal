using Kaizen;
using UnityEngine;

namespace Slime
{
    public class AudioManager : SingletonComponent<AudioManager>
    {
        public enum AudioType
        {
            Music,
            Sfx
        }
        public bool IsMusicMuted { get => audioSourceBGM.mute; }
        public bool IsSFXMuted { get => audioSourceSFX.mute; }

        public AudioSource audioSourceBGM;
        public AudioSource audioSourceSFX;

        public void PlayBGM()
        {
            audioSourceBGM.Play();
        }

        public void PauseBGM()
        {
            audioSourceBGM.Pause();
        }

        public void SetBGM(AudioClip clip)
        {
            audioSourceBGM.Pause();
            audioSourceBGM.clip = clip;
            audioSourceBGM.Play();
        }

        public void PlaySFX(AudioClip clip)
        {
            audioSourceSFX.PlayOneShot(clip);
        }

        public void ToggleBGM(bool state)
        {
            audioSourceBGM.mute = !state;
        }

        public void ToggleSFX(bool state)
        {
            audioSourceSFX.mute = !state;
        }
    }

}