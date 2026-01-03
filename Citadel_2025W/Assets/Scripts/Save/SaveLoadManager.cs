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
        private class SaveFile
        {
            public DateTime ElapsedTime;
            
            public List<SerializableBuilding> buildings = new();
            public List<ItemAmount> inventory = new();
        }

        [Header("Elapsed Time"), SerializeField] private TimeManager timeManager;

        [Header("Building"), SerializeField] private BuildingMetaDataList buildingsReference;
        [SerializeField] private BuildingManager buildingManager;

        [Header("Inventory"), SerializeField] private Inventory inventory;
        
        private string path;

        private void Awake() => path = Application.persistentDataPath + "/save.json";

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
                Save();

            if (Input.GetKeyDown(KeyCode.F2))
                Load();
        }

        private void Save()
        {
            SaveFile saveFile = new()
            {
                ElapsedTime = timeManager.TimeElapsed
            };

            foreach (PlacedBuilding placedBuilding in buildingManager.PlacedBuildings)
                saveFile.buildings.Add(new SerializableBuilding(placedBuilding.UniqueName, placedBuilding.Position, placedBuilding.Rotation));

            saveFile.inventory = inventory.ToList();
            
            File.WriteAllText(path, JsonConvert.SerializeObject(saveFile, Formatting.Indented));
            
            Debug.Log($"Saved to {path}.");
        }

        private void Load()
        {
            if (!File.Exists(path))
                return;

            SaveFile saveFile = JsonConvert.DeserializeObject<SaveFile>(File.ReadAllText(path));
            timeManager.Load(saveFile.ElapsedTime);
            buildingManager.Load(saveFile.buildings);
            inventory.Load(saveFile.inventory);
            
            inventory.PrintInventory();
            
            Debug.Log($"Loaded from {path}.");
        }
    }
}