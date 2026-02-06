using UnityEngine;

namespace Citadel
{
    public class ResourcePopupSpawner : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private ResourcePopupUI popupPrefab;

        [SerializeField] private IconTable iconTable;

        [Header("Spawn Option")] [SerializeField]
        private Vector3 spawnOffset = Vector3.up * 2f;

        private ItemProducer producer;

        private void Awake() => producer = GetComponent<ItemProducer>();

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
            if (popupPrefab == null || iconTable == null)
                return;

            ResourcePopupUI popup = Instantiate(popupPrefab,
                                                transform.position + spawnOffset,
                                                Quaternion.identity);

            popup.Init(iconTable.Find(itemAmount.item), itemAmount.amount);
        }
    }
}