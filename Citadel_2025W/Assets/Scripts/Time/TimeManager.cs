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

        private DateTime _timeElapsed = DateTime.MinValue + new TimeSpan(12, 0, 0);

        public DateTime TimeElapsed
        {
            get => _timeElapsed;

            private set
            {
                int hourBefore = _timeElapsed.Hour;
                _timeElapsed = value;
                
                if (hourBefore != _timeElapsed.Hour)
                    OnHourChange?.Invoke(_timeElapsed.Hour);

                bool isDayBefore = IsDay((_timeElapsed.Hour + 23) % 24), isDayNow = IsDay(_timeElapsed.Hour);
                switch (isDayBefore)
                {
                    case true when !isDayNow:
                        OnNight?.Invoke();
                        break;
                    
                    case false when isDayNow:
                        OnDay?.Invoke();
                        break;
                }
            }
        }

        public event Action<float> OnTimeScaleChange;
        public event Action<int> OnHourChange;
        
        /*
            6 ~ 17 (5): Day
            Other: Night
        */
        public event Action OnDay, OnNight;
        
        private static bool IsDay(int hour) => hour is >= 6 and <= 17;

        private void OnValidate() => UpdateLightning();
        
        private void Awake() => _factor = 1440f / minutesPerOneGameDay;

        private void Update()
        {
            TimeElapsed = TimeElapsed.AddSeconds(Time.deltaTime * _factor);
            
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

        public void Load(DateTime dateTime) => TimeElapsed = dateTime;
    }
}