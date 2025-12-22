using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewMonoBehaviourScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OnClickNewGame()
    {
        SceneManager.LoadScene("Main Scene");
        Debug.Log("새 게임");

    }


   public void OnClickLoad()
    {
        Debug.Log("불러오기");

    }

    public void OnClickOption()
    {
        Debug.Log("옵션");
    }


    public void OnClickQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }


}
