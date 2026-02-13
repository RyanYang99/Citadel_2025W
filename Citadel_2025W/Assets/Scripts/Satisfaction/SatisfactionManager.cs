using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Citadel
{
    public class SatisfactionManager : MonoBehaviour
    {
        public static SatisfactionManager Instance { get; private set; }

        private readonly List<SatisfactionProvider> _providers = new();

        private readonly List<SatisfactionInfluence> _influences = new();

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        public void RegisterInfluence(SatisfactionInfluence influence) => _influences.Add(influence);
        public void UnregisterInfluence(SatisfactionInfluence influence) => _influences.Remove(influence);

        public IReadOnlyList<SatisfactionInfluence> AllInfluences => _influences;

        public void Register(SatisfactionProvider provider)
        {
            if (!_providers.Contains(provider))
                _providers.Add(provider);
        }

        public void Unregister(SatisfactionProvider provider) => _providers.Remove(provider);

        // 1. ��ü ������ ��� (0~1)
        public float GetGlobalAverage()
        {
            //�ǹ��� �ƹ��͵� ���ٸ� -1�� ��ȯ
            if (_providers.Count == 0) return -1f;

            return _providers.Average(p => p.Satisfaction);
        }

        // 2. ī�װ����� ������ ��� (0~1)
        public float GetCategoryAverage(SatisfactionCategory category)
        {
            // �ش� ī�װ��� �ǹ��� ���͸�
            var targetProviders = _providers.Where(p => p.Category == category).ToList();
            
            if (targetProviders.Count == 0) return -1f;

            return targetProviders.Average(p => p.Satisfaction);
        }

        // 3. �ش� ī�װ������� ���� �������� ���� ��� �ǹ��� ã��
        public List<SatisfactionProvider> GetWorstBuildings(SatisfactionCategory category)
        {
            // �ش� ī�װ��� �ǹ��鸸 ����
            var targetProviders = _providers.Where(p => p.Category == category).ToList();

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
            return _providers.Any(p => p.Category == category && p.Satisfaction < threshold);
        }
    }
}