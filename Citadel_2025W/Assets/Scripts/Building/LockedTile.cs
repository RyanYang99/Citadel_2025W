using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Citadel
{
    public sealed class LockedTile : MonoBehaviour
    {
        private static readonly List<LockedTile> LockedTiles = new();
        
        [SerializeField] private MeshRenderer meshRenderer;
        
        [SerializeField] private bool locked;
        public bool Locked
        {
            get => locked;

            private set
            {
                locked = value;

                if (!locked)
                {
                    meshRenderer.materials[0] = originalMaterial;
                    Destroy(this);
                }
            }
        }

        [SerializeField] private int level = 1;
        
        [SerializeField] private Material originalMaterial;
        
        public static void Unlock(int level)
        {
            LockedTile[] copy = LockedTiles.Where(lockedTile => lockedTile.locked && lockedTile.level == level).ToArray();
            
            foreach (LockedTile lockedTile in copy)
                lockedTile.Locked = false;
        }

        private void OnEnable() => LockedTiles.Add(this);

        private void OnDisable() => LockedTiles.Remove(this);
    }
}