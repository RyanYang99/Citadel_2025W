#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class TerrainBrushTool : EditorWindow
{
    [Header("Paint")]
    GameObject brushPrefab;
    Transform parent;
    int brushRadius = 0;          // 0=1칸, 1=3x3, 2=5x5...
    float gridSize = 1f;
    bool stackOnTop = true;

    [Header("Masks")]
    LayerMask raycastMask = ~0;   // 기본: 모든 레이어
    LayerMask blockMask = ~0;     // 기본: 모든 레이어 (중복검사/삭제 범위용)

    [MenuItem("Tools/Terrain Brush (3D Blocks)")]
    static void Open() => GetWindow<TerrainBrushTool>("Terrain Brush");

    void OnGUI()
    {
        brushPrefab = (GameObject)EditorGUILayout.ObjectField("Block Prefab", brushPrefab, typeof(GameObject), false);
        parent = (Transform)EditorGUILayout.ObjectField("Parent (optional)", parent, typeof(Transform), true);

        brushRadius = EditorGUILayout.IntSlider("Brush Radius", brushRadius, 0, 6);
        gridSize = EditorGUILayout.FloatField("Grid Size", gridSize);
        stackOnTop = EditorGUILayout.Toggle("Stack On Top", stackOnTop);

        raycastMask = LayerMaskField("Raycast Mask", raycastMask);
        blockMask = LayerMaskField("Block Mask", blockMask);

        EditorGUILayout.Space(8);
        EditorGUILayout.HelpBox(
            "SceneView 좌클릭: 배치 / 우클릭: 삭제 (드래그도 됨)\nAlt(카메라 회전) 누르면 동작 안 함",
            MessageType.Info);
    }

    void OnEnable() => SceneView.duringSceneGui += OnSceneGUI;
    void OnDisable() => SceneView.duringSceneGui -= OnSceneGUI;

    void OnSceneGUI(SceneView sceneView)
    {
        Event e = Event.current;
        if (e == null) return;

        // Alt는 카메라 조작이니까 무시
        if (e.alt) return;

        // Scene 뷰에서 선택/컨텍스트 메뉴보다 먼저 입력을 잡기
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        // 마우스 다운/드래그로 칠하기
        bool paint = (e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && e.button == 0;
        bool erase = (e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && e.button == 1;

        if (!paint && !erase) return;

        // 레이캐스트
        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, 5000f, raycastMask, QueryTriggerInteraction.Ignore))
        {
            // 아무것도 안 맞으면 작업 못 함: 바닥(Plane 등) 콜라이더가 필요
            return;
        }

        Vector3 center = GetCenterFromHit(hit);

        if (paint)
        {
            if (brushPrefab == null) return;
            Paint(center);
            e.Use();
        }
        else if (erase)
        {
           // Erase(center, hit);
            e.Use();
        }
    }

    Vector3 GetCenterFromHit(RaycastHit hit)
    {
        Vector3 p = hit.point;

        if (stackOnTop && hit.collider != null)
        {
            // 클릭한 콜라이더의 상단 + 반칸 올려서 "다음 블록의 중심"으로
            float topY = hit.collider.bounds.max.y;
            p = new Vector3(p.x, topY + (gridSize * 0.5f), p.z);
        }

        return Snap(p, gridSize);
    }

    void Paint(Vector3 center)
    {
        // Edit 모드에서 Overlap 쿼리 정확도 위해 동기화
        Physics.SyncTransforms();

        int r = brushRadius;

        for (int x = -r; x <= r; x++)
            for (int z = -r; z <= r; z++)
            {
                Vector3 pos = center + new Vector3(x * gridSize, 0, z * gridSize);

                if (HasAnyColliderAt(pos)) continue;

                GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(brushPrefab);
                Undo.RegisterCreatedObjectUndo(obj, "Paint Block");
                obj.transform.position = pos;
                if (parent != null) obj.transform.SetParent(parent);
            }
    }

    void Erase(Vector3 center, RaycastHit hit)
    {
        // 1) 브러시가 0이면: 클릭한 오브젝트 바로 삭제(가장 확실)
        if (brushRadius == 0 && hit.collider != null)
        {
            Undo.DestroyObjectImmediate(hit.collider.gameObject);
            return;
        }

        // 2) 브러시 범위 삭제
        Physics.SyncTransforms();

        int r = brushRadius;
        for (int x = -r; x <= r; x++)
            for (int z = -r; z <= r; z++)
            {
                Vector3 pos = center + new Vector3(x * gridSize, 0, z * gridSize);

                Collider[] cols = Physics.OverlapBox(
                    pos,
                    Vector3.one * (gridSize * 0.49f),
                    Quaternion.identity,
                    blockMask,
                    QueryTriggerInteraction.Collide
                );

                foreach (var c in cols)
                    Undo.DestroyObjectImmediate(c.gameObject);
            }
    }

    bool HasAnyColliderAt(Vector3 pos)
    {
        Collider[] cols = Physics.OverlapBox(
            pos,
            Vector3.one * (gridSize * 0.49f),
            Quaternion.identity,
            blockMask,
            QueryTriggerInteraction.Collide
        );
        return cols != null && cols.Length > 0;
    }

    static Vector3 Snap(Vector3 p, float size)
    {
        p.x = Mathf.Round(p.x / size) * size;
        p.y = Mathf.Round(p.y / size) * size;
        p.z = Mathf.Round(p.z / size) * size;
        return p;
    }

    // LayerMask를 EditorGUILayout에서 편하게 다루기 위한 헬퍼
    static LayerMask LayerMaskField(string label, LayerMask selected)
    {
        string[] layers = UnityEditorInternal.InternalEditorUtility.layers;
        int maskWithoutEmpty = 0;

        for (int i = 0; i < layers.Length; i++)
        {
            int layer = LayerMask.NameToLayer(layers[i]);
            if (((1 << layer) & selected.value) != 0)
                maskWithoutEmpty |= (1 << i);
        }

        maskWithoutEmpty = EditorGUILayout.MaskField(label, maskWithoutEmpty, layers);

        int mask = 0;
        for (int i = 0; i < layers.Length; i++)
        {
            if ((maskWithoutEmpty & (1 << i)) != 0)
            {
                int layer = LayerMask.NameToLayer(layers[i]);
                mask |= (1 << layer);
            }
        }

        selected.value = mask;
        return selected;
    }
}
#endif