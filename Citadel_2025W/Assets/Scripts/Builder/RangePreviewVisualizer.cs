using UnityEngine;

namespace Citadel
{
    public sealed class RangePreviewVisualizer : MonoBehaviour
    {
        [SerializeField] private LineRenderer line;
        [SerializeField, Min(16)] private int segments = 64;
        [SerializeField] private float y = 0.05f; 

        private float _radius;
        private bool _active;

        public void Show(float radius)
        {
            _radius = Mathf.Max(0f, radius);
            _active = _radius > 0.01f;

            if (line != null)
                line.gameObject.SetActive(_active);

            if (_active)
                DrawCircle();
        }

        public void Hide()
        {
            _active = false;
            if (line != null)
                line.gameObject.SetActive(false);
        }

        public void SetCenter(Vector3 worldPos)
        {
            if (!_active) return;
            transform.position = new Vector3(worldPos.x, worldPos.y + y, worldPos.z);
            DrawCircle();
        }

        private void DrawCircle()
        {
            if (line == null) return;

            line.positionCount = segments + 1;

            for (int i = 0; i <= segments; i++)
            {
                float t = (float)i / segments * Mathf.PI * 2f;
                float x = Mathf.Cos(t) * _radius;
                float z = Mathf.Sin(t) * _radius;
                line.SetPosition(i, new Vector3(x, 0f, z)); // 로컬 기준
            }
        }
    }
}
