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
            var maxSellIn = items.Select(x => x.SellIn).OrderByDescending(x => x).First() + 1;

            for (int i = 0; i < maxSellIn; i++)
            {
                items = inventoryManager.ProcessInventory(items);
            }
            foreach (var item in items)
            {
                Assert.True(item.Quality >= 0);
            }
        }
    }
}
