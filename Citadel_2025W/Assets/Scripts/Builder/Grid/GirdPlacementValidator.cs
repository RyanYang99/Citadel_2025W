using UnityEngine;

namespace Citadel
{
    //설치 가능 / 불가능 계산
    public sealed class GridPlacementValidator : MonoBehaviour
    {
        [SerializeField] private GridService grid;
        [SerializeField] private GridOccupancy occupancy;
        [SerializeField] private FootprintDatabase footprintDB; // optional

        public bool TryGetFootprint(BuildingMetaData meta, out Vector2Int size, out GridPivot pivot)
        {
            pivot = GridPivot.BottomLeft;

            if (meta == null || meta.prefab == null)
            {
                size = Vector2Int.one;
                return false;
            }

            // 오버라이드 우선
            if (footprintDB != null && footprintDB.TryGet(meta.uniqueName, out var ov))
            {
                size = ov.size;
                pivot = ov.pivot;
                return true;
            }

            // 기본: scale 기반
            size = GridFootprintUtil.GetSizeFromScale(meta.prefab.transform);
            return true;
        }

        public bool CanPlace(BuildingMetaData meta, Vector3 snappedWorldPos, Quaternion rotation)
        {
            if (meta == null) return false;
            if (!TryGetFootprint(meta, out var size, out var pivot)) return false;

            Vector2Int centerCell = grid.WorldToCell(snappedWorldPos);
            int rot90 = GridFootprintUtil.YawToRot90(rotation.eulerAngles.y);

            foreach (var cell in GridFootprintUtil.GetOccupiedCells_Center(centerCell, size, rot90))
            {
                if (occupancy.IsOccupied(cell))
                    return false;
            }

            return true;
        }

    }
}
