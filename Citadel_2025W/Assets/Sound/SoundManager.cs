using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public AudioClip buttonClickClip;
    public AudioMixer mixer;

    public AudioClip[] bglist;

    public static SoundManager Instance;

    private AudioSource bgSound;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            bgSound = gameObject.AddComponent<AudioSource>();
            bgSound.loop = true;
            bgSound.playOnAwake = false;

            SceneManager.sceneLoaded += OnSceneLoaded;

            OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        for (int i = 0; i < bglist.Length; i++)
        {
            if (arg0.name == bglist[i].name)
            {
                BgSoundPlay(bglist[i]);
                return;
            }
        }
    }

    private void BGMVolume(float val)
    {
        mixer.SetFloat("BGMVolume", Mathf.Log10(Mathf.Clamp(val, 0.0001f, 1f)) * 20);
    }

    private void SFXVolume(float val)
    {
        mixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Clamp(val, 0.0001f, 1f)) * 20);
    }

    public void SFXXPlay(string sfxName, AudioClip clip)
    {
        if (clip == null) return;

        GameObject go = new GameObject(sfxName + "Sound");
        AudioSource audioSource = go.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];
        audioSource.clip = clip;
        audioSource.Play();

        Destroy(go, clip.length);
    }

    public void BgSoundPlay(AudioClip clip)
    {
        if (clip == null) return;

        bgSound.outputAudioMixerGroup = mixer.FindMatchingGroups("BgSound")[0];

        if (bgSound.clip == clip && bgSound.isPlaying)
            return;

        bgSound.clip = clip;
        bgSound.loop = true;
        bgSound.volume = 1.0f;
        bgSound.Play();
    }

    public void PlayButtonClick()
    {
        SFXXPlay("Button", buttonClickClip);
    }
}