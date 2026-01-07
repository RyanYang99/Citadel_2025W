using UnityEngine;
using UnityEngine.Audio;

namespace Citadel
{
    public sealed class OptionUIController : MonoBehaviour
    {
        [Header("AudioManager")]
        [SerializeField] private AudioMixer audioMixer;

        public void OnChangeMasterVolume(float value)
        {
            // Slider °ª
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20f);
        }

        public void OnChangeFullscreen(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
        }

        public void OnChangeResolution(int index)
        {
            Resolution[] resolutions = Screen.resolutions;

            if (index < 0 || index >= resolutions.Length)
                return;

            Resolution res = resolutions[index];
            Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        }
    }
}
