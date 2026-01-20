using UnityEngine;

namespace Citadel
{
    public sealed class GridService : MonoBehaviour
    {
        [SerializeField] private float cellSize = 1f;
        [SerializeField] private Vector3 origin = Vector3.zero;

        public float CellSize => cellSize;

        public Vector2Int WorldToCell(Vector3 world)
        {
            Vector3 local = world - origin;
            int x = Mathf.FloorToInt(local.x / cellSize);
            int z = Mathf.FloorToInt(local.z / cellSize);
            return new Vector2Int(x, z);
        }

        public Vector3 CellToWorldCenter(Vector2Int cell, float y)
        {
            float x = origin.x + cell.x * cellSize + cellSize * 0.5f;
            float z = origin.z + cell.y * cellSize + cellSize * 0.5f;
            return new Vector3(x, y, z);
        }

        public Vector3 SnapToCellCenter(Vector3 world)
        {
            var cell = WorldToCell(world);
            return CellToWorldCenter(cell, world.y);
        }
    }
}
