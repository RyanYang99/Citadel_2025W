using UnityEngine;

public class Tile
{
    private int posX;
    private int posY;
    private Facility facility;
    private GameObject tileObj;

    // === 프로퍼티 추가 ===
    public int PosX => posX;
    public int PosY => posY;

    // tileObj는 읽기/쓰기 모두 가능하게
    public GameObject TileObj
    {
        get { return tileObj; }
        set { tileObj = value; }
    }

    // Facility도 필요하다면 외부에서 읽기 가능하게
    public Facility Facility => facility;

    public Tile(int y, int x, GameObject _obj)
    {
        posX = x;
        posY = y;
        tileObj = _obj;
        facility = new NullFacility();
    }

    public bool ChangeFacility(Facility _facility)
    {
        if (facility.isNull())
        {
            _facility.CreateFacility(tileObj);
            facility = _facility;
            return true;
        }
        return false;
    }

    public void GetInformation()
    {
        Debug.Log($"posX = {posX}, posY = {posY}");
    }
}