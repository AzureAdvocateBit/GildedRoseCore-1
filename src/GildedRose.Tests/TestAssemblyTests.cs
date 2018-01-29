using System;
using Xunit;
using GildedRose.Console;
using System.Collections.Generic;
using System.Linq;

namespace GildedRose.Tests
{

    public class InventoryTests
    {
        IList<Item> items;
        IInventoryManager inventoryManager;

        public InventoryTests()
        {
            items = new List<Item>();
            inventoryManager = new LegacyInventoryManager();
        }

        [Fact]
        public void Purchase_Fills_Inventory()
        {
            Assert.Empty(items);
            items = new SupplyManager().PurchaseDefaultSupplies();
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

            items = inventoryManager.ProcessInventory(items);

            var vest = items.Single(x => x.Name == "+5 Dexterity Vest");
            var elixir = items.Single(x => x.Name == "Elixir of the Mongoose");

            Assert.True(vest.Quality == 18);
            Assert.True(elixir.Quality == 5);
        }

        [Fact]
        public void Quality_Is_Never_Negative()
        {
            items = new SupplyManager().PurchaseDefaultSupplies();
            var maxSellIn = items.Max(x => x.Quality) + 1;

            for (int i = 0; i < maxSellIn; i++)
            {
                items = inventoryManager.ProcessInventory(items);
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
            
            items = inventoryManager.ProcessInventory(items);

            var agedItem = items.Single(x => x.Name == "Aged Brie");

            Assert.True(agedItem.Quality == 1);
            Assert.True(agedItem.SellIn == 1);
        }

        [Fact]
        public void Quality_Is_Never_Higher_Than_50_Except_Legendary()
        {
            items = new SupplyManager().PurchaseDefaultSupplies();

            for (int i = 0; i < 50; i++)
            {
                items = inventoryManager.ProcessInventory(items);
            }
            foreach (var item in items)
            {
                if (item.Name == "Sulfuras, Hand of Ragnaros")
                {
                    Assert.True(item.Quality == 80);
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
            items = new SupplyManager().PurchaseDefaultSupplies();
            var initialLegendaryItem = items.Single(x => x.Name == "Sulfuras, Hand of Ragnaros");

            for (int i = 0; i < 50; i++)
            {
                items = inventoryManager.ProcessInventory(items);
                var legendaryItem = items.Single(x => x.Name == "Sulfuras, Hand of Ragnaros");
                Assert.True(legendaryItem.SellIn == initialLegendaryItem.SellIn);
                Assert.True(legendaryItem.SellIn == 0);
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
                items = inventoryManager.ProcessInventory(items);
            }

            Assert.True(map[15] == 20);
            Assert.True(map[11]-map[12] == 1);
            Assert.True(map[11] == 24);
            Assert.True(map[9]-map[10] == 2);
            Assert.True(map[9] == 27);
            Assert.True(map[4]-map[5] == 3);
            Assert.True(map[4] == 38);
            Assert.True(map[0] == 50);
            Assert.True(map[-1] == 0);
        }

        [Fact]
        public void Conjured_Items_Degrade_Twice_as_Fast_As_Normal()
        {
            items = new List<Item>
            {
                new Item {Name = "+5 Dexterity Vest", SellIn = 1, Quality = 10},
                new Item {Name = "Conjured Mana Cake", SellIn = 1, Quality = 10}
            };

            items = inventoryManager.ProcessInventory(items);

            Func<Item> getConjuredItem = () => items.Single(x => x.Name == "Conjured Mana Cake");
            Func<Item> getNormalItem = () => items.Single(x => x.Name == "+5 Dexterity Vest");

            var conjured = getConjuredItem();
            var normal = getNormalItem();

            Assert.Equal(0, conjured.SellIn);
            Assert.Equal(8, conjured.Quality);

            Assert.Equal(0, normal.SellIn);
            Assert.Equal(9, normal.Quality);

            items = inventoryManager.ProcessInventory(items);

            conjured = getConjuredItem();
            normal = getNormalItem();

            Assert.Equal(-1, conjured.SellIn);
            Assert.Equal(4, conjured.Quality);

            Assert.Equal(-1, normal.SellIn);
            Assert.Equal(7, normal.Quality);
        }
    }
}
