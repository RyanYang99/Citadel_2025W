using UnityEngine;
using Object = UnityEngine.Object;

namespace Citadel
{
    public sealed class PersistentManager : PersistentSingleton<PersistentManager>
    {
        [SerializeField] private SoundManager soundManager;
        [SerializeField] private SaveLoadManager saveLoadManager;
        
        private static void Create<T>(T prefab) where T : Object
        {
            if (FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length == 0)
                Instantiate(prefab);
        }

        protected override void Awake()
        {
            base.Awake();
            if (Destroyed)
                return;
            
            Create(soundManager);
            Create(saveLoadManager);
        }
    }
}