using UnityEngine.UI;
using UnityEngine;
using Citadel;
using System;
using System.Collections.Generic;
using TMPro;

public class InventoryUIController : MonoBehaviour
{
    public Inventory inventory;

    public ItemIconTable iconTable;

    public GameObject inventoryPanel;
    public Transform contentParent;
    public GameObject resourceRowPrefab;

    void Start()
    {
        inventoryPanel.SetActive(false);
        BuildUI();
    }

    public void Open()
    {
        inventoryPanel.SetActive(true);
        Refresh();
    }

    public void Close()
    {
        inventoryPanel.SetActive(false);
    }

    private Dictionary<Item, int> lastValues = new();

    void Update()
    {
        if (!inventoryPanel.activeSelf)
            return;

        bool changed = false;

        foreach (Item item in Enum.GetValues(typeof(Item)))
        {
            int current = inventory.GetAmount(item);

            if (!lastValues.ContainsKey(item) || lastValues[item] != current)
            {
                lastValues[item] = current;
                changed = true;
            }
        }
        if (changed)
            Refresh();
    }



    void BuildUI()
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
            
            ItemIconPair data = iconTable.Get(item);
            if (data != null)
                iconImage.sprite = data.icon;

        }
    }

    public void Refresh()
    {
        int index = 0;

        foreach (Item item in Enum.GetValues(typeof(Item)))
        {
            Transform row = contentParent.GetChild(index);
            TMP_Text amountText = row.GetComponentsInChildren<TMP_Text>()[1];
            amountText.text = inventory.GetAmount(item).ToString();
            index++;
        }
    }
}
