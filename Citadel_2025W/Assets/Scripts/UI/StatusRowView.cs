using TMPro;
using UnityEngine;

namespace Citadel
{
    public sealed class StatusRowView : MonoBehaviour
    {
        [SerializeField] private TMP_Text labelText;
        [SerializeField] private TMP_Text valueText;

        public void Set(string label, string value)
        {
            if (labelText) labelText.text = label;
            if (valueText) valueText.text = value;
        }
    }
}
