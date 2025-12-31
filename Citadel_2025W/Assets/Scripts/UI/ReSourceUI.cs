using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Citadel;

[System.Serializable]
public class ResourceUI
{
    public Item item;
    public Image iconImage;        // 아이콘 위치
    public TMP_Text amountText;    // 수량 텍스트

    [HideInInspector] public int lastValue = -1;
}
