using System.Collections.Generic;

public static class ZoneUnlockState
{
    public static readonly HashSet<int> UnlockedZones = new();

    public static void Add(int zoneId)
    {
        if (zoneId < 0) return;
        UnlockedZones.Add(zoneId);
    }

    public static bool IsUnlocked(int zoneId) => UnlockedZones.Contains(zoneId);

    public static int MaxUnlockedLevel()
    {
        int max = 0;
        foreach (var id in UnlockedZones)
            if (id > max) max = id;
        return max;
    }
    public static int NextAllowedLevel() => MaxUnlockedLevel() + 1;

    public static bool IsNextAllowed(int zoneId)=>zoneId==NextAllowedLevel();
}