using UnityEngine;
using UnityEngine.UI;

namespace Citadel
{
    public sealed class BuildItemButton : MonoBehaviour
    {
        public int index;
        public BuildingManager buildingManager;
        public BuildingPlacer buildingPlacer;
        public Image icon;

        public void Init(int idx, BuildingManager manager, BuildingPlacer placer,Sprite sprite)
        {
            index = idx;
            buildingManager = manager;
            icon.sprite = sprite;
            buildingPlacer = placer;
        }

        public void OnClick()
        {
            buildingManager.SelectBuilding(index);
            buildingPlacer.SetBuildMode();
        }
    }
}