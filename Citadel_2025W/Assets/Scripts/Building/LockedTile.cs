using System.Collections.Generic;
using UnityEngine;

namespace Citadel
{
    public sealed class LockedTile : MonoBehaviour
    {
        public static readonly List<LockedTile> LockedTiles = new();
        
        [SerializeField] private MeshRenderer meshRenderer;
        
        [SerializeField] private bool locked;
        public bool Locked
        {
            get => locked;

            set
            {
                locked = value;

                if (!locked)
                {
                    meshRenderer.material = originalMaterial;
                    Destroy(this);
                }
            }
        }

        [SerializeField] private int level = 1;
        public int Level => level;

        [SerializeField] private Material originalMaterial;

        private void OnEnable() => LockedTiles.Add(this);

        private void OnDisable() => LockedTiles.Remove(this);
    }
}