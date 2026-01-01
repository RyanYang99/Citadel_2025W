using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class ResearchUIManager : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private TMP_Text rpNameText;       // 제목 텍스트
    [SerializeField] private TMP_Text rpExplanationText;// 설명 텍스트
    [SerializeField] private TMP_Text rpCostText;       // 비용 텍스트
    [SerializeField] private Button acceptButton;       // 적용 버튼

    [Header("토스트 메시지 ")]
    [SerializeField] private GameObject warningPanel;
    [SerializeField] private CanvasGroup warningCanvasGroup;
    private float visibleDuration = 0.5f;   // 완전히 보일 시간
    private float fadeDuration = 0.7f;      // 사라지는 시간

    [Space(10)]
    [SerializeField] private LineDraw lineDrawer;

    [Space(5)]
    public int currentResearchPoints; //디버깅용 연구포인트

    private ResearchData currentSelectedResearch;       // 현재 선택된 연구 데이터를 임시 저장할 변수
    private ResearchSlot currentSelectedSlot;           // 현재 누른 슬롯 기억
    private Coroutine activeFadeCoroutine;              //코루틴 저장 변수

    void Start()
    {
        acceptButton.onClick.AddListener(ApplyResearchEffect);

    }


    //버튼을 누르면 설명창을 갱신하는 함수
    public void ShowContent(ResearchData data, ResearchSlot Slot)
    {
        if (data == null) return;

        currentSelectedResearch = data; // 현재 데이터 저장
        currentSelectedSlot     = Slot; // 현재 슬롯 저장

        // 텍스트 갱신
        rpNameText.text = data.researchName;

        
        rpExplanationText.text = data.explanation;

        rpCostText.text = data.cost.ToString();

        acceptButton.interactable = true;

        //accept버튼 비활성화
        //이미 해금된 연구
        if (data.isUnlocked)
        {
            acceptButton.interactable = false;
            return;
        }

        //선행연구 완료
        bool canAccept = true;
        foreach (var req in data.requiredResearches)
        {
            if (req != null && !req.isUnlocked)
            {
                canAccept = false;
                break;
            }
        }

        acceptButton.interactable = canAccept;
    }




    //Accept 버튼
    public void ApplyResearchEffect()
    {
        if (currentSelectedResearch == null || currentSelectedSlot == null ) return;

        //RP 부족할시 경고창 띄우기
        if (currentResearchPoints < currentSelectedResearch.cost)
        {
            ShowWarningToast();
            return;
        }
        // RP 차감
        currentResearchPoints -= currentSelectedResearch.cost;

        //리스트에 있는 모든 효과를 순회
        foreach (ResearchEffect effect in currentSelectedResearch.effects)
        {
            switch (effect.type)
            {
                case ResearchType.Gold:
                    Debug.Log($"골드수급 {effect.value} 증가 적용!");
                    break;

                case ResearchType.Cityzen:
                    Debug.Log($"인구수 {effect.value} 증가 적용!");
                    break;

                case ResearchType.Happy:
                    Debug.Log($"만족도 {effect.value} 증가 적용!");
                    break;

                case ResearchType.Attack:
                    Debug.Log($"전투력 {effect.value} 증가 적용!");
                    break;
            }
        }
        
        currentSelectedResearch.isUnlocked = true;
        Debug.Log($"{currentSelectedResearch.researchName} 연구 완료!");
        Debug.Log($"{currentResearchPoints} RP 남은갯수");

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

    //토스트 메시지 함수
    public void ShowWarningToast()
    {
        if (warningPanel == null || warningCanvasGroup == null) return;

        //실행 중인 코루틴이 있다면 정지
        if (activeFadeCoroutine != null)
        {
            StopCoroutine(activeFadeCoroutine);
        }

        // 새로운 코루틴 시작
        activeFadeCoroutine = StartCoroutine(FadeToastProcess());
    }

    IEnumerator FadeToastProcess()
    {
        //나타나기
        warningPanel.SetActive(true);
        warningCanvasGroup.alpha = 1f;

        //대기 (지정된 시간만큼 그대로 유지)
        yield return new WaitForSeconds(visibleDuration);

        //서서히 사라지기
        float currentTime = 0f;
        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            // 시간에 따라 alpha 값을 1에서 0으로 줄임
            warningCanvasGroup.alpha = Mathf.Lerp(1f, 0f, currentTime / fadeDuration);

            // 다음 프레임까지 대기
            yield return null;
        }

        warningCanvasGroup.alpha = 0f;
        warningPanel.SetActive(false);
        activeFadeCoroutine = null;
    }
}