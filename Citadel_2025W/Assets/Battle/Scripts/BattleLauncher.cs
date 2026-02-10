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
        {
            inventory = FindFirstObjectByType<Inventory>();
            Debug.Log($"[BattleLauncher] inventory found? {(inventory != null)}", this);
        }

        int soldierCount = 0;

        if (inventory != null)
            soldierCount = inventory.GetAmount(Item.Soldier);
        else
            Debug.LogError("[BattleLauncher] Inventory not found in MainScene.");

        Debug.Log($"[BattleLauncher] zoneId={zoneId}, castleLevel={castleLevel}, soldierCount={soldierCount}", this);

        BattleSession.SetRequest(new BattleSession.BattleRequest
        {
            zoneId = zoneId,
            castleLevel = castleLevel,
            playerSoldierCount = soldierCount
        });

        SceneManager.LoadScene("BattleScene");
    }


    public void ExitBattleToMain()
    {
        SceneManager.LoadScene("MainScene");
    }

}