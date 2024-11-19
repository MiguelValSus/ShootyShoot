using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace core.Audio { 
    public class AudioManager : SingletonBase<AudioManager>
    {
        [Header("Audio sources")]
        public AudioSource BGMPlayer;
        [Header("Music files")]
        public AudioClip   MainMenuMusic;
        public AudioClip   StageTrack;
        public AudioClip   BossTheme;
        public AudioClip   StageClearMusic;

        #region Initialization
        private void Awake()
        {
            InitSoundscape();
        }

        private void Start()
        {
            PlayMusic(MainMenuMusic);
        }

        private void OnDestroy() { }

        private void InitSoundscape()
        {
            Instance = this;
        }
        #endregion

        #region External Audio callbacks
        public void PlaySoundtrack()
        {
            CrossfadeToNewMusic(StageTrack, .4f);
        }

        public void PlaySFX(AudioClip sfxClip, Transform originPoint, float volume = 1f)
        {
            if (sfxClip == null) return;
            PlaySound(sfxClip, originPoint, volume);
        }

        public void CrossfadeToNewMusic(AudioClip nextTrack, float nextVolume = 1f)
        {
            StartCoroutine(CrossfadeMusic(nextTrack, nextVolume));
        }
        #endregion

        #region BGM Playback
        private void PlayMusic(AudioClip musicTrack)
        {
            BGMPlayer.clip = musicTrack;
            if (BGMPlayer.clip != null) BGMPlayer.Play();
        }

        private IEnumerator CrossfadeMusic(AudioClip nextmusicTrack, float nextVolume = 1f)
        {
            var startVolume = nextVolume == 1f ? BGMPlayer.volume : nextVolume;

            // Fade out the current music
            for (var t = 0f; t < 1f; t += Time.deltaTime)
            {
                BGMPlayer.volume = Mathf.Lerp(startVolume, 0f, t / 1f);
                yield return null;
            }
            BGMPlayer.volume = 0f;
            BGMPlayer.Stop();

            // Switch music track
            BGMPlayer.clip = nextmusicTrack;
            BGMPlayer.Play();

            // Fade in the new music
            for (float t = 0; t < .5f; t += Time.deltaTime)
            {
                BGMPlayer.volume = Mathf.Lerp(0f, startVolume, t / .5f);
                yield return null;
            }

            BGMPlayer.volume = startVolume;
        }
        #endregion

        #region SFX Playback
        private void PlaySound(AudioClip sfxClip, Transform originPoint, float volume = 1f)
        {
            // Create a one-shot audio source
            var audioObject = new GameObject("OneShotAudio");
            audioObject.transform.position = originPoint.position;
            AudioSource audioSource = audioObject.AddComponent<AudioSource>();
            audioSource.clip = sfxClip;
            audioSource.volume = volume;
            audioSource.Play();
            // Destroy the one-shot source after its clip has finished playing so memory is not saturated in longer play sessions
            var clipLength = sfxClip.length;
            Destroy(audioSource.gameObject, clipLength);
        }

        private void PlayRandomSound(List<AudioClip> sfxClips, Transform originPoint, float volume = 1f)
        {
            if (sfxClips.Count <= 0) return;

            // Pick a random SFX clip
            int randomIndex = Random.Range(0, sfxClips.Count);
            // Create a one-shot audio source
            AudioSource audioSource = Instantiate(new AudioSource(), originPoint.position, Quaternion.identity);
            float clipLength = sfxClips[randomIndex].length;
            audioSource.clip = sfxClips[randomIndex];
            audioSource.volume = volume;
            audioSource.Play();
            // Destroy the one-shot source after its clip has finished playing so memory is not saturated in longer play sessions
            Destroy(audioSource.gameObject, clipLength);
        }
        #endregion
    }
}