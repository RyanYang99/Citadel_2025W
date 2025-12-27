using System;
using UnityEngine;

namespace Citadel
{
    public sealed class GameManager : MonoBehaviour
    {
        public DateTime TimeElapsed { get; private set; } = DateTime.MinValue + new TimeSpan(12, 0, 0);

        public event Action<float> OnTimeScaleChange;

        private void Update()
        {
            TimeElapsed = TimeElapsed.AddSeconds(Time.deltaTime);
        }

        public void SetTimeScale(float newTimeScale)
        {
            Time.timeScale = newTimeScale;
            OnTimeScaleChange?.Invoke(Time.timeScale);
        }
    }
}