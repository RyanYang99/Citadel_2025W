namespace Citadel
{
    public sealed record Bonus
    {
        public Item? TargetItem;
        public RangeResource? TargetRangeResource;

        public readonly BonusValue TargetBonusValue;

        public Bonus(Item item, BonusValue targetBonusValue)
        {
            TargetItem = item;
            TargetBonusValue = targetBonusValue;
        }
        
        public Bonus(RangeResource rangeResource, BonusValue targetBonusValue)
        {
            TargetRangeResource = rangeResource;
            TargetBonusValue = targetBonusValue;
        }
    }
}