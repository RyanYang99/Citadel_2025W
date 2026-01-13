using UnityEngine;

namespace Citadel
{
    public sealed class BuildScrollController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform content;
        [SerializeField] private BuildingManager buildingManager;
        [SerializeField] private BuildItemButton itemPrefab;
        [SerializeField] private BuildingPlacer buildingPlacer;
        [SerializeField] private BuildingMetaDataList buildingMetaDataList;

        private BuildingCategory currentCategory = BuildingCategory.Tile;

   

        private void Start()
        {
            Refresh();
        }

        public void SetCategoryByIndex(int categoryIndex)
        {
            currentCategory = (BuildingCategory)categoryIndex;
            Refresh();
        }

        private void Refresh()
        {
            Clear();

            if (buildingMetaDataList == null)
            {
                Debug.LogError("[BuildScrollController] buildingMetaDataList is NULL");
                return;
            }

            foreach (var meta in buildingMetaDataList.list)
            {
                if (meta == null)
                {
                    Debug.LogError("[Scroll] metaList¿¡ NULL ÀÖÀ½");
                    continue;
                }

                if (meta.category != currentCategory)
                    continue;

                BuildItemButton btn = Instantiate(itemPrefab, content);
                btn.Init(buildingManager, buildingPlacer, meta);
            }
        }

        private void Clear()
        {
            for (int i = content.childCount - 1; i >= 0; i--)
            {
                Destroy(content.GetChild(i).gameObject);
            }
        }
    }
}



//using UnityEngine;

//namespace Citadel
//{
//    public sealed class BuildScrollController : MonoBehaviour
//    {
//        [SerializeField] private Transform _transform;
//        [SerializeField] private BuildingManager buildingManager;
//        [SerializeField] private BuildItemButton itemPrefab;
//        [SerializeField] private BuildingPlacer buildingPlacer;
//        private BuildingMetaData meta;

//        private BuildingCategory _currentCategory = BuildingCategory.Tile;
        
//        private void Start() => Refresh();

//        private void OnEnable()
//        {
//            buildingManager.OnBuildingCountChanged += Refresh;
//        }

//        private void OnDisable()
//        {
//            buildingManager.OnBuildingCountChanged -= Refresh;
//        }

//        private void SetCategory(BuildingCategory category)
//        {
//            _currentCategory = category;
//            Refresh();
//        }

//        private void Refresh()
//        {
//            Debug.Log($"[Refresh] meta = {(meta == null ? "NULL" : meta.uniqueName)}");

//            foreach (Transform child in _transform)
//                Destroy(child.gameObject);

//            BuildingMetaDataList buildings = buildingManager.Buildings;
//            for (int i = 0; i < buildings.list.Count; i++)
//            {
//                if (buildings.list[i].category != _currentCategory)
//                    continue;

//                BuildItemButton item = Instantiate(itemPrefab, _transform);
//                item.Init(i, buildingManager, buildingPlacer, meta);
//            }
//        }

//        public void SetCategoryByIndex(int categoryIndex) => SetCategory((BuildingCategory)categoryIndex);
//    }
//}