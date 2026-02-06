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

    [Header("Ground Snap")]
    [SerializeField] private LayerMask groundMask;    
    [SerializeField] private float raycastHeight = 50f;
    [SerializeField] private float groundOffsetY = 0f;

    [Header("Field Bounds (for XZ clamp)")]
    [Tooltip("필드(바닥) 전체를 대표하는 부모 오브젝트를 넣어라. 예: Field 또는 Tilemap(GameObject)")]
    [SerializeField] private Transform fieldRoot;   
    [SerializeField] private float edgeMargin = 1.0f;  

    private BattleRuntimeConfig _cfg;
    private bool _isConfigured;

    private Coroutine _playerCo;
    private Coroutine _enemyCo;
    private int _playerAlive;
    private int _enemyAlive;

    private Bounds _fieldBounds;
    private bool _hasFieldBounds;

    public void Configure(BattleRuntimeConfig cfg)
    {
        _cfg = cfg;
        _isConfigured = true;

        CacheFieldBounds();
    }

    public void Begin()
    {
        if (!_isConfigured)
        {
            Debug.LogError("[SpawnController] Configure(cfg) 먼저 호출해야 함");
            return;
        }

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

    private void CacheFieldBounds()
    {
        _hasFieldBounds = false;

        if (fieldRoot == null) return;

        // 바닥 블록들이 MeshRenderer를 갖고 있으니 Renderer bounds로 합산
        var renderers = fieldRoot.GetComponentsInChildren<Renderer>();
        if (renderers != null && renderers.Length > 0)
        {
            Bounds b = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
                b.Encapsulate(renderers[i].bounds);

            _fieldBounds = b;
            _hasFieldBounds = true;
        }
    }

    private IEnumerator PlayerLoop()
    {
        while (true)
        {
            if (_playerAlive < _cfg.playerMaxAlive)
            {
                Transform sp = playerSpawnPoints[Random.Range(0, playerSpawnPoints.Count)];
                Vector3 pos = ComputeSpawnPos(sp.position);

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

                Vector3 pos = ComputeSpawnPos(sp.position);

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

    private Vector3 ComputeSpawnPos(Vector3 desiredWorldPos)
    {
        Vector3 p = desiredWorldPos;

        // 1) XZ를 필드 bounds 안으로 강제
        if (_hasFieldBounds)
        {
            float minX = _fieldBounds.min.x + edgeMargin;
            float maxX = _fieldBounds.max.x - edgeMargin;
            float minZ = _fieldBounds.min.z + edgeMargin;
            float maxZ = _fieldBounds.max.z - edgeMargin;

            p.x = Mathf.Clamp(p.x, minX, maxX);
            p.z = Mathf.Clamp(p.z, minZ, maxZ);
        }

        // 2) Raycast로 바닥 높이(Y) 맞춤
        p = SnapYToGround(p);
        return p;
    }

    private Vector3 SnapYToGround(Vector3 worldPos)
    {
        if (groundMask.value != 0)
        {
            Vector3 origin = new Vector3(worldPos.x, worldPos.y + raycastHeight, worldPos.z);
            if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, raycastHeight * 2f, groundMask, QueryTriggerInteraction.Ignore))
            {
                worldPos.y = hit.point.y + groundOffsetY;
                return worldPos;
            }
        }

        // 못 찍으면 일단 y 유지/0으로
        worldPos.y = 0f + groundOffsetY;
        return worldPos;
    }

    // Archer 신경 쓰지 말랬으니: 지금은 적도 보병만 뽑게 해두는 게 제일 깔끔
    private UnitType RollEnemyType()
    {
        return UnitType.Infantry;
    }
}