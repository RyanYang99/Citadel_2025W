using System.Collections.Generic;
using UnityEngine;

namespace Citadel
{
    public sealed class NightLight : MonoBehaviour
    {
        private TimeManager _timeManager;

        [SerializeField] private List<Light> lights = new();
        
        private void Awake() => _timeManager = FindAnyObjectByType<TimeManager>();

        private void OnEnable()
        {
            _timeManager.OnDay += OnDay;
            _timeManager.OnNight += OnNight;
        }

        private void OnDisable()
        {
            _timeManager.OnDay -= OnDay;
            _timeManager.OnNight -= OnNight;
        }

        private void OnDay()
        {
            foreach (Light _light in lights)
                _light.enabled = false;
        }

        private void OnNight()
        {
            foreach (Light _light in lights)
                _light.enabled = true;
        }
    }
}