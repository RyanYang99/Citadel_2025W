using UnityEngine;

namespace Citadel
{
    public sealed class BuildingManager : MonoBehaviour
    {
        [Header("건물 데이터 목록 (프리팹 + 높이 보정)")]
        public BuildingMetaData[] buildings;

        public int CurrentIndex { get; private set; } = -1;

        public BuildingMetaData CurrentBuilding
        {
            get
            {
                if (CurrentIndex < 0 || CurrentIndex >= buildings.Length)
                    return null;

                return buildings[CurrentIndex];
            }
        }

        public void SelectBuilding(int index)
        {
            if (index < 0 || index >= buildings.Length)
                return;

            CurrentIndex = index;
            Debug.Log($"선택된 건물 인덱스: {index}");
        }

        public void ClearSelection() => CurrentIndex = -1;
    }
}