using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Citadel
{
    public sealed class BonusManager : MonoBehaviour
    {
        private readonly Dictionary<UnityEngine.Object, List<Bonus>> _activeBonuses = new();

        private readonly Dictionary<Item, BonusValue> _consolidatedItems = new();
        private readonly Dictionary<RangeResource, BonusValue> _consolidatedRangeResources = new();
        
        public Action OnBonusesChanged;
        
        private static BonusValue Add(BonusValue bonusValue1, BonusValue bonusValue2)
        {
            bonusValue1.flat += bonusValue2.flat;
            bonusValue1.percentage += bonusValue2.percentage;
            return bonusValue1;
        }

        private void Consolidate()
        {
            _consolidatedItems.Clear();
            _consolidatedRangeResources.Clear();
            
            foreach (Bonus bonus in _activeBonuses.Values.SelectMany(bonuses => bonuses))
                if (bonus.TargetItem.HasValue)
                    ConsolidateItem(bonus);
                else
                    ConsolidateRangeResource(bonus);
        }

        private void ConsolidateItem(Bonus bonus)
        {
            Item item = bonus.TargetItem.Value;
            if (!_consolidatedItems.TryGetValue(item, out BonusValue bonusValue))
                bonusValue = new BonusValue();

            _consolidatedItems[item] = Add(bonusValue, bonus.TargetBonusValue);
        }

        private void ConsolidateRangeResource(Bonus bonus)
        {
            RangeResource rangeResource = bonus.TargetRangeResource.Value;
            if (!_consolidatedRangeResources.TryGetValue(rangeResource, out BonusValue bonusValue))
                bonusValue = new BonusValue();
            
            _consolidatedRangeResources[rangeResource] = Add(bonusValue, bonus.TargetBonusValue);
        }

        private void Invoke() => OnBonusesChanged?.Invoke();

        public void AddBonus(UnityEngine.Object source, Bonus bonus)
        {
            if (!_activeBonuses.TryGetValue(source, out List<Bonus> bonuses)) 
            {
                bonuses = new List<Bonus>();
                _activeBonuses[source] = bonuses;
            }

            if (!bonuses.Contains(bonus))
                bonuses.Add(bonus);

            Consolidate();
            Invoke();
        }

        public void RemoveBonus(SatisfactionProvider satisfactionProvider)
        {
            _activeBonuses.Remove(satisfactionProvider);
            
            Consolidate();
            Invoke();
        }

        public void RemoveBonus(SatisfactionProvider satisfactionProvider, Bonus bonus)
        {
            if (_activeBonuses.TryGetValue(satisfactionProvider, out List<Bonus> bonuses))
                bonuses.Remove(bonus);
            
            Consolidate();
            Invoke();
        }

        public Dictionary<Item, BonusValue> GetItemBonuses() => _consolidatedItems;

        public Dictionary<RangeResource, BonusValue> GetRangeResourceBonuses() => _consolidatedRangeResources;
    }
}