using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private BattleBalanceSettings balance;
    [SerializeField] private BattleDebugConfig debugConfig;
    [SerializeField] private SpawnController spawner;

    private BattleRuntimeConfig _cfg;
    private int _enemyKills;

    private int _zoneId;
    private bool _battleEnded;

    private void Awake()
    {
        if (debugConfig == null) debugConfig = GetComponent<BattleDebugConfig>();
    }

    private void Start()
    {
        int castleLevel = 1;
        int soldierCount = 0;

        BattleSession.BattleRequest req = default;

        if (BattleSession.TryGetRequest(out req))
        {
            _zoneId = req.zoneId;
            castleLevel = req.castleLevel;
            soldierCount = req.playerSoldierCount;

            Debug.Log($"[BattleScene] Received Request: zoneId={req.zoneId}, castleLevel={req.castleLevel}, playerSoldierCount={req.playerSoldierCount}");
        }
        else
        {
            Debug.LogWarning("[BattleScene] No BattleRequest found. Using defaults.");
            _zoneId = 0;
        }

        _cfg = BattleRuntimeConfig.Build(balance, castleLevel);

        UnitRuntime.SetBattleManager(this);

        spawner.Configure(_cfg);
        spawner.SetPlayerTotalSupply(soldierCount);
        spawner.Begin();
    }


    public void NotifyEnemyKilled()
    {
        if (_battleEnded) return;

        _enemyKills++;

        if (_enemyKills >= _cfg.targetKills)
        {
            _battleEnded = true;
            spawner.Stop();

            Debug.Log("[Battle] Victory!");

            BattleSession.SetResult(new BattleSession.BattleResult
            {
                zoneId = _zoneId,
                victory = true
            });

            SceneManager.LoadScene("MainScene");
        }
    }


}
