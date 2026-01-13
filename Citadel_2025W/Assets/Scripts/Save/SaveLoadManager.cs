using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace Citadel
{
    public sealed class SaveLoadManager : PersistentSingleton<SaveLoadManager>
    {
        [Serializable]
        private class SaveFile
        {
            public DateTime ElapsedTime;
            
            public List<SerializableBuilding> buildings = new();
            public List<ItemAmount> inventory = new();
        }

        private TimeManager _timeManager;

        [Header("Building"), SerializeField] private BuildingMetaDataList buildingsReference;
        private BuildingManager _buildingManager;

        private Inventory _inventory;
        
        private string _path;

        protected override void Awake()
        {
            _timeManager = FindFirstObjectByType<TimeManager>();
            _buildingManager = FindFirstObjectByType<BuildingManager>();
            _inventory = FindFirstObjectByType<Inventory>();
            
            _path = Application.persistentDataPath + "/save.json";
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
                Save();

            if (Input.GetKeyDown(KeyCode.F2))
                Load();
        }

        public void Save()
        {
            SaveFile saveFile = new()
            {
                ElapsedTime = _timeManager.TimeElapsed
            };

            foreach (PlacedBuilding placedBuilding in _buildingManager.PlacedBuildings)
                saveFile.buildings.Add(new SerializableBuilding(placedBuilding.UniqueName, placedBuilding.Position, placedBuilding.Rotation));

            saveFile.inventory = _inventory.ToList();
            
            File.WriteAllText(_path, JsonConvert.SerializeObject(saveFile, Formatting.Indented));
            
            Debug.Log($"Saved to {_path}.");
        }

        public void Load()
        {
            if (!File.Exists(_path))
                return;

            SaveFile saveFile = JsonConvert.DeserializeObject<SaveFile>(File.ReadAllText(_path));
            _timeManager.Load(saveFile.ElapsedTime);
            _buildingManager.Load(saveFile.buildings);
            _inventory.Load(saveFile.inventory);
            
            _inventory.PrintInventory();
            
            Debug.Log($"Loaded from {_path}.");
        }
    }
}