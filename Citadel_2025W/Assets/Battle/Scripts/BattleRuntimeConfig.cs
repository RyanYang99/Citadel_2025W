using UnityEngine;

public struct BattleRuntimeConfig
{
    public int targetKills;

    public float playerInterval;
    public int playerMaxAlive;

    public float enemyInterval;
    public int enemyMaxAlive;

    public float enemyHpMul;
    public float enemyDmgMul;

    public int wInfantry, wArcher, wShield;

    public static BattleRuntimeConfig Build(BattleBalanceSettings s, int castleLevel)
    {
        int L = Mathf.Max(1, castleLevel);

        BattleRuntimeConfig c = new BattleRuntimeConfig();

        c.targetKills = s.baseTargetKills + s.killsPerCastleLevel * (L - 1);

        c.playerInterval = Mathf.Max(s.minSpawnInterval, s.playerBaseInterval + s.playerIntervalPerLevel * (L - 1));
        c.playerMaxAlive = Mathf.Min(s.maxMaxAliveCap, s.playerBaseMaxAlive + s.playerMaxAlivePerLevel * (L - 1));

        c.enemyInterval = Mathf.Max(s.minSpawnInterval, s.enemyBaseInterval + s.enemyIntervalPerLevel * (L - 1));
        c.enemyMaxAlive = Mathf.Min(s.maxMaxAliveCap, s.enemyBaseMaxAlive + s.enemyMaxAlivePerLevel * (L - 1));

        c.enemyHpMul = s.enemyHpMulBase + s.enemyHpMulPerLevel * (L - 1);
        c.enemyDmgMul = s.enemyDmgMulBase + s.enemyDmgMulPerLevel * (L - 1);

        c.wInfantry = Mathf.Clamp(s.infantryWeightBase, 0, 100);
        c.wArcher = Mathf.Clamp(s.archerWeightBase, 0, 100);
        c.wShield = (L >= s.shieldUnlockLevel) ? Mathf.Clamp(s.shieldWeightAfterUnlock, 0, 100) : 0;

        return c;
    }
}
