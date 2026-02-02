using System;
using UnityEngine;

public enum Team { Player, Enemy }

public class UnitRuntime : MonoBehaviour
{
    private static BattleManager _battle;

    public static void SetBattleManager(BattleManager bm) => _battle = bm;

    [SerializeField] private float hp = 100f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float range = 1.5f;
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float attackInterval = 1.0f;

    private Team _team;
    private Action _onDied;
    private float _nextAtk;

    public void Init(Team team, Action onDied)
    {
        _team = team;
        _onDied = onDied;

        // UnitData에서 스탯 세팅하고 싶으면 여기서 GetComponent로 받아서 세팅해도 됨
    }

    private void Update()
    {
        // 가장 가까운 적 찾기 (최소 구현)
        UnitRuntime target = FindClosestEnemy();
        if (target == null) return;

        float d = Vector3.Distance(transform.position, target.transform.position);

        if (d > range)
        {
            Vector3 dir = (target.transform.position - transform.position).normalized;
            transform.position += dir * moveSpeed * Time.deltaTime;
        }
        else
        {
            if (Time.time >= _nextAtk)
            {
                _nextAtk = Time.time + attackInterval;
                target.TakeDamage(damage, attackerTeam: _team);
            }
        }
    }

    private UnitRuntime FindClosestEnemy()
    {
        UnitRuntime[] all = FindObjectsOfType<UnitRuntime>();
        UnitRuntime best = null;
        float bestD = float.MaxValue;

        foreach (var u in all)
        {
            if (u == this) continue;
            if (u._team == _team) continue;

            float d = (u.transform.position - transform.position).sqrMagnitude;
            if (d < bestD)
            {
                bestD = d;
                best = u;
            }
        }
        return best;
    }

    public void TakeDamage(float amount, Team attackerTeam)
    {
        hp -= amount;
        if (hp <= 0f)
        {
            Die(attackerTeam);
        }
    }

    private void Die(Team killer)
    {
        _onDied?.Invoke();

        // 적이 죽었고, 킬러가 Player라면 킬 카운트 상승
        if (_team == Team.Enemy && killer == Team.Player)
            _battle?.NotifyEnemyKilled();

        Destroy(gameObject);
    }
}
