using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private BattleBalanceSettings balance;
    [SerializeField] private BattleDebugConfig debugConfig;
    [SerializeField] private SpawnController spawner;

    private BattleRuntimeConfig _cfg;
    private int _enemyKills;

    public void DebugForceWin()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        VictoryAndExit();
#endif
    }

    private void VictoryAndExit()
    {
        Debug.Log("[Battle] Victory (Forced)");

        if (spawner != null)
            spawner.Stop();

        SceneManager.LoadScene("MainScene");
    }

    //테스트 디버그 exe 용
    public void ForceWinAndExit()
    {
        Debug.Log("[Battle] Force Win (Test)");

        if (spawner != null)
            spawner.Stop();

        SceneManager.LoadScene("MainScene");
    }

    private void Awake()
    {
        if (debugConfig == null) debugConfig = GetComponent<BattleDebugConfig>();
    }

    private void Start()
    {
        int castleLevel = 1;
        int soldierCount = 0;

        if (BattleSession.TryGetRequest(out var req))
        {
            castleLevel = req.castleLevel;
            soldierCount = req.playerSoldierCount;
            Debug.Log($"[BattleScene] Received Request: zoneId={req.zoneId}, castleLevel={req.castleLevel}, playerSoldierCount={req.playerSoldierCount}");
        }

        _cfg = BattleRuntimeConfig.Build(balance, castleLevel);

        UnitRuntime.SetBattleManager(this);

        spawner.Configure(_cfg);

        spawner.SetPlayerTotalSupply(soldierCount);

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

            SceneManager.LoadScene("MainScene");
        }
    }
}
