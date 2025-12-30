using UnityEngine.UI;
using UnityEngine;
using Citadel;
using System;
using System.Collections.Generic;
using TMPro;

public class InventoryUIController : MonoBehaviour
{
    [Header("인벤토리 데이터")]
    public Inventory inventory;

    [Header("아이콘 / 카테고리 테이블")]
    public ItemIconTable iconTable;

    [Header("UI")]
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
        // 기존 UI 정리
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        // Item enum 전체 순회
        foreach (Item item in Enum.GetValues(typeof(Item)))
        {
            GameObject row = Instantiate(resourceRowPrefab, contentParent);

            //수량 텍스트
            TMP_Text[] texts = row.GetComponentsInChildren<TMP_Text>();
            texts[0].text = item.ToString();
            texts[1].text = inventory.GetAmount(item).ToString();

            //아이콘
            Image iconImage = row.transform.Find("IconImage").GetComponent<Image>();
            var data = iconTable.Get(item);
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
