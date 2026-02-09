using UnityEngine;
using UnityEngine.SceneManagement;

namespace Citadel
{
    public sealed class MainMenuUIController : MonoBehaviour
    {
        private bool _isLoadRequested;

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != SceneNames.MainScene)
                return;
            
            SceneManager.sceneLoaded -= OnSceneLoaded;
            _isLoadRequested = false;
            SaveLoadManager.Instance.Load();
        }
        
        public void NewGame() => SceneManager.LoadScene(SceneNames.MainScene);

        public void Load()
        {
            if (_isLoadRequested)
                return;
            
            _isLoadRequested = true;
            
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene(SceneNames.MainScene);
        }

        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}