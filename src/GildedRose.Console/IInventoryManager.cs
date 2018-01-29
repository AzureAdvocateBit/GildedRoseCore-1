using System.Collections.Generic;

namespace GildedRose.Console
{
    public interface IInventoryManager
    {
        IList<Item> ProcessInventory(IList<Item> inventory);
    }
}