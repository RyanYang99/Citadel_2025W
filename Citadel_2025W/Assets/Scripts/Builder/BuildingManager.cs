using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Citadel
{
    public class PlacedBuilding
    {
        public string UniqueName;
        public Vector3 Position, Rotation;
        public GameObject _GameObject;

        public PlacedBuilding(string uniqueName, GameObject gameObject, Vector3 position, Vector3 rotation)
        {
            UniqueName = uniqueName;
            _GameObject = gameObject;
            Position = position;
            Rotation = rotation;
        }
    }
    
    public sealed class BuildingManager : MonoBehaviour
    {
        private int _currentIndex = -1;

        [SerializeField] private BuildingMetaDataList buildings;
        public BuildingMetaDataList Buildings
        {
            get => buildings;
            private set => buildings = value;
        }
        
        public BuildingMetaData CurrentBuilding
        {
            get
            {
                if (_currentIndex < 0 || _currentIndex >= Buildings.list.Count)
                    return null;

                return Buildings.list[_currentIndex];
            }
        }
        
        public readonly List<PlacedBuilding> PlacedBuildings = new();
        
        public void SelectBuilding(int index)
        {
            if (index < 0 || index >= Buildings.list.Count)
                return;

            _currentIndex = index;
        }
        
        public PlacedBuilding FindPlacedBuilding(GameObject _gameObject) =>
            PlacedBuildings.Find(placedBuilding => placedBuilding._GameObject == _gameObject);
        
        private void AddPlacedBuilding(PlacedBuilding placedBuilding)
        {
            if (FindPlacedBuilding(placedBuilding._GameObject) != null)
            {
                Debug.LogError("PlacedBuilding already exists.");
                return;
            }

            PlacedBuildings.Add(placedBuilding);
        }

        private void RemovePlacedBuilding(GameObject _gameObject)
        {
            PlacedBuilding placedBuilding = FindPlacedBuilding(_gameObject);
            if (placedBuilding != null)
                PlacedBuildings.Remove(placedBuilding);
        }
        
        public void PlaceBuilding(Vector3 position)
        {
            BuildingMetaData buildingMetaData = CurrentBuilding;
            if (buildingMetaData == null)
                return;

            position.y += buildingMetaData.yOffset;

            foreach (PlacedBuilding placedBuilding in PlacedBuildings)
                if (placedBuilding.Position == position)
                    return;

            GameObject _gameObject = Instantiate(buildingMetaData.prefab, position, Quaternion.identity);
            AddPlacedBuilding(new PlacedBuilding(buildingMetaData.uniqueName, _gameObject, position, _gameObject.transform.rotation.eulerAngles));
        }

        public void PlaceBuilding(string uniqueName, Vector3 position, Vector3 rotation)
        {
            BuildingMetaData buildingMetaData = buildings.list.Find(bmd => bmd.uniqueName == uniqueName);
            if (buildingMetaData == null)
                return;
            
            AddPlacedBuilding(new PlacedBuilding(uniqueName, Instantiate(buildingMetaData.prefab, position, Quaternion.Euler(rotation)),position, rotation));
        }

        public void RotateBuilding(GameObject _gameObject)
        {
            _gameObject.transform.Rotate(Vector3.up, 90f);
            
            PlacedBuilding placedBuilding = FindPlacedBuilding(_gameObject);
            if (placedBuilding != null)
                placedBuilding.Rotation = _gameObject.transform.eulerAngles;
        }

        public void RemoveBuilding(GameObject _gameObject)
        {
            RemovePlacedBuilding(_gameObject);
            Destroy(_gameObject);
        }

        public void RemoveAllBuildings()
        {
            List<PlacedBuilding> copy = new(PlacedBuildings);
            
            foreach (PlacedBuilding placedBuilding in copy)
                RemoveBuilding(placedBuilding._GameObject);
        }

        public void Load(List<SerializableBuilding> serializableBuildings)
        {
            RemoveAllBuildings();
            
            foreach (SerializableBuilding serializableBuilding in serializableBuildings)
                PlaceBuilding(serializableBuilding.uniqueName, serializableBuilding.position.ToVector3(), serializableBuilding.rotation.ToVector3());
        }
    }
}