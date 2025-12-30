using UnityEngine;
using UnityEngine.UI;

namespace Citadel
{
    public sealed class BuildItemButton : MonoBehaviour
    {
        public int index;
        public BuildingManager buildingManager;
        public Image icon;

        public void Init(int idx, BuildingManager manager, Sprite sprite)
        {
            index = idx;
            buildingManager = manager;
            icon.sprite = sprite;
        }

        public void OnClick()
        {
            buildingManager.SelectBuilding(index);
        }
    }
}