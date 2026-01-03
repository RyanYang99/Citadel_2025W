using System;
using System.Collections.Generic;
using UnityEngine;

namespace Citadel
{
    [CreateAssetMenu(menuName = "Citadel/Building Metadata List"), Serializable]
    public sealed class BuildingMetaDataList : ScriptableObject
    {
        public List<BuildingMetaData> list;
    }
}