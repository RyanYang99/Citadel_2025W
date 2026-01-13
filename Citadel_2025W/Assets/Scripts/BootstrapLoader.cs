using UnityEngine;
using UnityEngine.SceneManagement;

namespace Citadel
{
    public sealed class BootstrapLoader : MonoBehaviour
    {
        private static bool bootstrapped;

        [SerializeField] private GameObject bootstrapPrefab;

        private void Awake()
        {
            if (bootstrapped)
                return;

          

            bootstrapped = true;
        }
    }
}
