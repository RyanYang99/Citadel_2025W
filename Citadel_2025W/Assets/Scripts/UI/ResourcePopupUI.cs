using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Citadel
{

public class ResourcePopupUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI amountText;

    [Header("Animation")]
    [SerializeField] private float moveUpDistance = 1.5f;
    [SerializeField] private float duration = 1.2f;

    private Vector3 startPos;
    private Vector3 targetPos;
    private Vector3 baseScale;
    private float time;

    private CanvasGroup canvasGroup;
    private Camera cam;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        cam = Camera.main;
    }

    public void Init(Sprite icon, int amount)
    {
            Debug.Log($"[Popup] amount = {amount}");
        iconImage.sprite = icon;
        amountText.text = $"+{amount}";

        startPos = transform.position;
        targetPos = startPos + Vector3.up * moveUpDistance;
        baseScale = transform.localScale;
    }

    private void Update()
    {
        if (cam != null)
            transform.forward = cam.transform.forward;

        time += Time.deltaTime;
        float t = time / duration;

        transform.position = Vector3.Lerp(startPos, targetPos, t);

        float scale = Mathf.Lerp(1.3f, 1f, t);
        transform.localScale = baseScale * scale;

        if (canvasGroup != null)
            canvasGroup.alpha = 1f - t;

        if (t >= 1f)
            Destroy(gameObject);
    }
}

}