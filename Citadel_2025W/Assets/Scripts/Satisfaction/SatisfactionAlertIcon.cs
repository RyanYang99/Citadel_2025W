using UnityEngine;

namespace Citadel
{
    public class SatisfactionAlertIcon : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float duration = 5f; // 아이콘이 떠있는 시간
        [SerializeField] private float floatSpeed = 1f; // 둥둥거리는 속도
        [SerializeField] private float floatAmount = 0.5f; // 둥둥거리는 높이
        [SerializeField] private Vector3 offset = new Vector3(0, 3f, 0); // 건물 중심으로부터의 높이

        private Vector3 _startPos;
        private Transform _cameraTransform;

        public void Setup(Transform targetBuilding)
        {
            // 건물 위치 + 오프셋 위치로 설정
            _startPos = targetBuilding.position + offset;
            transform.position = _startPos;

            // 메인 카메라 참조 캐싱
            if (Camera.main != null)
                _cameraTransform = Camera.main.transform;

            // 일정 시간 후 자동 파괴
            Destroy(gameObject, duration);
        }

        private void Update()
        {
            // 1. 위아래로 둥둥 떠다니는 애니메이션
            float newY = _startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatAmount;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }

        private void LateUpdate()
        {
            // 2. 항상 카메라를 정면으로 바라보게 함 (빌보드 효과)
            if (_cameraTransform != null)
            {
                transform.LookAt(transform.position + _cameraTransform.rotation * Vector3.forward,
                                 _cameraTransform.rotation * Vector3.up);
            }
        }
    }
}