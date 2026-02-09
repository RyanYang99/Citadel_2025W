using TMPro;
using UnityEngine;

namespace Citadel
{
    public sealed class CostItemController : MonoBehaviour
    {
        private Inventory _inventory;
        private ItemAmount _itemAmount;
        
        [SerializeField] private ResourceUI resourceUI;
        [SerializeField] private TMP_Text text;

        private void OnDestroy() => _inventory.OnTick -= Refresh;

        public void Initialize(ItemAmount itemAmount)
        {
            _inventory = FindAnyObjectByType<Inventory>();
            _itemAmount = itemAmount;
            
            resourceUI.SetItem(itemAmount.item);

            _inventory.OnTick += Refresh;
            Refresh();
        }

        private void Refresh() => text.text = $"{_itemAmount.item}: {_inventory.GetAmount(_itemAmount.item)} / {_itemAmount.amount}";
    }
}