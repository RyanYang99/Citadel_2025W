using System;
using UnityEngine;

namespace Citadel
{
    public sealed class TimeManager : MonoBehaviour
    {
        private float _factor;
        
        [SerializeField] private int minutesPerOneGameDay = 2;
        [SerializeField, Tooltip("알파 = 밝기 (-2 ~ 0)")] private Gradient ambientLight;
        [SerializeField, Range(0f, 1f)] private float dayPercent;
        
        public DateTime TimeElapsed { get; private set; } = DateTime.MinValue + new TimeSpan(12, 0, 0);

        public event Action<float> OnTimeScaleChange;
        public event Action<int> OnHourChange;

        private void OnValidate() => UpdateLightning();
        
        private void Awake() => _factor = 1440f / minutesPerOneGameDay;

        private void Update()
        {
            int hourBefore = TimeElapsed.Hour;
            TimeElapsed = TimeElapsed.AddSeconds(Time.deltaTime * _factor);
            if (hourBefore != TimeElapsed.Hour)
                OnHourChange?.Invoke(TimeElapsed.Hour);
            
            dayPercent = (TimeElapsed.Hour * 60f + TimeElapsed.Minute) / 1440f;
            UpdateLightning();
        }

        private void UpdateLightning()
        {
            Color newAmbientLight = ambientLight.Evaluate(dayPercent);

            RenderSettings.ambientLight = newAmbientLight.linear * newAmbientLight.a;
            RenderSettings.sun.transform.rotation = Quaternion.Euler(360f * dayPercent - 90f, 45f, 0f);
        }

        public void SetTimeScale(float newTimeScale)
        {
            Time.timeScale = newTimeScale;
            OnTimeScaleChange?.Invoke(Time.timeScale);
        }
    }
}