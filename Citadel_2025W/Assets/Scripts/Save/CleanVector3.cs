using System;
using UnityEngine;

namespace Citadel
{
    [Serializable]
    public class CleanVector3
    {
        public float x, y, z;

        public CleanVector3(Vector3 vector3)
        {
            x = vector3.x;
            y = vector3.y;
            z = vector3.z;
        }

        public Vector3 ToVector3() => new(x, y, z);
    }
}