using System.Collections.Generic;
using UnityEngine;

namespace Citadel
{
    public sealed class LevelHoverHighlighter : MonoBehaviour
    {
        [Header("Raycast")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private LayerMask hitMask;

        [Header("Target Root")]
        [SerializeField] private Transform tilemapRoot; // Ground/Grid/Tilemap

        [Header("Highlight")]
        [SerializeField] private Color hoverColor = new Color(1f, 0.8f, 0.2f, 1f);

        // 현재 하이라이트된 구역 루트
        private Transform _currentLevelRoot;

        // Renderer별 원본색 저장
        private readonly Dictionary<Renderer, Color> _baseColors = new();

        private MaterialPropertyBlock _mpb;

        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        private static readonly int ColorId = Shader.PropertyToID("_Color");

        private void Awake()
        {
            if (mainCamera == null) mainCamera = Camera.main;
            _mpb = new MaterialPropertyBlock();
        }

        private void Update()
        {
            if (mainCamera == null || tilemapRoot == null) return;

            Transform levelRoot = null;

            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, 999f, hitMask))
            {
                Debug.Log(hit.collider.transform.name);
                levelRoot = FindLevelRoot(hit.collider.transform);

                if (levelRoot != null && !IsLevelAccessible(levelRoot))
                    levelRoot = null;
            }

            if (levelRoot != _currentLevelRoot)
            {
                ClearCurrent();
                _currentLevelRoot = levelRoot;
                ApplyCurrent();
            }
        }

        private Transform FindLevelRoot(Transform hit)
        {
       
            if (!hit.IsChildOf(tilemapRoot)) return null;

            var t = hit;
            while (t != null && t != tilemapRoot)
            {
                if (t.name.StartsWith("Level"))
                    return t;

                t = t.parent;
            }
            return null;
        }

        private bool IsLevelAccessible(Transform levelRoot)
        {
            if (levelRoot == null) return false;

            var lockedTile = levelRoot.GetComponentInChildren<LockedTile>(true);
            if (lockedTile == null) return false; 

            // 다음 가능한 레벨만 허용
            return ZoneUnlockState.IsNextAllowed(lockedTile.ZoneId);
        }


        private void ApplyCurrent()
        {
            if (_currentLevelRoot == null) return;

            foreach (var r in GetHighlightRenderers(_currentLevelRoot))
            {
                if (r == null) continue;

                if (!_baseColors.ContainsKey(r))
                    _baseColors[r] = GetRendererColor(r);

                SetRendererColor(r, hoverColor);
            }
        }

        private void ClearCurrent()
        {
            if (_currentLevelRoot == null) return;

            foreach (var r in GetHighlightRenderers(_currentLevelRoot))
            {
                if (r == null) continue;

                if (_baseColors.TryGetValue(r, out var baseColor))
                    SetRendererColor(r, baseColor);
            }
        }

        //TopFill이 있으면 TopFill만 먼저 하이라이트
        private IEnumerable<Renderer> GetHighlightRenderers(Transform levelRoot)
        {
            //이름이 TopFill인 오브젝트들만 먼저
            var topFill = levelRoot.GetComponentsInChildren<Transform>(true);
            bool anyTopFill = false;

            for (int i = 0; i < topFill.Length; i++)
            {
                if (topFill[i].name.Contains("TopFill"))
                {
                    anyTopFill = true;
                    var rr = topFill[i].GetComponentsInChildren<Renderer>(true);
                    for (int j = 0; j < rr.Length; j++)
                        yield return rr[j];
                }
            }

            // TopFill 없으면 전체 렌더러 하이라이트
            if (!anyTopFill)
            {
                var rr = levelRoot.GetComponentsInChildren<Renderer>(true);
                for (int j = 0; j < rr.Length; j++)
                    yield return rr[j];
            }
        }

        private Color GetRendererColor(Renderer r)
        {
            var mat = r.sharedMaterial;
            if (mat == null) return Color.white;

            if (mat.HasProperty(BaseColorId)) return mat.GetColor(BaseColorId);
            if (mat.HasProperty(ColorId)) return mat.GetColor(ColorId);

            return Color.white;
        }

        private void SetRendererColor(Renderer r, Color c)
        {
            if (_mpb == null) return; 

            r.GetPropertyBlock(_mpb);

            var mat = r.sharedMaterial;
            if (mat != null && mat.HasProperty(BaseColorId))
                _mpb.SetColor(BaseColorId, c);
            else
                _mpb.SetColor(ColorId, c);

            r.SetPropertyBlock(_mpb);
        }
    }
}
