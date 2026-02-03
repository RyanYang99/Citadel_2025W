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
        
        private Inventory _inventory;
        
        [Header("생산 시 필요한 자원"), SerializeField]
        private List<ItemAmount> itemsUsed = new();

        [SerializeField] private List<RangeResource> rangeResourcesUsed = new();

        [Header("만족도만 충족시키는 자원 (틱 마다 소모)"), SerializeField]
        private List<ItemAmount> satisfactionItemUsed = new();

        [SerializeField] private List<RangeResource> satisfactionRangeResourceUsed = new();
        
        public int TotalRequiredResources { get; private set; }

        //Status Panel 읽기 전용
        public IReadOnlyList<ItemAmount> ItemsUsed => itemsUsed;
        public IReadOnlyList<RangeResource> RangeResourcesUsed => rangeResourcesUsed;

        private void Awake()
        {
            _inventory = FindAnyObjectByType<Inventory>();

            foreach (ItemAmount item in itemsUsed.Concat(satisfactionItemUsed))
                _currentItems.TryAdd(item.item, 0);

            TotalRequiredResources = itemsUsed.Count + rangeResourcesUsed.Count;
        }

        private void OnEnable()
        {
            _inventory.OnTick += OnTick;
            ActiveItemConsumers.Add(this);
        }

        private void OnDisable()
        {
            _inventory.OnTick -= OnTick;
            ActiveItemConsumers.Remove(this);
        }

        private void ConsumeItems(List<ItemAmount> itemAmounts)
        {
            foreach (ItemAmount item in itemAmounts)
            {
                int needed = item.amount - _currentItems[item.item];
                if (needed <= 0)
                    continue;

                _currentItems[item.item] += _inventory.Consume(item.item, needed);
            }
        }

        private void OnTick()
        {
            ConsumeItems(itemsUsed);
            ConsumeItems(satisfactionItemUsed);

            UpdateSnapshot();
            
            foreach (ItemAmount item in satisfactionItemUsed.Where(item => _currentItems[item.item] >= item.amount))
                _currentItems[item.item] -= item.amount;
        }
        
        private List<Item> GetProvidedItemsFrom(List<ItemAmount> items) =>
            items.FindAll(itemAmount => itemAmount.amount <= _currentItems[itemAmount.item])
                 .Select(itemAmount => itemAmount.item)
                 .ToList();

        private List<Item> GetProvidedItems() => GetProvidedItemsFrom(itemsUsed);

        private List<Item> GetProvidedSatisfactionItems() => GetProvidedItemsFrom(satisfactionItemUsed);

        private List<RangeResource> GetProvidedRangeResources()
        {
            List<RangeResource> provided = new();
            foreach (List<RangeResource> rangeResources in _providedRangeResources.Values)
                foreach (RangeResource rangeResource in rangeResources.Where(rangeResource => !provided.Contains(rangeResource) && (rangeResourcesUsed.Contains(rangeResource) || satisfactionRangeResourceUsed.Contains(rangeResource)))) 
                    provided.Add(rangeResource);

            return provided;
        }

        private void UpdateSnapshot()
        {
            _readyResourcesSnapshot.Clear();
            _readyResourcesSnapshot.AddRange(GetProvidedItems().Concat(GetProvidedSatisfactionItems()).Select(item => new AnyResource(item)));
            _readyResourcesSnapshot.AddRange(GetProvidedRangeResources().Select(rangeResource => new AnyResource(rangeResource)));
        }
        
        public int GetCurrentAmount(Item item) => _currentItems.GetValueOrDefault(item, 0);

        public bool AreItemsReady() => GetProvidedItems().Count >= itemsUsed.Count &&
                                       GetProvidedRangeResources().Count >= rangeResourcesUsed.Count;

        public List<AnyResource> GetReadyResources() => _readyResourcesSnapshot;
        
        public void ConsumeReadyItems()
        {
            foreach (ItemAmount item in itemsUsed)
                _currentItems[item.item] = Math.Max(0, _currentItems[item.item] - item.amount);
        }

        public void UpdateRangeResource(ItemProducer provider, RangeResource rangeResource, bool provided)
        {
            if (!rangeResourcesUsed.Contains(rangeResource) && !satisfactionRangeResourceUsed.Contains(rangeResource))
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