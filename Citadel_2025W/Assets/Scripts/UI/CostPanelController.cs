using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Citadel
{
    public sealed class CostPanelController : MonoBehaviour
    {
        private BuildingManager _buildingManager;
        private readonly List<CostItemController> _costItemControllers = new();
        
        [Header("Cost"), SerializeField] private TMP_Text costText;
        [SerializeField] private GameObject costPart, costItem;

        private void Awake() => _buildingManager = FindAnyObjectByType<BuildingManager>();

        private void OnEnable()
        {
            _buildingManager.OnBuildingChanged += OnBuildingChanged;
            OnBuildingChanged();
        }

        private void OnDisable()
        {
            _buildingManager.OnBuildingChanged -= OnBuildingChanged;
            Destroy();
        }

        private void OnBuildingChanged()
        {
            Destroy();
            
            if (_buildingManager.CurrentBuilding != null && _buildingManager.CurrentBuilding.costItems.Length > 0)
            {
                costText.gameObject.SetActive(true);
                
                foreach (ItemAmount itemAmount in _buildingManager.CurrentBuilding.costItems)
                {
                    CostItemController costItemController = Instantiate(costItem, costPart.transform).GetComponent<CostItemController>();
                    _costItemControllers.Add(costItemController);
                    
                    costItemController.Initialize(itemAmount);
                }
            }
        }

        private void Destroy()
        {
            costText.gameObject.SetActive(false);
            
            foreach (CostItemController costItemController in _costItemControllers)
                Destroy(costItemController.gameObject);
            
            _costItemControllers.Clear();
        }
    }
}