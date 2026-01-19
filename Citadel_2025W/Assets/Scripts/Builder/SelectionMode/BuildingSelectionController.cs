using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Citadel
{
    public sealed class BuildingSelectionController : MonoBehaviour
    {
        [SerializeField] private Camera cam;
        [SerializeField] private LayerMask buildingLayer;

        [Header("Highlight")]
        [SerializeField] private Color highlightColor = Color.yellow;

        private readonly Dictionary<MeshRenderer, Color> originalColors = new();
        public GameObject Selected { get; private set; }
        public System.Action<GameObject> OnSelected;
        public System.Action OnDeselected;
        public bool SelectionEnabled { get; private set; } = true;

        public void SetSelectionEnabled(bool enabled)
        {
            SelectionEnabled = enabled;

            if (!enabled)
                Deselect(); // 설치 모드로 들어가면 선택도 자동 해제
        }

        private void Awake()
        {
            if (cam == null) cam = Camera.main;
        }

        private void Update()
        {
            if (!SelectionEnabled)
                return;

            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            if (!Input.GetMouseButtonDown(0))
                return;

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, buildingLayer))
            {
                Select(hit.collider.transform.root.gameObject);
            }
            else
            {
                Deselect();
            }
        }

        public void Select(GameObject target)
        {
            if (target == null) { Deselect(); return; }
            if (Selected == target) return;

            Deselect(); // 기존 해제

            Selected = target;
            ApplyHighlight(Selected);
            OnSelected?.Invoke(Selected);
        }

        public void Deselect()
        {
            if (Selected == null) return;

            ResetHighlight();
            Selected = null;
            OnDeselected?.Invoke();
        }

        // 업그레이드로 오브젝트가 교체됐을 때 선택 타겟 갱신용
        public void ReplaceSelected(GameObject newTarget)
        {
            if (Selected == null) return;
            ResetHighlight();
            Selected = newTarget;
            ApplyHighlight(Selected);
            OnSelected?.Invoke(Selected);
        }

        private void ApplyHighlight(GameObject target)
        {
            originalColors.Clear();
            var renderers = target.GetComponentsInChildren<MeshRenderer>();
            foreach (var r in renderers)
            {
                if (r == null) continue;
                originalColors[r] = r.material.color;
                r.material.color = highlightColor;
            }
        }

        private void ResetHighlight()
        {
            foreach (var kv in originalColors)
            {
                if (kv.Key != null)
                    kv.Key.material.color = kv.Value;
            }
            originalColors.Clear();
        }
    }
}
