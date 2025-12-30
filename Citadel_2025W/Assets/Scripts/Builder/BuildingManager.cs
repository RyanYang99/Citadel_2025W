using UnityEngine;

[System.Serializable]
public class BuildingData
{
    public GameObject prefab;
    public Sprite icon;
    public float yOffset = 1f;
    public BuildingCategory category;
}

public class BuildingManager : MonoBehaviour
{
    [Header("건물 데이터 목록 (프리팹 + 높이 보정)")]
    public BuildingData[] buildings;

    public int CurrentIndex { get; private set; } = -1;

    public BuildingData CurrentBuilding
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

    public void ClearSelection()
    {
        CurrentIndex = -1;
    }
}
