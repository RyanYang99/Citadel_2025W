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

        [Header("Building Category")]
        public SatisfactionCategory category;

        public bool IsReady { get; private set; } = false;

        private bool _hasBonus;
        private float _totalWeight;
        private readonly Dictionary<ItemConsumer.AnyResource, float> _mappedImportance = new();

        [SerializeField] private Inventory inventory;
        [SerializeField] private BonusManager bonusManager;
        [SerializeField] private ItemConsumer itemConsumer;

        [SerializeField] private SatisfactionImportance satisfactionImportance, satisfactionImportanceOverride;
        [SerializeField] private float threshold;

        [SerializeField] private List<ItemBonus> itemBonuses = new();
        [SerializeField] private List<RangeResourceBonus> rangeResourceBonuses = new();

        private int readyDelayTicks = 3; // 만족도가 반영되기까지 몇 틱을 기다릴지 설정 / ex) 5틱
        private int _currentTickCount = 0;

        public float Satisfaction { get; private set; }

        private void OnEnable()
        {
            inventory.OnTick += OnTick;
            SatisfactionManager.Instance.Register(this);

            IsReady = false;
        }

        private void OnDisable()
        {
            inventory.OnTick -= OnTick;

            if (SatisfactionManager.Instance != null)
                SatisfactionManager.Instance.Unregister(this);

            IsReady = false;
            _hasBonus = false;
            bonusManager.RemoveBonus(this);
        }

        private void Awake()
        {
            if (inventory == null)
                inventory = FindFirstObjectByType<Inventory>();

            if (bonusManager == null)
                bonusManager = FindFirstObjectByType<BonusManager>();

            foreach (SatisfactionImportance.ItemImportance itemImportance in satisfactionImportance.itemImportances)
                _mappedImportance.TryAdd(new ItemConsumer.AnyResource(itemImportance.item), itemImportance.importance);

            foreach (SatisfactionImportance.RangeResourceImportance rangeResourceImportance in satisfactionImportance.rangeResourceImportances)
                _mappedImportance.TryAdd(new ItemConsumer.AnyResource(rangeResourceImportance.rangeResource), rangeResourceImportance.importance);

            //Override
            if (satisfactionImportanceOverride != null)
            {
                foreach (SatisfactionImportance.ItemImportance itemImportance in satisfactionImportanceOverride
                             .itemImportances)
                    _mappedImportance[new ItemConsumer.AnyResource(itemImportance.item)] = itemImportance.importance;

                foreach (SatisfactionImportance.RangeResourceImportance rangeResourceImportance in
                         satisfactionImportanceOverride.rangeResourceImportances)
                    _mappedImportance[new ItemConsumer.AnyResource(rangeResourceImportance.rangeResource)] =
                        rangeResourceImportance.importance;
            }

            //Total Weight
            foreach (float weight in _mappedImportance.Values)
                _totalWeight += weight;
        }

        private void OnTick()
        {
            if (!IsReady)
            {
                _currentTickCount++;

                if (_currentTickCount < readyDelayTicks) return;
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
                        bonusManager.AddBonus(this, new Bonus(itemBonus.targetItem, itemBonus.bonusValue));

                    foreach (RangeResourceBonus rangeResourceBonus in rangeResourceBonuses)
                        bonusManager.AddBonus(this, new Bonus(rangeResourceBonus.targetRangeResource, rangeResourceBonus.bonusValue));
                }
            }
            else
            {
                _hasBonus = false;
                bonusManager.RemoveBonus(this);
            }
        }
    }
}