using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private UnitFactory factory;
    [SerializeField] private Transform unitsRoot;

    [Header("Spawn Points")]
    [SerializeField] private List<Transform> playerSpawnPoints;
    [SerializeField] private List<Transform> enemySpawnPoints;

    [Header("Spawn Weights (%) - Both Teams")]
    [Range(0, 100)][SerializeField] private int wInfantry = 40;
    [Range(0, 100)][SerializeField] private int wArcher = 20;
    [Range(0, 100)][SerializeField] private int wShield = 40;

    [Header("Ground Snap (optional)")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float raycastHeight = 50f;
    [SerializeField] private float groundOffsetY = 0f;

    private BattleRuntimeConfig _cfg;
    private Coroutine _playerCo;
    private Coroutine _enemyCo;

    private int _playerAlive;
    private int _enemyAlive;

    private int _playerRemaining;

    public void Configure(BattleRuntimeConfig cfg)
    {
        _cfg = cfg;
    }

    public void SetPlayerPool(int soldierCount)
    {
        _playerRemaining = Mathf.Max(0, soldierCount);
    }

    public void Begin()
    {
        Stop();
        _playerAlive = 0;
        _enemyAlive = 0;

        _playerCo = StartCoroutine(PlayerLoop());
        _enemyCo = StartCoroutine(EnemyLoop());
    }

    public void Stop()
    {
        if (_playerCo != null) StopCoroutine(_playerCo);
        if (_enemyCo != null) StopCoroutine(_enemyCo);
        _playerCo = _enemyCo = null;
    }

    private IEnumerator PlayerLoop()
    {
        while (true)
        {
            // 남은 병력이 0이면 더 이상 스폰 안 함
            if (_playerRemaining <= 0)
            {
                yield return new WaitForSeconds(0.2f);
                continue;
            }

            if (_playerAlive < _cfg.playerMaxAlive)
            {
                Transform sp = playerSpawnPoints[Random.Range(0, playerSpawnPoints.Count)];

                UnitType t = RollUnitType_40_20_40();
                Vector3 pos = SnapToGround(sp.position);

                var go = factory.Spawn(t, pos, unitsRoot);
                if (go != null)
                {
                    var u = go.GetComponent<UnitRuntime>();
                    if (u == null) u = go.GetComponentInChildren<UnitRuntime>();

                    if (u != null)
                    {
                        _playerAlive++;
                        _playerRemaining--; // 스폰할 때 총량 감소

                        u.Init(team: Team.Player, onDied: () => _playerAlive--);
                    }
                    else
                    {
                        Destroy(go);
                        Debug.LogError($"[Spawn] UnitRuntime missing on spawned player unit: {go.name}");
                    }
                }
            }

            yield return new WaitForSeconds(_cfg.playerInterval);
        }
    }

    private IEnumerator EnemyLoop()
    {
        while (true)
        {
            if (_enemyAlive < _cfg.enemyMaxAlive)
            {
                Transform sp = enemySpawnPoints[Random.Range(0, enemySpawnPoints.Count)];

                UnitType t = RollUnitType_40_20_40();
                Vector3 pos = SnapToGround(sp.position);

                var go = factory.Spawn(t, pos, unitsRoot);
                if (go != null)
                {
                    var u = go.GetComponent<UnitRuntime>();
                    if (u == null) u = go.GetComponentInChildren<UnitRuntime>();

                    if (u != null)
                    {
                        _enemyAlive++;
                        u.Init(team: Team.Enemy, onDied: () => _enemyAlive--);
                    }
                    else
                    {
                        Destroy(go);
                        Debug.LogError($"[Spawn] UnitRuntime missing on spawned enemy unit: {go.name}");
                    }
                }
            }

            yield return new WaitForSeconds(_cfg.enemyInterval);
        }
    }

    private UnitType RollUnitType_40_20_40()
    {
        int a = Mathf.Max(0, wInfantry);
        int b = Mathf.Max(0, wArcher);
        int c = Mathf.Max(0, wShield);

        int sum = a + b + c;
        if (sum <= 0) return UnitType.Infantry;

        int r = Random.Range(0, sum);
        if (r < a) return UnitType.Infantry;
        r -= a;
        if (r < b) return UnitType.Archer;
        return UnitType.Shield;
    }

    private Vector3 SnapToGround(Vector3 p)
    {
        if (groundMask.value == 0)
        {
            p.y = groundOffsetY;
            return p;
        }

        Vector3 origin = new Vector3(p.x, p.y + raycastHeight, p.z);
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, raycastHeight * 2f, groundMask))
            return hit.point + Vector3.up * groundOffsetY;

        p.y = groundOffsetY;
        return p;
    }

    // UI/디버그용
    public int GetPlayerRemaining() => _playerRemaining;
}