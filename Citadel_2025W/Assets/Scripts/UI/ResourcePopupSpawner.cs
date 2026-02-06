using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Citadel;

public class ResourcePopupSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject popupPrefab; //멀티 프리팹 참조
    [SerializeField] private ItemIconTable iconTable;

    [Header("Spawn Option")]
    [SerializeField] private Vector3 spawnOffset = Vector3.up * 2f;

    [Header("Batching")]
    [Tooltip("시간 안에 들어온 생산 이벤트는 한 팝업으로 묶음")]
    [SerializeField] private float burstWindow = 0.08f;

    [Tooltip("겹침 방지용 랜덤 위치")]
    [SerializeField] private float horizontalJitter = 0.2f;

    private ItemProducer producer;

    // 버퍼: 같은 아이템은 합산
    private readonly Dictionary<Item, int> _buffer = new();
    private Coroutine _flushCo;

    private void Awake()
    {
        producer = GetComponent<ItemProducer>();
        if (iconTable == null)
            iconTable = FindFirstObjectByType<ItemIconTable>();
    }

    private void OnEnable()
    {
        if (producer != null)
            producer.OnItemProduced += OnItemProduced;
    }

    private void OnDisable()
    {
        if (producer != null)
            producer.OnItemProduced -= OnItemProduced;
    }

    private void OnItemProduced(ItemAmount itemAmount)
    {
        if (popupPrefab == null || iconTable == null)
            return;

        if (_buffer.ContainsKey(itemAmount.item))
            _buffer[itemAmount.item] += itemAmount.amount;
        else
            _buffer[itemAmount.item] = itemAmount.amount;

        // 타이머 리셋
        if (_flushCo != null) StopCoroutine(_flushCo);
        _flushCo = StartCoroutine(FlushAfterDelay());
    }

    private IEnumerator FlushAfterDelay()
    {
        yield return new WaitForSecondsRealtime(burstWindow);

        // 팝업 스폰 위치
        Vector3 basePos = transform.position + spawnOffset;
        Vector2 rand = Random.insideUnitCircle * horizontalJitter;
        Vector3 spawnPos = basePos + new Vector3(rand.x, 0f, rand.y);

        var popup = Instantiate(popupPrefab, spawnPos, Quaternion.identity).GetComponent<ResourcePopupUIMulti>();

        // entries 구성
        var entries = new List<(Sprite icon, int amount)>();
        foreach (var kv in _buffer)
        {
            var pair = iconTable.Get(kv.Key);
            if (pair == null) continue;
            entries.Add((pair.icon, kv.Value));
        }

        // 1개면 단일, 2개 이상 멀티
        if (entries.Count == 1)
            popup.InitSingle(entries[0].icon, entries[0].amount);
        else
            popup.InitMany(entries);

        _buffer.Clear();
        _flushCo = null;
    }
}
