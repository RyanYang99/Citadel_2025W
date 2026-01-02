using Citadel;
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


        private void OnEnable()
        {
            inventory.OnItemChange += OnItemChange;
            ForceRefresh();
        }

        private void OnDisable() => inventory.OnItemChange -= OnItemChange;

        private void ForceRefresh() => Refresh(inventory.GetAmount(item));
        
        private void OnItemChange(Item _, int amount) => Refresh(amount);

        private void Refresh(int amount) => amountText.text = amount.ToString();
    }
}
