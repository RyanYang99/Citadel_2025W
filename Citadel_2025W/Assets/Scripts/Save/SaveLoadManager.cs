using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace Citadel
{
    public sealed class SaveLoadManager : MonoBehaviour
    {
        [Serializable]
        private class SerializableBuilding
        {
            public string uniqueName;
            public CleanVector3 position, rotation;

            public SerializableBuilding(string uniqueName, Vector3 position, Vector3 rotation)
            {
                this.uniqueName = uniqueName;
                this.position = new CleanVector3(position);
                this.rotation = new CleanVector3(rotation);
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
                serializableBuildings.Add(new SerializableBuilding(placedBuilding.UniqueName, placedBuilding.Position, placedBuilding.Rotation));
            
            File.WriteAllText(path, JsonConvert.SerializeObject(serializableBuildings, Formatting.Indented));
            
            Debug.Log($"Saved to {path}.");
        }

        private void Load()
        {
            if (!File.Exists(path))
                return;
            
            List<SerializableBuilding> serializableBuildings = JsonConvert.DeserializeObject<List<SerializableBuilding>>(File.ReadAllText(path));
            
            foreach (SerializableBuilding serializableBuilding in serializableBuildings)
                buildingManager.PlaceBuilding(serializableBuilding.uniqueName, serializableBuilding.position.ToVector3(), serializableBuilding.rotation.ToVector3());
        }
    }
}