using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Citadel
{
    public sealed class ItemProducer : MonoBehaviour
    {
        [Serializable]
        private sealed class RangeResourceAmount
        {
            public RangeResource rangeResource;
            public int tickDuration;

            public RangeResourceAmount(RangeResourceAmount rangeResourceAmount)
            {
                rangeResource = rangeResourceAmount.rangeResource;
                tickDuration = rangeResourceAmount.tickDuration;
            }
            
            public RangeResourceAmount(RangeResource rangeResource, int tickDuration)
            {
                this.rangeResource = rangeResource;
                this.tickDuration = tickDuration;
            }
        }
        
        private int _ticks;
        
        private readonly List<RangeResourceAmount> _originalRangeResourceDurations = new();
        private readonly List<RangeResourceAmount> _rangeResourceDurations = new();
        private readonly Dictionary<RangeResource, List<ItemConsumer>> _itemConsumersInRange = new();
        
        private Inventory _inventory;
        private BonusManager _bonusManager;
        
        [SerializeField, Tooltip("만약 없을 시, ticksNeeded 마다 자원 생산")]
        private ItemConsumer itemConsumer;
        
        [SerializeField] private int ticksNeeded;

        [SerializeField, Tooltip("생산하는 자원, 필요한 자원이 존재할 때 생산")]
        private List<ItemAmount> itemsProduced = new();
        
        [SerializeField] private float range;
        
        [SerializeField, Tooltip("공급하는 자원, 필요한 자원이 존재할 때 공급")]
        private List<RangeResourceAmount> rangeResourceProvided = new();

        [SerializeField, Tooltip("작동 시 한번 공급하는 자원")]
        private List<ItemAmount> oneTimeItemsProduced = new();

        [SerializeField]
        private List<ItemAmount> permanentItemsAdded = new();

        public Action<ItemAmount> OnItemProduced;

        //Status 판넬에 데이터를 넘기기 위한 , 읽기 전용
        public int TicksNeeded => ticksNeeded;
        public IReadOnlyList<ItemAmount> ItemsProduced => itemsProduced;
        public float Range => range;

        public IReadOnlyList<(RangeResource resource, int tickDuration)> RangeResourcesProvided =>
            rangeResourceProvided.Select(rangeResourceAmount => (rangeResourceAmount.rangeResource, rangeResourceAmount.tickDuration)).ToList();

        private void Awake()
        {
            _inventory = FindAnyObjectByType<Inventory>();
            _bonusManager = FindAnyObjectByType<BonusManager>();
        }

        private void OnEnable()
        {
            _inventory.OnTick += OnTick;
            
            foreach (ItemAmount itemAmount in oneTimeItemsProduced)
                _inventory.Add(itemAmount.item, itemAmount.amount);
        }

        private void Start()
        {
            foreach (RangeResourceAmount rangeResourceAmount in rangeResourceProvided)
            {
                _originalRangeResourceDurations.Add(new RangeResourceAmount(rangeResourceAmount));
                _rangeResourceDurations.Add(new RangeResourceAmount(rangeResourceAmount));
            }

            foreach (ItemAmount itemAmount in permanentItemsAdded)
                _inventory.Add(itemAmount.item, itemAmount.amount);
        }

        private void OnDisable()
        {
            _inventory.OnTick -= OnTick;
            
            foreach (ItemAmount itemAmount in oneTimeItemsProduced)
                _inventory.ForceSubtract(itemAmount.item, itemAmount.amount);
            
            UpdateRange();
            UpdateRangeResource(false);
        }

        private void OnTick()
        {
            UpdateRange();
            foreach (RangeResourceAmount rangeResourceAmount in _rangeResourceDurations.Where(rangeResourceAmount => rangeResourceAmount.tickDuration > 0))
                UpdateRangeResource(rangeResourceAmount.rangeResource, --rangeResourceAmount.tickDuration > 0);
            
            if (++_ticks < ticksNeeded)
                return;

            _ticks = 0;
            
            if (itemConsumer == null || itemConsumer.AreItemsReady())
                Produce();
            
            if (itemConsumer)
                itemConsumer.ConsumeReadyItems();
        }

        private void UpdateRange()
        {
            foreach (RangeResource rangeResource in rangeResourceProvided.Select(rangeResourceAmount => rangeResourceAmount.rangeResource))
            {
                float result = range;
                if (_bonusManager.GetRangeResourceBonuses().TryGetValue(rangeResource, out BonusValue bonusValue))
                    result = result + bonusValue.flat + (result * bonusValue.percentage);
                
                _itemConsumersInRange[rangeResource] = ItemConsumer.ActiveItemConsumers
                                                                   .Where(_itemConsumer => Vector3.Distance(transform.position, _itemConsumer.transform.position) <= result)
                                                                   .ToList();
            }
        }

        private void Produce()
        {
            foreach (ItemAmount item in itemsProduced)
            {
                int result = item.amount;
                if (_bonusManager.GetItemBonuses().TryGetValue(item.item, out BonusValue bonusValue))
                    result = (int)Math.Round((result + bonusValue.flat) * (1f + bonusValue.percentage));

                _inventory.Add(item.item, result);
                OnItemProduced?.Invoke(new ItemAmount(item.item, result));
            }

            foreach (RangeResourceAmount duration in _rangeResourceDurations)
                duration.tickDuration = _originalRangeResourceDurations.Find(rangeResourceAmount => rangeResourceAmount.rangeResource == duration.rangeResource).tickDuration;
            
            UpdateRangeResource(true);
        }

        private void UpdateRangeResource(bool provided)
        {
            foreach (RangeResourceAmount rangeResourceAmount in rangeResourceProvided)
                UpdateRangeResource(rangeResourceAmount.rangeResource, provided);
        }

        private void UpdateRangeResource(RangeResource rangeResource, bool provided)
        {
            foreach (ItemConsumer _itemConsumer in _itemConsumersInRange[rangeResource])
                _itemConsumer.UpdateRangeResource(this, rangeResource, provided);
        }
    }
}