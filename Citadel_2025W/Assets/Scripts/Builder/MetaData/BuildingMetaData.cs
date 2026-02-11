using System;
using UnityEngine;

namespace Citadel
{
    [CreateAssetMenu(menuName = "Citadel/Building Metadata"), Serializable]
    public sealed class BuildingMetaData : ScriptableObject
    {
        public string uniqueName;
        [TextArea] public string description;
        
        public GameObject prefab;
        public Sprite icon;
        public BuildingCategory category;
        public BuildingSubCategory subCategory;

        public float yOffset = 1f;

        [Header("Build Limit")] public int maxBuildCount = -1;
        [Header("Cost")] public ItemAmount[] costItems;
    }
}