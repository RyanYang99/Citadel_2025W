using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Citadel
{
    public sealed class SaveLoadManager : MonoBehaviour
    {
        [Serializable]
        private struct SerializableBuilding
        {
            public string uniqueName;
            public Vector3 position, rotation;

            public SerializableBuilding(string uniqueName, Vector3 position, Vector3 rotation)
            {
                this.uniqueName = uniqueName;
                this.position = position;
                this.rotation = rotation;
            }
        }
        
        [SerializeField] private BuildingMetaDataList buildingsReference;
        [SerializeField] private BuildingManager buildingManager;
        
        private string path;

        private void Awake() => path = Application.persistentDataPath + "/map.json";

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
                Save();

            if (Input.GetKeyDown(KeyCode.F2))
                Load();
        }

        private void Save()
        {
            List<SerializableBuilding> serializableBuildings = new(); 
            foreach (PlacedBuilding placedBuilding in buildingManager.PlacedBuildings)
            {
                serializableBuildings.Add(new SerializableBuilding(placedBuilding.UniqueName, placedBuilding.Position, placedBuilding.Rotation));
            }
            
            //File.WriteAllText(
            //    path,
            //    JsonUtility.ToJson(new Serialization<BuildingData>(dataList), true)
            //);
        }

        void Load()
        {
            if (!File.Exists(path)) return;

            //string json = File.ReadAllText(path);
            //var data = JsonUtility.FromJson<Serialization<BuildingData>>(json);

            //foreach (var d in data.items)
            //    Instantiate(prefabs[d.prefabIndex], d.position, Quaternion.identity);
        }
    }

    [System.Serializable]
    public class Serialization<T>
    {
        public List<T> items;
        public Serialization(List<T> items) => this.items = items;
    }
}