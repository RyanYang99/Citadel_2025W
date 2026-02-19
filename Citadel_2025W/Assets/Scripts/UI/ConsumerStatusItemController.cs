using System.Linq;
using TMPro;
using UnityEngine;

namespace Citadel
{
    public sealed class ConsumerStatusItemController : MonoBehaviour
    {
        private ItemConsumer _itemConsumer;
        private ItemAmount _itemAmount;
        private RangeResource _rangeResource;
        
        [SerializeField] private ResourceUI resourceUI;
        [SerializeField] private TMP_Text text;

        public void Initialize(ItemConsumer itemConsumer, ItemAmount itemAmount, RangeResource rangeResource)
        {
            _itemConsumer = itemConsumer;
            _itemAmount = itemAmount;
            _rangeResource = rangeResource;
            
            if (rangeResource == RangeResource.None)
                resourceUI.SetItem(itemAmount.item);
            else
                resourceUI.SetRangeResource(rangeResource);

            Refresh();
        }

        public void Refresh()
        { 
            if (_rangeResource == RangeResource.None)
            {
                bool isItemProvided = _itemConsumer.Snapshot.Any(anyResource =>
                anyResource.AnyItem.HasValue && anyResource.AnyItem == _itemAmount.item);

                int displayAmount = isItemProvided ? _itemAmount.amount : 0;
                text.text = $"{_itemAmount.item}: {displayAmount} / {_itemAmount.amount}";
            }
                
            else
            {
                string status = _itemConsumer.Snapshot.Any(anyResource => anyResource.AnyRangeResource.HasValue &&
                                                                          anyResource.AnyRangeResource == _rangeResource)
                                    ? "충족" : "부족";
                text.text = $"{_rangeResource}: {status}";
            }
        }
    }
}