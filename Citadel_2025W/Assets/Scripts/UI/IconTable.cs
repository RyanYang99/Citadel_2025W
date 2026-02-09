using UnityEngine;
using System;
using System.Linq;

namespace Citadel
{
    [Serializable]
    public class ItemIcon
    {
        public Item item;
        public Sprite sprite;
    }

    [Serializable]
    public sealed class RangeResourceIcon
    {
        public RangeResource rangeResource;
        public Sprite sprite;
    }

    [CreateAssetMenu(menuName = "Citadel/Icon Table")]
    public sealed class IconTable : ScriptableObject
    {
        [SerializeField] private ItemIcon[] itemIcons;
        [SerializeField] private RangeResourceIcon[] rangeResourceIcons;
        
        public Sprite Find(Item item) => itemIcons.FirstOrDefault(itemIcon => itemIcon.item == item)?.sprite;
        
        public Sprite Find(RangeResource rangeResource) => rangeResourceIcons.FirstOrDefault(itemIcon => itemIcon.rangeResource == rangeResource)?.sprite;
    }
}