using UnityEngine;

namespace Citadel
{
    public sealed class UIButtonSound : MonoBehaviour
    {
        public void OnClick() => SoundManager.Instance.PlayButtonClick();
    }
}