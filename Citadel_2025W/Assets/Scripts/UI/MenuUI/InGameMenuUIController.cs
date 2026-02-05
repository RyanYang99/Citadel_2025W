using UnityEngine;
using UnityEngine.SceneManagement;

namespace Citadel
{
    public sealed class InGameMenuUIController : MonoBehaviour
    {
        private SaveLoadManager _saveLoadManager;

        [SerializeField] private TimeManager timeManager;
        [SerializeField] private GameObject menu;

        private void Awake() => _saveLoadManager = FindAnyObjectByType<SaveLoadManager>();

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (menu.activeInHierarchy)
                    Close();
                else
                    Open();
            }
        }

        public void Open()
        {
            timeManager.SetTimeScale(0f);
            menu.SetActive(true);
        }

        public void Close()
        {
            timeManager.SetTimeScale(1f);
            menu.SetActive(false);
        }

        public void SaveAndExit()
        {
            _saveLoadManager.Save();
            SceneManager.LoadScene(SceneNames.MainMenu);
        }    
    }
}