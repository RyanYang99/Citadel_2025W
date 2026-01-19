using UnityEngine;
using UnityEngine.UI;

namespace Citadel
{
    public sealed class BuildingContextUIController : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private BuildingSelectionController selection;
        [SerializeField] private BuildingManager buildingManager;
        [SerializeField] private WorldToScreenUIFollower follower;

        [Header("UI")]
        [SerializeField] private GameObject rootPanel;
        [SerializeField] private Button statusButton;
        [SerializeField] private Button upgradeButton;
        [SerializeField] private Button rotateButton;
        [SerializeField] private Button destroyButton;

        [Header("Status Panel (optional)")]
        [SerializeField] private GameObject statusPanel;
        [SerializeField] private Inventory inventory;
        [SerializeField] private StatusPanelController statusPanelController;
        [SerializeField] private RectTransform statusButtonRect;
        [SerializeField] private Vector2 statusPanelOffset = new Vector2(200f, 0f);

        private GameObject current;

        private void Awake()
        {
            if (statusPanelController != null && inventory != null)
                statusPanelController.BindInventory(inventory);

            rootPanel?.SetActive(false);
            statusPanel?.SetActive(false);

            statusButton?.onClick.AddListener(OnClickStatus);
            upgradeButton?.onClick.AddListener(OnClickUpgrade);
            rotateButton?.onClick.AddListener(OnClickRotate);
            destroyButton?.onClick.AddListener(OnClickDestroy);
        }

        public void ForceHide()
        {
            // 패널/팔로워/선택 상태와 무관하게 UI만숨김
            if (statusPanel != null) statusPanel.SetActive(false);
            if (rootPanel != null) rootPanel.SetActive(false);
            follower?.ClearTarget();
        }


        private void OnEnable()
        {
            selection.OnSelected += HandleSelected;
            selection.OnDeselected += HandleDeselected;
        }

        private void OnDisable()
        {
            selection.OnSelected -= HandleSelected;
            selection.OnDeselected -= HandleDeselected;
        }

        private void HandleSelected(GameObject obj)
        {
            Debug.Log($"[ContextUI] Selected: {obj.name}");
            current = obj;

            Debug.Log($"[ContextUI] rootPanel null? {(rootPanel == null)} / activeBefore: {(rootPanel != null && rootPanel.activeSelf)}");
            rootPanel?.SetActive(true);
            Debug.Log($"[ContextUI] activeAfter: {(rootPanel != null && rootPanel.activeSelf)}");

            follower?.SetTarget(current.transform);
        }


        private void HandleDeselected()
        {
            current = null;
            statusPanel?.SetActive(false);
            rootPanel?.SetActive(false);

            if (follower != null)
                follower.ClearTarget();
        }

        private void OnClickStatus()
        {
            Debug.Log("[Context] Status clicked");

            if (current == null || statusPanelController == null) return;

            var root = current.transform.root.gameObject;

            BuildingMetaData meta = null;

            var placed = buildingManager.FindPlacedBuilding(root);
            if (placed != null)
                meta = buildingManager.Buildings.list.Find(b => b.uniqueName == placed.UniqueName);

            statusPanelController.Show(root, meta);
            statusPanelController.SnapToButton(statusButtonRect, statusPanelOffset);
        }


        private void OnClickUpgrade()
        {
            if (current == null) return;

            var placed = buildingManager.FindPlacedBuilding(current);
            if (placed == null) return;

            var meta = buildingManager.Buildings.list.Find(b => b.uniqueName == placed.UniqueName);
            if (meta == null) return;

           
            if(BuildingUpgrade.Instance==null)
            {
                Debug.LogWarning("[Upgrade] BuildingUpgrade instance missing");
                return;
            }

            try
            {
                bool ok = BuildingUpgrade.Instance.TryUpgrade(current, meta.subCategory);
                if (!ok) return;

                var newObj = placed._GameObject;
                if (newObj != null)
                {
                    selection.ReplaceSelected(newObj);
                    current = newObj;
                    follower?.SetTarget(newObj.transform);
                }
            }
            catch (System.ArgumentNullException)
            {
                Debug.LogWarning("[Upgrade] Upgrade prefab not set for this level/subCategory yet.");
                // TODO: UI에 '업그레이드 데이터 준비중' 같은 문구 표시
            }
        }

        private void OnClickRotate()
        {
            if (current == null) return;
            GameObject root = current.transform.root.gameObject;
            buildingManager.RotateBuilding(root);
            follower?.SetTarget(root.transform); 
        }

        private void OnClickDestroy()
        {
            if (current == null) return;

            GameObject root= current.transform.root.gameObject;
            buildingManager.RemoveBuilding(root,playSfx:true);

            selection.Deselect();
            ForceHide();
        }
    }
}
