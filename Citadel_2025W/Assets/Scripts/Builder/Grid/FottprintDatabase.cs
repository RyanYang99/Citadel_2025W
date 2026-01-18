using System;
using System.Collections.Generic;
using UnityEngine;

namespace Citadel
{
    public enum GridPivot { BottomLeft } // 필요하면 나중에 확장

    [Serializable]
    public class FootprintOverride
    {
        public string uniqueName;
        public Vector2Int size = Vector2Int.one; // (x,z)
        public GridPivot pivot = GridPivot.BottomLeft;
    }

    [CreateAssetMenu(menuName = "Citadel/Grid/Footprint Database")]
    public sealed class FootprintDatabase : ScriptableObject
    {
        public List<FootprintOverride> overrides = new();

        public bool TryGet(string uniqueName, out FootprintOverride ov)
        {
            ov = overrides.Find(x => x != null && x.uniqueName == uniqueName);
            return ov != null;
        }
    }
}
