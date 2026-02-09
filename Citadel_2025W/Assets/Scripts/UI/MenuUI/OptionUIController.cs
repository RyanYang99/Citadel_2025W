using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Citadel
{
    public sealed class OptionUIController : MonoBehaviour
    {
        private class VolumeStatus
        {
            public bool Muted;
            public float Volume = 1f;
        }
        
        [Serializable]
        private class VolumeControl
        {
            public Slider slider;
            public Image icon;
        }

        private readonly Dictionary<VolumeType, VolumeControl> _volumeControls = new();
        private readonly Dictionary<VolumeType, VolumeStatus> _volumeStatuses = new();

        [Header("Common"), SerializeField] private Sprite off;
        [SerializeField] private Sprite on;

        [Header("Controls"), SerializeField] private VolumeControl master;
        [SerializeField] private VolumeControl backgroundMusic, soundEffect;
        
        private void Awake()
        {
            _volumeControls.Add(VolumeType.Master, master);
            _volumeStatuses.Add(VolumeType.Master, new VolumeStatus());
            
            _volumeControls.Add(VolumeType.BackgroundMusic, backgroundMusic);
            _volumeStatuses.Add(VolumeType.BackgroundMusic, new VolumeStatus());
            
            _volumeControls.Add(VolumeType.SoundEffect, soundEffect);
            _volumeStatuses.Add(VolumeType.SoundEffect, new VolumeStatus());

            foreach (VolumeType volumeType in _volumeControls.Keys)
                _volumeControls[volumeType].slider.value = _volumeStatuses[volumeType].Volume;
        }
        
        /*
        private void UpdateBgmIcon(float value) => backgroundMusic.icon.sprite = value <= 0.001f ? off : on;
        
        private void UpdateSfxIcon(float value) => soundEffect.icon.sprite = value <= 0.001f ? off : on;
        */

        private void OnVolumeChange(VolumeType volumeType, float value)
        {
            if (value <= 0.001f)
                value = 0f;
            
            _volumeStatuses[volumeType].Volume = value;
            _volumeControls[volumeType].icon.sprite = value > 0f ? on : off;
            
            if (SoundManager.Instance != null && !_volumeStatuses[volumeType].Muted)
                SoundManager.Instance.SetVolume(volumeType, value);
        }

        private void OnToggle(VolumeType volumeType)
        {
            VolumeStatus volumeStatus = _volumeStatuses[volumeType];
            volumeStatus.Muted = !volumeStatus.Muted;
            
            _volumeControls[volumeType].icon.sprite = volumeStatus.Muted ? off : on;
            if (SoundManager.Instance != null)
                SoundManager.Instance.SetVolume(volumeType, volumeStatus.Muted ? 0f : volumeStatus.Volume);
        }

        public void OnMasterChange(float value) => OnVolumeChange(VolumeType.Master, value);

        public void OnBackgroundMusicChange(float value) => OnVolumeChange(VolumeType.BackgroundMusic, value);

        public void OnSoundEffectChange(float value) => OnVolumeChange(VolumeType.SoundEffect, value);

        public void OnMasterToggle() => OnToggle(VolumeType.Master);

        public void OnBackgroundMusicToggle() => OnToggle(VolumeType.BackgroundMusic);

        public void OnSoundEffectToggle() => OnToggle(VolumeType.SoundEffect);
        
        public void Open() => gameObject.SetActive(true);

        public void Close() => gameObject.SetActive(false);
    }
}