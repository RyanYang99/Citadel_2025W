using UnityEngine;
using UnityEngine.UI;

public class ResearchSlot : MonoBehaviour
{
    
    [Header("ResearchData 파일")]
    [SerializeField] private ResearchData data;

    [Header("UI Manager 연결")]
    [SerializeField] private ResearchUIManager uiManager;

    private Button btn;

    private void Start()
    {
        btn = GetComponent<Button>();
        // 버튼 클릭 시 내 데이터를 매니저에게 전달함
        btn.onClick.AddListener(() => uiManager.ShowContent(data,this));
        RefreshUI();
    }

    // 버튼의 상태(잠금, 완료, 클릭가능)를 갱신하는 함수
    public void RefreshUI()
    {
        if (data == null || btn == null) return;

        // 1. 이미 완료된 연구라면? -> 비활성화
        if (data.isUnlocked)
        {
            btn.interactable = false;
            //이후 완료 ->이어진 선(이미지)의 색상 변경 처리 예정
            return;
        }

        // 2. 선행 연구가 모두 완료되었는지 확인
        bool canUnlock = true;
        foreach (var req in data.requiredResearches)
        {
            if (req != null && !req.isUnlocked)
            {
                canUnlock = false;
                break;
            }
        }

        // 선행 연구가 완료 안 됐으면 버튼 클릭 불가
        btn.interactable = canUnlock;
    }
}