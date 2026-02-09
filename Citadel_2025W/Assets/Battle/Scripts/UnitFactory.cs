using System.Collections.Generic;
using UnityEngine;

public class UnitFactory : MonoBehaviour
{
    [SerializeField] private List<UnitData> unitDatabase = new();

    private Dictionary<UnitType, UnitData> _map;

    private void Awake()
    {
        _map = new Dictionary<UnitType, UnitData>();

        foreach (var data in unitDatabase)
        {
            if (data == null) continue;

            if (_map.ContainsKey(data.type))
            {
                Debug.LogWarning($"[UnitFactory] Duplicate UnitData for {data.type}. Using first one.");
                continue;
            }

            _map.Add(data.type, data);
        }
    }

    public GameObject Spawn(UnitType type, Vector3 position, Transform parent)
    {
        if (_map == null) Awake();

        if (!_map.TryGetValue(type, out var data) || data == null)
        {
            Debug.LogError($"[UnitFactory] Missing UnitData for {type}");
            return null;
        }

        if (data.prefab == null)
        {
            Debug.LogError($"[UnitFactory] Prefab missing for {type}");
            return null;
        }

        var go = Instantiate(data.prefab, position, Quaternion.identity, parent);

        var runtime = go.GetComponent<UnitRuntime>();
        if (runtime == null) runtime = go.GetComponentInChildren<UnitRuntime>();

        if (runtime != null)
        {
            runtime.ApplyData(data);
        }
        else
        {
            Debug.LogError($"[UnitFactory] Spawned prefab has no UnitRuntime: {go.name}");
        }

        return go;
    }
}