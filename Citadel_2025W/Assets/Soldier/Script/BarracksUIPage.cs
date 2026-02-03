using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Citadel
{
    public sealed class BarracksUIPage : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private TMP_Text moneyText;
        [SerializeField] private TMP_Text soldierText;
        [SerializeField] private TMP_Text queueText;
        [SerializeField] private Button produceButton;
        [SerializeField] private Button closeButton;

        [Header("Data")]
        [SerializeField] private UnitRuleData rules;

        private Inventory _inventory;
        private TimeManager _time;
        private BarracksProductionQueue _queue;

        private void Awake()
        {
            _inventory = FindAnyObjectByType<Inventory>();
            _time = FindAnyObjectByType<TimeManager>();

            if (produceButton != null) produceButton.onClick.AddListener(OnClickProduce);
            if (closeButton != null) closeButton.onClick.AddListener(Close);

            if (panelRoot != null) panelRoot.SetActive(false);
        }

        private void OnEnable()
        {
            if (_inventory != null) _inventory.OnItemChange += OnItemChange;
            if (_time != null) _time.OnHourChange += OnHourChange;
        }

        private void OnDisable()
        {
            if (_inventory != null) _inventory.OnItemChange -= OnItemChange;
            if (_time != null) _time.OnHourChange -= OnHourChange;
            UnbindQueue();
        }

        public void Open(BarracksProductionQueue queue)
        {
            if (queue == null) return;

            BindQueue(queue);

            if (panelRoot != null) panelRoot.SetActive(true);
            RefreshAll();
        }

        public void Close()
        {
            UnbindQueue();
            if (panelRoot != null) panelRoot.SetActive(false);
        }

        private void BindQueue(BarracksProductionQueue queue)
        {
            UnbindQueue();
            _queue = queue;
            _queue.OnStateChanged += OnQueueStateChanged;
        }

        private void UnbindQueue()
        {
            if (_queue != null)
                _queue.OnStateChanged -= OnQueueStateChanged;
            _queue = null;
        }

        private void OnQueueStateChanged() => RefreshAll();

        private void OnItemChange(Item item, int after)
        {
            // 돈/병력 UI만 갱신
            if (item == Item.Money || item == Item.Soldier)
                RefreshAll();
        }

        private void OnHourChange(int hourNow)
        {
            RefreshAll();
        }

        private void OnClickProduce()
        {
            if (_queue == null) return;

            bool ok = _queue.TryEnqueueSoldier();
            RefreshAll();
        }

        private void RefreshAll()
        {
            if (_inventory == null || rules == null)
                return;

            // 1) Money
            int money = _inventory.GetAmount(Item.Money);
            if (moneyText != null) moneyText.text = $"Money: {money}";

            // 2) Soldier / Cap
            int soldier = _inventory.GetAmount(Item.Soldier);
            if (soldierText != null) soldierText.text = $"Soldier: {soldier}/{rules.soldierCap}";

            // 3) Queue
            if (queueText != null)
            {
                if (_queue == null) queueText.text = "대기: - / 다음 완료: -";
                else queueText.text = _queue.GetQueueSummaryText();

            }

            // 4) Produce 버튼 활성/비활성
            if (produceButton != null && _queue != null)
            {
                bool canMoney = money >= rules.soldierMoneyCost;
                bool canCap = (soldier + _queue.Queued) < rules.soldierCap;
                bool canQueue = _queue.Queued < rules.barracksQueueCap;

                produceButton.interactable = canMoney && canCap && canQueue;
            }
        }
    }
}