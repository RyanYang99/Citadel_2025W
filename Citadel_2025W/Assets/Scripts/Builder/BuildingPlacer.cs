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
        [SerializeField] private LayerMask buildingLayer;
        [SerializeField] private BuildPreviewController previewController;

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
            Debug.Log("철거 모드");
        }

        public void SetBuildMode()
        {
            currentMode = BuildMode.Build;
            previewController.SetMode(BuildMode.Build);
            Debug.Log("설치 모드");
        }

        public void SetIdleMode()
        {
            currentMode = BuildMode.None;
            previewController.SetMode(BuildMode.None);
            Debug.Log("대기 모드");
        }

        private bool GetRaycastHitFromMouse(out RaycastHit raycastHit) =>
            Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out raycastHit);

        private static bool IsGround(GameObject _gameObject) => _gameObject.CompareTag(Tags.Ground);
      
        private void Place()
        {
            if (!GetRaycastHitFromMouse(out RaycastHit hit)) return;
            if (!IsGround(hit.transform.gameObject)) return;

            buildingManager.PlaceBuilding(hit.point,previewController.CurrentRotation); 

        }

        private void Rotate()
        {
            if (!GetRaycastHitFromMouse(out RaycastHit hit)) return;
            if (IsGround(hit.transform.gameObject)) return;

            buildingManager.RotateBuilding(hit.transform.gameObject);
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