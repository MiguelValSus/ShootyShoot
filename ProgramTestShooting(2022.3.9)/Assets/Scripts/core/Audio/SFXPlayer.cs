using UnityEngine;

namespace core.Audio {
    public class SFXPlayer : MonoBehaviour
    {
        [Header("Audio")]
        public AudioSource Audio;
        public AudioClip m_sound_effect;

        #region SFX Playback
        public void PlaySound()
        {
            AudioManager.Instance.PlaySFX(m_sound_effect, transform, Audio.volume);
        }
        #endregion
    }
}