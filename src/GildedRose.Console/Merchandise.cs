namespace GildedRose.Console
{
    /// <summary>
    /// Shared specification for Merchandise.
    /// </summary>
    interface IMerchandise
    {
        /// <summary>
        /// Update the internal state to reflect a new cycle/day.
        /// </summary>
        void UpdateQuality();
    }

    /// <summary>
    /// The different merchandise types available.
    /// </summary>
    public enum MerchandiseType
    {
        Normal,
        AgedWell,
        EventPass,
        Conjured,
        Legendary
    }

    /// <summary>
    /// Base class for merchandise, which handles the implementation of IMerchandise and inherits from Item for easy casting.
    /// </summary>
    public abstract class Merchandise : Item, IMerchandise
    {
        protected const int BASE_QUALITY_STEP = 1;
        protected const int SELL_DATE_PASSED_FACTOR = 2;

        public Merchandise(Item item)
        {
            this.Name = item.Name;
            this.Quality = item.Quality;
            this.SellIn = item.SellIn;
        }

        public abstract void UpdateQuality();

        protected virtual void UpgradeQuality(int multiplier = 1, int constant = 0)
        {
            Quality += (BASE_QUALITY_STEP * multiplier) + constant;
        }

        protected virtual void DegradeQuality(int multiplier = 1, int constant = 0)
        {
            Quality -= (BASE_QUALITY_STEP * multiplier) + constant;
        }

        protected void EnforceNonNegativeQuality()
        {
            if (Quality < 0)
            {
                Quality = 0;
            }
        }
        protected virtual void EnforceMaxQuality()
        {
            if (Quality > 50)
            {
                Quality = 50;
            }
        }

        protected virtual void DecrementSellIn()
        {
            SellIn -= 1;
        }
    }

    class NormalItem : Merchandise
    {
        public NormalItem(Item item) : base(item) { }

        public override void UpdateQuality()
        {
            int multiplier = 1;
            if (SellIn <= 0)
            {
                multiplier = multiplier * SELL_DATE_PASSED_FACTOR;
            }

            DegradeQuality(multiplier);
            EnforceNonNegativeQuality();
            EnforceMaxQuality();
            DecrementSellIn();
        }
    }

    class AgedWellItem : Merchandise
    {
        public AgedWellItem(Item item) : base(item) { }

        public override void UpdateQuality()
        {
            UpgradeQuality();
            EnforceNonNegativeQuality();
            EnforceMaxQuality();
            DecrementSellIn();
        }
    }

    class LegendaryItem : Merchandise
    {
        public LegendaryItem(Item item) : base(item) { }

        public override void UpdateQuality()
        {
            // As the item is legendary, it never changes stats.
            EnforceNonNegativeQuality();
        }
    }

    class EventPassItem : Merchandise
    {
        public EventPassItem(Item item) : base(item) { }

        public override void UpdateQuality()
        {
            if (SellIn <= 0)
            {
                Quality = 0;
            }
            else if (SellIn <= 5)
            {
                UpgradeQuality(constant:2);
            }
            else if (SellIn <= 10)
            {
                UpgradeQuality(constant:1);
            }
            else
            {
                UpgradeQuality();
            }
            EnforceNonNegativeQuality();
            EnforceMaxQuality();
            DecrementSellIn();
        }
    }

    class ConjuredItem : Merchandise
    {
        private const int CONJURED_DEGRADATION_FACTOR = 2;
        public ConjuredItem(Item item) : base(item) { }

        public override void UpdateQuality()
        {
            var multiplier = CONJURED_DEGRADATION_FACTOR;
            if (SellIn <= 0)
            {
                multiplier = multiplier * SELL_DATE_PASSED_FACTOR;
            }

            DegradeQuality(multiplier);
            EnforceNonNegativeQuality();
            EnforceMaxQuality();
            DecrementSellIn();
        }
    }
}