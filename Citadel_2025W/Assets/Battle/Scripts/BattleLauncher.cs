using Citadel;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleLauncher : MonoBehaviour
{
    [Header("Refs (MainScene)")]
    [SerializeField] private Inventory inventory;

    public void EnterBattle(int zoneId, int castleLevel)
    {
        if (inventory == null)
            inventory = FindFirstObjectByType<Inventory>();

        int soldierCount = 0;

        if (inventory != null)
            soldierCount = inventory.GetAmount(Item.Soldier);
        else
            Debug.LogError("[BattleLauncher] Inventory not found in MainScene.");

        BattleSession.SetRequest(new BattleSession.BattleRequest
        {
            zoneId = zoneId,
            castleLevel = castleLevel,
            playerSoldierCount = soldierCount
        });

        SceneManager.LoadScene("BattleScene");
    }
}