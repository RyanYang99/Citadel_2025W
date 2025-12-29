using UnityEngine;

public class BuildScrollController : MonoBehaviour
{
    public BuildingManager buildingManager;
    public Transform content;
    public BuildItemButton itemPrefab;

    public BuildingCategory currentCategory = BuildingCategory.Tile;

    void Start()
    {
        Refresh();
    }
    public void SetCategoryByIndex(int categoryIndex)
    {
        SetCategory((BuildingCategory)categoryIndex);
    }

    public void SetCategory(BuildingCategory category)
    {
        currentCategory = category;
        Refresh();
    }
     
    void Refresh()
    {
        // 기존 아이템 제거
        foreach (Transform child in content)
            Destroy(child.gameObject);

        var buildings = buildingManager.buildings;

        for (int i = 0; i < buildings.Length; i++)
        {
            if (buildings[i].category != currentCategory)
                continue;

            var item = Instantiate(itemPrefab, content);
            item.Init(i, buildingManager, buildings[i].icon);
        }
    }
}
