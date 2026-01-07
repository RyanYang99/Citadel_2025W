using UnityEngine;
using UnityEngine.SceneManagement;

namespace Citadel
{
    public sealed class InGameMenuUIController : MonoBehaviour
    {
        [Header("Menu Panel")]
        [SerializeField] private GameObject menuPanel;

        private bool isOpen;

        private void Awake()
        {
            if (menuPanel != null)
                menuPanel.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isOpen)
                    CloseMenu();
                else
                    OpenMenu();
            }
        }

        public void OpenMenu()
        {
            isOpen = true;
            Time.timeScale = 0f;
            menuPanel.SetActive(true);
        }

        public void CloseMenu()
        {
            isOpen = false;
            Time.timeScale = 1f;
            menuPanel.SetActive(false);
        }
        //저장하고 메인화면으로
        public void OnClickSaveAndExit()
        {
            SaveGame.Instance.Save();
            SceneManager.LoadScene("MainMenu");
        }    
    }

}
