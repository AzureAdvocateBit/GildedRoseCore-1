using System;
using Xunit;
using GildedRose.Console;
using System.Collections.Generic;
using System.Linq;

namespace GildedRose.Tests
{
    public abstract class InventoryTests
    {
        IList<Item> items;
        protected IInventoryManager InventoryManager { get; set; }

        public InventoryTests()
        {
            items = new List<Item>();
        }

        private IList<Item> DefaultItems()
        {
            return new List<Item>
            {
                new Item {Name = "+5 Dexterity Vest", SellIn = 10, Quality = 20},
                new Item {Name = "Aged Brie", SellIn = 2, Quality = 0},
                new Item {Name = "Elixir of the Mongoose", SellIn = 5, Quality = 7},
                new Item {Name = "Sulfuras, Hand of Ragnaros", SellIn = 0, Quality = 80},
                new Item
                    {
                        Name = "Backstage passes to a TAFKAL80ETC concert",
                        SellIn = 15,
                        Quality = 20
                    },
                new Item {Name = "Conjured Mana Cake", SellIn = 3, Quality = 6}
            };
        }

        [Fact]
        public void Purchase_Fills_Inventory()
        {
            Assert.Empty(items);
            items = DefaultItems();
            Assert.NotEmpty(items);
        }

        [Fact]
        public void Items_With_Passed_SellBy_Degrades_Twice_As_Fast()
        {
            items = new List<Item>
            {
                new Item {Name = "+5 Dexterity Vest", SellIn = 0, Quality = 20},
                new Item {Name = "Elixir of the Mongoose", SellIn = 0, Quality = 7},
            };

            items = InventoryManager.ProcessInventory(items);

            var vest = items.Single(x => x.Name == "+5 Dexterity Vest");
            var elixir = items.Single(x => x.Name == "Elixir of the Mongoose");

            Assert.Equal(18, vest.Quality);
            Assert.Equal(5, elixir.Quality);
        }

        [Fact]
        public void Quality_Is_Never_Negative()
        {
            items = DefaultItems();
            var maxSellIn = items.Max(x => x.Quality) + 1;

            for (int i = 0; i < maxSellIn; i++)
            {
                items = InventoryManager.ProcessInventory(items);
            }
            foreach (var item in items)
            {
                Assert.True(item.Quality >= 0);
            }
        }

        [Fact]
        public void Aged_Items_Increase_Quality_On_Aging()
        {
            items = new List<Item>
            {
                new Item {Name = "Aged Brie", SellIn = 2, Quality = 0}
            };
            
            items = InventoryManager.ProcessInventory(items);

            var agedItem = items.Single(x => x.Name == "Aged Brie");

            Assert.Equal(1, agedItem.Quality);
            Assert.Equal(1, agedItem.SellIn);
        }

        [Fact]
        public void Quality_Is_Never_Higher_Than_50_Except_Legendary()
        {
            items = DefaultItems();

            for (int i = 0; i < 50; i++)
            {
                items = InventoryManager.ProcessInventory(items);
            }
            foreach (var item in items)
            {
                if (item.Name == "Sulfuras, Hand of Ragnaros")
                {
                    Assert.Equal(80, item.Quality);
                }
                else
                {
                    Assert.True(item.Quality <= 50);
                }
            }
        }

        [Fact]
        public void Legendary_Never_Changes_Status()
        {
            items = DefaultItems();
            var initialLegendaryItem = items.Single(x => x.Name == "Sulfuras, Hand of Ragnaros");

            for (int i = 0; i < 50; i++)
            {
                items = InventoryManager.ProcessInventory(items);
                var legendaryItem = items.Single(x => x.Name == "Sulfuras, Hand of Ragnaros");
                Assert.True(legendaryItem.SellIn == initialLegendaryItem.SellIn);
                Assert.Equal(0, legendaryItem.SellIn);
                Assert.True(legendaryItem.Quality == initialLegendaryItem.Quality);
            }
        }

        [Fact]
        public void Backstage_Pass_Quality_Surges_Until_Event_And_Then_Drop()
        {
            items = new List<Item>
            {
                new Item
                {
                    Name = "Backstage passes to a TAFKAL80ETC concert",
                    SellIn = 15,
                    Quality = 20
                }
            };
            var maxSellIn = items.Max(i => i.SellIn);

            Func<Item> getBackstagePass = () => items.Single(i => i.Name == "Backstage passes to a TAFKAL80ETC concert");
            Dictionary<int, int> map = new Dictionary<int, int>();

            for (int i = 0; i < maxSellIn + 2; i++)
            {
                var item = getBackstagePass();
                map[item.SellIn] = item.Quality;
                items = InventoryManager.ProcessInventory(items);
            }

            Assert.Equal(20, map[15]);
            Assert.Equal(1, map[11]-map[12]);
            Assert.Equal(24, map[11]);
            Assert.Equal(2, map[9]-map[10]);
            Assert.Equal(27, map[9]);
            Assert.Equal(3, map[4]-map[5]);
            Assert.Equal(38, map[4]);
            Assert.Equal(50, map[0]);
            Assert.Equal(0, map[-1]);
        }

        [Fact]
        public void Conjured_Items_Degrade_Twice_as_Fast_As_Normal()
        {
            items = new List<Item>
            {
                new Item {Name = "+5 Dexterity Vest", SellIn = 1, Quality = 10},
                new Item {Name = "Conjured Mana Cake", SellIn = 1, Quality = 10}
            };

            items = InventoryManager.ProcessInventory(items);

            Func<Item> getConjuredItem = () => items.Single(x => x.Name == "Conjured Mana Cake");
            Func<Item> getNormalItem = () => items.Single(x => x.Name == "+5 Dexterity Vest");

            var conjured = getConjuredItem();
            var normal = getNormalItem();

            Assert.Equal(0, conjured.SellIn);
            Assert.Equal(8, conjured.Quality);

            Assert.Equal(0, normal.SellIn);
            Assert.Equal(9, normal.Quality);

            items = InventoryManager.ProcessInventory(items);

            conjured = getConjuredItem();
            normal = getNormalItem();

            Assert.Equal(-1, conjured.SellIn);
            Assert.Equal(4, conjured.Quality);

            Assert.Equal(-1, normal.SellIn);
            Assert.Equal(7, normal.Quality);
        }
    }

    public class LegacyInventoryTests : InventoryTests
    {
        public LegacyInventoryTests() : base()
        {
            this.InventoryManager = new LegacyInventoryManager();
        }
    }

    public class HAL9000InventoryTests : InventoryTests
    {
        public HAL9000InventoryTests() : base()
        {
            this.InventoryManager = new HAL9000InventoryManager();
        }
    }
}
