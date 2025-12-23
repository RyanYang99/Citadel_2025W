using UnityEngine;


namespace Citadel
{
public class CameraController : MonoBehaviour
{
    public float moveSpeed = 15f;
    public float rotateSpeed = 5f;
    public float zoomSpeed = 200f;

    void Update()
    {
        // WASD 이동
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(h, 0, v);
        transform.Translate(move * moveSpeed * Time.deltaTime, Space.World);

        // 중클릭 회전
        if (Input.GetMouseButton(2))
        {
            float mouseX = Input.GetAxis("Mouse X");
            transform.Rotate(Vector3.up, mouseX * rotateSpeed, Space.World);
        }

        // 마우스 휠 줌
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        transform.Translate(Vector3.forward * scroll * zoomSpeed * Time.deltaTime, Space.Self);

        // F : 카메라 중앙
        if (Input.GetKeyDown(KeyCode.F))
        {
            transform.position = new Vector3(0, transform.position.y, 0);
        }
    }
}

}