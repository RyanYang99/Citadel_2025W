using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ResearchUIManager : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private TMP_Text rpNameText;       // 제목 텍스트
    [SerializeField] private TMP_Text rpExplanationText;// 설명 텍스트
    [SerializeField] private TMP_Text rpCostText;       // 비용 텍스트
    [SerializeField] private Button acceptButton;       // 적용 버튼
    
    [SerializeField] private LineDraw lineDrawer;

    private ResearchData currentSelectedResearch;       // 현재 선택된 연구 데이터를 임시 저장할 변수
    private ResearchSlot currentSelectedSlot;           // 현재 누른 슬롯 기억


    void Start()
    {
        acceptButton.onClick.AddListener(ApplyResearchEffect);
    }

    // 1. 버튼을 누르면 설명창을 갱신하는 함수
    public void ShowContent(ResearchData data, ResearchSlot Slot)
    {
        if (data == null) return;

        currentSelectedResearch = data; // 현재 데이터 저장
        currentSelectedSlot     = Slot; // 현재 슬롯 저장

        // 텍스트 갱신
        rpNameText.text = data.researchName;

        // (참고) 만약 설명을 수동으로 적지 않고 자동으로 만들고 싶다면 아래 주석 참고
        rpExplanationText.text = data.explanation;

        rpCostText.text = data.cost.ToString();

        acceptButton.interactable = true; // 이제 선택됐으니 누를 수 있게 활성화
    }

    // 2. Accept 버튼 누르면 실제 효과 적용
    public void ApplyResearchEffect()
    {
        if (currentSelectedResearch == null || currentSelectedSlot == null) return;

        // [비용 처리 로직 예시]
        // if (PlayerMoney < currentSelectedResearch.cost) return;
        // PlayerMoney -= currentSelectedResearch.cost;

        // ★ 핵심 변경점: 리스트에 있는 모든 효과를 순회하며 적용 ★
        foreach (ResearchEffect effect in currentSelectedResearch.effects)
        {
            switch (effect.type)
            {
                case ResearchType.Gold:
                    // StartTree에 대한 특수 로직이 있다면 작성
                    Debug.Log($"골드수급 {effect.value} 증가 적용!");
                    break;

                case ResearchType.Cityzen:
                    Debug.Log($"인구수 {effect.value} 증가 적용!");
                    // 실제 코드 예: GameManager.Instance.AddCapital(effect.value);
                    break;

                case ResearchType.Happy:
                    Debug.Log($"만족도 {effect.value} 증가 적용!");
                    // 실제 코드 예: GameManager.Instance.AddCulture(effect.value);
                    break;

                case ResearchType.Attack:
                    Debug.Log($"전투력 {effect.value} 증가 적용!");
                    // 실제 코드 예: GameManager.Instance.AddMilitary(effect.value);
                    break;
            }
        }
        
        currentSelectedResearch.isUnlocked = true;
        Debug.Log($"{currentSelectedResearch.researchName} 연구 완료!");

        ResearchSlot[] allSlots = Object.FindObjectsByType<ResearchSlot>(FindObjectsSortMode.None);

        foreach (ResearchSlot slot in allSlots)
        {
            slot.RefreshUI();
        }

        if (lineDrawer != null)
        {
            lineDrawer.UpdateLineColors();
        }

        acceptButton.interactable = false;
    }
}