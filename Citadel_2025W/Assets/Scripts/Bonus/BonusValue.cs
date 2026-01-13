using System;

namespace Citadel
{
    [Serializable]
    public record BonusValue
    {
        public int flat;
        public float percentage;

        public BonusValue() {}

        public BonusValue(int flat) => this.flat = flat;

        public BonusValue(float percentage) => this.percentage = percentage;
    }
}