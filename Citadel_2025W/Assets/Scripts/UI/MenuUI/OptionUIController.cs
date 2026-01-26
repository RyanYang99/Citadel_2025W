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

        private float lastBgmVolume = 1f;
        private float lastSfxVolume = 1f;
        private bool isBgmMuted;
        private bool isSfxMuted;

        private void Awake()
        {
            // Slider 기본 설정
            InitSlider(bgmSlider, 1f);
            InitSlider(sfxSlider, 1f);

            isBgmMuted = false;
            isSfxMuted = false;

            // 리스너 등록
            bgmSlider.onValueChanged.AddListener(OnChangeBGM);
            sfxSlider.onValueChanged.AddListener(OnChangeSFX);

            UpdateBgmIcon(1f);
            UpdateSfxIcon(1f);
        }

        private void InitSlider(Slider slider, float value)
        {
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.SetValueWithoutNotify(value);
        }

        // ================= BGM =================
        public void OnChangeBGM(float value)
        {
            if (value <= 0.001f)
            {
                isBgmMuted = true;
                UpdateBgmIcon(0f);
                return;
            }

            isBgmMuted = false;
            lastBgmVolume = value;
            UpdateBgmIcon(value);

            // SoundManager 연동 시
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
            if (value <= 0.001f)
            {
                isSfxMuted = true;
                UpdateSfxIcon(0f);
                return;
            }

            isSfxMuted = false;
            lastSfxVolume = value;
            UpdateSfxIcon(value);

            // SoundManager 연동
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
