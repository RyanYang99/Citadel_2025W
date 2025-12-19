using System.Collections.Generic;
using UnityEngine;

namespace Citadel
{
    public sealed class ItemProducer : MonoBehaviour
    {
        [SerializeField, Tooltip("생산하는 자원, 각 틱 마다 필요한 자원이 존재할 때 생산")]
        private List<ItemAmount> itemsProducedPerTick;

        [SerializeField, Tooltip("만약 없을 시 매 틱 마다 자원 생산")]
        private ItemConsumer itemConsumer;

        private Inventory _inventory;

        private void OnValidate()
        {
            CheckProduced();
        }

        private void Awake()
        {
            CheckProduced();
            _inventory = FindAnyObjectByType<Inventory>();
        }

        private void CheckProduced()
        {
            for (int i = 0; i < itemsProducedPerTick.Count; ++i)
            {
                ItemAmount item = itemsProducedPerTick[i];
                
                if (item.amount < 0)
                {
                    Debug.LogError($"{nameof(itemsProducedPerTick)} can not be a negative value.");
                    itemsProducedPerTick[i] = new ItemAmount(item.item);
                }
            }
        }

        private void Produce()
        {
            foreach (ItemAmount item in itemsProducedPerTick)
                _inventory.Add(item.item, item.amount);
        }

        public void Tick()
        {
            if (itemConsumer == null || itemConsumer.AreItemsReady())
                Produce();
            
            if (itemConsumer)
                itemConsumer.ConsumeReadyItems();
        }
    }
}