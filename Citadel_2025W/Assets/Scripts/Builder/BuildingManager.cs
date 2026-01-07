using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//실제 배치 / 회전 / 제거 / 데이터 관리
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
        public event Action OnBuildingChanged;
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

            if (_currentIndex == index) return;

            _currentIndex = index;

            OnBuildingChanged?.Invoke();
        
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

        private void PlaceInternal(
    BuildingMetaData meta,
    Vector3 position,
    Quaternion rotation)
        {
            position.y += meta.yOffset;

            foreach (PlacedBuilding placed in PlacedBuildings)
                if (placed.Position == position)
                    return;
            GameObject obj = Instantiate(meta.prefab, position, rotation);
            //초기화
          
            AddPlacedBuilding(
                new PlacedBuilding(
                    meta.uniqueName,
                    obj,
                    position,
                    rotation.eulerAngles
                )
            );
        }


        //설치 전용
        public void PlaceBuilding(Vector3 position)
        {
            if (CurrentBuilding == null)
                return;

            PlaceInternal(
                CurrentBuilding,
                position,
                Quaternion.identity
            );
        }

        //로드 전용 
        public void PlaceBuilding(string uniqueName, Vector3 position, Vector3 rotation)
        {
            BuildingMetaData meta =
                buildings.list.Find(bmd => bmd.uniqueName == uniqueName);

            if (meta == null)
                return;

            PlaceInternal(
                meta,
                position,
                Quaternion.Euler(rotation)
            );
        }


        //building 프리뷰 회전 설치
        public void PlaceBuilding(Vector3 position, Quaternion rotation)
        {
            if (CurrentBuilding == null)
                return;

            PlaceInternal(
                CurrentBuilding,
                position,
                rotation
            );
        }

        public bool CanPlaceBuildingAt(Vector3 position)
        {
            foreach (PlacedBuilding placed in PlacedBuildings)
            {
                if (placed.Position == position)
                    return false;
            }

            return true;
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