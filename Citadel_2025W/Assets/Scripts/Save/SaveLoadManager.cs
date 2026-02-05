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
            public int groundLevel;
            
            public List<SerializableBuilding> buildings = new();
            public List<ItemAmount> inventory = new();
        }

        private TimeManager _timeManager;

        [Header("Building"), SerializeField] private BuildingMetaDataList buildingsReference;
        private BuildingManager _buildingManager;

        private Inventory _inventory;
        private GroundLevelManager _groundLevelManager;
        
        private string _path;

        protected override void Awake()
        {
            base.Awake();
            
            Ensure();
            _path = Application.persistentDataPath + "/save.json";
        }

        private void Ensure()
        {
            if (_timeManager == null)
                _timeManager = FindFirstObjectByType<TimeManager>();
            
            if (_buildingManager == null)
                _buildingManager = FindFirstObjectByType<BuildingManager>();
            
            if (_inventory == null)
                _inventory = FindFirstObjectByType<Inventory>();
            
            if (_groundLevelManager == null)
                _groundLevelManager = FindFirstObjectByType<GroundLevelManager>();
        }

        public void Save()
        {
            Ensure();
            
            SaveFile saveFile = new()
            {
                ElapsedTime = _timeManager.TimeElapsed,
                groundLevel = _groundLevelManager.CurrentLevel
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

            Ensure();
            
            SaveFile saveFile = JsonConvert.DeserializeObject<SaveFile>(File.ReadAllText(_path));
            _groundLevelManager.CurrentLevel = saveFile.groundLevel;
            _timeManager.Load(saveFile.ElapsedTime);
            _buildingManager.Load(saveFile.buildings);
            _inventory.Load(saveFile.inventory);
            
            _inventory.PrintInventory();
            
            Debug.Log($"Loaded from {_path}.");
        }
    }
}