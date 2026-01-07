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
        private List<ItemConsumer> _itemConsumersInRange = new();
        
        [SerializeField] private Inventory inventory;
        
        [SerializeField, Tooltip("만약 없을 시, ticksNeeded 마다 자원 생산")]
        private ItemConsumer itemConsumer;
        
        [SerializeField] private int ticksNeeded;
        
        [SerializeField, Tooltip("생산하는 자원, 필요한 자원이 존재할 때 생산")]
        private List<ItemAmount> itemsProduced = new();
        
        [SerializeField] private float range;
        
        [SerializeField, Tooltip("공급하는 자원, 필요한 자원이 존재할 때 공급")]
        private List<RangeResourceAmount> rangeResourceProvided = new();

        public Action<ItemAmount> OnItemProduced;

        private void OnValidate() => CheckParameters();

        private void Awake()
        {
            CheckParameters();

            foreach (RangeResourceAmount rangeResourceAmount in rangeResourceProvided)
            {
                _originalRangeResourceDurations.Add(new RangeResourceAmount(rangeResourceAmount));
                _rangeResourceDurations.Add(new RangeResourceAmount(rangeResourceAmount));
            }
        }

        private void OnEnable() => inventory.OnTick += Tick;

        private void OnDisable()
        {
            inventory.OnTick -= Tick;
            
            UpdateRange();
            UpdateRangeResource(false);
        }

        private void CheckParameters()
        {
            if (ticksNeeded < 0)
            {
                Debug.LogError($"{nameof(ticksNeeded)} can not be a negative value.");
                ticksNeeded = 0;
            }
            
            for (int i = 0; i < itemsProduced.Count; ++i)
            {
                ItemAmount item = itemsProduced[i];
                
                if (item.amount < 0)
                {
                    Debug.LogError($"{nameof(itemsProduced)} can not be a negative value.");
                    itemsProduced[i] = new ItemAmount(item.item);
                }
            }
        }

        private void Tick()
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

        private void UpdateRange() =>
            _itemConsumersInRange = ItemConsumer.ActiveItemConsumers
                                                .Where(_itemConsumer => Vector3.Distance(transform.position, _itemConsumer.transform.position) <= range)
                                                .ToList();
        
        private void Produce()
        {
            foreach (ItemAmount item in itemsProduced)
            {
                inventory.Add(item.item, item.amount);
                OnItemProduced?.Invoke(item);
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
            foreach (ItemConsumer _itemConsumer in _itemConsumersInRange)
                _itemConsumer.UpdateRangeResource(this, rangeResource, provided);
        }
    }
}