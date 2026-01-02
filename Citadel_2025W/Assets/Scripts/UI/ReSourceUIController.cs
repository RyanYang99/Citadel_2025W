using UnityEngine;
using Citadel;

public class ReSourceUIController : MonoBehaviour
{
    [Header("Data")]
    public Inventory inventory;
    public ItemIconTable iconTable;

    [Header("Slots")]
    public ResourceUI[] slots;

    void Start()
    {
        InitIcons();
        ForceRefresh();
    }

    void Update()
    {
        UpdateIfChanged();
    }

    // 아이콘 1회 세팅
    void InitIcons()
    {
        foreach (var slot in slots)
        {
            var data = iconTable.Get(slot.item);
            slot.iconImage.sprite = data.icon;
        }
    }

    // 값 변경 감지
    void UpdateIfChanged()
    {
        foreach (var slot in slots)
        {
            int current = inventory.GetAmount(slot.item);

            if (slot.lastValue != current)
            {
                slot.lastValue = current;
                slot.amountText.text = current.ToString();
            }
        }
    }

    // 강제 갱신
    void ForceRefresh()
    {
        foreach (var slot in slots)
        {
            int value = inventory.GetAmount(slot.item);
            slot.lastValue = value;
            slot.amountText.text = value.ToString();
        }
    }
}
