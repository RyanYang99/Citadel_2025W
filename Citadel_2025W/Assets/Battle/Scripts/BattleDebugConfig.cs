using UnityEngine;

public class BattleDebugConfig : MonoBehaviour
{
    [Header("Debug Override")]
    [Min(1)] public int castleLevel = 1;

    [Tooltip("체크하면 항상 위 castleLevel을 사용. (메인씬 연동 전 테스트용)")]
    public bool forceUseDebugCastleLevel = true;
}