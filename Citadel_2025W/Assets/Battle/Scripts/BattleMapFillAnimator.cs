using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Citadel
{
    public sealed class BattleMapFillAnimator : MonoBehaviour
    {
       

        [Header("Tile")]
        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private Transform tileParent;

        [Header("Grid")]
        [SerializeField] private float cellSize = 1f;
        [SerializeField] private Vector3 origin = Vector3.zero;

        [Header("Fill Settings")]
        [SerializeField] private float spawnInterval = 0.02f;   // 타일 깔리는 속도
        [SerializeField] private float popDuration = 0.08f;     // 애니 길이
        [SerializeField] private float popScale = 1.1f;         // 최대 스케일

        [Header("Bar Size (ㅡ shape)")]
        [SerializeField] private int width = 29;   // 가로
        [SerializeField] private int height = 6;   // 세로
        [SerializeField] private bool centerPivot = true; // 가운데 정렬

        [Header("Generated Cells (Read Only)")]
        [SerializeField] private List<Vector2Int> cells = new();

        private readonly List<GameObject> _spawned = new();

        private void Reset()
        {
            // 컴포넌트 추가 시 자동으로 한번 생성
            GenerateBarCells();
        }

        private void OnValidate()
        {
            // 인스펙터 값 바꿀 때 음수/0 방지
            if (width < 1) width = 1;
            if (height < 1) height = 1;
            if (cellSize <= 0f) cellSize = 1f;
        }

        [ContextMenu("Generate Bar Cells (width x height)")]
        private void GenerateBarCells()
        {
            if (cells == null)
                cells = new List<Vector2Int>();

            cells.Clear();

            int xStart = 0;
            int yStart = 0;

            if (centerPivot)
            {
                // width=29 => x: -14 ~ +14 (29칸)
                // height=6  => y: -3 ~ +2 (6칸) 
                xStart = -width / 2;
                yStart = -height / 2;
            }

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    cells.Add(new Vector2Int(xStart + x, yStart + y));

            Debug.Log($"[BattleMapFillAnimator] Generated {width}x{height} = {cells.Count} cells", this);
        }

        public void Clear()
        {
            foreach (var go in _spawned)
                if (go != null) Destroy(go);

            _spawned.Clear();
        }

        public IEnumerator PlayFill()
        {
            if (tilePrefab == null)
            {
                Debug.LogError("[BattleMapFillAnimator] tilePrefab missing", this);
                yield break;
            }

            if (tileParent == null)
                tileParent = transform;

         
            if (cells == null || cells.Count == 0)
                GenerateBarCells();

            int minY = int.MaxValue, maxY = int.MinValue;
            int maxAbsX = 0;

            for (int k = 0; k < cells.Count; k++)
            {
                var c = cells[k];
                if (c.y < minY) minY = c.y;
                if (c.y > maxY) maxY = c.y;
                maxAbsX = Mathf.Max(maxAbsX, Mathf.Abs(c.x));
            }

            var set = new HashSet<Vector2Int>(cells);

            for (int d = 0; d <= maxAbsX; d++)
            {
          
                for (int y = minY; y <= maxY; y++)
                {
                    if (d == 0)
                    {
                        var c0 = new Vector2Int(0, y);
                        if (set.Contains(c0))
                            SpawnCell(c0);
                    }
                    else
                    {
                        var cl = new Vector2Int(-d, y);
                        if (set.Contains(cl))
                            SpawnCell(cl);

                        var cr = new Vector2Int(+d, y);
                        if (set.Contains(cr))
                            SpawnCell(cr);
                    }
                }
                if (spawnInterval > 0f)
                    yield return new WaitForSeconds(spawnInterval);
            }
        }

        private void SpawnCell(Vector2Int c)
        {
            var pos = origin + new Vector3((c.x - 0.5f) * cellSize, 0f, (c.y - 0.5f) * cellSize);
            var go = Instantiate(tilePrefab, pos, Quaternion.identity, tileParent);
            _spawned.Add(go);

            go.transform.localScale = Vector3.zero;
            StartCoroutine(Pop(go.transform));
        }

        private IEnumerator Pop(Transform t)
        {
            float t0 = 0f;

            // popScale
            while (t0 < popDuration)
            {
                t0 += Time.deltaTime;
                float k = Mathf.Clamp01(t0 / popDuration);
                float s = Mathf.Lerp(0f, popScale, EaseOutBack(k));
                t.localScale = new Vector3(s, s, s);
                yield return null;
            }

            // popScale 1
            float t1 = 0f;
            float dur2 = Mathf.Max(0.01f, popDuration * 0.6f);

            while (t1 < dur2)
            {
                t1 += Time.deltaTime;
                float k = Mathf.Clamp01(t1 / dur2);
                float s = Mathf.Lerp(popScale, 1f, k);
                t.localScale = new Vector3(s, s, s);
                yield return null;
            }

            t.localScale = Vector3.one;
        }

        private static float EaseOutBack(float x)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;
            return 1f + c3 * Mathf.Pow(x - 1f, 3) + c1 * Mathf.Pow(x - 1f, 2);
        }
    }
}
