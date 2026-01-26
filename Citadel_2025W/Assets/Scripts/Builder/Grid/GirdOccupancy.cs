using System.Collections.Generic;
using UnityEngine;

namespace Citadel
{
    //PlaceBuildingÀ» ¼¿·Î Ä³½Ì 
    public sealed class GridOccupancy : MonoBehaviour
    {
        [SerializeField] private BuildingManager buildingManager;
        [SerializeField] private GridService grid;

        private readonly HashSet<Vector2Int> occupied = new();

        private void OnEnable()
        {
            if (buildingManager != null)
                buildingManager.OnPlacedBuildingChanged += Rebuild;
            Rebuild();
        }

        private void OnDisable()
        {
            if (buildingManager != null)
                buildingManager.OnPlacedBuildingChanged -= Rebuild;
        }

        public bool IsOccupied(Vector2Int cell) => occupied.Contains(cell);

        public void Rebuild()
        {
            Debug.Log($"occupied count: {occupied.Count}");

            occupied.Clear();
            if (buildingManager == null || grid == null) return;

            foreach (var pb in buildingManager.PlacedBuildings)
                occupied.Add(grid.WorldToCell(pb.Position));
        }
    }
}
