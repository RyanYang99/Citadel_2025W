using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Citadel
{
    public sealed class Inventory : MonoBehaviour
    {
        private float _timer;
        private readonly Dictionary<Item, int> _resourcesCount = new();
        
        [SerializeField, Tooltip("게임을 처음 시작할 때 플레이어가 가지는 것")] private List<ItemAmount> startingResources;

        public event Action OnTick; 
        
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

            _timer = 0f;
            OnTick?.Invoke();
        }
        
        public int GetAmount(Item item)
        {
            _resourcesCount.TryAdd(item, 0);
            return _resourcesCount[item];
        }

        public void Add(Item item, int amount) => _resourcesCount[item] = GetAmount(item) + amount;

        public int Consume(Item item, int amount)
        {
            int currentAmount = GetAmount(item);
            
            /*
            만약 대출 시스템을 추가할 경우
            if (item == Item.Money)
            {
                _resourcesCount[item] = currentAmount - amount;
                return amount;
            }
            */
            
            int consumableAmount = Math.Clamp(amount, 0, currentAmount);
            currentAmount -= consumableAmount;
            _resourcesCount[item] = currentAmount;

            return consumableAmount;
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