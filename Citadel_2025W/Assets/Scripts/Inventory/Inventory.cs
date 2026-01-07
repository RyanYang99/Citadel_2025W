using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Citadel
{
    public sealed class Inventory : MonoBehaviour
    {
        //Only for logging purposes.
        private ulong _tick;
        
        private float _timer;
        private readonly Dictionary<Item, int> _resourcesCount = new();
        
        [SerializeField, Tooltip("게임을 처음 시작할 때 플레이어가 가지는 것")] private List<ItemAmount> startingResources = new();

        public event Action OnTick;
        public event Action<Item, int> OnItemChange;
        
        private void Awake()
        {
            foreach (ItemAmount startingResource in startingResources)
                Add(startingResource.item, startingResource.amount);
        }
        
        private void Update()
        {
            _timer += Time.deltaTime;

            if (_timer < 1.0f)
                return;

            Debug.Log($"[{nameof(Inventory)}] === Start Tick {_tick} ===");
            
            _timer = 0f;
            OnTick?.Invoke();
            
            PrintInventory();
            
            Debug.Log($"[{nameof(Inventory)}] === End Tick {_tick++} ===");
        }
        
        public int GetAmount(Item item)
        {
            _resourcesCount.TryAdd(item, 0);
            return _resourcesCount[item];
        }

        public void Add(Item item, int amount)
        {
            int before = GetAmount(item), after = before + amount;

            if (before != after)
            {
                _resourcesCount[item] = after;
                OnItemChange?.Invoke(item, after);
            }
        }

        public int Consume(Item item, int amount)
        {
            int before = GetAmount(item),
                consumableAmount = Math.Clamp(amount, 0, before),
                after = before - consumableAmount;
            
            if (before != after)
            {
                _resourcesCount[item] = after;
                OnItemChange?.Invoke(item, after);
            }
            return consumableAmount;
        }

        public List<ItemAmount> ToList() => _resourcesCount.Select(item => new ItemAmount(item.Key, item.Value)).ToList();

        public void Load(List<ItemAmount> items)
        {
            _resourcesCount.Clear();

            foreach (ItemAmount item in items)
                Add(item.item, item.amount);
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public void PrintInventory()
        {
            StringBuilder stringBuilder = new("Inventory Status:" + Environment.NewLine);

            foreach (KeyValuePair<Item, int> resource in _resourcesCount)
            {
                stringBuilder.Append(resource.Key);
                stringBuilder.Append(": ");
                stringBuilder.AppendLine(resource.Value.ToString());
            }
            
            Debug.Log(stringBuilder);
        }
    }
}