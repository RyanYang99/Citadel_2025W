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
        
        public float Satisfaction { get; private set; }

        private void OnEnable() => inventory.OnTick += OnTick;

        private void OnDisable()
        {
            inventory.OnTick -= OnTick;

            _hasBonus = false;
            bonusManager.RemoveBonus(this);
        }

        private void Awake()
        {
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
            if (_totalWeight == 0f)
            {
                Satisfaction = 1f;
                return;
            }
            
            float sum = itemConsumer.GetReadyResources().Where(anyResource => _mappedImportance.ContainsKey(anyResource))
                                    .Sum(anyResource => _mappedImportance[anyResource]);
            Satisfaction = sum / _totalWeight;
            
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