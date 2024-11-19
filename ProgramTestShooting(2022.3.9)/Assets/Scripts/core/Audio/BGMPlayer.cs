using UnityEngine;

namespace core.Audio {
    public class BGMPlayer : MonoBehaviour
    {
        [Header("Audio")]
        public AudioSource Audio;
        public AudioClip m_music_track;

        #region BGM Playback
        public void PlayMusic()
        {
            AudioManager.Instance.PlaySoundtrack();
        }
        #endregion
    }
}