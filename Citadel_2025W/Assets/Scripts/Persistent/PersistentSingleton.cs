using UnityEngine;

namespace Citadel
{
    public abstract class PersistentSingleton<T> : MonoBehaviour where T : PersistentSingleton<T>
    {
        public static T Instance { get; private set; }

        protected bool Destroyed;

        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = (T)this;
                DontDestroyOnLoad(transform.root);
            }
            else
            {
                Destroyed = true;
                Destroy(gameObject);
            }
        }
    }
}