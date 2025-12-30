using UnityEngine;
using TMPro;




namespace Citadel
{
    public class ResourceUI : MonoBehaviour
    {
        [SerializeField] Inventory inventory;
        [SerializeField] Item item;
        [SerializeField] TMP_Text amountText;

        void OnEnable()
        {
            Refresh();
        }

        void Start()
        {
            Refresh();
        }

        public void Refresh()
        {
            int amount = inventory.GetAmount(item);
            amountText.text = amount.ToString();

            Debug.Log($"{item} amount = {amount}");
        }
    }
}