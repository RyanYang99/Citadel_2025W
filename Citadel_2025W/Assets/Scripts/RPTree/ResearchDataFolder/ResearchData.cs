using UnityEngine;
using System.Collections.Generic;

// 2. 개별 효과를 담는 클래스
[System.Serializable]
public class ResearchEffect
{
    // 아이템 보너스인지 범위 자원 보너스인지 체크
    public bool isItem;

    public Citadel.Item targetItem;
    public Citadel.RangeResource targetResource;

    public float value;
    public bool isPercentage;
}

// 3. 실제 데이터 파일 클래스
[CreateAssetMenu(fileName = "New Research", menuName = "Research/Research Data")]
public class ResearchData : ScriptableObject
{
    [Header("기본 정보")]
    public string researchName;
    [TextArea] public string explanation;
    public int cost;

    [Header("선행이 필요한 연구")]
    public List<ResearchData> requiredResearches = new List<ResearchData>();

    [Header("효과 설정")]
    public List<ResearchEffect> effects = new List<ResearchEffect>();

    [System.NonSerialized] public bool isUnlocked = false;
}