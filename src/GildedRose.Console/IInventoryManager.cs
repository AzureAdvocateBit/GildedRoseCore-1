using System.Collections.Generic;

namespace GildedRose.Console
{
    /// <summary>
    /// A Manager that when can be used to inject different implementations of an inventory manager. 
    /// </summary>
    public interface IInventoryManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inventory">Inventory list to process</param>
        /// <returns>Processed inventory list</returns>
        IList<Item> ProcessInventory(IList<Item> inventory);
    }
}