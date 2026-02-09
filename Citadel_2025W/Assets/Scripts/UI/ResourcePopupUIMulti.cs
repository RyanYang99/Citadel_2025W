using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Citadel
{
    public sealed class ResourcePopupUIMulti : MonoBehaviour
    {
        [System.Serializable]
        private class EntryRefs
        {
            public Image icon;
            public TMP_Text amountText;
        }

        [Header("UI")]
        [SerializeField] private Transform content;
        [SerializeField] private GameObject entryTemplate;

        [Header("Template Refs")]
        [SerializeField] private Image templateIcon;
        [SerializeField] private TMP_Text templateAmountText;

        [Header("Anim")]
        [SerializeField] private float moveUpDistance = 1f;
        [SerializeField] private float duration = 1f;

        private Vector3 _startPos;
        private Camera _cam;

        private void Awake()
        {
            _startPos = transform.position;
            _cam = Camera.main;

            if (entryTemplate != null)
                entryTemplate.SetActive(false);
        }

        private void LateUpdate()
        {
            if (_cam != null)
                transform.forward = _cam.transform.forward;
        }

        public void InitMany(List<(Sprite icon, int amount)> entries)
        {
            for (int i = content.childCount - 1; i >= 0; i--)
            {
                var child = content.GetChild(i).gameObject;
                if (child == entryTemplate) continue;
                Destroy(child);
            }

            foreach (var (icon, amount) in entries)
            {
                GameObject go = Instantiate(entryTemplate, content);
                go.SetActive(true);

                var iconImg = go.GetComponentInChildren<Image>(true);
                var texts = go.GetComponentsInChildren<TMP_Text>(true);

                if (iconImg != null) iconImg.sprite = icon;

                if (texts != null && texts.Length > 0)
                    texts[0].text = $"+{amount}";
            }

            StopAllCoroutines();
            StartCoroutine(MoveUpAndDestroy());
        }

        public void InitSingle(Sprite icon, int amount)
        {
            InitMany(new List<(Sprite, int)> { (icon, amount) });
        }

        private System.Collections.IEnumerator MoveUpAndDestroy()
        {
            float t = 0f;
            Vector3 endPos = _startPos + Vector3.up * moveUpDistance;

            while (t < duration)
            {
                t += Time.deltaTime;
                float a = Mathf.Clamp01(t / duration);
                transform.position = Vector3.Lerp(_startPos, endPos, a);
                yield return null;
            }

            Destroy(gameObject);
        }
    }
}
