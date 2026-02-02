using UnityEngine;

public class FieldSizePrinter : MonoBehaviour
{
    [ContextMenu("Print Field Bounds From Children")]
    public void PrintBounds()
    {
        if (transform.childCount == 0)
        {
            Debug.LogError("[FieldSizePrinter] No children found under this object.");
            return;
        }

        bool hasAny = false;

        float minX = float.PositiveInfinity;
        float maxX = float.NegativeInfinity;
        float minZ = float.PositiveInfinity;
        float maxZ = float.NegativeInfinity;

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform c = transform.GetChild(i);
            Vector3 p = c.position;

            // 블럭 프리팹들만 대상으로 잡고 싶으면 여기서 필터 가능
            // 예: if (!c.name.Contains("block-grass")) continue;

            hasAny = true;
            if (p.x < minX) minX = p.x;
            if (p.x > maxX) maxX = p.x;
            if (p.z < minZ) minZ = p.z;
            if (p.z > maxZ) maxZ = p.z;
        }

        if (!hasAny)
        {
            Debug.LogError("[FieldSizePrinter] No valid children found.");
            return;
        }

        float widthWorld = maxX - minX;
        float heightWorld = maxZ - minZ;

        Debug.Log(
            $"[FieldSizePrinter]\n" +
            $"- minX={minX}, maxX={maxX} (widthWorld={widthWorld})\n" +
            $"- minZ={minZ}, maxZ={maxZ} (heightWorld={heightWorld})\n" +
            $"- centerX={(minX + maxX) * 0.5f}, centerZ={(minZ + maxZ) * 0.5f}\n"
        );
    }
}