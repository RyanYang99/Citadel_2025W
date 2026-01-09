using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Citadel
{
    public sealed class ItemConsumer : MonoBehaviour
    {
        public sealed record AnyResource
        {
            public Item? AnyItem;
            public RangeResource? AnyRangeResource;

            public AnyResource(Item item) => AnyItem = item;

            public AnyResource(RangeResource rangeResource) => AnyRangeResource = rangeResource;
        }
        
        public static readonly List<ItemConsumer> ActiveItemConsumers = new();
        
        private readonly Dictionary<Item, int> _currentItems = new();
        private readonly Dictionary<ItemProducer, List<RangeResource>> _providedRangeResources = new();

        private readonly List<AnyResource> _readyResourcesSnapshot = new();
        
        [SerializeField] private Inventory inventory;
        
        [SerializeField, Tooltip("필요한 자원")]
        private List<ItemAmount> itemsUsed = new();

        [SerializeField] private List<RangeResource> rangeResourcesUsed = new();
        
        public int TotalRequiredResources { get; private set; }

        private void OnValidate() => CheckUsage();

        private void Awake()
        {
            CheckUsage();

            foreach (ItemAmount item in itemsUsed)
                _currentItems.TryAdd(item.item, 0);

            TotalRequiredResources = itemsUsed.Count + rangeResourcesUsed.Count;
        }

        private void OnEnable()
        {
            inventory.OnTick += OnTick;
            ActiveItemConsumers.Add(this);
        }

        private void OnDisable()
        {
            inventory.OnTick -= OnTick;
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

        private void OnTick()
        {
            foreach (ItemAmount item in itemsUsed)
            {
                int needed = item.amount - _currentItems[item.item];
                if (needed <= 0)
                    continue;

                _currentItems[item.item] += inventory.Consume(item.item, needed);
            }

            UpdateSnapshot();
        }

        private List<Item> GetProvidedItems() =>
            itemsUsed.FindAll(itemAmount => itemAmount.amount <= _currentItems[itemAmount.item])
                     .Select(itemAmount => itemAmount.item)
                     .ToList();

        private List<RangeResource> GetProvidedRangeResources()
        {
            List<RangeResource> provided = new();
            foreach (List<RangeResource> rangeResources in _providedRangeResources.Values)
            foreach (RangeResource rangeResource in rangeResources)
                if (!provided.Contains(rangeResource) && rangeResourcesUsed.Contains(rangeResource))
                    provided.Add(rangeResource);

            return provided;
        }

        private void UpdateSnapshot()
        {
            _readyResourcesSnapshot.Clear();
            _readyResourcesSnapshot.AddRange(GetProvidedItems().Select(item => new AnyResource(item)));
            _readyResourcesSnapshot.AddRange(GetProvidedRangeResources().Select(rangeResource => new AnyResource(rangeResource)));
        }

        public bool AreItemsReady()
        {
            if (GetProvidedItems().Count < itemsUsed.Count)
                return false;

            return GetProvidedRangeResources().Count >= rangeResourcesUsed.Count;
        }

        public List<AnyResource> GetReadyResources() => _readyResourcesSnapshot;

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