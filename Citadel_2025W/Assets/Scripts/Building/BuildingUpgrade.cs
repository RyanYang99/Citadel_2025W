using UnityEngine;
using System.Collections.Generic;

namespace Citadel
{
    public sealed class BuildingUpgrade : MonoBehaviour
    {
        public static BuildingUpgrade Instance { get; private set; }

        [SerializeField, Tooltip("자원을 관리하는 인벤토리 참조")]
        private Inventory inventory;

        // 건물 타입별 레벨 프리팹 리스트를 에디터에서 설정하기 위한 구조체
        [System.Serializable]
        public struct BuildingLevelData
        {
            public BuildingSubCategory subCategory;
            [Tooltip("레벨 1부터 5까지의 프리팹을 순서대로 할당하세요.")]
            public GameObject[] levelPrefabs;
        }

        [Header("건물 레벨별 모델 설정")]
        public List<BuildingLevelData> levelDataList;

        private Dictionary<GameObject, int> buildingLevels = new Dictionary<GameObject, int>();

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            if (inventory == null)
                inventory = FindFirstObjectByType<Inventory>();
        }

        /// <summary>
        /// 건물을 처음 설치할 때 1레벨로 등록
        /// </summary>
        public void RegisterNewBuilding(GameObject buildingObj)
        {
            if (!buildingLevels.ContainsKey(buildingObj))
            {
                buildingLevels.Add(buildingObj, 1);
            }
        }

        /// <summary>
        /// 특정 건물을 다음 레벨로 업그레이드 (최대 5레벨)
        /// </summary>
        public bool TryUpgrade(GameObject buildingObj, BuildingSubCategory subCategory)
        {
            if (!buildingLevels.ContainsKey(buildingObj))
            {
                // 만약 리스트에 없다면 1레벨로 간주하고 시작
                buildingLevels[buildingObj] = 1;
            }

            int currentLevel = buildingLevels[buildingObj];
            int nextLevel = currentLevel + 1;

            // 최대 레벨 체크
            if (nextLevel > 5)
            {
                Debug.LogWarning($"{subCategory}: 이미 최고 레벨(5)입니다.");
                return false;
            }

            // 자원 소모 체크 (레벨별 2배씩 증가)
            if (TryConsumeForLevel(subCategory, nextLevel))
            {
                // 프리팹 변경
                ReplaceBuildingModel(buildingObj, subCategory, nextLevel);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 레벨에 따른 자원 소모 (2의 n승 배수)
        /// </summary>
        private bool TryConsumeForLevel(BuildingSubCategory subCategory, int level)
        {
            List<ItemAmount> baseReqs = GetBaseRequirements(subCategory);
            // 배수 계산: 1레벨(1배), 2레벨(2배), 3레벨(4배), 4레벨(8배), 5레벨(16배)
            int multiplier = Mathf.RoundToInt(Mathf.Pow(2, level - 1));

            // 자원 충분한지 확인
            foreach (var req in baseReqs)
            {
                int needed = req.amount * multiplier;
                if (inventory.GetAmount(req.item) < needed)
                {
                    Debug.LogWarning($"자원 부족: {req.item} (보유: {inventory.GetAmount(req.item)} / 필요: {needed})");
                    return false;
                }
            }

            // 실제 자원 차감
            foreach (var req in baseReqs)
            {
                inventory.Consume(req.item, req.amount * multiplier);
            }

            return true;
        }

        /// <summary>
        /// 기존 건물을 다음 레벨의 프리팹으로 교체
        /// </summary>
        private void ReplaceBuildingModel(GameObject oldObj, BuildingSubCategory subCategory, int nextLevel)
        {
            var data = levelDataList.Find(x => x.subCategory == subCategory);
            if (data.levelPrefabs == null || data.levelPrefabs.Length < nextLevel)
            {
                Debug.LogError($"{subCategory}의 {nextLevel}레벨 프리팹이 설정되지 않았습니다.");
                return;
            }

            GameObject nextPrefab = data.levelPrefabs[nextLevel - 1];

            // 위치 및 회전값 복사
            Vector3 pos = oldObj.transform.position;
            Quaternion rot = oldObj.transform.rotation;

            // 새 건물 생성
            GameObject newObj = Instantiate(nextPrefab, pos, rot);

            var manager = FindFirstObjectByType<BuildingManager>();
            var placed = manager.FindPlacedBuilding(oldObj);
            if (placed != null)
            {
                placed._GameObject = newObj;
            }

            // 레벨 데이터 이전 및 기존 오브젝트 삭제
            buildingLevels.Add(newObj, nextLevel);
            buildingLevels.Remove(oldObj);
            Destroy(oldObj);

            Debug.Log($"{subCategory}가 {nextLevel}레벨로 업그레이드 되었습니다.");
        }

        private List<ItemAmount> GetBaseRequirements(BuildingSubCategory subCategory)
        {
            List<ItemAmount> reqs = new List<ItemAmount>();
            switch (subCategory)
            {
                case BuildingSubCategory.House:
                    reqs.Add(new ItemAmount(Item.Wood, 2)); break;
                case BuildingSubCategory.Castle:
                    reqs.Add(new ItemAmount(Item.Wood, 2));
                    reqs.Add(new ItemAmount(Item.Stone, 2));
                    reqs.Add(new ItemAmount(Item.Brick, 2)); break;
                case BuildingSubCategory.Warehouse:
                    reqs.Add(new ItemAmount(Item.Wood, 3));
                    reqs.Add(new ItemAmount(Item.Stone, 2)); break;
                case BuildingSubCategory.Well:
                    reqs.Add(new ItemAmount(Item.Stone, 3)); break;
            }
            return reqs;
        }
    }
}