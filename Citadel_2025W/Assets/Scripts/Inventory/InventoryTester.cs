using UnityEngine;

namespace Citadel
{
    public sealed class InventoryTester : MonoBehaviour
    {
        [SerializeField] private Inventory inventory;
        [SerializeField] private ItemConsumer consumer;
        [SerializeField] private ItemProducer producer;
        
        public void Tick()
        {
            Debug.Log("Start Tick");
            inventory.PrintInventory();
            
            consumer.Tick();
            producer.Tick();
            
            inventory.PrintInventory();
            Debug.Log("End Tick");
        }
    }
}