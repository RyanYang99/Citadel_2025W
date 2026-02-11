using UnityEngine;
using Citadel;

public sealed class SatisfactionInfluence : MonoBehaviour
{
    [Header("만족도 설정")]
    [SerializeField] private float score = 10f; // 제공할 고정 점수
    [SerializeField] private float range = 5f;  // 영향 범위 반경

    public float Score => score;
    public float Range => range;

    private void OnEnable() => SatisfactionManager.Instance?.RegisterInfluence(this);
    private void OnDisable() => SatisfactionManager.Instance?.UnregisterInfluence(this);

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 1f, 0.5f);

        Gizmos.DrawWireSphere(transform.position, range);

        Gizmos.color = new Color(0f, 1f, 1f, 0.1f);
        Gizmos.DrawSphere(transform.position, range);
    }
}