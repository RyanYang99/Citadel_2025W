using System;

namespace Citadel
{
    [Serializable]
    public struct ItemAmount
    {
        public Item item;
        public int amount;
        
        public ItemAmount(Item item = Item.Money, int amount = 0)
        {
            this.item = item;
            this.amount = amount;
        }
    }
}