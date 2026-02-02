using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private BattleBalanceSettings balance;
    [SerializeField] private BattleDebugConfig debugConfig;
    [SerializeField] private SpawnController spawner;

    private BattleRuntimeConfig _cfg;
    private int _enemyKills;

    private void Awake()
    {
        if (debugConfig == null) debugConfig = GetComponent<BattleDebugConfig>();
    }

    private void Start()
    {
        int castleLevel = 1;
        if (debugConfig != null && debugConfig.forceUseDebugCastleLevel)
            castleLevel = debugConfig.castleLevel;

        _cfg = BattleRuntimeConfig.Build(balance, castleLevel);

        Debug.Log($"[Battle] L={castleLevel} targetKills={_cfg.targetKills}");

        // 킬 카운트는 UnitRuntime에서 콜백으로 올라오게 할 거라
        // UnitRuntime에 BattleManager 참조를 주는 방식/이벤트 방식 중 택1 가능
        UnitRuntime.SetBattleManager(this);

        spawner.Configure(_cfg);
        spawner.Begin();
    }

    public void NotifyEnemyKilled()
    {
        _enemyKills++;
        // UI 붙일 거면 여기서 갱신
        if (_enemyKills >= _cfg.targetKills)
        {
            Debug.Log("[Battle] Victory!");
            spawner.Stop();
        }
    }
}
