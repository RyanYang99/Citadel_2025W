using System;
using System.Collections;
using UnityEngine;

public enum Team { Player, Enemy }

public class UnitRuntime : MonoBehaviour
{
    private static BattleManager _battle;
    public static void SetBattleManager(BattleManager bm) => _battle = bm;

    [Header("Stats")]
    [SerializeField] private float hp = 100f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float range = 1.5f;
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float attackInterval = 1.0f;

    [Header("Animation")]
    [Tooltip("죽는 애니메이션 재생 후 Destroy까지 기다릴 시간(클립 길이에 맞춰 조절)")]
    [SerializeField] private float deathDestroyDelay = 1.2f;

    private Team _team;
    private Action _onDied;
    private float _nextAtk;

    private UnitAnimDriver _anim;
    private bool _isDead;

    public void Init(Team team, Action onDied)
    {
        _team = team;
        _onDied = onDied;

        // Knight 프리팹 루트에 UnitAnimDriver를 붙였다는 전제
        _anim = GetComponent<UnitAnimDriver>();
        if (_anim == null)
            Debug.LogWarning("[UnitRuntime] UnitAnimDriver not found on this GameObject.", this);
    }

    private void Update()
    {
        if (_isDead) return;

        // 가장 가까운 적 찾기
        UnitRuntime target = FindClosestEnemy();
        if (target == null)
        {
            _anim?.SetMove(false);
            return;
        }

        float d = Vector3.Distance(transform.position, target.transform.position);

        if (d > range)
        {
            // 이동
            _anim?.SetMove(true);

            Vector3 dir = (target.transform.position - transform.position).normalized;
            transform.position += dir * moveSpeed * Time.deltaTime;
        }
        else
        {
            // 공격 범위 안: 정지 + 공격
            _anim?.SetMove(false);

            if (Time.time >= _nextAtk)
            {
                _nextAtk = Time.time + attackInterval;

                _anim?.PlayAttack();
                target.TakeDamage(damage, attackerTeam: _team);
            }
        }
    }

    private UnitRuntime FindClosestEnemy()
    {
        UnitRuntime[] all = FindObjectsByType<UnitRuntime>(FindObjectsSortMode.None);
        UnitRuntime best = null;
        float bestD = float.MaxValue;

        foreach (var u in all)
        {
            if (u == this) continue;
            if (u._team == _team) continue;
            if (u._isDead) continue;

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
        if (_isDead) return;

        hp -= amount;

        // 피격 애니메이션(원하면)
        _anim?.PlayHit();

        if (hp <= 0f)
            Die(attackerTeam);
    }

    private void Die(Team killer)
    {
        if (_isDead) return;
        _isDead = true;

        // 스폰 카운트 감소 콜백
        _onDied?.Invoke();

        // 적이 죽었고 & 킬러가 Player라면 킬 카운트 상승
        if (_team == Team.Enemy && killer == Team.Player)
            _battle?.NotifyEnemyKilled();

        // 죽는 애니메이션
        _anim?.PlayDead();

        // 죽는 동안 이동/공격 중단 + 일정 시간 후 제거
        StartCoroutine(DieRoutine());
    }

    private IEnumerator DieRoutine()
    {
        yield return new WaitForSeconds(deathDestroyDelay);
        Destroy(gameObject);
    }

    public void ApplyData(UnitData data)
    {
        hp = data.maxHp;
        damage = data.damage;
        range = data.range;
        moveSpeed = data.moveSpeed;
        attackInterval = data.attackInterval;

        _nextAtk = 0f;
    }
}