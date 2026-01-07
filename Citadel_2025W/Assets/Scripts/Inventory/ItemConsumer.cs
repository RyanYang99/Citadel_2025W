using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Citadel
{
    public sealed class ItemConsumer : MonoBehaviour
    {
        public static readonly List<ItemConsumer> ActiveItemConsumers = new();
        
        private readonly Dictionary<Item, int> _currentItems = new();
        private readonly Dictionary<ItemProducer, List<RangeResource>> _providedRangeResources = new();
        
        [SerializeField] private Inventory inventory;
        
        [SerializeField, Tooltip("필요한 자원")]
        private List<ItemAmount> itemsUsed = new();

        [SerializeField] private List<RangeResource> rangeResourcesUsed = new();

        private void OnValidate() => CheckUsage();

        private void Awake()
        {
            CheckUsage();

            foreach (ItemAmount item in itemsUsed)
                _currentItems.TryAdd(item.item, 0);
        }

        private void OnEnable()
        {
            inventory.OnTick += Tick;
            ActiveItemConsumers.Add(this);
        }

        private void OnDisable()
        {
            inventory.OnTick -= Tick;
            ActiveItemConsumers.Remove(this);
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
            if (itemsUsed.Any(item => _currentItems[item.item] < item.amount))
                return false;

            List<RangeResource> provided = new();
            foreach (List<RangeResource> rangeResources in _providedRangeResources.Values)
                foreach (RangeResource rangeResource in rangeResources)
                    if (!provided.Contains(rangeResource) && rangeResourcesUsed.Contains(rangeResource))
                        provided.Add(rangeResource);

            return provided.Count >= rangeResourcesUsed.Count;
        }

        public void ConsumeReadyItems()
        {
            foreach (ItemAmount item in itemsUsed)
                _currentItems[item.item] = Math.Max(0, _currentItems[item.item] - item.amount);
        }

        public void UpdateRangeResource(ItemProducer provider, RangeResource rangeResource, bool provided)
        {
            if (!rangeResourcesUsed.Contains(rangeResource))
                return;

            _providedRangeResources.TryAdd(provider, new List<RangeResource>());

            List<RangeResource> list = _providedRangeResources[provider];
            bool contains = list.Contains(rangeResource);
            
            if (provided)
            {
                if (!contains)
                    list.Add(rangeResource);
            }
            else
            {
                if (contains)
                    list.Remove(rangeResource);

                if (list.Count == 0)
                    _providedRangeResources.Remove(provider);
            }
        }
    }
}