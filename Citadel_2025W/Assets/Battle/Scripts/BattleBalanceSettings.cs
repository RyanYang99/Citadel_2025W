using UnityEngine;

[CreateAssetMenu(menuName = "Battle/Balance Settings", fileName = "BattleBalanceSettings")]
public class BattleBalanceSettings : ScriptableObject
{
    [Header("Kill Target")]
    public int baseTargetKills = 20;
    public int killsPerCastleLevel = 2;

    [Header("Player Spawn")]
    public float playerBaseInterval = 1.4f;
    public float playerIntervalPerLevel = -0.05f; // ·¹º§ºñ·Ê ½ºÆùºóµµ
    public int playerBaseMaxAlive = 10;
    public int playerMaxAlivePerLevel = 1;

    [Header("Enemy Spawn")]
    public float enemyBaseInterval = 1.8f;
    public float enemyIntervalPerLevel = -0.05f;
    public int enemyBaseMaxAlive = 8;
    public int enemyMaxAlivePerLevel = 1;

    [Header("Enemy Stat Multipliers")]
    public float enemyHpMulBase = 1.0f;
    public float enemyHpMulPerLevel = 0.08f;
    public float enemyDmgMulBase = 1.0f;
    public float enemyDmgMulPerLevel = 0.06f;

    [Header("Enemy Composition")]
    [Range(0, 100)] public int infantryWeightBase = 70;
    [Range(0, 100)] public int archerWeightBase = 30;

    public int shieldUnlockLevel = 3;
    [Range(0, 100)] public int shieldWeightAfterUnlock = 25;

    [Header("Clamps")]
    public float minSpawnInterval = 0.35f;
    public int maxMaxAliveCap = 60;
}