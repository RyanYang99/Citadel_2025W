using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace Citadel
{
    public sealed class SoundManager : PersistentSingleton<SoundManager>
    {
        [SerializeField] private AudioClip buttonClickClip;
        [SerializeField] private AudioMixer mixer;
        [SerializeField] private AudioSource bgSound;
        [SerializeField] private AudioClip[] bglist;

        private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;

        private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

        private void OnSceneLoaded(Scene scene, LoadSceneMode _)
        {
            foreach (AudioClip audioClip in bglist)
                if (scene.name == audioClip.name)
                    BgSoundPlay(audioClip);
        }

        private void SFXXPlay(string sfxName, AudioClip clip)
        {
            GameObject go = new GameObject(sfxName + "Sound");
            AudioSource audioSource = go.AddComponent<AudioSource>();
            audioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];
            audioSource.clip = clip;
            audioSource.Play();

            Destroy(go, clip.length);
        }

        private void BgSoundPlay(AudioClip clip)
        {
            bgSound.outputAudioMixerGroup = mixer.FindMatchingGroups("BgSound")[0];
            bgSound.clip = clip;
            bgSound.loop = true;
            bgSound.volume = 1.0f;
            bgSound.Play();
        }
        
        public void BGMVolume(float val) => mixer.SetFloat("BGMVolume", Mathf.Log10(val) * 20);

        public void SFXVolume(float val) => mixer.SetFloat("SFXVolume", Mathf.Log10(val) * 20);

        public void PlayButtonClick() => SFXXPlay("Button", buttonClickClip);
    }
}