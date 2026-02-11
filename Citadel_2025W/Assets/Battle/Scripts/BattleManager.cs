using Citadel;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{



    [SerializeField] private BattleBalanceSettings balance;
    [SerializeField] private BattleDebugConfig debugConfig;
    [SerializeField] private SpawnController spawner;

    [Header("Animator")]
    [SerializeField] private Citadel.BattleMapFillAnimator mapFill;

    private BattleRuntimeConfig _cfg;
    private int _enemyKills;

    private int _zoneId;
    private bool _battleEnded;
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

        BattleSession.SetResult(new BattleSession.BattleResult
        {
            zoneId = _zoneId,
            victory = true
        });

        SceneManager.LoadScene("MainScene");
    }

    //테스트 디버그 exe 용
    public void ForceWinAndExit()
    {
        Debug.Log("[Battle] Force Win (Test)");

        if (spawner != null)
            spawner.Stop();

        BattleSession.SetResult(new BattleSession.BattleResult
        {
            zoneId = _zoneId,
            victory = true
        });
        SceneManager.LoadScene("MainScene");
    }


    private void Awake()
    {
        if (debugConfig == null) debugConfig = GetComponent<BattleDebugConfig>();
    }

    private IEnumerator Start()
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

        // 맵 깔기 
        if (mapFill != null)
        {
            mapFill.Clear();
            yield return mapFill.PlayFill(); // 6x29 중앙 ㅡ 깔림
        }
        else
        {
            Debug.LogWarning("[BattleManager] mapFill not assigned");
        }
        // 전투 시작
        spawner.Begin();
    }

    //private void Start()
    //{
    //    int castleLevel = 1;
    //    int soldierCount = 0;

    //    BattleSession.BattleRequest req = default;

    //    if (BattleSession.TryGetRequest(out req))
    //    {
    //        _zoneId = req.zoneId;
    //        castleLevel = req.castleLevel;
    //        soldierCount = req.playerSoldierCount;

    //        Debug.Log($"[BattleScene] Received Request: zoneId={req.zoneId}, castleLevel={req.castleLevel}, playerSoldierCount={req.playerSoldierCount}");
    //    }
    //    else
    //    {
    //        Debug.LogWarning("[BattleScene] No BattleRequest found. Using defaults.");
    //        _zoneId = 0;
    //    }

    //    _cfg = BattleRuntimeConfig.Build(balance, castleLevel);

    //    UnitRuntime.SetBattleManager(this);

    //    spawner.Configure(_cfg);

    //    spawner.SetPlayerTotalSupply(soldierCount);

    //    spawner.Begin();
    //}


    public void NotifyEnemyKilled()
    {
        if (_battleEnded) return;

        _enemyKills++;


        // UI 붙일 거면 여기서 갱신

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