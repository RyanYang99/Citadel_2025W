using UnityEngine;

namespace Citadel
{
    public sealed class LockedTile : MonoBehaviour
    {
        [SerializeField] private bool locked;
        public bool Locked
        {
            get => locked;

            private set
            {
                locked = value;

                if (locked)
                {
                    meshRenderer.materials[0] = originalMaterial;
                    Destroy(this);
                }
            }
        }

        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Material originalMaterial;
    }
}