using UnityEngine;

namespace Citadel
{
    [CreateAssetMenu(menuName = "Citadel/Rules/UnitRuleData")]
    public sealed class UnitRuleData : ScriptableObject
    {
        [Header("Soldier Production")]
        public int soldierMoneyCost = 10;
        public int soldierHoursPerUnit = 3;

        [Header("Limits")]
        public int soldierCap = 30;
        public int barracksQueueCap = 5;
    }
}