using System;
using UnityEngine;

namespace Citadel
{
    [Serializable]
    public class SerializableBuilding
    {
        public string uniqueName;
        public CleanVector3 position, rotation;

        public SerializableBuilding(string uniqueName, Vector3 position, Vector3 rotation)
        {
            this.uniqueName = uniqueName;
            this.position = new CleanVector3(position);
            this.rotation = new CleanVector3(rotation);
        }
    }
}