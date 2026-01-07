using UnityEngine;
using UnityEngine.SceneManagement;

namespace Citadel
{
    public sealed class TitleMenuUIController : MonoBehaviour
    {
        private const string MainSceneName = "MainScene";
        private bool isLoadRequested;

        public void OnClickNewGame()
        {
            Debug.Log($"Loading {MainSceneName}");
            SceneManager.LoadScene(MainSceneName);
        }

        public void OnClickLoad()
        {
            Debug.Log("OnClickLoad");
            if (isLoadRequested)
                return;
            isLoadRequested = true;
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene(MainSceneName);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != MainSceneName)
                return;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SaveGame.Instance.Load();
            isLoadRequested = false;
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
