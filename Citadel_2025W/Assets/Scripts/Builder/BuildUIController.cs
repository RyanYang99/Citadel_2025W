using UnityEngine;

public class BuildUIController : MonoBehaviour
{
    public GameObject buildPanel;

    public void Open()
    {
        buildPanel.SetActive(true);
    }

    public void Close()
    {
        buildPanel.SetActive(false);
    }


}
