using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Citadel
{
    public sealed class BattleConfirmPopup : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private Button startButton;
        [SerializeField] private Button closeButton;

        private int _zoneId;
        private int _tileLevel;
        private BattleLauncher _battleLauncher;

        private void Awake()
        {
            startButton.onClick.AddListener(OnClickStart);
            closeButton.onClick.AddListener(Hide);
            gameObject.SetActive(false);
        }

        public void Show(BattleLauncher battleLauncher, int zoneId, int tileLevel)
        {
            if (!ZoneUnlockState.IsNextAllowed(zoneId))
                return;

            _battleLauncher = battleLauncher;
            _zoneId = zoneId;
            _tileLevel = tileLevel;

            levelText.text = $"Tile Level: {_tileLevel}";
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnClickStart()
        {
            Hide();
            _battleLauncher.EnterBattle(_zoneId, _tileLevel);
        }
    }
}
