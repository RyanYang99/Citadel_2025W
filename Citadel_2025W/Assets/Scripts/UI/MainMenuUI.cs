using UnityEngine;
using UnityEngine.SceneManagement;

namespace Citadel
{
    public sealed class MainMenuUI : MonoBehaviour
    {
        public void OnClickNewGame()
        {
            const string MainScene = "MainScene";
            
            Debug.Log($"Loading {MainScene}");
            SceneManager.LoadScene(MainScene);
        }

        public void OnClickLoad()
        {
            Debug.Log("OnClickLoad");
        }

        public void OnClickOption()
        {
            Debug.Log("OnClickOption");
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
}