using UnityEngine;

namespace Citadel
{
    public class BuildingPlacer : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private GameObject buildScrollView;
        [SerializeField] private BuildingManager buildingManager;

        private void Update()
        {
            if (!buildScrollView.activeSelf)
                return;
            
            // 좌클릭: 배치
            if (Input.GetMouseButtonDown(0))
                Place();

            // 우클릭: 회전
            if (Input.GetMouseButtonDown(1))
                RotateBuilding();

            // DEL: 제거
            if (Input.GetKeyDown(KeyCode.Delete))
                Remove();
        }

        private bool GetRaycastHitFromMouse(out RaycastHit raycastHit) => 
            Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out raycastHit);

        private static bool IsGround(GameObject _gameObject) => _gameObject.CompareTag(Tags.Ground);

        //우선은 땅위에만 지을 수 있도록
        private void Place()
        {
            if (!GetRaycastHitFromMouse(out RaycastHit raycastHit))
                return;

            if (!IsGround(raycastHit.transform.gameObject))
                return;
            
            buildingManager.PlaceBuilding(raycastHit.transform.position);
        }

        private void RotateBuilding()
        {
            if (!GetRaycastHitFromMouse(out RaycastHit raycastHit))
                return;

            if (IsGround(raycastHit.transform.gameObject))
                return;
            
            buildingManager.RotateBuilding(raycastHit.transform.gameObject);
        }

        private void Remove()
        {
            if (!GetRaycastHitFromMouse(out RaycastHit raycastHit))
                return;

            if (IsGround(raycastHit.transform.gameObject))
                return;
            
            buildingManager.RemoveBuilding(raycastHit.transform.gameObject);
        }
    }
}