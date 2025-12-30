using Citadel;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace Citadel
{

[System.Serializable]
public class BuildingData
{
    public Vector3 position;
    public int prefabIndex;
}

public class SaveLoadManager : MonoBehaviour
{
    public GameObject[] prefabs;
    string path;

    void Start()
    {
        path = Application.persistentDataPath + "/map.json";
    }


        void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            Save();

        if (Input.GetKeyDown(KeyCode.F2))
            Load();
    }

        void Save()
        {
            BuildingPlacer[] buildings = Object.FindObjectsByType<BuildingPlacer>(FindObjectsSortMode.None);

            List<BuildingData> dataList = new();

            foreach (var b in buildings)
            {
                dataList.Add(new BuildingData
                {
                    position = b.transform.position,
                    prefabIndex = 0
                });
            }
            File.WriteAllText(
                path,
                JsonUtility.ToJson(new Serialization<BuildingData>(dataList), true)
            );
        }

        void Load()
    {
        if (!File.Exists(path)) return;

        string json = File.ReadAllText(path);
        var data = JsonUtility.FromJson<Serialization<BuildingData>>(json);

        foreach (var d in data.items)
            Instantiate(prefabs[d.prefabIndex], d.position, Quaternion.identity);
    }
}

[System.Serializable]
public class Serialization<T>
{
    public List<T> items;
    public Serialization(List<T> items) => this.items = items;
}

}