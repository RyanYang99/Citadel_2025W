using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SkillConnection
{
    public RectTransform from;
    public RectTransform to;
    public ResearchData targetData;
    [HideInInspector] public Image lineImage;
}

public class LineDraw : MonoBehaviour
{
    public RectTransform linePrefab;
    public RectTransform parent;
    public SkillConnection[] connections;

    private Color lockedColor = Color.gray;
    private Color unlockedColor = Color.cyan;


    void Start()
    {
        foreach (var conn in connections)
        {
            RectTransform line = Instantiate(linePrefab, parent);

            line.gameObject.name = $"Line_{conn.from.name}_to_{conn.to.name}";

            line.SetSiblingIndex(0);

            conn.lineImage = line.GetComponent<Image>();
            conn.lineImage.color = lockedColor;

            DrawLine(line, conn.from.anchoredPosition, conn.to.anchoredPosition, 6f);
        }

        UpdateLineColors();
    }

    public void UpdateLineColors()
    {
        foreach (var conn in connections)
        {
            if (conn.targetData != null && conn.targetData.isUnlocked)
            {
                conn.lineImage.color = unlockedColor;
            }
        }
    }

    public static void DrawLine(RectTransform line, Vector2 start, Vector2 end, float thickness)
    {
        Vector2 direction = end - start;
        float length = direction.magnitude;

        line.sizeDelta = new Vector2(length, thickness);
        line.anchoredPosition = (start + end) / 2f;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        line.localRotation = Quaternion.Euler(0, 0, angle);
    }
}