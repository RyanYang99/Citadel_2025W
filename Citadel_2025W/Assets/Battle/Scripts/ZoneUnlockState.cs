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
}