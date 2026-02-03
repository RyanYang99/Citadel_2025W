using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Citadel
{
    public sealed class BonusManager : MonoBehaviour
    {
        private readonly Dictionary<Object, List<Bonus>> _activeBonuses = new();

        private readonly Dictionary<Item, BonusValue> _consolidatedItems = new();
        private readonly Dictionary<RangeResource, BonusValue> _consolidatedRangeResources = new();

        [SerializeField] private bool log;
        
        public Action OnBonusesChanged;
        
        private static BonusValue Add(BonusValue bonusValue1, BonusValue bonusValue2)
        {
            bonusValue1.flat += bonusValue2.flat;
            bonusValue1.percentage += bonusValue2.percentage;
            return bonusValue1;
        }

        private void Awake()
        {
#if !UNITY_EDITOR && !DEVELOPMENT_BUILD
            log = false;
#endif
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

        private void Invoke()
        {
            OnBonusesChanged?.Invoke();

            if (!log)
                return;

            Debug.Log("=== START ACTIVE BONUS LOG ===");
            StringBuilder stringBuilder = new();
            
            foreach (KeyValuePair<Object, List<Bonus>> bonuses in _activeBonuses)
                foreach (Bonus bonus in bonuses.Value)
                {
                    stringBuilder.Append($"{bonuses.Key.name}: ");
                
                    if (bonus.TargetItem.HasValue)
                        stringBuilder.Append(bonus.TargetItem.Value);
                    else
                        stringBuilder.Append(bonus.TargetRangeResource);
                
                    stringBuilder.Append(Environment.NewLine);
                }

            Debug.Log(stringBuilder);
            Debug.Log("=== END ACTIVE BONUS LOG ===");
        }

        public void AddBonus(Object source, Bonus bonus)
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