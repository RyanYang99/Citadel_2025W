using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Citadel
{
    public sealed class BuildingUpgrade : MonoBehaviour
    {
        public static BuildingUpgrade Instance { get; private set; }

        [SerializeField, Tooltip("자원을 관리하는 인벤토리 참조")]
        private Inventory inventory;

        [Header("업그레이드 연출 설정")]
        [SerializeField] private GameObject upgradeParticlePrefab; // 먼지 구름 등 파티클 프리팹
        [SerializeField] private float bounceDuration = 0.15f;      // 튀어오르는 속도
        [SerializeField] private float bounceScaleMultiplier = 1.2f; // 얼마나 크게 튈지 (1.2배)

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

        public void RegisterNewBuilding(GameObject buildingObj)
        {
            if (!buildingLevels.ContainsKey(buildingObj))
            {
                buildingLevels.Add(buildingObj, 1);
            }
        }

        public bool TryUpgrade(GameObject buildingObj, BuildingSubCategory subCategory)
        {
            if (!buildingLevels.ContainsKey(buildingObj))
            {
                buildingLevels[buildingObj] = 1;
            }

            int currentLevel = buildingLevels[buildingObj];
            int nextLevel = currentLevel + 1;

            if (nextLevel > 5)
            {
                Debug.LogWarning($"{subCategory}: 이미 최고 레벨(5)입니다.");
                return false;
            }

            if (TryConsumeForLevel(subCategory, nextLevel))
            {
                // 모델 교체 및 모션 실행
                ReplaceBuildingWithMotion(buildingObj, subCategory, nextLevel);
                return true;
            }

            return false;
        }

        private void ReplaceBuildingWithMotion(GameObject oldObj, BuildingSubCategory subCategory, int nextLevel)
        {
            var data = levelDataList.Find(x => x.subCategory == subCategory);
            if (data.levelPrefabs == null || data.levelPrefabs.Length < nextLevel)
            {
                Debug.LogError($"{subCategory}의 {nextLevel}레벨 프리팹이 설정되지 않았습니다.");
                return;
            }

            GameObject nextPrefab = data.levelPrefabs[nextLevel - 1];
            Vector3 pos = oldObj.transform.position;
            Quaternion rot = oldObj.transform.rotation;

            // 1. 파티클 효과 생성
            if (upgradeParticlePrefab != null)
            {
                Instantiate(upgradeParticlePrefab, pos, Quaternion.identity);
            }

            // 2. 새 건물 생성
            GameObject newObj = Instantiate(nextPrefab, pos, rot);

            // 3. 관리 데이터 갱신
            var manager = FindFirstObjectByType<BuildingManager>();
            var placed = manager.FindPlacedBuilding(oldObj);
            if (placed != null)
            {
                placed._GameObject = newObj;
            }

            buildingLevels.Add(newObj, nextLevel);
            buildingLevels.Remove(oldObj);
            Destroy(oldObj);

            // 4. [모션 연출] 튀어오르는 코루틴 실행
            StartCoroutine(AnimateUpgradeScale(newObj.transform));
        }

        private IEnumerator AnimateUpgradeScale(Transform target)
        {
            if (target == null) yield break;

            Vector3 originalScale = target.localScale;
            Vector3 peakScale = originalScale * bounceScaleMultiplier;

            // 커지는 구간
            float elapsed = 0f;
            while (elapsed < bounceDuration)
            {
                elapsed += Time.deltaTime;
                target.localScale = Vector3.Lerp(Vector3.zero, peakScale, elapsed / bounceDuration);
                yield return null;
            }

            // 원래 크기로 돌아오는 구간
            elapsed = 0f;
            while (elapsed < bounceDuration)
            {
                elapsed += Time.deltaTime;
                target.localScale = Vector3.Lerp(peakScale, originalScale, elapsed / bounceDuration);
                yield return null;
            }

            target.localScale = originalScale;
        }

        private bool TryConsumeForLevel(BuildingSubCategory subCategory, int level)
        {
            List<ItemAmount> baseReqs = GetBaseRequirements(subCategory);
            int multiplier = Mathf.RoundToInt(Mathf.Pow(2, level - 1));

            foreach (var req in baseReqs)
            {
                int needed = req.amount * multiplier;
                if (inventory.GetAmount(req.item) < needed)
                {
                    Debug.LogWarning($"자원 부족: {req.item} 필요: {needed}");
                    return false;
                }
            }

            foreach (var req in baseReqs)
            {
                inventory.Consume(req.item, req.amount * multiplier);
            }

            return true;
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