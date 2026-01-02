using System.Collections.Generic;
using UnityEngine;

namespace Citadel
{
    public sealed class ItemProducer : MonoBehaviour
    {
        private int _ticks;
        
        [SerializeField] private Inventory inventory;
        
        [SerializeField, Tooltip("만약 없을 시 매 틱 마다 자원 생산")]
        private ItemConsumer itemConsumer;
        
        [SerializeField] private int ticksNeeded;
        
        [SerializeField, Tooltip("생산하는 자원, 필요한 자원이 존재할 때 생산")]
        private List<ItemAmount> itemsProduced;

        private void OnValidate() => CheckParameters();

        private void Awake() => CheckParameters();
        
        private void OnEnable()
        {
            inventory.OnTick += Tick;
        }
        
        private void OnDisable()
        {
            inventory.OnTick -= Tick;
        }
        
        private void CheckParameters()
        {
            if (ticksNeeded < 0)
            {
                Debug.LogError($"{nameof(ticksNeeded)} can not be a negative value.");
                ticksNeeded = 0;
            }
            
            for (int i = 0; i < itemsProduced.Count; ++i)
            {
                ItemAmount item = itemsProduced[i];
                
                if (item.amount < 0)
                {
                    Debug.LogError($"{nameof(itemsProduced)} can not be a negative value.");
                    itemsProduced[i] = new ItemAmount(item.item);
                }
            }
        }

        private void Tick()
        {
            if (++_ticks < ticksNeeded)
                return;

            _ticks = 0;
            
            if (itemConsumer == null || itemConsumer.AreItemsReady())
                Produce();
            
            if (itemConsumer)
                itemConsumer.ConsumeReadyItems();
            
            inventory.PrintInventory();
        }
        
        private void Produce()
        {
            foreach (ItemAmount item in itemsProduced)
                inventory.Add(item.item, item.amount);
        }
    }
}