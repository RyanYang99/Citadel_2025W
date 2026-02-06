using System.Linq;
using TMPro;
using UnityEngine;

namespace Citadel
{
    public sealed class ProducerStatusItemController : MonoBehaviour
    {
        private ItemProducer _itemProducer;
        private ItemAmount _itemAmount;
        private RangeResource _rangeResource;
        private int _tickDuration;
        
        [SerializeField] private ResourceUI resourceUI;
        [SerializeField] private TMP_Text text;

        public void Initialize(ItemProducer itemProducer,
                               ItemAmount itemAmount,
                               RangeResource rangeResource,
                               int tickDuration)
        {
            _itemProducer = itemProducer;
            _itemAmount = itemAmount;
            _rangeResource = rangeResource;
            _tickDuration = tickDuration;
            
            if (rangeResource == RangeResource.None)
                resourceUI.SetItem(itemAmount.item);
            else
                resourceUI.SetRangeResource(rangeResource);

            Refresh();
        }

        public void Refresh() => text.text = _rangeResource == RangeResource.None ?
                                                 $"{_itemAmount.item}: {_itemAmount.amount}" :
                                                 $"{_rangeResource}: {_itemProducer.RangeResourceDurations.First(rangeResourceAmount => rangeResourceAmount.rangeResource == _rangeResource).tickDuration} / {_tickDuration}";
    }
}