using UnityEngine;
using UnityEngine.UI;

namespace Citadel
{
    public sealed class BuildingContextUIController : MonoBehaviour
    {
        private GameObject _current;
        
        [Header("Refs")]
        [SerializeField] private BuildingSelectionController selection;
        [SerializeField] private BuildingManager buildingManager;
        [SerializeField] private WorldToScreenUIFollower follower;

        [Header("UI")]
        [SerializeField] private GameObject rootPanel;
        [SerializeField] private Button produceButton;
        [SerializeField] private BarracksUIPage barracksPage;

        [Header("Status Panel (optional)")]
        [SerializeField] private GameObject statusPanel;
        [SerializeField] private Inventory inventory;
        [SerializeField] private StatusPanelController statusPanelController;
        [SerializeField] private RectTransform statusButtonRect;
        [SerializeField] private Vector2 statusPanelOffset = new(200f, 0f);

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
        
        public void ForceHide()
        {
            if (statusPanel != null)
            {
                statusPanel.SetActive(false);
                statusPanelController.Hide();
            }

            if (rootPanel != null)
                rootPanel.SetActive(false);
            
            follower?.ClearTarget();
        }

        private void HandleSelected(GameObject obj)
        {
            Debug.Log($"[ContextUI] Selected: {obj.name}");
            _current = obj;

            Debug.Log($"[ContextUI] rootPanel null? {(rootPanel == null)} / activeBefore: {(rootPanel != null && rootPanel.activeSelf)}");
            rootPanel?.SetActive(true);
            Debug.Log($"[ContextUI] activeAfter: {(rootPanel != null && rootPanel.activeSelf)}");

            follower?.SetTarget(_current.transform);

            _current = obj;
            rootPanel?.SetActive(true);
            follower?.SetTarget(_current.transform);

            bool isBarracks = _current.GetComponentInParent<BarracksProductionQueue>() != null ||
                              _current.GetComponentInChildren<BarracksProductionQueue>() != null;

            if (produceButton != null)
                produceButton.gameObject.SetActive(isBarracks);
        }

        private void HandleDeselected()
        {
            _current = null;
            statusPanel?.SetActive(false);
            rootPanel?.SetActive(false);
            statusPanelController.Hide();

            if (follower != null)
                follower.ClearTarget();

            barracksPage?.Close();
        }

        public void ShowStatus()
        {
            Debug.Log("[Context] Status clicked");

            if (_current == null)
                return;

            GameObject root = _current.transform.root.gameObject;
            BuildingMetaData meta = null;
            
            PlacedBuilding placed = buildingManager.FindPlacedBuilding(root);
            if (placed != null)
                meta = buildingManager.Buildings.list.Find(buildingMetaData => buildingMetaData.uniqueName == placed.UniqueName);

            statusPanelController.Show(root, meta);
            statusPanelController.SnapToButton(statusButtonRect, statusPanelOffset);
        }
        
        public void Upgrade()
        {
            if (_current == null)
                return;

            PlacedBuilding placed = buildingManager.FindPlacedBuilding(_current);
            if (placed == null)
                return;

            BuildingMetaData meta = buildingManager.Buildings.list.Find(buildingMetaData => buildingMetaData.uniqueName == placed.UniqueName);
            if (meta == null)
                return;
           
            if(BuildingUpgrade.Instance==null)
            {
                Debug.LogWarning("[Upgrade] BuildingUpgrade instance missing");
                return;
            }

            try
            {
                bool ok = BuildingUpgrade.Instance.TryUpgrade(_current, meta.subCategory);
                if (!ok)
                    return;

                GameObject newObj = placed._GameObject;
                if (newObj != null)
                {
                    selection.ReplaceSelected(newObj);
                    _current = newObj;
                    follower?.SetTarget(newObj.transform);
                }
            }
            catch (System.ArgumentNullException)
            {
                Debug.LogWarning("[Upgrade] Upgrade prefab not set for this level/subCategory yet.");
            }
            
            ForceHide();
        }

        public void Rotate()
        {
            if (_current == null)
                return;
            
            GameObject root = _current.transform.root.gameObject;
            buildingManager.RotateBuilding(root);
            
            ForceHide();
        }

        public void Produce()
        {
            if (_current == null || barracksPage == null)
                return;

            GameObject root = _current.transform.root.gameObject;
            BarracksProductionQueue queue = root.GetComponent<BarracksProductionQueue>();
            if (queue == null)
                return;

            barracksPage.Open(queue);
            
            ForceHide();
        }

        public void Destroy()
        {
            if (_current == null)
                return;

            GameObject root= _current.transform.root.gameObject;
            buildingManager.RemoveBuilding(root,playSfx:true);

            selection.Deselect();
            ForceHide();
        }
    }
}