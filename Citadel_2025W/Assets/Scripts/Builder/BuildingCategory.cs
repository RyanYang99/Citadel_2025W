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
        House,
        Castle,
        Warehouse,
        Well
    }
}