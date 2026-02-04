namespace Citadel
{
    public enum BuildingCategory
    {
        Tile,
        Road,
        Building,
        Function
    }

    public enum BuildingSubCategory // 건물 종류 추가 
    {
        None, // Building이 아닐 경우
        House, // 시민집
        Castle, // 영주 성
        Warehouse, // 벌목소 
        Well, // 우물
        Froge, // 대장간
        Quarry, // 채석상
        Market // 노점상(은행)
    }
}