using System;
using System.Collections.Generic;
using UnityEngine;

namespace Citadel
{
    [CreateAssetMenu(menuName = "Citadel/SatisfactionImportance")]
    public sealed class SatisfactionImportance : ScriptableObject
    {
        [Serializable]
        public sealed class ItemImportance
        {
            public Item item;
            [Range(0f, 1f)] public float importance;
        }

        [Serializable]
        public sealed class RangeResourceImportance
        {
            public RangeResource rangeResource;
            [Range(0f, 1f)] public float importance;
        }

        public List<ItemImportance> itemImportances = new();
        public List<RangeResourceImportance> rangeResourceImportances = new();
    }
}