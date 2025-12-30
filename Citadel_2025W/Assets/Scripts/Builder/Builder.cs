using Unity.VisualScripting;
using UnityEngine;


namespace Citadel
{

public class BuildingPlacer : MonoBehaviour
{
        public BuildingManager buildingManager;
    public GameObject[] buildingPrefabs;
    private int currentIndex = 0;




    void Update()
    {
        // 좌클릭 : 배치
        if (Input.GetMouseButtonDown(0))
            Place();

        // 우클릭 : 회전
        if (Input.GetMouseButtonDown(1))
            RotateBuilding();

        // DEL : 제거
        if (Input.GetKeyDown(KeyCode.Delete))
            Remove();
    }

        //우선은 땅위에만 지을 수 있도록 
        void Place()
        {
            if (buildingManager == null)
            {
                Debug.Log("BuildingManager 연결 안됨");
                return;
            }

            // 현재 선택된 건물 데이터 
            var building = buildingManager.CurrentBuilding;
            if (building == null)
                return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (!hit.collider.CompareTag("Ground"))
                    return;

                building = buildingManager.CurrentBuilding;
                if (building == null) return;

                //타일 중심 위치
                Vector3 pos = hit.collider.transform.position;

                // 건물별 높이 보정
                pos.y += building.yOffset;

                Instantiate(building.prefab, pos, Quaternion.identity);
            }
        }


        void RotateBuilding()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            hit.collider.transform.Rotate(Vector3.up, 90f);
        }
    }

    void Remove()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Destroy(hit.collider.gameObject);
        }
    }
}
}
