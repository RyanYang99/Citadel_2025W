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

        private static float GetValueFromVolume(float volume) => Mathf.Log10(Mathf.Clamp(volume, 0.001f, 1f)) * 20f;

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

        private void MasterVolume(float volume) => mixer.SetFloat("Master", GetValueFromVolume(volume));
        
        private void BGMVolume(float volume) => mixer.SetFloat("BgSound", GetValueFromVolume(volume));

        private void SFXVolume(float volume) => mixer.SetFloat("SFX", GetValueFromVolume(volume));

        public void SetVolume(VolumeType volumeType, float volume)
        {
            switch (volumeType)
            {
                case VolumeType.BackgroundMusic:
                    BGMVolume(volume);
                    break;
                
                case VolumeType.SoundEffect:
                    SFXVolume(volume);
                    break;
                
                case VolumeType.Master:
                default:
                    MasterVolume(volume);
                    //BGMVolume(volume);
                    //SFXVolume(volume);
                    break;
            }
        }

        public void PlayButtonClick() => SFXXPlay("Button", buttonClickClip);
    }
}