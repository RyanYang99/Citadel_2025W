using UnityEngine;
using Citadel;

public class ResourcePopupSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ResourcePopupUI popupPrefab;
    [SerializeField] private ItemIconTable iconTable;

    [Header("Spawn Option")]
    [SerializeField] private Vector3 spawnOffset = Vector3.up * 2f;

    private ItemProducer producer;

    private void Awake()
    {
        producer = GetComponent<ItemProducer>();
        if (iconTable == null)
            iconTable = FindFirstObjectByType<ItemIconTable>();
    }

    private void OnEnable()
    {
        if (producer != null)
            producer.OnItemProduced += OnItemProduced;
    }

    private void OnDisable()
    {
        if (producer != null)
            producer.OnItemProduced -= OnItemProduced;
    }

    private void OnItemProduced(ItemAmount itemAmount)
    {
        Debug.Log($"[Spawner] Item: {itemAmount.item}, Amount: {itemAmount.amount}");
        
        if (popupPrefab == null || iconTable == null)
            return;

        ItemIconPair pair = iconTable.Get(itemAmount.item);
        if (pair == null)
        {
            Debug.LogWarning($"[ResourcePopup] Icon not found for item: {itemAmount.item}");
            return;
        }

        ResourcePopupUI popup = Instantiate(
            popupPrefab,
            transform.position + spawnOffset,
            Quaternion.identity
        );

        popup.Init(pair.icon, itemAmount.amount);
    }
}
