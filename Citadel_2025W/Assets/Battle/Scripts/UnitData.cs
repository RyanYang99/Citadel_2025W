using UnityEngine;

public enum UnitType
{
    Infantry,
    Archer,
    Shield
}

[CreateAssetMenu(menuName = "Battle/Unit Data", fileName = "UnitData_")]
public class UnitData : ScriptableObject
{
    [Header("ID")]
    public UnitType type;

    [Header("Prefab")]
    public GameObject prefab;

    [Header("Stats")]
    public float maxHp = 100f;
    public float damage = 10f;
    public float range = 1.5f;
    public float moveSpeed = 3.5f;
    public float attackInterval = 1.0f;
}