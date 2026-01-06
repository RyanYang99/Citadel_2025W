using System;
using UnityEngine;

namespace Citadel
{
    [CreateAssetMenu(menuName = "Citadel/Building Metadata"), Serializable]
    public sealed class BuildingMetaData : ScriptableObject
    {
        public string uniqueName;
        
        public GameObject prefab;
        public Sprite icon;
        public BuildingCategory category;
        public BuildingSubCategory subCategory; // 건물종류 카테고리 추가

        public float yOffset = 1f;
    }
}