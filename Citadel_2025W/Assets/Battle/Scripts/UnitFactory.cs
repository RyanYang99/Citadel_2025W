using System.Collections.Generic;
using UnityEngine;

public class UnitFactory : MonoBehaviour
{
    [SerializeField] private List<UnitData> unitDatabase; // Infantry/Archer/Shield 3개 넣는 곳

    public UnitData Get(UnitType type)
    {
        foreach (var d in unitDatabase)
            if (d != null && d.type == type) return d;

        Debug.LogError($"[UnitFactory] Missing UnitData for {type}");
        return null;
    }

    public GameObject Spawn(UnitType type, Vector3 pos, Transform parent)
    {
        var data = Get(type);
        if (data == null || data.prefab == null)
        {
            Debug.LogError($"[UnitFactory] Prefab missing for {type}");
            return null;
        }

        return Instantiate(data.prefab, pos, Quaternion.identity, parent);
    }
}