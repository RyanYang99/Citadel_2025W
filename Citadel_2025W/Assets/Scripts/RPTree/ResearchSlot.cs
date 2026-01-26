using UnityEngine;
using UnityEngine.UI;

public class ResearchSlot : MonoBehaviour
{
    
    [Header("ResearchData 파일")]
    [SerializeField] private ResearchData data;

    [Header("UI Manager 연결")]
    [SerializeField] private ResearchUIManager uiManager;

    private Button treeBtn;


    private void Start()
    {
        treeBtn = GetComponent<Button>();
        // 버튼 클릭 시 내 데이터를 매니저에게 전달함
        treeBtn.onClick.AddListener(() => uiManager.ShowContent(data,this));
        RefreshUI();
    }

    // 버튼의 상태(잠금, 완료, 클릭가능)를 갱신하는 함수
    public void RefreshUI()
    {
        if (data == null || treeBtn == null) return;

        treeBtn.interactable = true;

        if (data.isUnlocked) SetButtonColor(Color.yellow); //이미 해금된 버튼
        else if (!CheckCanUnlock()) SetButtonColor(Color.gray); //잠긴 버튼
        else SetButtonColor(Color.white); //해금 가능한 버튼

    }

    private void SetButtonColor(Color color)
    {
        var colors = treeBtn.colors;
        colors.normalColor = color;
        colors.highlightedColor = color * 1.2f;
        treeBtn.colors = colors;
    }

    // 선행 연구 완료 여부 확인함수
    public bool CheckCanUnlock()
    {
        foreach (var req in data.requiredResearches)
        {
            if (req != null && !req.isUnlocked) return false;
        }
        return true;
    }
}