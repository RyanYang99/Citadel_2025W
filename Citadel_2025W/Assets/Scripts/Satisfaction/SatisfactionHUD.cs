using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Citadel
{
    public class SatisfactionHUD : MonoBehaviour
    {
        [Header("Global UI")]
        [SerializeField] private TextMeshProUGUI globalText; // ex) "전체 만족도: 85%"

        [Header("Details UI (Tooltip)")]
        [SerializeField] private GameObject detailPanel;
        [SerializeField] private CategoryUI[] categoryUIs;

        [Header("Settings")]
        [SerializeField] private float alertThreshold = 0.4f; // 40% 미만이면 경고표시
        [SerializeField] private SatisfactionAlertIcon alertIconPrefab; // 아이콘 프리팹

        [Serializable]
        public class CategoryUI
        {
            public SatisfactionCategory category;
            public TextMeshProUGUI statusText; 
            public Button alertButton;         // 경고 아이콘 / 버튼

            [HideInInspector] public float displayScore = 1f;
        }
        private float _displayGlobalAvg = 1f;

        // 카테고리 영문 이름 -> 한글 이름 변환용
        private readonly Dictionary<SatisfactionCategory, string> _categoryNames = new()
        {
            { SatisfactionCategory.Residential, "주거" },
            { SatisfactionCategory.Industrial,  "공업" },
            { SatisfactionCategory.Commercial,  "상업" },
            { SatisfactionCategory.Service,     "서비스" }
        };

        private void Start()
        {
            foreach (var catUI in categoryUIs)
            {
                var currentCategory = catUI.category;
                catUI.alertButton.onClick.AddListener(() => OnClickAlert(currentCategory));
            }
        }

        private void Update()
        {
            UpdateGlobalUI();

            if (detailPanel.activeSelf)
            {
                UpdateCategoryUI();
            }
        }

        public void OnPointerClick()
        {
            if (detailPanel != null)
            {
                detailPanel.SetActive(!detailPanel.activeSelf);
            }
        }
        
        private void UpdateGlobalUI()
        {
            if (globalText == null) return;

            float actualGlobalAvg = SatisfactionManager.Instance.GetGlobalAverage();

            // 건물이 하나도 없을 때
            if (actualGlobalAvg < 0)
            {
                globalText.text = "-%";
                globalText.color = Color.black;
                return;
            }

            
            _displayGlobalAvg = Mathf.Lerp(_displayGlobalAvg, actualGlobalAvg, Time.deltaTime * 1f);

            globalText.text = $"{_displayGlobalAvg * 100:F0}%";
            globalText.color = GetColorByScore(_displayGlobalAvg);
        }


        private void UpdateCategoryUI()
        {
            foreach (var uiItem in categoryUIs)
            {
                float targetAvg = SatisfactionManager.Instance.GetCategoryAverage(uiItem.category);
                string kName = _categoryNames.ContainsKey(uiItem.category) ? _categoryNames[uiItem.category] : uiItem.category.ToString();

                if (targetAvg < 0)
                {
                    uiItem.statusText.text = $"{kName} ( -% )";
                    uiItem.statusText.color = Color.black;
                    uiItem.alertButton.gameObject.SetActive(false);
                    continue;
                }

                uiItem.displayScore = Mathf.Lerp(uiItem.displayScore, targetAvg, Time.deltaTime * 1f);

                // 텍스트 갱신 ex) "주거 (00%)"
                uiItem.statusText.text = $"{kName} ({uiItem.displayScore * 100:F0}%)";

                // 글자 색상 변경
                uiItem.statusText.color = GetColorByScore(uiItem.displayScore);

                // 경고 버튼 활성화 여부
                bool hasProblem = SatisfactionManager.Instance.HasWorstBuilding(uiItem.category, alertThreshold);

                // 버튼 상태 갱신
                if (uiItem.alertButton.gameObject.activeSelf != hasProblem)
                {
                    uiItem.alertButton.gameObject.SetActive(hasProblem);
                }
            }
        }

        // 점수에 따라 색상을 변경하는 함수
        private Color GetColorByScore(float score)
        {
            if (score >= 0.8f) return Color.green;       // 80% 이상: 초록
            if (score >= 0.4f) return Color.yellow;      // 40~79%: 노랑
            return Color.red;                            // 40% 미만: 빨강
        }

        private void OnClickAlert(SatisfactionCategory category)
        {
            // 만족도가 낮은 건물 리스트를 받아옴
            var worstBuildings = SatisfactionManager.Instance.GetWorstBuildings(category);

            // 가져온 건물들 위에 아이콘 생성
            foreach (var provider in worstBuildings)
            {
                if (provider != null)
                {
                    SatisfactionAlertIcon newIcon = Instantiate(alertIconPrefab);
                    newIcon.Setup(provider.transform);

                    Debug.Log($"{provider.name} (만족도: {provider.Satisfaction * 100:F0}%) 위에 아이콘 생성");
                }
            }
        }
    }

}