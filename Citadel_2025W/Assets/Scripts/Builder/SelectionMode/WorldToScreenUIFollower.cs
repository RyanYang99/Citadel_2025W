using UnityEngine;

namespace Citadel
{
    public sealed class WorldToScreenUIFollower : MonoBehaviour
    {
        [SerializeField] private Camera cam;
        [SerializeField] private RectTransform ui;          // 따라다닐 UI 패널
        [SerializeField] private Vector3 worldOffset = new Vector3(0.8f, 0f, 0f); // 건물 기준 오른쪽
        [SerializeField] private Vector2 screenOffset = new Vector2(20f, 0f);     // 픽셀 오프셋
        [SerializeField] private bool clampToScreen = true;
        [SerializeField] private Vector2 clampPadding = new Vector2(20f, 20f);

        private Transform target;

        private void Awake()
        {
            if (cam == null) cam = Camera.main;
        }

        public void SetTarget(Transform t)
        {
            target = t;
        }

        public void ClearTarget()
        {
            target = null;
        }

        private void LateUpdate()
        {
            if (ui == null) return;

            if (target == null || cam == null)
                return;

            Vector3 worldPos = target.position + cam.transform.right * 0.8f; // 화면 기준 오른쪽
            Vector3 screenPos = cam.WorldToScreenPoint(worldPos);

            // 카메라 뒤로 가면 숨기거나 위치 업데이트 중단
            if (screenPos.z < 0f)
                return;

            Vector2 final = (Vector2)screenPos + screenOffset;

            if (clampToScreen)
            {
                float minX = clampPadding.x;
                float maxX = Screen.width - clampPadding.x;
                float minY = clampPadding.y;
                float maxY = Screen.height - clampPadding.y;

                final.x = Mathf.Clamp(final.x, minX, maxX);
                final.y = Mathf.Clamp(final.y, minY, maxY);
            }

            ui.position = final;
        }
    }
}
