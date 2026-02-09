using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Citadel
{
    public sealed class ResourceUI : MonoBehaviour
    {
        private Inventory _inventory;
        
        [SerializeField] private IconTable iconTable;
        [SerializeField] private Item item;
        [SerializeField] private RangeResource rangeResource;
        [SerializeField] private TMP_Text amountText;
        [SerializeField] private Image iconImage;

        private void Awake()
        {
            _inventory = FindAnyObjectByType<Inventory>();
            ForceRefresh();
        }

        private void OnEnable()
        {
            _inventory.OnItemChange += OnItemChange;

            SetIcon();  
            ForceRefresh();
        }

        private void SetIcon()
        {
            if (iconImage == null) 
                return;

            if (item != Item.None)
                iconImage.sprite = iconTable.Find(item);
            else
                iconImage.sprite = iconTable.Find(rangeResource);
        }

        private void OnDisable() => _inventory.OnItemChange -= OnItemChange;

        private void OnItemChange(Item itemChanged, int amount)
        {
            if (itemChanged == item)
                Refresh(amount);
        }

        private void Refresh(int amount)
        {
            if (amountText != null)
                amountText.text = amount.ToString();
        }

        public void SetItem(Item _item)
        {
            item = _item;
            
            SetIcon();
            ForceRefresh();
        }

        public void SetRangeResource(RangeResource _rangeResource)
        {
            rangeResource = _rangeResource;
            
            SetIcon();
            ForceRefresh();
        }

        public void ForceRefresh() => Refresh(_inventory.GetAmount(item));
    }
}