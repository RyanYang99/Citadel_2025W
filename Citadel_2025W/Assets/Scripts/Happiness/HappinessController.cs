using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HappinessController : MonoBehaviour
{
    // 티어별 설정을 인스펙터에서 한눈에 관리하기 위한 구조체
    [System.Serializable]
    public struct TierVisualData
    {
        public HappinessTier tier;
        public string statusName;       // 화면에 표시될 한글 이름
        public Color tierColor;         // 텍스트와 아이콘에 적용할 색상
        public Sprite statusIcon;       // 표시할 아이콘 이미지
    }

    [Header("UI References")]
    public TextMeshProUGUI statusText;
    public Image statusIconImage;

    [Header("Tier Visual Settings")]
    
    public TierVisualData[] visualSettings;

    private string currentStatusName;   // 현재 티어 이름 저장용
    private float displayScore;         // UI에 표시될 만족도 수치


    void Start()
    {
        HappinessManager.Instance.OnTierChanged += HandleTierChange;
        HandleTierChange(HappinessManager.Instance.currentTier);
    }

    void OnDestroy()
    {
        if (HappinessManager.Instance != null)
            HappinessManager.Instance.OnTierChanged -= HandleTierChange;
    }

    // 티어에 따라 UI텍스트,이미지 변경
    private void HandleTierChange(HappinessTier newTier)
    {
        foreach (var data in visualSettings)
        {
            if (data.tier == newTier)
            {
                statusText.color = data.tierColor;

                if (statusIconImage != null) statusIconImage.sprite = data.statusIcon;
                
                currentStatusName = data.statusName;
                break;
            }
        }
    }

    void Update()
    {
        // 수치(%)는 매 프레임 실시간으로 갱신
        if (statusText != null && HappinessManager.Instance != null)
        {
            float targetScore = HappinessManager.Instance.TotalHappiness;
            
            displayScore = Mathf.Lerp(displayScore, targetScore, Time.deltaTime * 5f);

            statusText.text = $"{currentStatusName} ({displayScore:F1}%)";
        }
    }
    
}
