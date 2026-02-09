using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Citadel
{
    public sealed class SatisfactionProvider : MonoBehaviour
    {
        [Serializable]
        private struct ItemBonus
        {
            public Item targetItem;
            public BonusValue bonusValue;
        }

        [Serializable]
        private struct RangeResourceBonus
        {
            public RangeResource targetRangeResource;
            public BonusValue bonusValue;
        }

        [Serializable]
        private sealed class ItemImportance
        {
            public Item item;
            [Range(0f, 1f)] public float importance;
        }

        [Serializable]
        private sealed class RangeResourceImportance
        {
            public RangeResource rangeResource;
            [Range(0f, 1f)] public float importance;
        }

        public bool IsReady { get; private set; }

        private Inventory _inventory;
        private BonusManager _bonusManager;
        
        private bool _hasBonus;
        private float _totalWeight;
        private int _currentTickCount;
        private readonly Dictionary<ItemConsumer.AnyResource, float> _mappedImportance = new();
        
        [SerializeField] private ItemConsumer itemConsumer;
        
        [Header("카테고리"), SerializeField] private SatisfactionCategory category;
        public SatisfactionCategory Category => category;
        
        [Header("중요도"), SerializeField] private List<ItemImportance> itemImportances = new();
        [SerializeField] private List<RangeResourceImportance> rangeResourceImportances = new();

        [Header("보너스"), SerializeField] private List<ItemBonus> itemBonuses = new();
        [SerializeField] private List<RangeResourceBonus> rangeResourceBonuses = new();

        [Header("수치"), SerializeField] private float threshold;
        [SerializeField, Tooltip("만족도가 반영되기까지 몇 틱을 기다릴지 설정")] private int readyDelayTicks;

        public float Satisfaction { get; private set; }

        private void OnEnable()
        {
            _inventory.OnTick += OnTick;
            SatisfactionManager.Instance.Register(this);

            IsReady = false;
        }

        private void OnDisable()
        {
            _inventory.OnTick -= OnTick;
            
            if (SatisfactionManager.Instance != null)
                SatisfactionManager.Instance.Unregister(this);

            IsReady = false;
            _hasBonus = false;
            _bonusManager.RemoveBonus(this);
        }

        private void Awake()
        {
            _inventory = FindFirstObjectByType<Inventory>();
            _bonusManager = FindFirstObjectByType<BonusManager>();

            foreach (ItemImportance itemImportance in itemImportances)
                _mappedImportance.TryAdd(new ItemConsumer.AnyResource(itemImportance.item), itemImportance.importance);

            foreach (RangeResourceImportance rangeResourceImportance in rangeResourceImportances)
                _mappedImportance.TryAdd(new ItemConsumer.AnyResource(rangeResourceImportance.rangeResource), rangeResourceImportance.importance);
            
            foreach (float weight in _mappedImportance.Values)
                _totalWeight += weight;
        }

        private void OnTick()
        {
            if (!IsReady)
            {
                if (++_currentTickCount < readyDelayTicks)
                    return;
                
                IsReady = true;
            }

            if (_totalWeight == 0f)
            {
                Satisfaction = 1f;
                IsReady = true;
                return;
            }

            float sum = itemConsumer.GetReadyResources().Where(anyResource => _mappedImportance.ContainsKey(anyResource))
                                    .Sum(anyResource => _mappedImportance[anyResource]);
            Satisfaction = sum / _totalWeight;

            IsReady = true;

            if (Satisfaction >= threshold)
            {
                if (!_hasBonus)
                {
                    _hasBonus = true;

                    foreach (ItemBonus itemBonus in itemBonuses)
                        _bonusManager.AddBonus(this, new Bonus(itemBonus.targetItem, itemBonus.bonusValue));

                    foreach (RangeResourceBonus rangeResourceBonus in rangeResourceBonuses)
                        _bonusManager.AddBonus(this, new Bonus(rangeResourceBonus.targetRangeResource, rangeResourceBonus.bonusValue));
                }
            }
            else
            {
                _hasBonus = false;
                _bonusManager.RemoveBonus(this);
            }
        }
    }
}