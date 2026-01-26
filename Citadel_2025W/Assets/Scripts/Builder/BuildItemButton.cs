using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Citadel
{
    public sealed class BuildItemButton : MonoBehaviour
    {
        private BuildingMetaData meta;
        private BuildingManager buildingManager;
        private BuildingPlacer buildingPlacer;
        private BuildPreviewController previewController;

        private BuildingSelectionController selectionController;
        private BuildingContextUIController contextUI;

        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI countText;

        public void Init(
            BuildingManager manager,
            BuildingPlacer placer,
            BuildingMetaData metaData)
        {
            meta = metaData;
            buildingManager = manager;
            buildingPlacer = placer;

            if (meta == null)
            {
                Debug.LogError("[BuildItemButton] meta is NULL");
                return;
            }

            if (icon != null && meta.icon != null)
                icon.sprite = meta.icon;

            buildingManager.OnPlacedBuildingChanged += Refresh;

            Refresh();
        }
        private void OnDestroy()
        {
            if (buildingManager != null)
                buildingManager.OnPlacedBuildingChanged -= Refresh;
        }


        public void Refresh()
        {
            if (meta == null || buildingManager == null)
                return;

            int current = buildingManager.GetPlacedCount(meta.uniqueName);
            if (meta.maxBuildCount < 0)
            {
                countText.text = "¡Ä";
                GetComponent<Button>().interactable = true;
                return;
            }

            if (current >= meta.maxBuildCount)
            {
                countText.text = "MAX";
                GetComponent<Button>().interactable = false;
            }
            else
            {
                countText.text = $"{current} / {meta.maxBuildCount}";
                GetComponent<Button>().interactable = true;
            }

            if (previewController != null)
                previewController.SetMode(BuildMode.Build);

        }
        private bool IsBuildable()
        {
            if (meta == null)
                return false;

            if (meta.maxBuildCount < 0)
                return true;

            int current = buildingManager.GetPlacedCount(meta.uniqueName);
            return current < meta.maxBuildCount;
        }


        public void OnClick()
        {
            if (meta == null)
                return;

            if (!IsBuildable())
                return;


            buildingManager.SelectBuilding(
                buildingManager.Buildings.list.IndexOf(meta)
            );
            buildingPlacer.SetBuildMode();

            selectionController?.SetSelectionEnabled(false);
            contextUI?.ForceHide();


            if (previewController != null)
                previewController.SetMode(BuildMode.Build);
        }
    }
}
