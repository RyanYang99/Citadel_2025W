using UnityEngine;
using Citadel;

public sealed class BattleResultApplier : MonoBehaviour
{
    private void Start()
    {
        if (!BattleSession.TryGetResult(out var res)) return;

        Debug.Log($"[MainScene] BattleResult: victory={res.victory}, zoneId={res.zoneId}");

        if (res.victory)
        {
            // zoneId가 같은 타일 전부 해제
            for (int i = LockedTile.LockedTiles.Count - 1; i >= 0; i--)
            {
                var t = LockedTile.LockedTiles[i];
                if (t == null) continue;

                if (t.ZoneId == res.zoneId)
                    t.Locked = false;
            }
        }

        BattleSession.ClearResult();
    }
}
