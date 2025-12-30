using UnityEngine;
using System.Collections.Generic; // List를 쓰기 위해 반드시 필요

// 1. 타입을 정의 (이건 파일 안에 같이 있어도 됩니다)
public enum ResearchType
{
    Gold,
    Cityzen,
    Happy,
    Attack
}

// 2. 개별 효과를 담는 클래스 (반드시 [System.Serializable]이 있어야 인스펙터에 보입니다)
[System.Serializable]
public class ResearchEffect
{
    public ResearchType type;
    public float value;
}

// 3. 실제 데이터 파일이 될 클래스 (파일 이름과 동일해야 함)
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