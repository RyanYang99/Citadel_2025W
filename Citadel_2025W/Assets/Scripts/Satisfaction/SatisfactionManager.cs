using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Citadel
{
    public class SatisfactionManager : MonoBehaviour
    {
        public static SatisfactionManager Instance { get; private set; }

        private readonly List<SatisfactionProvider> _providers = new();

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void Register(SatisfactionProvider provider)
        {
            if (!_providers.Contains(provider)) _providers.Add(provider);
        }

        public void Unregister(SatisfactionProvider provider)
        {
            _providers.Remove(provider);
        }

        // 1. 전체 만족도 평균 (0~1)
        public float GetGlobalAverage()
        {
            var readyProviders = _providers.Where(p => p.IsReady).ToList();

            
            if (readyProviders.Count == 0) return -1f;

            return readyProviders.Average(p => p.Satisfaction);
        }

        // 2. 카테고리별 만족도 평균 (0~1)
        public float GetCategoryAverage(SatisfactionCategory category)
        {
            // 해당 카테고리 건물만 필터링
            var targetProviders = _providers.Where(p => p.category == category && p.IsReady).ToList();
            
            if (targetProviders.Count == 0) return -1f;
            return targetProviders.Average(p => p.Satisfaction);
        }

        // 3. 해당 카테고리에서 최저 만족도를 가진 모든 건물들 찾기
        public List<SatisfactionProvider> GetWorstBuildings(SatisfactionCategory category)
        {
            // 해당 카테고리 건물들만 추출
            var targetProviders = _providers.Where(p => p.category == category && p.IsReady).ToList();

            if (targetProviders.Count == 0) return new List<SatisfactionProvider>();

            // 최소 점수 찾기
            float minScore = targetProviders.Min(p => p.Satisfaction);

            // 최소 점수와 동일한 점수를 가진 모든 건물 리스트 반환
            return targetProviders
                .Where(p => Mathf.Approximately(p.Satisfaction, minScore))
                .ToList();
        }

        public bool HasWorstBuilding(SatisfactionCategory category, float threshold)
        {
            // IsReady가 true인 건물 중에서 만족도가 threshold보다 낮은 게 하나라도 있는지 검사
            return _providers.Any(p => p.category == category && p.IsReady && p.Satisfaction < threshold);
        }
    }
}