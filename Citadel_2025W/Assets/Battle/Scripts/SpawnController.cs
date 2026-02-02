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

    private BattleRuntimeConfig _cfg;
    private Coroutine _playerCo;
    private Coroutine _enemyCo;

    private int _playerAlive;
    private int _enemyAlive;

    public void Configure(BattleRuntimeConfig cfg)
    {
        _cfg = cfg;
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
            if (_playerAlive < _cfg.playerMaxAlive)
            {
                Transform sp = playerSpawnPoints[Random.Range(0, playerSpawnPoints.Count)];
                Vector3 pos = sp.position;
                pos.y = 0f; // ← Ground 높이가 0이 아니라면 그 값으로
                var go = factory.Spawn(UnitType.Infantry, pos, unitsRoot);
                if (go != null)
                {
                    _playerAlive++;
                    var u = go.GetComponent<UnitRuntime>();
                    u.Init(team: Team.Player, onDied: () => _playerAlive--);
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
                UnitType t = RollEnemyType();
                Vector3 pos = sp.position;
                pos.y = 0f; // ← Ground 높이로 고정
                var go = factory.Spawn(t, pos, unitsRoot);
                if (go != null)
                {
                    _enemyAlive++;
                    var u = go.GetComponent<UnitRuntime>();
                    u.Init(team: Team.Enemy, onDied: () => _enemyAlive--);
                }
            }
            yield return new WaitForSeconds(_cfg.enemyInterval);
        }
    }

    private UnitType RollEnemyType()
    {
        int a = Mathf.Max(0, _cfg.wInfantry);
        int b = Mathf.Max(0, _cfg.wArcher);
        int sum = a + b;
        if (sum <= 0) return UnitType.Infantry;

        int r = Random.Range(0, sum);
        if (r < a) return UnitType.Infantry;
        r -= a;
        if (r < b) return UnitType.Archer;
        return UnitType.Shield;
    }
}
