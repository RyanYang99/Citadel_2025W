using UnityEngine.UI;
using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;

namespace Citadel
{
    public sealed class InventoryUIController : MonoBehaviour
    {
        private readonly Dictionary<Item, int> _lastValues = new();
        
        [SerializeField] private Inventory inventory;
        [SerializeField] private IconTable iconTable;
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private Transform contentParent;
        [SerializeField] private GameObject resourceRowPrefab;

        private void Start() => BuildUI();

        private void Update()
        {
            if (!inventoryPanel.activeSelf)
                return;

            bool changed = false;

            foreach (Item item in Enum.GetValues(typeof(Item)))
            {
                int current = inventory.GetAmount(item);

                if (!_lastValues.ContainsKey(item) || _lastValues[item] != current)
                {
                    _lastValues[item] = current;
                    changed = true;
                }
            }

            if (changed)
                Refresh();
        }

        private void BuildUI()
        {
            foreach (Transform child in contentParent)
                Destroy(child.gameObject);

            foreach (Item item in Enum.GetValues(typeof(Item)))
            {
                GameObject row = Instantiate(resourceRowPrefab, contentParent);

                TMP_Text[] texts = row.GetComponentsInChildren<TMP_Text>();
                texts[0].text = item.ToString();
                texts[1].text = inventory.GetAmount(item).ToString();

                Image iconImage = row.transform.Find("IconImage").GetComponent<Image>();
                iconImage.sprite = iconTable.Find(item);
            }
        }

        private void Refresh()
        {
            int index = 0;

            foreach (Item item in Enum.GetValues(typeof(Item)))
            {
                Transform row = contentParent.GetChild(index);
                TMP_Text amountText = row.GetComponentsInChildren<TMP_Text>()[1];
                amountText.text = inventory.GetAmount(item).ToString();
                ++index;
            }
        }
        
        public void Open()
        {
            inventoryPanel.SetActive(true);
            Refresh();
        }
    }
}