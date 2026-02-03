using UnityEngine;
using System.Collections.Generic;
using System;
using Citadel;

[CreateAssetMenu(fileName = "NewBuildingUpgradeTable", menuName = "Citadel/Upgrade Table")]
public class UpgradeRewardSO : ScriptableObject
{
    [Serializable]
    public struct LevelReward
    {
        [Tooltip("보상을 지급할 목표 레벨을 설정합니다.")]
        public int level;

        [Tooltip("해당 레벨 달성 시 인벤토리에 즉시 추가될 아이템 리스트입니다.")]
        public List<ItemAmount> rewardItems;

        //[Tooltip("이 건물에서 생산하는 자원의 기본 수치를 이만큼 증가시킵니다.")]
        //public int bonusProductionFlat;
    }

    [Header("대상 설정")]
    [Tooltip("어떤 종류의 건물을 위한 업그레이드 테이블인지 선택하세요.")]
    public BuildingSubCategory targetCategory;

    [Header("레벨별 보상")]
    [Tooltip("레벨별로 받을 보상 데이터들을 리스트로 관리합니다.")]
    public List<LevelReward> levelRewards;

    public LevelReward GetRewardForLevel(int level)
    {
        return levelRewards.Find(r => r.level == level);
    }
}
