using System.Collections.Generic;
using UnityEngine;

namespace Citadel
{
    public sealed class ItemConsumer : MonoBehaviour
    {
        [SerializeField, Tooltip("필요한 자원, 각 틱 마다 차감")]
        private List<ItemAmount> itemsUsedPerTick;

        private Inventory _inventory;
        private readonly Dictionary<Item, int> _currentItems = new();

        private void OnValidate()
        {
            CheckUsage();
        }

        private void Awake()
        {
            CheckUsage();
            _inventory = FindAnyObjectByType<Inventory>();

            foreach (ItemAmount item in itemsUsedPerTick)
                _currentItems.TryAdd(item.item, 0);
        }

        private void CheckUsage()
        {
            for (int i = 0; i < itemsUsedPerTick.Count; ++i)
            {
                ItemAmount item = itemsUsedPerTick[i];
                
                if (item.amount < 0)
                {
                    Debug.LogError($"{nameof(itemsUsedPerTick)} can not contain a negative value.");
                    itemsUsedPerTick[i] = new ItemAmount(item.item);
                }
            }
        }

        public void Tick()
        {
            foreach (ItemAmount item in itemsUsedPerTick)
            {
                int needed = item.amount - _currentItems[item.item];
                if (needed <= 0)
                    continue;

                _currentItems[item.item] += _inventory.Consume(item.item, needed);
            }
        }

        public bool AreItemsReady()
        {
            foreach (ItemAmount item in itemsUsedPerTick)
                if (_currentItems[item.item] < item.amount)
                    return false;

            return true;
        }

        public void ConsumeReadyItems()
        {
            foreach (ItemAmount item in itemsUsedPerTick)
                _currentItems[item.item] -= item.amount;
        }
    }
}