using Citadel;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Citadel
{
    public sealed class ResourceUI : MonoBehaviour
    {
        [SerializeField] private Inventory inventory;
        [SerializeField] private Item item;
        [SerializeField] private TMP_Text amountText;
        [SerializeField] private Image iconImage;
        [SerializeField] private ItemIconTable iconTable;


        private void OnEnable()
        {
            inventory.OnItemChange += OnItemChange;

            SetIcon();  
            ForceRefresh();
        }

        private void SetIcon()
        {
            var data = iconTable.Get(item);
            if (data != null)
                iconImage.sprite = data.icon;
        }

        private void OnDisable() => inventory.OnItemChange -= OnItemChange;

        private void ForceRefresh() => Refresh(inventory.GetAmount(item));

        private void OnItemChange(Item itemChanged, int amount)
        {
            if (itemChanged == item)
                Refresh(amount);
        }

        private void Refresh(int amount) => amountText.text = amount.ToString();
    }
}
