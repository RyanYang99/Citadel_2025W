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

        private readonly Dictionary<ItemConsumer.AnyResource, float> _mappedImportance = new();
        
        [SerializeField] private ItemConsumer itemConsumer;
        
        [Header("카테고리"), SerializeField] private SatisfactionCategory category;
        public SatisfactionCategory Category => category;
        
        [Header("중요도"), SerializeField] private List<ItemImportance> itemImportances = new();
        [SerializeField] private List<RangeResourceImportance> rangeResourceImportances = new();

        [Header("보너스"), SerializeField] private List<ItemBonus> itemBonuses = new();
        [SerializeField] private List<RangeResourceBonus> rangeResourceBonuses = new();

        [Header("수치"), SerializeField] private float threshold;

        private float _lastInternalScore;
        private float _lastExternalScore;

        public float InternalScore => _lastInternalScore;
        public float ExternalScore => _lastExternalScore;

        public float Satisfaction { get; private set; }

        private void OnEnable()
        {
            _inventory.OnTick += OnTick;
            SatisfactionManager.Instance.Register(this);
        }

        private void OnDisable()
        {
            _inventory.OnTick -= OnTick;
            
            if (SatisfactionManager.Instance != null)
                SatisfactionManager.Instance.Unregister(this);

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
            // 내실 만족도 계산
            if (_totalWeight <= 0f)
            {
                _lastInternalScore = 50f;
            }
            else
            {
                var snapshot = itemConsumer.Snapshot;
                float currentWeightSum = 0f;

                foreach (var kvp in _mappedImportance)
                {
                    bool isResourceReady = snapshot.Any(res =>
                        (kvp.Key.AnyItem.HasValue && res.AnyItem == kvp.Key.AnyItem) ||
                        (kvp.Key.AnyRangeResource.HasValue && res.AnyRangeResource == kvp.Key.AnyRangeResource)
                    );

                    if (isResourceReady)
                    {
                        currentWeightSum += kvp.Value;
                    }
                }

                _lastInternalScore = (currentWeightSum / _totalWeight) * 50f;
            }

            // 문화 건물 만족도 계산
            float externalRawSum = 0f;
            var allInfluences = SatisfactionManager.Instance.AllInfluences;

            foreach (var influence in allInfluences)
            {
                float distance = Vector3.Distance(transform.position, influence.transform.position);
                if (distance <= influence.Range)
                {
                    externalRawSum += influence.Score;
                }
            }
            _lastExternalScore = Mathf.Min(externalRawSum, 50f);

            // 최종 만족도 합산 (0.0 ~ 1.0)
            // (50 + 0) / 100 = 0.5 (50%)가 되어야 함
            Satisfaction = (_lastInternalScore + _lastExternalScore) / 100f;


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
                if (_hasBonus)
                {
                    _hasBonus = false;
                    _bonusManager.RemoveBonus(this);
                }
            }
        }
    }
}