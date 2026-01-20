using UnityEngine;

public class OptionUI : MonoBehaviour
{
    public static OptionUI Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void SetMaster(float value)
    {
        SoundManager.Instance?.SetMasterVolume(value);
    }

    public void SetBGM(float value)
    {
        SoundManager.Instance?.SetBGMVolume(value);
    }

    public void SetSFX(float value)
    {
        SoundManager.Instance?.SetSFXVolume(value);
    }
}