using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Citadel
{
    public sealed class SatisfactionProvider : MonoBehaviour
    {
        private float _totalWeight;
        private readonly Dictionary<ItemConsumer.AnyResource, float> _mappedImportance = new();
        
        [SerializeField] private Inventory inventory;
        [SerializeField] private ItemConsumer itemConsumer;

        [SerializeField] private SatisfactionImportance satisfactionImportance, satisfactionImportanceOverride;
        
        public float Satisfaction { get; private set; }

        private void OnEnable() => inventory.OnTick += OnTick;

        private void OnDisable() => inventory.OnTick -= OnTick;

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
            List<ItemConsumer.AnyResource> provided = itemConsumer.GetReadyResources();

            if (_totalWeight == 0f)
            {
                Satisfaction = 1f;
                return;
            }
            
            float sum = provided.Where(anyResource => _mappedImportance.ContainsKey(anyResource))
                                .Sum(anyResource => _mappedImportance[anyResource]);
            Satisfaction = sum / _totalWeight;
            
            Debug.LogWarning(Satisfaction);
        }
    }
}