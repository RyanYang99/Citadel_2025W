using UnityEngine;

namespace Citadel
{
    public sealed class BuildScrollController : MonoBehaviour
    {
        [SerializeField] private Transform _transform;
        [SerializeField] private BuildingManager buildingManager;
        [SerializeField] private BuildItemButton itemPrefab;

        private BuildingCategory _currentCategory = BuildingCategory.Tile;

        private void Start() => Refresh();
        
        private void SetCategory(BuildingCategory category)
        {
            _currentCategory = category;
            Refresh();
        }
        
        private void Refresh()
        {
            foreach (Transform child in _transform)
                Destroy(child.gameObject);

            BuildingMetaDataList buildings = buildingManager.Buildings;
            for (int i = 0; i < buildings.list.Count; i++)
            {
                if (buildings.list[i].category != _currentCategory)
                    continue;

                BuildItemButton item = Instantiate(itemPrefab, _transform);
                item.Init(i, buildingManager, buildings.list[i].icon);
            }
        }
        
        public void SetCategoryByIndex(int categoryIndex) => SetCategory((BuildingCategory)categoryIndex);
    }
}