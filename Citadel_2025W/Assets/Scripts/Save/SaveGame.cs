using System.Collections;
using UnityEngine;

namespace Citadel
{
    public sealed class SaveGame : MonoBehaviour
    {
        public static SaveGame Instance { get; private set; }

        private SaveLoadManager saveLoadManager;
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        public void Register(SaveLoadManager manager)
        {
            saveLoadManager = manager;
        }
        public void Save()
        {
            if (saveLoadManager == null)
            {
                Debug.LogWarning("SaveLoadManager not registered.");
                return;
            }

            saveLoadManager.SendMessage(
                "Save",
                SendMessageOptions.DontRequireReceiver
            );
        }
        public void Load()
        {
            if (saveLoadManager == null)
            {
                Debug.LogWarning("SaveLoadManager not registered.");
                return;
            }

            StartCoroutine(LoadWhenReady());
        }

        private IEnumerator LoadWhenReady()
        {
            BuildingManager buildingManager =
                saveLoadManager.GetComponent<SaveLoadManager>()
                               .GetComponentInChildren<BuildingManager>();

            while (buildingManager == null || !buildingManager.IsReady)
                yield return null;
            saveLoadManager.SendMessage(
                "Load",
                SendMessageOptions.DontRequireReceiver
            );
        }
    }
}
