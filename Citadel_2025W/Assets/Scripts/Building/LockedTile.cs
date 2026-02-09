using System.Collections.Generic;
using UnityEngine;

namespace Citadel
{
    public sealed class LockedTile : MonoBehaviour
    {
        public static readonly List<LockedTile> LockedTiles = new();

        [Header("Renderers")]
        [SerializeField] private MeshRenderer meshRenderer;        // block-grass
        [SerializeField] private MeshRenderer topFillRenderer;     // TopFill

        [SerializeField] private bool locked;

        public bool Locked
        {
            get => locked;
            set
            {
                locked = value;

                if (!locked)
                {
                    // 본체 타일 머티리얼 복구
                    if (meshRenderer != null)
                        meshRenderer.material = originalMaterial;

                    // TopFill 머티리얼 복구
                    if (topFillRenderer != null)
                        topFillRenderer.material = originalTopFill;

                    Destroy(this);
                }
            }
        }

        [Header("Level")]
        [SerializeField] private int level = 1;
        public int Level => level;

        [Header("Original Materials")]
        [SerializeField] private Material originalMaterial;
        [SerializeField] private Material originalTopFill;

        private void OnEnable() => LockedTiles.Add(this);
        private void OnDisable() => LockedTiles.Remove(this);

        // =========================
        // 디버그 / 테스트 전용
        // =========================
#if UNITY_EDITOR || DEVELOPMENT_BUILD

        /// <summary>
        /// (디버그용) targetLevel 이하의 LockedTile을 모두 해제
        /// </summary>
        public static void UnlockByLevel(int targetLevel)
        {
            int unlocked = 0;

            for (int i = LockedTiles.Count - 1; i >= 0; i--)
            {
                var tile = LockedTiles[i];
                if (tile == null) continue;

                if (tile.Level <= targetLevel)
                {
                    tile.Locked = false;
                    unlocked++;
                }
            }

            Debug.Log(
                $"[LockedTile] UnlockByLevel({targetLevel}) done | unlocked={unlocked}"
            );
        }
#endif
    }
}
