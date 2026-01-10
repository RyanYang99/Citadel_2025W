using UnityEngine;

public class UIButtonSound : MonoBehaviour
{
    public void OnClick()
    {
        SoundManager.Instance.PlayButtonClick();
    }
}