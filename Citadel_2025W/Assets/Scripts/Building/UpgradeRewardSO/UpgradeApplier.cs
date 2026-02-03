using UnityEngine;
using System.Collections.Generic;
using Citadel;

public class UpgradeApplier : MonoBehaviour
{
    [SerializeField] private List<UpgradeRewardSO> upgradeTables;

    private void OnEnable()
    {
        BuildingUpgrade.OnBuildingUpgraded += HandleUpgradeEffect;
    }

    private void OnDisable()
    {
        BuildingUpgrade.OnBuildingUpgraded -= HandleUpgradeEffect;
    }

    private void HandleUpgradeEffect(GameObject building, BuildingSubCategory category, int level)
    {
        // 해당 건물 카테고리에 맞는 테이블 찾기
        var table = upgradeTables.Find(t => t.targetCategory == category);
        if (table == null) return;

        // 해당 레벨의 보상 데이터 가져오기
        var reward = table.GetRewardForLevel(level);

        // 아이템 보상 지급 로직 추가
        if (reward.rewardItems != null && reward.rewardItems.Count > 0)
        {
            var inv = FindFirstObjectByType<Inventory>();
            if (inv != null)
            {
                foreach (var itemAmt in reward.rewardItems)
                {
                    inv.Add(itemAmt.item, itemAmt.amount);
                    Debug.Log($"<color=yellow>[업그레이드 보상]</color> {category} {level}레벨 달성! {itemAmt.item} {itemAmt.amount}개 지급");
                }
            }
        }
    }
}