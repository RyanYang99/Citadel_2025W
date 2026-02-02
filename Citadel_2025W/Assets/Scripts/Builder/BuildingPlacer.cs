using UnityEngine;
using UnityEngine.EventSystems;

//입력 처리 (마우스 클릭 , 모드 전환)
namespace Citadel
{
    public enum BuildMode
    {
       Build,
       Destroy,
       None
    }

    public class BuildingPlacer : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private GameObject buildScrollView;
        [SerializeField] private BuildingManager buildingManager;
        [SerializeField] private AnimationManager animationManager;
        [SerializeField] private LayerMask buildingLayer;
        [SerializeField] private BuildPreviewController previewController;
        [SerializeField] private SFXLooper SFXLooper;
        [SerializeField] private GridService grid;
        [SerializeField] private GridPlacementValidator validator;
        [SerializeField] private BuildingSelectionController selectionController;
        [SerializeField] private BuildingContextUIController contextUI;


        private BuildMode currentMode = BuildMode.Build;
        private void Update()
        {
            if (currentMode == BuildMode.None)
                return;

            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (Input.GetMouseButtonDown(0))
            {
                if (currentMode == BuildMode.Destroy)
                    TryDestroyBuilding();
                else
                    Place();
            }

            if(Input.GetMouseButtonDown(1))
            {
                if (currentMode == BuildMode.Build)
                    previewController.RotatePreview();
                else if (currentMode == BuildMode.Destroy)
                    Rotate();
            }
        }
        public void SetDestroyMode()
        {
            currentMode = BuildMode.Destroy;
            previewController.SetMode(BuildMode.Destroy);

            selectionController?.SetSelectionEnabled(false);
            contextUI?.ForceHide();
            Debug.Log("철거 모드");
        }


        public void SetBuildMode()
        {
            currentMode = BuildMode.Build;
            previewController.SetMode(BuildMode.Build);

            selectionController?.SetSelectionEnabled(false);
            contextUI?.ForceHide();
            Debug.Log("설치 모드");
        }

        public void SetIdleMode()
        {
            currentMode = BuildMode.None;
            previewController.SetMode(BuildMode.None);

            selectionController?.SetSelectionEnabled(true);
            Debug.Log("대기 모드");
        }

        private bool GetRaycastHitFromMouse(out RaycastHit raycastHit) =>
            Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out raycastHit);

        private static bool IsGround(GameObject _gameObject) => _gameObject.CompareTag(Tags.Ground);
      
        private void Place()
        {
            if (!buildingManager.CanBuild(buildingManager.CurrentBuilding))
                return;

            if (!GetRaycastHitFromMouse(out RaycastHit hit)) return;
            if (!IsGround(hit.transform.gameObject)) return;
            
            if (previewController.buildPreviewInstance != null)
                if (BuildingManager.OverLockedTilesOrBuildings(previewController.buildPreviewInstance.GetComponent<BoxCollider>()))
                    return;
            Vector3 snapped = grid.SnapToCellCenter(hit.point);

            // yOffset은 BuildingManager.PlaceBuilding(Vector3)에서 더해짐
            if (!validator.CanPlace(buildingManager.CurrentBuilding, snapped, previewController.CurrentRotation))
                return;

            buildingManager.PlaceBuilding(snapped, previewController.CurrentRotation);

            var placedBuildings = buildingManager.PlacedBuildings;
            if (placedBuildings.Count > 0)
            {
                GameObject justPlaced = placedBuildings[placedBuildings.Count - 1]._GameObject;
                // 설치시 애니메이션 추가
                if (animationManager != null)
                {
                    animationManager.ApplyConstructionEffect(justPlaced);
                }
            }

            SFXLooper.PlayLoop(1.5f, 2.0f);
        }

        private void Rotate()
        {
            if (!GetRaycastHitFromMouse(out RaycastHit hit)) return;
            if (IsGround(hit.transform.gameObject)) return;

            buildingManager.RotateBuilding(hit.transform.root.gameObject);
        }

        [SerializeField] private LayerMask destroyLayer;
        private void TryDestroyBuilding()
        {

            if (!Physics.Raycast(
                _camera.ScreenPointToRay(Input.mousePosition),
                out RaycastHit hit,
                Mathf.Infinity,
                destroyLayer))
            {
                Debug.Log("Raycast failed");
                return;
            }

            buildingManager.RemoveBuilding(hit.collider.transform.root.gameObject);
        }
    }
}