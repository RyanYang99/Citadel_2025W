using UnityEngine;
using UnityEngine.UI;

namespace Citadel
{
    public sealed class OptionUIController : MonoBehaviour
    {
        [Header("Master")]
        [SerializeField] private Slider masterSlider;
        [SerializeField] private Image masterIcon;
        [SerializeField] private Sprite masterOnSprite;
        [SerializeField] private Sprite masterOffSprite;


        [Header("BGM")]
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Image bgmIcon;
        [SerializeField] private Sprite bgmOnSprite;
        [SerializeField] private Sprite bgmOffSprite;

        [Header("SFX")]
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Image sfxIcon;
        [SerializeField] private Sprite sfxOnSprite;
        [SerializeField] private Sprite sfxOffSprite;

        private float lastMasterVolume = 1f;
        private float lastBgmVolume = 1f;
        private float lastSfxVolume = 1f;
        private bool isMasterMuted;
        private bool isBgmMuted;
        private bool isSfxMuted;

        private void Awake()
        {
            // Slider 기본 설정
            InitSlider(masterSlider, 1f);
            InitSlider(bgmSlider, 1f);
            InitSlider(sfxSlider, 1f);

            isMasterMuted = false;
            isBgmMuted = false;
            isSfxMuted = false;

            // 리스너 등록
            masterSlider.onValueChanged.AddListener(OnChangeMaster);
            bgmSlider.onValueChanged.AddListener(OnChangeBGM);
            sfxSlider.onValueChanged.AddListener(OnChangeSFX);

            UpdateMasterIcon(1f);
            UpdateBgmIcon(1f);
            UpdateSfxIcon(1f);
        }

        private void InitSlider(Slider slider, float value)
        {
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.SetValueWithoutNotify(value);
        }
        // ================= MASTER =================
        public void OnChangeMaster(float value)
        {
            UpdateMasterIcon(value);

            lastMasterVolume = value;

            SoundManager.Instance?.SetMasterVolume(value);
        }

        public void OnClickMasterToggle()
        {
            if (!isMasterMuted)
            {
                lastMasterVolume = masterSlider.value;
                isMasterMuted = true;

                masterSlider.SetValueWithoutNotify(0f);
                UpdateMasterIcon(0f);
            }
            else
            {
                isMasterMuted = false;
                float restore = lastMasterVolume <= 0f ? 0.5f : lastMasterVolume;

                masterSlider.SetValueWithoutNotify(restore);
                UpdateMasterIcon(restore);
            }
        }

        private void UpdateMasterIcon(float value)
        {
            masterIcon.sprite = value <= 0.001f ? masterOffSprite : masterOnSprite;
        }


        // ================= BGM =================
        public void OnChangeBGM(float value)
        {
            UpdateBgmIcon(value);

            lastBgmVolume = value;

            SoundManager.Instance?.SetBGMVolume(value);
        }

        public void OnClickBgmToggle()
        {
            if (!isBgmMuted)
            {
                lastBgmVolume = bgmSlider.value;
                isBgmMuted = true;

                bgmSlider.SetValueWithoutNotify(0f);
                UpdateBgmIcon(0f);
            }
            else
            {
                isBgmMuted = false;
                float restore = lastBgmVolume <= 0f ? 0.5f : lastBgmVolume;

                bgmSlider.SetValueWithoutNotify(restore);
                UpdateBgmIcon(restore);
            }
        }

        private void UpdateBgmIcon(float value)
        {
            bgmIcon.sprite = value <= 0.001f ? bgmOffSprite : bgmOnSprite;
        }

        // ================= SFX =================
        public void OnChangeSFX(float value)
        {
            UpdateSfxIcon(value);

            lastSfxVolume = value;

            SoundManager.Instance?.SetSFXVolume(value);
        }

        public void OnClickSfxToggle()
        {
            if (!isSfxMuted)
            {
                lastSfxVolume = sfxSlider.value;
                isSfxMuted = true;

                sfxSlider.SetValueWithoutNotify(0f);
                UpdateSfxIcon(0f);
            }
            else
            {
                isSfxMuted = false;
                float restore = lastSfxVolume <= 0f ? 0.5f : lastSfxVolume;

                sfxSlider.SetValueWithoutNotify(restore);
                UpdateSfxIcon(restore);
            }
        }

        private void UpdateSfxIcon(float value)
        {
            sfxIcon.sprite = value <= 0.001f ? sfxOffSprite : sfxOnSprite;
        }

        // ================= Panel =================
        public void Open()
        {
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}
