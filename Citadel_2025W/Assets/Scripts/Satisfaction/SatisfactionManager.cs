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
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        public void Register(SatisfactionProvider provider)
        {
            if (!_providers.Contains(provider))
                _providers.Add(provider);
        }

        public void Unregister(SatisfactionProvider provider) => _providers.Remove(provider);

        // 1. ��ü ������ ��� (0~1)
        public float GetGlobalAverage()
        {
            var readyProviders = _providers.Where(p => p.IsReady).ToList();

            //�ǹ��� �ƹ��͵� ���ٸ� -1�� ��ȯ
            if (readyProviders.Count == 0) return -1f;

            return readyProviders.Average(p => p.Satisfaction);
        }

        // 2. ī�װ����� ������ ��� (0~1)
        public float GetCategoryAverage(SatisfactionCategory category)
        {
            // �ش� ī�װ��� �ǹ��� ���͸�
            var targetProviders = _providers.Where(p => p.Category == category && p.IsReady).ToList();
            
            if (targetProviders.Count == 0) return -1f;

            return targetProviders.Average(p => p.Satisfaction);
        }

        // 3. �ش� ī�װ������� ���� �������� ���� ��� �ǹ��� ã��
        public List<SatisfactionProvider> GetWorstBuildings(SatisfactionCategory category)
        {
            // �ش� ī�װ��� �ǹ��鸸 ����
            var targetProviders = _providers.Where(p => p.Category == category && p.IsReady).ToList();

            if (targetProviders.Count == 0) return new List<SatisfactionProvider>();

            // �ּ� ���� ã��
            float minScore = targetProviders.Min(p => p.Satisfaction);

            // �ּ� ������ ������ ������ ���� ��� �ǹ� ����Ʈ ��ȯ
            return targetProviders
                .Where(p => Mathf.Approximately(p.Satisfaction, minScore))
                .ToList();
        }

        public bool HasWorstBuilding(SatisfactionCategory category, float threshold)
        {
            // IsReady�� true�� �ǹ� �߿��� �������� threshold���� ���� �� �ϳ��� �ִ��� �˻�
            return _providers.Any(p => p.Category == category && p.IsReady && p.Satisfaction < threshold);
        }
    }
}