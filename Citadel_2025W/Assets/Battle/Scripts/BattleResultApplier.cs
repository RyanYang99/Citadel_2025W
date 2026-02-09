using UnityEngine;
using Citadel;

public sealed class BattleResultApplier : MonoBehaviour
{
    private void Start()
    {
        if (BattleSession.TryGetResult(out var res))
        {
            Debug.Log($"[MainScene] BattleResult: victory={res.victory}, zoneId={res.zoneId}");

            if (res.victory)
                ZoneUnlockState.Add(res.zoneId);

            BattleSession.ClearResult();
        }
        ApplyAllUnlockedZones();
    }

    private void ApplyAllUnlockedZones()
    {
        int unlocked = 0;

        for (int i = LockedTile.LockedTiles.Count - 1; i >= 0; i--)
        {
            var t = LockedTile.LockedTiles[i];
            if (t == null) continue;

            if (ZoneUnlockState.IsUnlocked(t.ZoneId))
            {
                t.Locked = false;
                unlocked++;
            }
        }

        Debug.Log($"[MainScene] Applied unlocked zones. count={unlocked}");
    }
}