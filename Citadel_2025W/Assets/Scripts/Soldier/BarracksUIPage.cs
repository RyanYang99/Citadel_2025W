using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Citadel
{
    public class BarracksUIPage : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private TMP_Text moneyText;
        [SerializeField] private TMP_Text soldierText;
        [SerializeField] private TMP_Text queueText;
        [SerializeField] private Button produceButton;
        [SerializeField] private Button closeButton; // optional

        [Header("Data")]
        [SerializeField] private UnitRuleData rules;

        private void Awake()
        {
            if (produceButton != null)
                produceButton.onClick.AddListener(OnClickProduce);

            if (closeButton != null)
                closeButton.onClick.AddListener(Close);

            if (panelRoot != null)
                panelRoot.SetActive(false);
        }

        public void Open()
        {
            if (panelRoot != null) panelRoot.SetActive(true);
            Refresh(); // UI 갱신(일단 더미)
        }

        public void Close()
        {
            if (panelRoot != null) panelRoot.SetActive(false);
        }

        private void OnClickProduce()
        {
            // 아직 큐 연결 전이므로, 지금은 버튼 눌림 확인용 로그만
            Debug.Log("[BarracksUI] Produce Soldier clicked");

            // TODO: 다음 단계에서 여기서 Money 차감/큐 추가/완료 처리 연결
            Refresh();
        }

        private void Refresh()
        {
            // 아직 Inventory/Queue를 바인딩 안 했으니,
            // 인스펙터 연결이 정상인지 확인용으로만 표시
            if (moneyText != null) moneyText.text = "Money: (bind later)";
            if (soldierText != null) soldierText.text = "Soldier: (bind later)";
            if (queueText != null) queueText.text = "Queue: (bind later)";
        }
    }
}