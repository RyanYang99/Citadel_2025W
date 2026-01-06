using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace Citadel
{
    public class UpgradeController : MonoBehaviour
    {
        [Header("참조 설정")]
        [SerializeField, Tooltip("초록색 건설 프리뷰를 생성하는 스크립트를 여기에 드래그하세요.")]
        private MonoBehaviour buildScript;

        private bool isUpgradeMode = false;
        private GameObject lastHoveredBuilding;

        // 여러 개의 자식 메쉬 색상을 복구하기 위한 딕셔너리
        private Dictionary<MeshRenderer, Color> originalColors = new Dictionary<MeshRenderer, Color>();

        /// <summary>
        /// 업그레이드 버튼 클릭 시 호출 (토글 방식)
        /// </summary>
        public void ToggleUpgradeMode()
        {
            isUpgradeMode = !isUpgradeMode;

            if (isUpgradeMode)
            {
                // 업그레이드 모드 진입 시 건설 프리뷰 스크립트 비활성화
                if (buildScript != null) buildScript.enabled = false;
                Debug.Log("<color=yellow>업그레이드 모드 활성화</color>");
            }
            else
            {
                DeactivateUpgradeMode();
            }
        }

        /// <summary>
        /// 업그레이드 모드를 완전히 종료하고 모든 상태를 초기화
        /// </summary>
        public void DeactivateUpgradeMode()
        {
            isUpgradeMode = false;
            ResetHighlight();

            // 건설 프리뷰 스크립트 다시 활성화
            if (buildScript != null) buildScript.enabled = true;

            Debug.Log("업그레이드 모드 비활성화");
        }

        private void Update()
        {
            if (!isUpgradeMode) return;

            if (EventSystem.current.IsPointerOverGameObject())
            {
                ResetHighlight();
                if (Input.GetMouseButtonDown(0)) DeactivateUpgradeMode();
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                int buildingLayer = LayerMask.NameToLayer("Building");

                if (hit.collider.gameObject.layer == buildingLayer)
                {
                    GameObject currentBuilding = hit.collider.gameObject;

                    if (currentBuilding != lastHoveredBuilding)
                    {
                        ResetHighlight();
                        lastHoveredBuilding = currentBuilding;
                        ApplyHighlight(lastHoveredBuilding, true);
                    }

                    if (Input.GetMouseButtonDown(0))
                    {
                        ExecuteUpgradeOnce(currentBuilding);
                    }
                }
                else
                {
                    ResetHighlight();
                }
            }
            else
            {
                ResetHighlight();
            }
        }

        private void ExecuteUpgradeOnce(GameObject target)
        {
            var manager = FindFirstObjectByType<BuildingManager>();
            var placed = manager.FindPlacedBuilding(target);

            if (placed != null)
            {
                var metadata = manager.Buildings.list.Find(b => b.uniqueName == placed.UniqueName);
                if (metadata != null)
                {
                    // BuildingUpgrade 싱글톤을 통해 업그레이드 시도
                    if (BuildingUpgrade.Instance.TryUpgrade(target, metadata.subCategory))
                    {
                        // 단 한 번 업그레이드 성공 후 모드 종료
                        DeactivateUpgradeMode();
                    }
                }
            }
        }

        private void ApplyHighlight(GameObject target, bool highlight)
        {
            // 건물에 포함된 모든 자식 메쉬를 찾아 노란색으로 변경
            MeshRenderer[] renderers = target.GetComponentsInChildren<MeshRenderer>();
            foreach (var renderer in renderers)
            {
                if (!originalColors.ContainsKey(renderer))
                {
                    originalColors.Add(renderer, renderer.material.color);
                }
                renderer.material.color = Color.yellow;
            }
        }

        private void ResetHighlight()
        {
            if (lastHoveredBuilding != null)
            {
                // 저장해둔 원래 색상으로 모든 메쉬 복구
                foreach (var kvp in originalColors)
                {
                    if (kvp.Key != null) kvp.Key.material.color = kvp.Value;
                }
                originalColors.Clear();
                lastHoveredBuilding = null;
            }
        }
    }
}