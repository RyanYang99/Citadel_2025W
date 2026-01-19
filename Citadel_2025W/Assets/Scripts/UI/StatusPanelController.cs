using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Citadel
{
  

    public sealed class StatusPanelController : MonoBehaviour
    {
        [Header("Header UI")]
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text descText;

        [Header("Scroll Content")]
        [SerializeField] private Transform contentRoot;

        [Header("Prefabs")]
        [SerializeField] private TMP_Text sectionTitlePrefab;
        [SerializeField] private StatusRowView rowPrefab;

        [Header("Optional")]
        [SerializeField] private Button closeButton;
        [SerializeField] private RectTransform panelRect;


        private GameObject currentBuilding;
        private BuildingMetaData currentMeta;
        private Inventory inventory;

        private void Awake()
        {
            Debug.Log($"[StatusPanel] Awake on {name}", this);

            if (closeButton != null)
                closeButton.onClick.AddListener(Hide);

            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            // 구독 해제
            if (inventory != null)
                inventory.OnTick -= HandleTick;
        }

        public void BindInventory(Inventory inv)
        {
            if (inventory != null)
                inventory.OnTick -= HandleTick;

            inventory = inv;

            if (inventory != null)
                inventory.OnTick += HandleTick;
        }

        private void HandleTick()
        {
            if (!gameObject.activeSelf) return;
            Refresh();
        }

        public void Show(GameObject buildingRoot, BuildingMetaData meta)
        {
            Debug.Log("[StatusPanel] Show called");
            if (buildingRoot == null) return;

            currentBuilding = buildingRoot.transform.root.gameObject; 
            currentMeta = meta;

            gameObject.SetActive(true);
            Refresh();
        }


        public void Hide()
        {
            gameObject.SetActive(false);
            currentBuilding = null;
            currentMeta = null;
        }

        public void SnapToButton(RectTransform buttonRect, Vector2 offset)
        {
            if (buttonRect == null) return;

            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas == null) return;

            RectTransform canvasRect = canvas.transform as RectTransform;
            RectTransform panel = panelRect != null ? panelRect : (RectTransform)transform;

            // 버튼의 오른쪽 중간 지점을 기준점으로
            Vector3[] corners = new Vector3[4];
            buttonRect.GetWorldCorners(corners);
            Vector3 worldPoint = (corners[2] + corners[3]) * 0.5f; // 오른쪽 가운데

            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                worldPoint
            );
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                screenPoint,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                out Vector2 localPoint
            );
            panel.anchoredPosition = localPoint + offset;
        }


        public void Refresh()
        {
            Debug.Log("[StatusPanel] Refresh called");
            GameObject root = currentBuilding.transform.root.gameObject;
            Debug.Log($"[StatusPanel] target={currentBuilding.name}, root={root.name}");
            Debug.Log($"[StatusPanel] hasProducer={root.TryGetComponent<ItemProducer>(out _)} hasConsumer={root.TryGetComponent<ItemConsumer>(out _)}");



            if (currentBuilding == null) return;

            ClearContent();

            // Header
            if (nameText != null)
                nameText.text = currentMeta != null ? currentMeta.uniqueName : root.name;

            if (descText != null)
            {
                // meta에 건물 설명 있으면 사용하기 위하여 남겨놓음
                // descText.text = currentMeta.description;
                descText.text = "";
            }

            if (iconImage != null)
            {
                iconImage.sprite = (currentMeta != null) ? currentMeta.icon : null;
                iconImage.enabled = iconImage.sprite != null;
            }

            Debug.Log($"[StatusPanel] contentRoot={(contentRoot != null)} rowPrefab={(rowPrefab != null)} titlePrefab={(sectionTitlePrefab != null)}");

            //  Producer 
            if (root.TryGetComponent<ItemProducer>(out var producer))
            {
                AddSection("생산");
                AddRow("Ticks Needed", producer.TicksNeeded.ToString());

                if (producer.ItemsProduced != null && producer.ItemsProduced.Count > 0)
                {
                    foreach (var item in producer.ItemsProduced)
                        AddRow(item.item.ToString(), $"+{item.amount}");
                }

                AddRow("Range", producer.Range.ToString("0.##"));

                var provided = producer.RangeResourcesProvided;
                if (provided != null && provided.Count > 0)
                {
                    foreach (var (res, dur) in provided)
                        AddRow($"Provides ({res})", $"{dur} ticks");
                }
            }

            //  Consumer (현재/필요 표시)
            if (root.TryGetComponent<ItemConsumer>(out var consumer))
            {
                AddSection("필요");

                AddRow("Ready", consumer.AreItemsReady() ? "Yes" : "No");
                AddRow("Total Required", consumer.TotalRequiredResources.ToString());

                var ready = consumer.GetReadyResources();
                var readyRanges = new HashSet<RangeResource>();

                if (ready != null)
                {
                    foreach (var r in ready)
                    {
                        if (r.AnyRangeResource.HasValue)
                            readyRanges.Add(r.AnyRangeResource.Value);
                    }
                }

                // 아이템 요구량: 현재/필요 표시
                if (consumer.ItemsUsed != null && consumer.ItemsUsed.Count > 0)
                {
                    AddSection("Item Requirements");
                    foreach (var need in consumer.ItemsUsed)
                    {
                        int cur = consumer.GetCurrentAmount(need.item); 
                        int req = need.amount;

                        AddRow(need.item.ToString(), $"{cur} / {req}");
                    }
                }

                // 범위 자원 요구: Provided/Missing 표시
                if (consumer.RangeResourcesUsed != null && consumer.RangeResourcesUsed.Count > 0)
                {
                    AddSection("Range Requirements");
                    foreach (var rr in consumer.RangeResourcesUsed)
                    {
                        bool ok = readyRanges.Contains(rr);
                        AddRow(rr.ToString(), ok ? "Provided " : "Missing ");
                    }
                }
            }

            // 아무것도 없으면 안내
            if (contentRoot != null && contentRoot.childCount == 0)
            {
                AddSection("정보");
                AddRow("No data", "No producer/consumer components found.");
            }

           

        }

        private void ClearContent()
        {
            if (contentRoot == null) return;

            for (int i = contentRoot.childCount - 1; i >= 0; i--)
                Destroy(contentRoot.GetChild(i).gameObject);
        }

        private void AddSection(string title)
        {
            if (sectionTitlePrefab == null || contentRoot == null) return;
            var t = Instantiate(sectionTitlePrefab, contentRoot);
            t.text = title;
        }

        private void AddRow(string label, string value)
        {
            if (rowPrefab == null || contentRoot == null) return;
            var row = Instantiate(rowPrefab, contentRoot);
            row.Set(label, value);
        }
    }
}
