using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Citadel
{
    public sealed class StatusPanelController : MonoBehaviour
    {
        private GameObject _currentBuilding;
        private BuildingMetaData _currentMeta;

        private RectTransform _buttonRect;
        private Vector2 _offset;

        private ItemProducer _itemProducer;
        private SatisfactionProvider _satisfactionProvider;

        private readonly List<ConsumerStatusItemController> _consumerItems = new();
        private readonly List<ProducerStatusItemController> _producerItems = new();
        
        [SerializeField] private Inventory inventory;
        [SerializeField] private Canvas canvas;
        
        [Header("Header UI"), SerializeField] private TMP_Text nameText;

        [Header("Scroll Content"), SerializeField] private Transform contentRoot;

        [Header("Prefabs")]
        [SerializeField] private TMP_Text sectionTitlePrefab;
        [SerializeField] private StatusRowView rowPrefab;

        [Header("Optional"), SerializeField] private RectTransform panelRect;
        
        [Header("Viewport Consumer")]
        [SerializeField] private TMP_Text consumerText;
        [SerializeField] private GameObject consumerPart, consumerItem;

        [Header("Viewport Producer")]
        [SerializeField] private TMP_Text producerText, producerTicksLeftText;
        [SerializeField] private GameObject producerPart, producerItem;
        
        [Header("Viewport OneTime")]
        [SerializeField] private TMP_Text oneTimeText;
        [SerializeField] private GameObject oneTimePart;
        
        [Header("Viewport Satisfaction")]
        [SerializeField] private TMP_Text satisfactionText, satisfactionStatusText;
        [SerializeField] private GameObject satisfactionPart;
        
        private void OnEnable() => inventory.OnTick += OnTick;

        private void LateUpdate() => SnapToButton(_buttonRect, _offset);

        private void OnDisable() => inventory.OnTick -= OnTick;

        private void OnTick()
        {
            if (!gameObject.activeSelf)
                return;
            
            Refresh();
        }
        
        private void Refresh()
        {
            foreach (ConsumerStatusItemController buildingStatusPanelItemController in _consumerItems)
                buildingStatusPanelItemController.Refresh();
            
            foreach (ProducerStatusItemController producerStatusItemController in _producerItems)
                producerStatusItemController.Refresh();
            
            RefreshProducerTicksNeededText();
            RefreshSatisfactionStatusText();
        }

        private ConsumerStatusItemController CreateConsumerStatusItemController(GameObject parent)
        {
            ConsumerStatusItemController consumerStatusItemController = Instantiate(consumerItem, parent.transform).GetComponent<ConsumerStatusItemController>();
            _consumerItems.Add(consumerStatusItemController);

            return consumerStatusItemController;
        }

        private ProducerStatusItemController CreateProducerStatusItemController(GameObject parent)
        {
            ProducerStatusItemController producerStatusItemController = Instantiate(producerItem, parent.transform).GetComponent<ProducerStatusItemController>();
            _producerItems.Add(producerStatusItemController);

            return producerStatusItemController;
        }
        
        public void Show(GameObject buildingRoot, BuildingMetaData meta)
        {
            if (buildingRoot == null)
                return;

            _currentBuilding = buildingRoot.transform.root.gameObject; 
            _currentMeta = meta;

            gameObject.SetActive(true);
            
            nameText.text = _currentMeta != null ? _currentMeta.uniqueName : buildingRoot.name;
            
            if (_currentBuilding.TryGetComponent(out ItemConsumer itemConsumer))
            {
                consumerText.gameObject.SetActive(true);

                foreach (ItemAmount itemAmount in itemConsumer.ItemsUsed)
                    CreateConsumerStatusItemController(consumerPart).Initialize(itemConsumer, itemAmount, RangeResource.None);

                foreach (RangeResource rangeResource in itemConsumer.RangeResourcesUsed)
                    CreateConsumerStatusItemController(consumerPart).Initialize(itemConsumer, new ItemAmount(), rangeResource);

                if (_currentBuilding.TryGetComponent(out _satisfactionProvider))
                {
                    satisfactionText.gameObject.SetActive(true);
                    satisfactionStatusText.gameObject.SetActive(true);
                    RefreshSatisfactionStatusText();

                    foreach (ItemAmount itemAmount in itemConsumer.SatisfactionItemUsed)
                        CreateConsumerStatusItemController(satisfactionPart).Initialize(itemConsumer, itemAmount, RangeResource.None);
                    
                    foreach (RangeResource rangeResource in itemConsumer.SatisFactionRangeResourceUsed)
                        CreateConsumerStatusItemController(satisfactionPart).Initialize(itemConsumer, new ItemAmount(), rangeResource);
                }
            }

            if (_currentBuilding.TryGetComponent(out _itemProducer))
            {
                producerText.gameObject.SetActive(true);
                
                producerTicksLeftText.gameObject.SetActive(true);
                RefreshProducerTicksNeededText();

                foreach (ItemAmount itemAmount in _itemProducer.ItemsProduced)
                    CreateProducerStatusItemController(producerPart).Initialize(_itemProducer,
                                                                                itemAmount,
                                                                                RangeResource.None, 
                                                                                0);

                foreach ((RangeResource resource, int tickDuration) rangeResource in _itemProducer.RangeResourcesProvided)
                    CreateProducerStatusItemController(producerPart).Initialize(_itemProducer,
                                                                                new ItemAmount(),
                                                                                rangeResource.resource,
                                                                                rangeResource.tickDuration);

                if (_itemProducer.OneTimeItemsProduced.Count > 0)
                {
                    oneTimeText.gameObject.SetActive(true);

                    foreach (ItemAmount itemAmount in _itemProducer.OneTimeItemsProduced)
                        CreateProducerStatusItemController(oneTimePart).Initialize(_itemProducer,
                                                                                   itemAmount,
                                                                                   RangeResource.None,
                                                                                   0);
                }
            }
        }

        private void RefreshProducerTicksNeededText()
        {
            if (_itemProducer != null)
                producerTicksLeftText.text = $"남은 시간: {_itemProducer.Ticks + 1} / {_itemProducer.TicksNeeded}";
        }

        private void RefreshSatisfactionStatusText()
        {
            if (_satisfactionProvider != null)
            {
                float total = _satisfactionProvider.Satisfaction * 100f;
                float internalPt = _satisfactionProvider.InternalScore;
                float externalPt = _satisfactionProvider.ExternalScore;

                // ex. 만족도: 85% (내실 50 + 환경 35)
                satisfactionStatusText.text = $"만족도: {total:F0}% <size=70%>(내실 {internalPt:F0} + 환경 {externalPt:F0})</size>";
            }
        }

        public void Hide()
        {
            _currentBuilding = null;
            _currentMeta = null;
            _itemProducer = null;
            _satisfactionProvider = null;
            gameObject.SetActive(false);
            consumerText.gameObject.SetActive(false);
            producerText.gameObject.SetActive(false);
            producerTicksLeftText.gameObject.SetActive(false);
            oneTimeText.gameObject.SetActive(false);
            satisfactionText.gameObject.SetActive(false);
            satisfactionStatusText.gameObject.SetActive(false);
            
            foreach (ConsumerStatusItemController consumerStatusItemController in _consumerItems)
                Destroy(consumerStatusItemController.gameObject);
            _consumerItems.Clear();
            
            foreach (ProducerStatusItemController producerStatusItemController in _producerItems)
                Destroy(producerStatusItemController.gameObject);
            _producerItems.Clear();
        }

        public void SnapToButton(RectTransform buttonRect, Vector2 offset)
        {
            if (buttonRect == null)
                return;

            _buttonRect = buttonRect;
            _offset = offset;

            RectTransform canvasRect = canvas.transform as RectTransform;
            RectTransform panel = panelRect != null ? panelRect : (RectTransform)transform;
            
            Vector3[] corners = new Vector3[4];
            buttonRect.GetWorldCorners(corners);
            Vector3 worldPoint = (corners[2] + corners[3]) * 0.5f;

            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                                                                          worldPoint);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect,
                                                                    screenPoint,
                                                                    canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                                                                    out Vector2 localPoint);
            panel.anchoredPosition = localPoint + offset;
        }
    }
}