using System.Collections.Generic;
using UnityEngine;

namespace Citadel
{
    //Scale -> size , 회전 반영, 점유 셀 계산하기 
    public static class GridFootprintUtil
    {
        public static Vector2Int GetSizeFromScale(Transform prefabTransform)
        {
            // scale 기반
            int sx = Mathf.Max(1, Mathf.RoundToInt(prefabTransform.localScale.x));
            int sz = Mathf.Max(1, Mathf.RoundToInt(prefabTransform.localScale.z));
            return new Vector2Int(sx, sz);
        }

        // rot90: 0,1,2,3 (0/90/180/270)
        public static int YawToRot90(float yawDeg)
        {
            int rot = Mathf.RoundToInt(yawDeg / 90f) % 4;
            if (rot < 0) rot += 4;
            return rot;
        }

        public static Vector2Int ApplyRotationToSize(Vector2Int size, int rot90)
        {
            // 90/270이면 swap
            return (rot90 % 2 == 0) ? size : new Vector2Int(size.y, size.x);
        }

        public static IEnumerable<Vector2Int> GetOccupiedCells_Center(Vector2Int centerCell, Vector2Int size, int rot90)
        {
            Vector2Int rotated = ApplyRotationToSize(size, rot90);

            int halfX = rotated.x / 2;
            int halfZ = rotated.y / 2;

            // 홀수 size면 대칭. (3->half=1)
            for (int z = -halfZ; z <= halfZ; z++)
                for (int x = -halfX; x <= halfX; x++)
                    yield return centerCell + new Vector2Int(x, z);
        }

    }
}
