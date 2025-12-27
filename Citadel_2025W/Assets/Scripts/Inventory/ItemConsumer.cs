using System.Collections.Generic;
using UnityEngine;

namespace Citadel
{
    public sealed class ItemConsumer : MonoBehaviour
    {
        private readonly Dictionary<Item, int> _currentItems = new();
        
        [SerializeField] private Inventory inventory;
        
        [SerializeField, Tooltip("필요한 자원")]
        private List<ItemAmount> itemsUsed;

        private void OnValidate()
        {
            CheckUsage();
        }

        private void Awake()
        {
            CheckUsage();

            foreach (ItemAmount item in itemsUsed)
                _currentItems.TryAdd(item.item, 0);
        }

        private void OnEnable()
        {
            inventory.OnTick += Tick;
        }
        
        private void OnDisable()
        {
            inventory.OnTick -= Tick;
        }

        private void CheckUsage()
        {
            for (int i = 0; i < itemsUsed.Count; ++i)
            {
                ItemAmount item = itemsUsed[i];
                
                if (item.amount < 0)
                {
                    Debug.LogError($"{nameof(itemsUsed)} can not contain a negative value.");
                    itemsUsed[i] = new ItemAmount(item.item);
                }
            }
        }

        private void Tick()
        {
            foreach (ItemAmount item in itemsUsed)
            {
                int needed = item.amount - _currentItems[item.item];
                if (needed <= 0)
                    continue;

                _currentItems[item.item] += inventory.Consume(item.item, needed);
            }
        }

        public bool AreItemsReady()
        {
            foreach (ItemAmount item in itemsUsed)
                if (_currentItems[item.item] < item.amount)
                    return false;

            return true;
        }

        public void ConsumeReadyItems()
        {
            foreach (ItemAmount item in itemsUsed)
                _currentItems[item.item] -= item.amount;
        }
    }
}