using UnityEngine;

public class NullFacility : Facility
{
    public override void CreateFacility(GameObject tileObj)
    {
        // 아무것도 설치하지 않음
    }

    public override bool isNull()
    {
        return true;
    }
}