using System;
using UnityEngine;

namespace Citadel
{
    public sealed class BarracksProductionQueue : MonoBehaviour
    {
        [SerializeField] private UnitRuleData rules;

        private Inventory _inventory;
        private TimeManager _time;

        private int _queued;                 // 큐에 대기 중인 Soldier 수
        private int _hoursUntilNextComplete; // 다음 1명 완료까지 남은 시간(시간 단위)

        public int Queued => _queued;
        public int HoursUntilNextComplete => _queued > 0 ? _hoursUntilNextComplete : -1;

        public event Action OnStateChanged;  // UI 갱신용

        private void Awake()
        {
            _inventory = FindAnyObjectByType<Inventory>();
            _time = FindAnyObjectByType<TimeManager>();
        }

        private void OnEnable()
        {
            if (_time != null) _time.OnHourChange += OnHourChange;
        }

        private void OnDisable()
        {
            if (_time != null) _time.OnHourChange -= OnHourChange;
        }

        public bool TryEnqueueSoldier()
        {
            if (rules == null || _inventory == null) return false;

            // 1) 큐 용량 체크
            if (_queued >= rules.barracksQueueCap) return false;

            // 2) 캡 체크: 현재 병력 + 큐 대기 병력 합산
            int currentSoldier = _inventory.GetAmount(Item.Soldier);
            if (currentSoldier + _queued >= rules.soldierCap) return false;

            // 3) 돈 체크 + 즉시 결제
            int money = _inventory.GetAmount(Item.Money);
            if (money < rules.soldierMoneyCost) return false;

            _inventory.Consume(Item.Money, rules.soldierMoneyCost);

            // 4) 큐 추가
            _queued++;

            // 5) 큐가 비어있던 상태에서 처음 들어온 거면 타이머 세팅
            if (_queued == 1)
                _hoursUntilNextComplete = rules.soldierHoursPerUnit;

            OnStateChanged?.Invoke();
            return true;
        }

        private void OnHourChange(int hourNow)
        {
            if (rules == null || _inventory == null) return;
            if (_queued <= 0) return;

            _hoursUntilNextComplete--;
            if (_hoursUntilNextComplete > 0)
            {
                OnStateChanged?.Invoke();
                return;
            }

            // 1명 생산 완료
            _inventory.Add(Item.Soldier, 1);
            _queued--;

            // 다음 생산 예약
            if (_queued > 0)
                _hoursUntilNextComplete = rules.soldierHoursPerUnit;

            OnStateChanged?.Invoke();
        }

        // UI용 문구: "대기: N명 / 다음 완료: 2h 0m"
        public string GetQueueSummaryText()
        {
            if (_queued <= 0) return "대기: 0명 / 다음 완료: -";
            int h = Mathf.Max(0, _hoursUntilNextComplete);
            return $"대기: {_queued}명 / 다음 완료: {h}h 0m";
        }
    }
}