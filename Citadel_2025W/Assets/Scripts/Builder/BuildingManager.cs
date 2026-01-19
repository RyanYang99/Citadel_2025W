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
        public bool IsReady { get; private set; }

        public event Action OnBuildingChanged;
        public event Action OnPlacedBuildingChanged;
        private int _currentIndex = -1;
        
        [SerializeField] private LayerMask groundLayer;

        [SerializeField] private BuildingMetaDataList buildings;
        [SerializeField] private Inventory inventory;

        [SerializeField] private SFXLooper SFXLooper;
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
        
        public static bool OverLockedTilesOrBuildings(BoxCollider boxCollider)
        {
            Vector3 center = boxCollider.transform.TransformPoint(boxCollider.center),
                    half = Vector3.Scale(boxCollider.size, boxCollider.transform.lossyScale) * 0.5f;
            half.y = 1000f;
            
            foreach (Collider _collider in Physics.OverlapBox(center, half, Quaternion.identity))
            {
                if (_collider.TryGetComponent(out LockedTile lockedTile) && lockedTile.Locked)
                    return true;
                
                if (_collider.CompareTag(Tags.Building)) 
                    return true;
            }

            return false;
        }

        private void Start()
        {
            IsReady = true;
        }

        public int GetPlacedCount(string uniqueName)
        {
            int count = 0;

            foreach (var placed in PlacedBuildings)
            {
                if (placed.UniqueName == uniqueName)
                    count++;
            }

            return count;
        }

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
            OnPlacedBuildingChanged?.Invoke();
        }

        private void RemovePlacedBuilding(GameObject _gameObject)
        {
            PlacedBuilding placedBuilding = FindPlacedBuilding(_gameObject);
            if (placedBuilding != null)
            {
                PlacedBuildings.Remove(placedBuilding);
            }
                OnPlacedBuildingChanged?.Invoke();
            SFXLooper.PlayOneSecond();

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

        public bool CanBuild(BuildingMetaData meta)
        {
            if (meta == null)
                return false;

            if (meta.maxBuildCount < 0)
                return true;

            int current = GetPlacedCount(meta.uniqueName);
            return current < meta.maxBuildCount;
        }

        //설치 전용
        public void PlaceBuilding(Vector3 position)
        {
            if (CurrentBuilding == null)
                return;

            PlaceInternal(
                CurrentBuilding,
                position,
                CurrentBuilding.prefab.transform.rotation
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

        /*
        public bool CanPlaceBuildingAt(Vector3 position)
        {
            foreach (PlacedBuilding placed in PlacedBuildings)
            {
                if (placed.Position == position)
                    return false;
            }

            return true;
        }
        */

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