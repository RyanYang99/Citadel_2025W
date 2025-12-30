using UnityEngine;




namespace Citadel
{

    public class ResourceUIController : MonoBehaviour
    {
        [Header("항상 보이는 자원")]
        [SerializeField] GameObject[] basicResourceUI;

        [Header("업그레이드 상위 자원")]
        [SerializeField] GameObject[] upgradeResourceUI;

        void Start()
        {
            ShowBasicOnly();
        }

        public void ShowBasicOnly()
        {
            foreach (var go in basicResourceUI)
                go.SetActive(true);

            foreach (var go in upgradeResourceUI)
                go.SetActive(false);
        }

        public void ShowExtraResources()
        {
            foreach (var go in upgradeResourceUI)
                go.SetActive(true);
        }

        public void HideExtraResources()
        {
            foreach (var go in upgradeResourceUI)
                go.SetActive(false);
        }
    }

}