using UnityEngine;
using UnityEngine.UI;

public class TileUIButton : MonoBehaviour
{
    public GameObject prefabToPlace; // Inspector에서 연결할 Prefab

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void OnClick()
    {
    }
}