using System.Collections.Generic;

namespace GildedRose.Console
{
    public class LegacyInventoryManager : IInventoryManager
    {
        public IList<Item> ProcessInventory(IList<Item> inventory)
        {
            for (var i = 0; i < inventory.Count; i++)
            {
                if (inventory[i].Name != "Aged Brie" && inventory[i].Name != "Backstage passes to a TAFKAL80ETC concert")
                {
                    if (inventory[i].Quality > 0)
                    {
                        if (inventory[i].Name != "Sulfuras, Hand of Ragnaros")
                        {
                            inventory[i].Quality = inventory[i].Quality - 1;
                        }
                    }
                }
                else
                {
                    if (inventory[i].Quality < 50)
                    {
                        inventory[i].Quality = inventory[i].Quality + 1;

                        if (inventory[i].Name == "Backstage passes to a TAFKAL80ETC concert")
                        {
                            if (inventory[i].SellIn < 11)
                            {
                                if (inventory[i].Quality < 50)
                                {
                                    inventory[i].Quality = inventory[i].Quality + 1;
                                }
                            }

                            if (inventory[i].SellIn < 6)
                            {
                                if (inventory[i].Quality < 50)
                                {
                                    inventory[i].Quality = inventory[i].Quality + 1;
                                }
                            }
                        }
                    }
                }

                if (inventory[i].Name != "Sulfuras, Hand of Ragnaros")
                {
                    inventory[i].SellIn = inventory[i].SellIn - 1;
                }

                if (inventory[i].SellIn < 0)
                {
                    if (inventory[i].Name != "Aged Brie")
                    {
                        if (inventory[i].Name != "Backstage passes to a TAFKAL80ETC concert")
                        {
                            if (inventory[i].Quality > 0)
                            {
                                if (inventory[i].Name != "Sulfuras, Hand of Ragnaros")
                                {
                                    inventory[i].Quality = inventory[i].Quality - 1;
                                }
                            }
                        }
                        else
                        {
                            inventory[i].Quality = inventory[i].Quality - inventory[i].Quality;
                        }
                    }
                    else
                    {
                        if (inventory[i].Quality < 50)
                        {
                            inventory[i].Quality = inventory[i].Quality + 1;
                        }
                    }
                }
            }
            return inventory;
        }
    }
}