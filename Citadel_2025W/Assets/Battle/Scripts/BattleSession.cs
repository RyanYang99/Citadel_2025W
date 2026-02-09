public static class BattleSession
{
    public struct BattleRequest
    {
        public int zoneId;
        public int castleLevel;

        // 메인에서 넘어오는 총 병력수(Soldier)
        public int playerSoldierCount;
    }

    private static bool _hasRequest;
    private static BattleRequest _request;

    public static void SetRequest(BattleRequest req)
    {
        _request = req;
        _hasRequest = true;
    }

    public static bool TryGetRequest(out BattleRequest req)
    {
        req = _request;
        return _hasRequest;
    }

    public static void Clear()
    {
        _hasRequest = false;
        _request = default;
    }
}