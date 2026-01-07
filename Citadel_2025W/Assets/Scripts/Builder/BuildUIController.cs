using UnityEngine;

namespace Citadel
{
    public sealed class BuildUIController : MonoBehaviour
    {
        [SerializeField] private GameObject buildPanel;

        public void Open() => buildPanel.SetActive(true);

        public void Close() => buildPanel.SetActive(false);
    }
}